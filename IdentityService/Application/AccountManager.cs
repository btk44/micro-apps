using AutoMapper;
using IdentityService.Common;
using IdentityService.Application.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityService.Domain.Entities;
using IdentityService.Application.Interfaces;

namespace IdentityService.Application;

public class AccountManager {
    private IApplicationDbContext _dbContext;
    private AccountValidator _accountValidator;
    private PasswordHasher<string> _passwordHasher;
    private IMapper _accountMapper;

    public AccountManager(IApplicationDbContext dbContext, IMapper accountMapper)
    {
        _dbContext = dbContext;
        _accountValidator = new AccountValidator();
        _passwordHasher = new PasswordHasher<string>();
        _accountMapper = accountMapper;
    }

    public async Task<Result<TokenDataDto>> Login(LoginCredentialsDto credentials){

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
            await UpdateFailedAuthAttempt(account, true);
            return new Result<TokenDataDto>(new AppException("Account is blocked, try again in 5 minutes"));
        }     
        
        if(!_accountValidator.IsPasswordValid(account, credentials.Password)){
            await UpdateFailedAuthAttempt(account, true);
            return new Result<TokenDataDto>(new AppException("Invalid email or password"));
        }

        await UpdateFailedAuthAttempt(account, false);
        return new Result<TokenDataDto>(new TokenDataDto());
    }

    public async Task<Result<AccountDto>> Register(AccountDto accountDto){
        if(!_accountValidator.IsDataProvided(accountDto.Email, accountDto.Password)){
            return new Result<AccountDto>(new AppException("Empty email or password"));
        }

        if(!_accountValidator.IsEmailValid(accountDto.Email)){
            return new Result<AccountDto>(new AppException("Incorrect email format"));
        }

        if(!_accountValidator.IsPasswordSecure(accountDto.Password)){
            return new Result<AccountDto>(new AppException("Password does not fulfill requirements: [to do]"));
        }

        if(await _dbContext.Accounts.AnyAsync(x => x.Active && x.Email == accountDto.Email)){
            return new Result<AccountDto>(new AppException("Account with this email already exists"));
        };

        var accountEntity = new AccountEntity(){
            Email = accountDto.Email,
            Password = _passwordHasher.HashPassword(accountDto.Email, accountDto.Password)
        };

        _dbContext.Accounts.Add(accountEntity);      

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new Result<AccountDto>(new AppException("Save error - please try again"));
        }

        accountDto = _accountMapper.Map<AccountDto>(accountEntity);

        return new Result<AccountDto>(accountDto);
    }

    private async Task UpdateFailedAuthAttempt(AccountEntity account, bool isFail){
        if (account.FailedAuthInfo == null){
            account.FailedAuthInfo = new FailedAuthInfoEntity();
        }

        account.FailedAuthInfo.FailureCounter = isFail ? account.FailedAuthInfo.FailureCounter+1 : 0;
        account.FailedAuthInfo.LastAttempt = DateTime.Now;

        await _dbContext.SaveChangesAsync();
    }
}