using IdentityService.Database;
using IdentityService.DataObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly DatabaseContext _dbContext;
    private readonly PasswordHasher<string> _passwordHasher;

    public AccountController(ILogger<AccountController> logger, DatabaseContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
        _passwordHasher = new PasswordHasher<string>();
    }

    [HttpPost("register")]
    public async Task<string> Register()
    {
        return "works!";
    }

    [HttpPost("login")]
    public async Task<TokenData> Login([FromBody] LoginCredentials credentials)
    {
        if(string.IsNullOrEmpty(credentials.Email) || string.IsNullOrEmpty(credentials.Password)){
            throw new AppException("Invalid email or password", 401);
            // insert failed attempt if email exists
        }

        var account = await _dbContext.Accounts
                                        .Include(x => x.FailedAuthInfo)
                                        .FirstOrDefaultAsync(x => x.Active && x.Email == credentials.Email);

        if(account == null){
            throw new AppException("Invalid email or password", 401);
        }

        if(account.FailedAuthInfo != null && 
           account.FailedAuthInfo.FailureCounter > 5 && 
           account.FailedAuthInfo.LastAttempt > DateTime.Now.AddMinutes(5))  {
            throw new AppException("Account is blocked, try again in 5 minutes", 401);
            // change last attempt value and save
        }     
        
        if(_passwordHasher.VerifyHashedPassword(account.Email, account.Password, credentials.Password) != PasswordVerificationResult.Success){
            throw new AppException("Invalid email or password", 401);
            // update failed attempt on email
        }

        return new TokenData();
    }

    [HttpPost("refreshToken")]
    public async Task<string> RefreshToken()
    {
        throw new Exception("not implemented");
    }

    [HttpDelete("delete")]
    public async Task<string> Delete()
    {
        throw new Exception("not implemented");
    }

    [HttpPut("update")]
    public async Task<string> Update()
    {
        throw new Exception("not implemented");
    }

    [HttpPost("resetPasswordRequest")]
    public async Task<string> ResetPasswordRequest()
    {
        throw new Exception("not implemented");
    }

    [HttpPost("resetPassword")]
    public async Task<string> ResetPassword()
    {
        throw new Exception("not implemented");
    }
}
