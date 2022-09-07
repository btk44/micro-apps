using Microsoft.AspNetCore.Mvc;
using IdentityService.Application.Authorization;
using IdentityService.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using IdentityService.Application.Common.Constants;

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
    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenDataDto>> RefreshToken([FromBody] string refreshToken)
    {
        var command = new RefreshTokenCommand() { 
            RefreshToken = refreshToken,
            AccountId = Convert.ToInt32(TokenService.GetClaimFromToken(User, Claims.AccountId))
        };
        var result = await Mediator.Send(command);
        return result.Match<ActionResult>(
            tokenData => Ok(tokenData),
            exception => Unauthorized(exception.Message)
        ); 
    }
}
