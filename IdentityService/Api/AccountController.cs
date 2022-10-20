using Microsoft.AspNetCore.Mvc;
using IdentityService.Application.Accounts;
using IdentityService.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using IdentityService.Application.Common.Constants;

namespace IdentityService.Api;

[ApiController]
[Route("api/[controller]")]
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

    [Authorize]
    [HttpPut("update")]
    public async Task<ActionResult<bool>> Update([FromBody] UpdateAccountCommand command)
    {
        var accountId = Convert.ToInt32(GetClaimFromToken(User, Claims.AccountId));
        var result = await Mediator.Send(command);
        return result.Match<ActionResult>(
            success => Ok(),
            exception => BadRequest(exception.Message)
        );
    }

    [Authorize]
    [HttpDelete("delete")]
    public async Task<ActionResult<bool>> Delete([FromQuery] int accountId)
    {
        var tokenAccountId = Convert.ToInt32(GetClaimFromToken(User, Claims.AccountId));
        if(tokenAccountId != accountId){
            return BadRequest();
        }

        var result = await Mediator.Send(new DeleteAccountCommand(){ AccountId = accountId });
        return result.Match<ActionResult>(
            success => Ok(),
            exception => BadRequest(exception.Message)
        );
    }

    
    [HttpPost("reset-password-request")]
    public async Task<ActionResult<bool>> ResetPasswordRequest([FromBody] string email)
    {
        var result = await Mediator.Send(new ResetPasswordRequestCommand(){ Email = email });
        return result.Match<ActionResult>(
            success => Ok(),
            exception => BadRequest(exception.Message)
        );
    }
    
    [HttpPost("reset-password")]
    public async Task<ActionResult<bool>> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Match<ActionResult>(
            success => Ok(),
            exception => BadRequest(exception.Message)
        );
    }
    
}
