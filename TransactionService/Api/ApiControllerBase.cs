using TransactionService.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender _mediator = null!;
    //private ITokenService _tokenService;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    //protected ITokenService TokenService => _tokenService ??= HttpContext.RequestServices.GetRequiredService<ITokenService>();
}