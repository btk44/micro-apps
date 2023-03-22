using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Api;

public abstract class ApiControllerBase : ControllerBase
{
    private ISender _mediator = null!;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected string GetClaimFromToken(ClaimsPrincipal claimsPrincipal, string claimType){
        var claim = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == claimType);

        if(claim != null){
            return claim.Value;
        }

        return string.Empty;
    }
}