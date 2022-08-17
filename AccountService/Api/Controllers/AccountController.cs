using Microsoft.AspNetCore.Mvc;

namespace AccountService.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<string> Register()
    {
        return "works!";
    }

    [HttpPost("login")]
    public async Task<string> Login()
    {
        throw new Exception("not implemented");
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
