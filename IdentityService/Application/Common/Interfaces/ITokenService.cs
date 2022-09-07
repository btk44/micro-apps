using System.Security.Claims;
using IdentityService.Application.Common.Models;

namespace IdentityService.Application.Common.Interfaces;

public interface ITokenService
{
    TokenDataDto CreateTokenData(Dictionary<string, string> tokenClaims);
    string GenerateSimpleToken();
    string GetClaimFromToken(ClaimsPrincipal claimPrincipal, string claimName);
}