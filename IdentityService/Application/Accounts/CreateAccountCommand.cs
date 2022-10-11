using AutoMapper;
using IdentityService.Application.Common.Exceptions;
using IdentityService.Application.Common.Interfaces;
using IdentityService.Application.Common.Models;
using IdentityService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Tools;

namespace IdentityService.Application.Accounts;

public class CreateAccountCommand: IRequest<Either<AccountDto, AccountValidationException>> {
    public string Email { get; set; }
    public string Password { get; set; }
}

public class CreateAccountCommandHandler: IRequestHandler<CreateAccountCommand, Either<AccountDto, AccountValidationException>> {
    private IApplicationDbContext _dbContext;
    private AccountValidator _accountValidator;
    private PasswordHasher<string> _passwordHasher;
    private IMapper _accountMapper;

    public CreateAccountCommandHandler(IApplicationDbContext dbContext, IMapper accountMapper){
        _dbContext = dbContext;
        _accountValidator = new AccountValidator();  // move validator file to Accounts! consider splitting
        _passwordHasher = new PasswordHasher<string>(); 
        _accountMapper = accountMapper; // consider moving mapping into Dto file?
    }

    public async Task<Either<AccountDto,AccountValidationException>> Handle(CreateAccountCommand command, CancellationToken cancellationToken){
        if(!_accountValidator.IsDataProvided(command.Email, command.Password)){
            return new AccountValidationException("Empty email or password");
        }

        if(!_accountValidator.IsEmailValid(command.Email)){
            return new AccountValidationException("Incorrect email format");
        }

        if(!_accountValidator.IsPasswordSecure(command.Password)){
            return new AccountValidationException("Password does not fulfill requirements: [to do]");
        }

        if(await _dbContext.Accounts.AnyAsync(x => x.Active && x.Email == command.Email)){
            return new AccountValidationException("Account with this email already exists");
        };

        var accountEntity = new AccountEntity(){
            Email = command.Email,
            Password = _passwordHasher.HashPassword(command.Email, command.Password)
        };

        _dbContext.Accounts.Add(accountEntity);      

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new AccountValidationException("Save error - please try again");
        }

        var dto = _accountMapper.Map<AccountDto>(accountEntity);

        return dto;
    }
}