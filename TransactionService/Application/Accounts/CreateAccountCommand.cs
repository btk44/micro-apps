using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common.Exceptions;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;
using TransactionService.Application.Common.Tools;
using TransactionService.Domain.Entities;

namespace TransactionService.Application.Accounts;

public class CreateAccountCommand: IRequest<Result<AccountDto>> {
    public int OwnerId { get; set; }
    public string Name { get; set; }
    public int CurrencyId { get; set; }
}

public class CreateAccountCommandHandler: IRequestHandler<CreateAccountCommand, Result<AccountDto>> {
    private IApplicationDbContext _dbContext;
    private AccountValidator _accountValidator;
    private IMapper _accountMapper;

    public CreateAccountCommandHandler(IApplicationDbContext dbContext, IMapper accountMapper){
        _dbContext = dbContext;
        _accountValidator = new AccountValidator();
        _accountMapper = accountMapper; // consider moving mapping into Dto file?
    }

    public async Task<Result<AccountDto>> Handle(CreateAccountCommand command, CancellationToken cancellationToken){
        if(!_accountValidator.IsNameValid(command.Name)){
            return new AccountValidationException("Incorrect account name");
        }

        if(command.OwnerId <= 0){
            return new AccountValidationException("Incorrect owner id");
        }
                
        var currency = await _dbContext.Currencies.FirstOrDefaultAsync(x => x.Active && x.Id == command.CurrencyId);

        if (currency == null){
            return new AccountValidationException("Unsupported currency");
        }

        var accountEntity = new AccountEntity(){
            Name = command.Name,
            Amount = 0,
            OwnerId = command.OwnerId,
            CurrencyId = command.CurrencyId
        };

        _dbContext.Accounts.Add(accountEntity);      

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new AccountValidationException("Save error - please try again");
        }

        var dto = _accountMapper.Map<AccountDto>(accountEntity);

        return dto;
    }
}