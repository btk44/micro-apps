using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TransactionService.Application.Common.Models;
using TransactionService.Application.Categories;
using Shared.Constants;
using Shared.Api;

namespace TransactionService.Api;

//[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoryController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CategoryDto>> Get([FromQuery] int id){
        var accountId = Convert.ToInt32(GetClaimFromToken(User, Claims.AccountId));
        return await Mediator.Send(new GetCategoryQuery(){ 
            OwnerId = accountId, 
            Id = id
        });
    }

    [HttpPost("search")]
    public async Task<ActionResult<List<CategoryDto>>> Search([FromBody] SearchCategoriesCommand command){
        // possible upgrade: unlock searching objects for other owners
        // command.OwnerId = Convert.ToInt32(GetClaimFromToken(User, Claims.AccountId));
        return await Mediator.Send(command);
    }

    [HttpPost]
    public async Task<ActionResult<List<CategoryDto>>> Create([FromBody] ProcessCategoriesCommand command)
    {
        // possible upgrade: unlock creating objects for other owners
        command.ProcessingUserId = Convert.ToInt32(GetClaimFromToken(User, Claims.AccountId));
        var result = await Mediator.Send(command);
        return result.Match<ActionResult>(
            account => Ok(account),
            exception => BadRequest(exception.Message)
        );
    } 
}
