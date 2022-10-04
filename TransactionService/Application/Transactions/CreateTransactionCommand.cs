using AutoMapper;
using MediatR;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;
using TransactionService.Application.Common.Tools;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Transactions;

public class CreateTransactionCommand: IRequest<Result<TransactionDto>> {
    public int OwnerId { get; set; }
    public string Name { get; set; }
    public int CurrencyId { get; set; }
}

public class CreateTransactionCommandHandler: IRequestHandler<CreateTransactionCommand, Result<TransactionDto>> {
    private IApplicationDbContext _dbContext;
    private TransactionValidator _accountValidator;
    private IMapper _accountMapper;

    public CreateTransactionCommandHandler(IApplicationDbContext dbContext, IMapper accountMapper){
        _dbContext = dbContext;
        _accountValidator = new TransactionValidator();
        _accountMapper = accountMapper; // consider moving mapping into Dto file?
    }

    public async Task<Result<TransactionDto>> Handle(CreateTransactionCommand command, CancellationToken cancellationToken){
        if(command.OwnerId <= 0){
            return new TransactionValidationException("Incorrect owner id");
        }
        

        var accountEntity = new TransactionEntity(){
            // to do
        };

        _dbContext.Transactions.Add(accountEntity);      

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new TransactionValidationException("Save error - please try again");
        }

        var dto = _accountMapper.Map<TransactionDto>(accountEntity);

        return dto;
    }
}