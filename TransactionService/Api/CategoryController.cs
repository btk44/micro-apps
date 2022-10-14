using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TransactionService.Application.Common.Models;
using TransactionService.Application.Categories;
using Shared.Constants;
using Shared.Api;

namespace TransactionService.Api;

[Authorize]
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
        command.OwnerId = Convert.ToInt32(GetClaimFromToken(User, Claims.AccountId));
        return await Mediator.Send(command);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryCommand command)
    {
        // possible upgrade: unlock creating objects for other owners
        command.OwnerId = Convert.ToInt32(GetClaimFromToken(User, Claims.AccountId));
        var result = await Mediator.Send(command);
        return result.Match<ActionResult>(
            category => Ok(category),
            exception => BadRequest(exception.Message)
        );
    }

    [HttpPut]
    public async Task<ActionResult<bool>> Update([FromBody] UpdateCategoryCommand command)
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
        var result = await Mediator.Send(new DeleteCategoryCommand(){ Id = accountId, OwnerId = ownerId });
        return result.Match<ActionResult>(
            success => Ok(),
            exception => BadRequest(exception.Message)
        );
    }    
}
