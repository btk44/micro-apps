using Microsoft.AspNetCore.Mvc;
using IdentityService.Application.Authorization;
using IdentityService.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Shared.Api;
using Shared.Constants;

namespace IdentityService.Api;

[ApiController]
[Route("api/[controller]")]
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
    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenDataDto>> RefreshToken([FromBody] string refreshToken)
    {
        var command = new RefreshTokenCommand() { 
            RefreshToken = refreshToken,
            AccountId = Convert.ToInt32(GetClaimFromToken(User, Claims.AccountId))
        };
        var result = await Mediator.Send(command);
        return result.Match<ActionResult>(
            tokenData => Ok(tokenData),
            exception => Unauthorized(exception.Message)
        ); 
    }
}
