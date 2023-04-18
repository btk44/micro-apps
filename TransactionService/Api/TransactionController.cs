using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TransactionService.Application.Common.Models;
using TransactionService.Application.Transactions;
using Shared.Constants;
using Shared.Api;

namespace TransactionService.Api;

//[Authorize]
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
        // possible upgrade: unlock searching objects for other owners
        // command.OwnerId = Convert.ToInt32(GetClaimFromToken(User, Claims.AccountId));
        return await Mediator.Send(command);
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> ProcessTransactions([FromBody] ProcessTransactionsCommand command)
    {
        // possible upgrade: unlock creating objects for other owners
        command.ProcessingUserId = Convert.ToInt32(GetClaimFromToken(User, Claims.AccountId));
        var result = await Mediator.Send(command);
        return result.Match<ActionResult>(
            transaction => Ok(transaction),
            exception => BadRequest(exception.Message)
        );
    }  
}
