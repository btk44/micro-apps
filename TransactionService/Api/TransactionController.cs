using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TransactionService.Application.Common.Models;
using TransactionService.Application.Transactions;
using Shared.Constants;
using Shared.Api;

namespace TransactionService.Api;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransactionController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<TransactionDto>> Get([FromQuery] int id){
        var accountId = Convert.ToInt32(GetClaimFromToken(User, Claims.AccountId));
        return await Mediator.Send(new GetTransactionQuery(){ 
            OwnerId = accountId, 
            Id = id
        });
    }

    [HttpPost("search")]
    public async Task<ActionResult<List<TransactionDto>>> Search([FromBody] SearchTransactionsCommand command){
        var accountId = Convert.ToInt32(GetClaimFromToken(User, Claims.AccountId));
        return await Mediator.Send(command);
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> Create([FromBody] CreateTransactionCommand command)
    {
        // to do: consider getting owner id from token 
        var result = await Mediator.Send(command);
        return result.Match<ActionResult>(
            transaction => Ok(transaction),
            exception => BadRequest(exception.Message)
        );
    }

    [HttpPut]
    public async Task<ActionResult<bool>> Update([FromBody] UpdateTransactionCommand command)
    {
        var accountId = Convert.ToInt32(GetClaimFromToken(User, Claims.AccountId));
        var result = await Mediator.Send(command);
        return result.Match<ActionResult>(
            success => Ok(),
            exception => BadRequest(exception.Message)
        );
    }

    [HttpDelete]
    public async Task<ActionResult<bool>> Delete([FromQuery] int accountId)
    {
        var tokenAccountId = Convert.ToInt32(GetClaimFromToken(User, Claims.AccountId));
        if(tokenAccountId != accountId){
            return BadRequest();
        }

        var result = await Mediator.Send(new DeleteTransactionCommand(){ Id = accountId });
        return result.Match<ActionResult>(
            success => Ok(),
            exception => BadRequest(exception.Message)
        );
    }    
}
