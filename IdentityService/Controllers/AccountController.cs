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
    private readonly AccountLogic _accountLogic;
    private readonly PasswordHasher<string> _passwordHasher;

    public AccountController(ILogger<AccountController> logger, DatabaseContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
        _accountLogic = new AccountLogic(dbContext);
    }

    [HttpPost("register")]
    public async Task<string> Register()
    {
        return "works!";
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCredentials credentials)
    {
        var result = await _accountLogic.Login(credentials);
        return result.Match<IActionResult>(
            tokenData => Ok(tokenData),
            exception => BadRequest(exception.Message)
        ); 
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
