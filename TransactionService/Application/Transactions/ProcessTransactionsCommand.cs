using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Tools;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Transactions;

public class ProcessTransactionsCommand: IRequest<Either<List<TransactionDto>, TransactionValidationException>> {
    public int ProcessingUserId { get; set; }
    public List<TransactionDto> Transactions { get; set; }
}

public class ProcessTransactionsCommandHandler: IRequestHandler<ProcessTransactionsCommand, Either<List<TransactionDto>, TransactionValidationException>> {
    private IApplicationDbContext _dbContext;
    private TransactionValidator _transactionValidator;
    private IMapper _transactionMapper;

    public ProcessTransactionsCommandHandler(IApplicationDbContext dbContext, IMapper transactionMapper){
        _dbContext = dbContext;
        _transactionValidator = new TransactionValidator();
        _transactionMapper = transactionMapper; // consider moving mapping into Dto file?
    }

    public async Task<Either<List<TransactionDto>, TransactionValidationException>> Handle(ProcessTransactionsCommand command, CancellationToken cancellationToken){
        if(command.Transactions.Any(x => x.OwnerId <= 0)){
            var incorrectTransactionIds = string.Join(", ", command.Transactions.Where(x => x.OwnerId <= 0).Select(x => x.Id));
            return new TransactionValidationException($"Incorrect owner id in transactions: { incorrectTransactionIds }");
        }

        var accountIdList = command.Transactions.Select(x => x.AccountId).Distinct();
        var categoryIdList = command.Transactions.Select(x => x.CategoryId).Distinct();
        var transactionIdList = command.Transactions.Where(x => x.Id > 0).Select(x => x.Id);

        // check if currency exist and consider processing inactive

        var accounts = await _dbContext.Accounts
                                    .Include(x => x.AdditionalInfo)
                                    .Where(x => x.Active && accountIdList.Contains(x.Id)) // to do: active? maybe not?
                                    .ToDictionaryAsync(x => x.Id);

        var categories = await _dbContext.Categories
                                    .Where(x => x.Active && categoryIdList.Contains(x.Id))
                                    .ToDictionaryAsync(x => x.Id);

        var existingTransactions = await _dbContext.Transactions
                                    .Where(x => x.Active && transactionIdList.Contains(x.Id))
                                    .ToListAsync();

        var existingTransactionIds = existingTransactions.Select(x => x.Id);
        var missingTransactions = transactionIdList.Except(existingTransactionIds).ToList();

        if(missingTransactions.Any()){
            return new TransactionValidationException($"Missing transactions: { string.Join(", ", missingTransactions) }");
        }

        var processedEntities = new List<TransactionEntity>();

        foreach(var commandTransaction in command.Transactions){
            AccountEntity account;
            CategoryEntity category;

            if(!accounts.TryGetValue(commandTransaction.AccountId, out account)){
                return new TransactionValidationException($"Account does not exist for transaction: {commandTransaction.Id}");
            }

            if(!categories.TryGetValue(commandTransaction.CategoryId, out category)){
                return new TransactionValidationException($"Category does not exist for transaction: {commandTransaction.Id}");
            }

            if(commandTransaction.OwnerId != account.OwnerId){
                return new TransactionValidationException($"Owner id between account and transaction does not match for transaction: {commandTransaction.Id}");
            }

            if(commandTransaction.OwnerId != category.OwnerId){
                return new TransactionValidationException($"Owner id between category and transaction does not match for transaction: {commandTransaction.Id}");
            }

            TransactionEntity transactionEntity = existingTransactions.FirstOrDefault(x => x.Id == commandTransaction.Id);
            if (transactionEntity == null){
                transactionEntity = new TransactionEntity();
                _dbContext.Transactions.Add(transactionEntity);  // will that work?
            } 
            
            _transactionMapper.Map(commandTransaction, transactionEntity);
            transactionEntity.Account = account;
            transactionEntity.Category = category;

            processedEntities.Add(transactionEntity);
        }

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new TransactionValidationException("Save error - please try again");
        }

        return processedEntities.Select(x => _transactionMapper.Map<TransactionDto>(x)).ToList();  
    }
}