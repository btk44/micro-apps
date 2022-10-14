using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TransactionService.Application.Common.Models;
using TransactionService.Application.Accounts;
using Shared.Constants;
using Shared.Api;

namespace TransactionService.Api;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<AccountDto>> Get([FromQuery] int id){
        var accountId = Convert.ToInt32(GetClaimFromToken(User, Claims.AccountId));
        return await Mediator.Send(new GetAccountQuery(){ 
            OwnerId = accountId,
            Id = id
        });
    }

    [HttpPost("search")]
    public async Task<ActionResult<List<AccountDto>>> Search([FromBody] SearchAccountsCommand command){
        // possible upgrade: unlock searching objects for other owners
        command.OwnerId = Convert.ToInt32(GetClaimFromToken(User, Claims.AccountId));
        return await Mediator.Send(command);
    }

    [HttpPost]
    public async Task<ActionResult<AccountDto>> Create([FromBody] CreateAccountCommand command)
    {
        // possible upgrade: unlock creating objects for other owners
        command.OwnerId = Convert.ToInt32(GetClaimFromToken(User, Claims.AccountId));
        var result = await Mediator.Send(command);
        return result.Match<ActionResult>(
            account => Ok(account),
            exception => BadRequest(exception.Message)
        );
    }

    [HttpPut]
    public async Task<ActionResult<bool>> Update([FromBody] UpdateAccountCommand command)
    {
        // possible upgrade: unlock update objects for other owners
        command.OwnerId = Convert.ToInt32(GetClaimFromToken(User, Claims.AccountId));
        var result = await Mediator.Send(command);
        return result.Match<ActionResult>(
            success => Ok(),
            exception => BadRequest(exception.Message)
        );
    }

    [HttpDelete]
    public async Task<ActionResult<bool>> Delete([FromQuery] int accountId)
    {
        var ownerId = Convert.ToInt32(GetClaimFromToken(User, Claims.AccountId));
        var result = await Mediator.Send(new DeleteAccountCommand(){ Id = accountId, OwnerId = ownerId });
        return result.Match<ActionResult>(
            success => Ok(),
            exception => BadRequest(exception.Message)
        );
    }    
}
