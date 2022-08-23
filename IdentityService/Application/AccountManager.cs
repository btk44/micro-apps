using IdentityService.Common;
using IdentityService.Database;
using IdentityService.DataObjects;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application;

public class AccountManager {
    private DatabaseContext _dbContext;
    private AccountValidator _accountValidator;

    public AccountManager(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
        _accountValidator = new AccountValidator();
    }

    public  async Task<Result<TokenDataDto>> Login(LoginCredentialsDto credentials){

        if(!_accountValidator.IsDataProvided(credentials.Email, credentials.Password)){
            return new Result<TokenDataDto>(new AppException("Empty email or password"));
        }

        if(!_accountValidator.IsEmailValid(credentials.Email)){
            return new Result<TokenDataDto>(new AppException("Incorrect email format"));
        }

        var account = await _dbContext.Accounts
                                .Include(x => x.FailedAuthInfo)
                                .FirstOrDefaultAsync(x => x.Active && x.Email == credentials.Email);

        if(account == null){
            return new Result<TokenDataDto>(new AppException("Invalid email or password"));
        }

        if(_accountValidator.IsAccountBlocked(account))  {
            // change last attempt and counter values and save
            return new Result<TokenDataDto>(new AppException("Account is blocked, try again in 5 minutes"));
        }     
        
        // password policy?
        if(!_accountValidator.IsPasswordValid(account, credentials.Password)){
            // update failed attempt on email
            return new Result<TokenDataDto>(new AppException("Invalid email or password"));
        }

        return new Result<TokenDataDto>(new TokenDataDto());
    }
}