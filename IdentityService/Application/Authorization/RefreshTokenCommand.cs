using IdentityService.Application.Common.Tools;
using IdentityService.Application.Common.Exceptions;
using IdentityService.Application.Common.Interfaces;
using IdentityService.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using IdentityService.Application.Common.Constants;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Authorization;

public class RefreshTokenCommand: IRequest<Result<TokenDataDto>> {
    public string RefreshToken { get; set; }
    public int AccountId { get; set; } 
}

public class RefreshTokenCommandHandler: IRequestHandler<RefreshTokenCommand, Result<TokenDataDto>> {
    private IApplicationDbContext _dbContext;
    private AuthValidator _authValidator;
    private ITokenService _tokenService;
    private AuthHelper _authHelper;

    public RefreshTokenCommandHandler(IApplicationDbContext dbContext, ITokenService tokenService){
        _dbContext = dbContext;
        _authValidator = new AuthValidator();
        _tokenService = tokenService;
        _authHelper = new AuthHelper(dbContext);
    }

    public async Task<Result<TokenDataDto>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken){
        if (string.IsNullOrEmpty(command.RefreshToken)){
            return new Result<TokenDataDto>(new AuthException("No token provided"));
        }

        var account = await _dbContext.Accounts
            .Include(x => x.FailedAuthInfo)
            .Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.Active && x.Id == command.AccountId);

        if(account == null){
            await _authHelper.UpdateFailedAuthAttemptAndSave(account, true);
            return new Result<TokenDataDto>(new AuthException("Account does not exist"));
        }

        if(_authValidator.IsAccountBlocked(account))  {
            await _authHelper.UpdateFailedAuthAttemptAndSave(account, true);
            return new Result<TokenDataDto>(new AuthException("Account is blocked, try again in 5 minutes"));
        }     

        var oldRefreshToken = account.RefreshTokens.FirstOrDefault(x => x.Active && x.Token == command.RefreshToken);

        if(oldRefreshToken == null || oldRefreshToken.ExpiresAt < DateTime.UtcNow){
            await _authHelper.UpdateFailedAuthAttemptAndSave(account, true);
            return new Result<TokenDataDto>(new AuthException("Token is invalid"));
        }

        // do not reset failed attempts here - only in login

        var claims = new Dictionary<string, string>() {
            { Claims.AccountId, account.Id.ToString() }
        };
        var tokenData = _tokenService.CreateTokenData(claims);

        account.RefreshTokens.Add(new RefreshTokenEntity(){
            Token = tokenData.RefreshToken,
            ExpiresAt = tokenData.ExpirationTime
        });
        
        oldRefreshToken.Active = false;
        _authHelper.CleanupOldTokens(account);

        await _dbContext.SaveChangesAsync();
        return new Result<TokenDataDto>(tokenData);
    }
}