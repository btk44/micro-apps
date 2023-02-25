using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Tools;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Transactions;

public class CreateTransactionsCommand: IRequest<Either<List<TransactionDto>, TransactionValidationException>> {
    public List<CreateTransactionCommand> Transactions { get; set; }
}

public class CreateTransactionsCommandHandler: IRequestHandler<CreateTransactionsCommand, Either<List<TransactionDto>, TransactionValidationException>> {
    private IApplicationDbContext _dbContext;
    private TransactionValidator _transactionValidator;
    private IMapper _transactionMapper;

    public CreateTransactionsCommandHandler(IApplicationDbContext dbContext, IMapper transactionMapper){
        _dbContext = dbContext;
        _transactionValidator = new TransactionValidator();
        _transactionMapper = transactionMapper; // consider moving mapping into Dto file?
    }

    public async Task<Either<List<TransactionDto>, TransactionValidationException>> Handle(CreateTransactionsCommand command, CancellationToken cancellationToken){
        if(command.Transactions.Any(x => x.OwnerId <= 0)){
            return new TransactionValidationException("Incorrect owner id");
        }

        return new TransactionValidationException("test");
    }
}