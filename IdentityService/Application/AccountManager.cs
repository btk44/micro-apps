using IdentityService.Common;
using IdentityService.Database;
using IdentityService.DataObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application;

public class AccountManager {
    private DatabaseContext _dbContext;
    private PasswordHasher<string> _passwordHasher;

    public AccountManager(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
        _passwordHasher = new PasswordHasher<string>();
    }

    public  async Task<Result<TokenDataDto>> Login(LoginCredentialsDto credentials){
        if(string.IsNullOrEmpty(credentials.Email) || string.IsNullOrEmpty(credentials.Password)){
            return new Result<TokenDataDto>(new AppException("Empty email or password", 400));
        }

        var account = await _dbContext.Accounts
                                        .Include(x => x.FailedAuthInfo)
                                        .FirstOrDefaultAsync(x => x.Active && x.Email == credentials.Email);

        if(account == null){
            return new Result<TokenDataDto>(new AppException("Invalid email or password", 401));
        }

        if(account.FailedAuthInfo != null && 
           account.FailedAuthInfo.FailureCounter > 5 && 
           account.FailedAuthInfo.LastAttempt > DateTime.Now.AddMinutes(5))  {
            // change last attempt value and save
            return new Result<TokenDataDto>(new AppException("Account is blocked, try again in 5 minutes", 401));
        }     
        
        if(_passwordHasher.VerifyHashedPassword(account.Email, account.Password, credentials.Password) != PasswordVerificationResult.Success){
            // update failed attempt on email
            return new Result<TokenDataDto>(new AppException("Invalid email or password", 401));
        }

        return new Result<TokenDataDto>(new TokenDataDto());
    }
}