using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Tools;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Transactions;

public class CreateTransactionCommand: IRequest<Either<TransactionDto, TransactionValidationException>> {
    public int OwnerId { get; set; }
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int AccountId { get; set; }
    public double Amount { get; set; }
    public string Payee { get; set; }
    public int CategoryId { get; set; }
    public string Comment { get; set; }
    public string GroupKey { get; set; }
}

public class CreateTransactionCommandHandler: IRequestHandler<CreateTransactionCommand, Either<TransactionDto, TransactionValidationException>> {
    private IApplicationDbContext _dbContext;
    private TransactionValidator _transactionValidator;
    private IMapper _transactionMapper;

    public CreateTransactionCommandHandler(IApplicationDbContext dbContext, IMapper transactionMapper){
        _dbContext = dbContext;
        _transactionValidator = new TransactionValidator();
        _transactionMapper = transactionMapper; // consider moving mapping into Dto file?
    }

    public async Task<Either<TransactionDto, TransactionValidationException>> Handle(CreateTransactionCommand command, CancellationToken cancellationToken){
        if(command.OwnerId <= 0){
            return new TransactionValidationException("Incorrect owner id");
        }

        var account = await _dbContext.Accounts
                                .Include(x => x.Transactions.Where(t => t.Active))
                                .Include(x => x.AdditionalInfo)
                                .FirstOrDefaultAsync(x => x.Active && x.OwnerId == command.OwnerId && x.Id == command.AccountId);
        if (account == null){
            return new TransactionValidationException("Account does not exist");
        }

        var category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Active && x.OwnerId == command.OwnerId && x.Id == command.CategoryId);
        if (category == null){
            return new TransactionValidationException("Category does not exist");
        }

        var transactionEntity = new TransactionEntity(){ // to do: consider mapping
            OwnerId = command.OwnerId,
            Date = command.Date,
            Account = account,
            AccountId = account.Id,
            Amount = command.Amount,
            Category = category,
            CategoryId = category.Id,
            AdditionalInfo = new TransactionAdditionalInfoEntity(){
                Payee = command.Payee,
                Comment = command.Comment
            }
        };

        _dbContext.Transactions.Add(transactionEntity);
        account.AdditionalInfo.Amount = account.Transactions.Sum(x => x.Amount); // to do : this is not optimal

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new TransactionValidationException("Save error - please try again");
        }

        var dto = _transactionMapper.Map<TransactionDto>(transactionEntity);

        return dto;
    }
}