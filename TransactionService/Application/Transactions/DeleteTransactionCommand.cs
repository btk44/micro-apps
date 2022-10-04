using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Tools;

namespace TransactionService.Application.Transactions;

public class DeleteTransactionCommand: IRequest<Result<bool>> {
    public int OwnerId { get; set; }
    public int Id { get; set; }
}

public class DeleteTransactionCommandHandler: IRequestHandler<DeleteTransactionCommand, Result<bool>> {
    private IApplicationDbContext _dbContext;

    public DeleteTransactionCommandHandler(IApplicationDbContext dbContext){
        _dbContext = dbContext;
    }

    public async Task<Result<bool>> Handle(DeleteTransactionCommand command, CancellationToken cancellationToken){
        var accountEntity = await _dbContext.Transactions.FirstOrDefaultAsync(x => x.Active && x.OwnerId == command.OwnerId && x.Id == command.Id);

        if(accountEntity == null){
            return true;
        }

        accountEntity.Active = false;

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new TransactionValidationException("Save error - please try again");
        }

        return true;
    }
}