using AutoMapper;
using IdentityService.Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using IdentityService.Application.Interfaces;
using IdentityService.Application.Accounts;
using IdentityService.Application.Authorization;

namespace IdentityService.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public AccountController(ILogger<AccountController> logger, IApplicationDbContext dbContext,
        IMapper mapper)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateAccountCommand command)
    {
        var createAccountHandler = new CreateAccountCommandHandler(_dbContext, _mapper); // to do: this is not the way we want it
        var result = await createAccountHandler.Handle(command);
        return result.Match<IActionResult>(
            account => Ok(account),
            exception => BadRequest(exception.Message)
        );
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var loginHandler = new LoginCommandHandler(_dbContext);
        var result = await loginHandler.Handle(command);
        return result.Match<IActionResult>(
            tokenData => Ok(tokenData),
            exception => Unauthorized(exception.Message)
        ); 
    }

    /*
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
    */
}
