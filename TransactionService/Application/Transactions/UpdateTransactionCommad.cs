using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Tools;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;

namespace TransactionService.Application.Transactions;

public class UpdateTransactionCommand: IRequest<Either<bool, TransactionValidationException>> {
    public int OwnerId { get; set; }
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int AccountId { get; set; }
    public double Amount { get; set; }
    public string Payee { get; set; }
    public int CategoryId { get; set; }
    public string Comment { get; set; }
}

public class UpdateTransactionCommandHandler: IRequestHandler<UpdateTransactionCommand, Either<bool, TransactionValidationException>> {
    private IApplicationDbContext _dbContext;
    private TransactionValidator _transactionValidator;

    public UpdateTransactionCommandHandler(IApplicationDbContext dbContext){
        _dbContext = dbContext;
        _transactionValidator = new TransactionValidator();
    }

    public async Task<Either<bool, TransactionValidationException>> Handle(UpdateTransactionCommand command, CancellationToken cancellationToken){
        if(command.OwnerId <= 0){
            return new TransactionValidationException("Incorrect owner id");
        }

        var account = await _dbContext.Accounts
                                .Include(x => x.Transactions.Where(t => t.Active))
                                .FirstOrDefaultAsync(x => x.Active && x.OwnerId == command.OwnerId && x.Id == command.AccountId);
        if (account == null){
            return new TransactionValidationException("Account does not exist");
        }

        var category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Active && x.OwnerId == command.OwnerId && x.Id == command.CategoryId);
        if (category == null){
            return new TransactionValidationException("Category does not exist");
        }

        var transactionEntity = await _dbContext.Transactions.FirstOrDefaultAsync(x => x.Active && x.OwnerId == command.OwnerId && x.Id == command.Id);

        if(transactionEntity == null){
            return new TransactionValidationException("Transaction does not exist");
        }

        // to do: consider mapping            
        transactionEntity.Date = command.Date;
        transactionEntity.Account = account;
        transactionEntity.AccountId = account.Id;
        transactionEntity.Amount = command.Amount;
        transactionEntity.Payee = command.Payee;
        transactionEntity.Category = category;
        transactionEntity.CategoryId = category.Id;
        transactionEntity.Comment = command.Comment;  

        account.Amount = account.Transactions.Sum(x => x.Amount); // to do : this is not optimal

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new TransactionValidationException("Save error - please try again");
        }

        return true;
    }
}