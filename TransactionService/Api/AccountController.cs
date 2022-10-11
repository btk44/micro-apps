using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TransactionService.Application.Common.Models;
using TransactionService.Application.Accounts;
using Shared.Constants;
using Shared.Api;

namespace TransactionService.Api;

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

        var result = await Mediator.Send(new DeleteAccountCommand(){ Id = accountId });
        return result.Match<ActionResult>(
            success => Ok(),
            exception => BadRequest(exception.Message)
        );
    }    
}
