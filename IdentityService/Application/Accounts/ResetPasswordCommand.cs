using IdentityService.Application.Common.Exceptions;
using IdentityService.Application.Common.Interfaces;
using IdentityService.Application.Common.Tools;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Accounts;

public class ResetPasswordCommand : IRequest<Either<bool, AccountValidationException>>  {
    public int AccountId { get; set; }
    public string ResetToken { get; set; }
    public string NewPassword  { get; set; }
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Either<bool, AccountValidationException>>
{
    private IApplicationDbContext _dbContext;
    private PasswordHasher<string> _passwordHasher;
    private AccountValidator _accountValidator;

    public ResetPasswordCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _passwordHasher = new PasswordHasher<string>(); 
        _accountValidator = new AccountValidator();
    }

    public async Task<Either<bool, AccountValidationException>> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var account = await _dbContext.Accounts
                                .Include(x => x.ResetPasswordTokens)
                                .FirstOrDefaultAsync(x => x.Active && x.Id == command.AccountId);

        if (account == null){
            return new AccountValidationException("Account does not exist");
        }

        var oldToken = account.ResetPasswordTokens.FirstOrDefault(x => x.Active && x.Token == command.ResetToken);

        if (oldToken == null){
            return new AccountValidationException("Token is invalid");
        }

        if(!_accountValidator.IsPasswordSecure(command.NewPassword)){
            return new AccountValidationException("Password does not fulfill requirements: [to do]");
        }

        oldToken.Active = false;
        account.Password = _passwordHasher.HashPassword(account.Email, command.NewPassword);

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new AccountValidationException("Save error - please try again");
        }

        return true;
    }
}