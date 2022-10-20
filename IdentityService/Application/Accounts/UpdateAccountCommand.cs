using AutoMapper;
using IdentityService.Application.Common.Exceptions;
using IdentityService.Application.Common.Interfaces;
using IdentityService.Application.Common.Tools;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Accounts;

public class UpdateAccountCommand: IRequest<Either<bool, AccountValidationException>> {
    public int AccountId { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string Email { get; set; }
}

public class UpdateAccountCommandHandler: IRequestHandler<UpdateAccountCommand, Either<bool, AccountValidationException>> {
    private IApplicationDbContext _dbContext;
    private AccountValidator _accountValidator;
    private PasswordHasher<string> _passwordHasher;
    private IMapper _accountMapper;

    public UpdateAccountCommandHandler(IApplicationDbContext dbContext, IMapper accountMapper){
        _dbContext = dbContext;
        _accountValidator = new AccountValidator();
        _passwordHasher = new PasswordHasher<string>(); 
        _accountMapper = accountMapper; // consider moving mapping into Dto file?
    }

    public async Task<Either<bool, AccountValidationException>> Handle(UpdateAccountCommand command, CancellationToken cancellationToken){
        var account = await _dbContext.Accounts
            .FirstOrDefaultAsync(x => x.Active && x.Id == command.AccountId);

        if (account == null){
            return new AccountValidationException("Account was deactivated");
        }

        if(!string.IsNullOrEmpty(command.Email)){
            if(!_accountValidator.IsEmailValid(command.Email)){
                return new AccountValidationException("Incorrect email format");
            }

            if(await _dbContext.Accounts.AnyAsync(x=> x.Active && x.Email == command.Email)){
                return new AccountValidationException("Email address is already taken"); 
            }

            account.Email = command.Email;
        }

        if(!string.IsNullOrEmpty(command.OldPassword) || !string.IsNullOrEmpty(command.NewPassword)){
            if(command.OldPassword == command.NewPassword){
                return new AccountValidationException("Passwords cannot be the same");
            }

            if(!_accountValidator.IsPasswordValid(account, command.OldPassword)){
                return new AccountValidationException("Wrong old password provided");   
            }

            if(!_accountValidator.IsPasswordSecure(command.NewPassword)){
                return new AccountValidationException("Password does not fulfill requirements: [to do]");
            }

            account.Password = _passwordHasher.HashPassword(account.Email, command.NewPassword);
        }

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new AccountValidationException("Save error - please try again");
        }

        return true;
    }
}