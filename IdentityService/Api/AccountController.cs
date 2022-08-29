using Microsoft.AspNetCore.Mvc;
using IdentityService.Application.Accounts;
using IdentityService.Application.Common.Models;

namespace IdentityService.Api;

[ApiController]
public class AccountController : ApiControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AccountDto>> Create([FromBody] CreateAccountCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Match<ActionResult>(
            account => Ok(account),
            exception => BadRequest(exception.Message)
        );
    }

    /*

    [HttpPut("update")]
    public async Task<string> Update()
    {
        throw new Exception("not implemented");
    }

    [HttpDelete("delete")]
    public async Task<string> Delete()
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
