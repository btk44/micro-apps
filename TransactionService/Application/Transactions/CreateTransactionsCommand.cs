using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Tools;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Transactions;

public class TransactionAction {
    public int OwnerId { get; set; }
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int AccountId { get; set; }
    public double Amount { get; set; }
    public string Payee { get; set; }
    public int CategoryId { get; set; }
    public string Comment { get; set; }
    public string GroupKey { get; set; }
    public bool Deleted { get; set; }
}

public class ProcessTransactionsCommand: IRequest<Either<List<TransactionDto>, TransactionValidationException>> {
    public List<TransactionAction> Transactions { get; set; }
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

        var accounts = await _dbContext.Accounts
                                    .Include(x => x.AdditionalInfo)
                                    .Where(x => x.Active && accountIdList.Contains(x.Id))
                                    .ToDictionaryAsync(x => x.Id);

        var categories = await _dbContext.Categories
                                    .Where(x => x.Active && categoryIdList.Contains(x.Id))
                                    .ToDictionaryAsync(x => x.Id);

        var existingTransactions = await _dbContext.Transactions
                                    .Where(x => x.Active && transactionIdList.Contains(x.Id))
                                    .ToListAsync();

        var missingTransactions = transactionIdList.Except(existingTransactions.Select(x => x.Id)).ToList();

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

            if (transactionEntity != null){
                if(commandTransaction.Deleted){
                    _dbContext.Transactions.Remove(transactionEntity);
                } 
                else {
                    transactionEntity.Date = commandTransaction.Date;
                    transactionEntity.AccountId = commandTransaction.AccountId;
                    transactionEntity.Account = account;
                    transactionEntity.Category = category;
                    transactionEntity.CategoryId = commandTransaction.CategoryId;
                    transactionEntity.AdditionalInfo.Payee = commandTransaction.Payee;
                    transactionEntity.AdditionalInfo.Comment = commandTransaction.Comment;
                    transactionEntity.GroupKey = commandTransaction.GroupKey;
                }
            } 
            else {
                if(!commandTransaction.Deleted){              
                    transactionEntity = new TransactionEntity(){ // to do: consider mapping
                        OwnerId = commandTransaction.OwnerId,
                        Date = commandTransaction.Date,
                        Account = account,
                        AccountId = account.Id,
                        Amount = commandTransaction.Amount,
                        Category = category,
                        CategoryId = category.Id,
                        AdditionalInfo = new TransactionAdditionalInfoEntity(){
                            Payee = commandTransaction.Payee,
                            Comment = commandTransaction.Comment
                        },
                        GroupKey = commandTransaction.GroupKey
                    };

                    _dbContext.Transactions.Add(transactionEntity);
                }
            }

            if(!commandTransaction.Deleted){
                processedEntities.Add(transactionEntity);
            }
        }

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new TransactionValidationException("Save error - please try again");
        }

        return processedEntities.Select(x => _transactionMapper.Map<TransactionDto>(x)).ToList();  
    }
}