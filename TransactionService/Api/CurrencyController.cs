using Microsoft.AspNetCore.Mvc;
using TransactionService.Application.Common.Models;
using Shared.Api;
using TransactionService.Application.Currencies;

namespace TransactionService.Api;

//[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CurrencyController : ApiControllerBase
{
    [HttpPost("search")]
    public async Task<ActionResult<List<CurrencyDto>>> Search([FromBody] SearchCurrenciesCommand command){
        return await Mediator.Send(command);
    }
}
