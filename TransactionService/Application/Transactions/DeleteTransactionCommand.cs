using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Tools;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;

namespace TransactionService.Application.Transactions;

public class DeleteTransactionCommand: IRequest<Either<bool, TransactionValidationException>> {
    public int OwnerId { get; set; }
    public int Id { get; set; }
}

public class DeleteTransactionCommandHandler: IRequestHandler<DeleteTransactionCommand, Either<bool, TransactionValidationException>> {
    private IApplicationDbContext _dbContext;

    public DeleteTransactionCommandHandler(IApplicationDbContext dbContext){
        _dbContext = dbContext;
    }

    public async Task<Either<bool, TransactionValidationException>> Handle(DeleteTransactionCommand command, CancellationToken cancellationToken){
        var transactionEntity = await _dbContext.Transactions.FirstOrDefaultAsync(x => x.Active && x.OwnerId == command.OwnerId && x.Id == command.Id);

        if(transactionEntity == null){
            return true;
        }

        var account = await _dbContext.Accounts
                                .Include(x => x.Transactions.Where(t => t.Active))
                                .FirstOrDefaultAsync(x => x.Active && x.OwnerId == command.OwnerId && x.Id == transactionEntity.AccountId);

        if(account != null){
            account.Amount = account.Transactions.Sum(x => x.Amount); // to do : this is not optimal
        }

        transactionEntity.Active = false;

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new TransactionValidationException("Save error - please try again");
        }

        return true;
    }
}