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

    [HttpGet("path1")]
    public async Task<string> Get1()
    {
        return "works!";
    }

    [HttpGet("path2")]
    public async Task<string> Get2()
    {
        throw new Exception("error handler should work and you should not see a callstack");
        return "works!";
    }
}
