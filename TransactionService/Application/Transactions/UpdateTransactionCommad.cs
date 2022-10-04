using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;
using TransactionService.Application.Common.Tools;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Transactions;

public class UpdateTransactionCommand: IRequest<Result<bool>> {
    public int OwnerId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public int CurrencyId { get; set; }
}

public class UpdateTransactionCommandHandler: IRequestHandler<UpdateTransactionCommand, Result<bool>> {
    private IApplicationDbContext _dbContext;
    private TransactionValidator _transactionValidator;

    public UpdateTransactionCommandHandler(IApplicationDbContext dbContext){
        _dbContext = dbContext;
        _transactionValidator = new TransactionValidator();
    }

    public async Task<Result<bool>> Handle(UpdateTransactionCommand command, CancellationToken cancellationToken){
        if(command.OwnerId <= 0){
            return new TransactionValidationException("Incorrect owner id");
        }

        var transactionEntity = await _dbContext.Transactions.FirstOrDefaultAsync(x => x.Active && x.OwnerId == command.OwnerId && x.Id == command.Id);

        if(transactionEntity == null){
            return new TransactionValidationException("Transaction does not exist");
        }

        _dbContext.Transactions.Add(transactionEntity);      

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new TransactionValidationException("Save error - please try again");
        }

        return true;
    }
}