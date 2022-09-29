using AutoMapper;
using MediatR;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Application.Common.Models;
using TransactionService.Application.Common.Tools;

namespace TransactionService.Application.Accounts;

public class CreateAccountCommand: IRequest<Result<AccountDto>> {
    public string Email { get; set; }
    public string Password { get; set; }
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
        return new Result<AccountDto>(new AccountDto());
    }
}