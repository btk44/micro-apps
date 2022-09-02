using Microsoft.AspNetCore.Mvc;
using IdentityService.Application.Authorization;
using IdentityService.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;

namespace IdentityService.Api;

[ApiController]
public class AuthController : ApiControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<TokenDataDto>> Login([FromBody] LoginCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Match<ActionResult>(
            tokenData => Ok(tokenData),
            exception => Unauthorized(exception.Message)
        ); 
    }

    [Authorize]
    [HttpPost("refreshToken")]
    public async Task<ActionResult<TokenDataDto>> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Match<ActionResult>(
            tokenData => Ok(tokenData),
            exception => Unauthorized(exception.Message)
        ); 
    }
}
