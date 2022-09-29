using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;
using TransactionService.Application.Common.Tools;

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
        var currency = await _dbContext.Currencies.FirstOrDefaultAsync(x => x.Active && x.Id == command.CurrencyId);

        if (currency == null){
            return new Result<AccountDto>(new Exception("Unsupported currency"));
        }

        return new Result<AccountDto>(new AccountDto());
    }
}