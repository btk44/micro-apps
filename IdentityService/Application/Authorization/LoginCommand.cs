using IdentityService.Application.Common.Constants;
using IdentityService.Application.Common.Exceptions;
using IdentityService.Application.Common.Interfaces;
using IdentityService.Application.Common.Models;
using IdentityService.Application.Common.Tools;
using IdentityService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Authorization;

public class LoginCommand: IRequest<Either<TokenDataDto, AuthException>> {
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginCommandHandler: IRequestHandler<LoginCommand, Either<TokenDataDto, AuthException>> {
    private IApplicationDbContext _dbContext;
    private AuthValidator _authValidator;
    private ITokenService _tokenService;
    private AuthHelper _authHelper;

    public LoginCommandHandler(IApplicationDbContext dbContext, ITokenService tokenService){
        _dbContext = dbContext;
        _authValidator = new AuthValidator();
        _tokenService = tokenService;
        _authHelper = new AuthHelper(dbContext);
    }

    public async Task<Either<TokenDataDto, AuthException>> Handle(LoginCommand command, CancellationToken cancellationToken){
        var account = await _dbContext.Accounts
                                .Include(x => x.FailedAuthInfo)
                                .Include(x => x.RefreshTokens)
                                .FirstOrDefaultAsync(x => x.Active && x.Email == command.Email);

        if(account == null){
            return new AuthException("Invalid email or password");
        }

        if(_authValidator.IsAccountBlocked(account))  {
            await _authHelper.UpdateFailedAuthAttemptAndSave(account, true);
            return new AuthException("Account is blocked, try again in 5 minutes");
        }     
        
        if(!_authValidator.IsPasswordValid(account, command.Password)){
            await _authHelper.UpdateFailedAuthAttemptAndSave(account, true);
            return new AuthException("Invalid email or password");
        }

        _authHelper.UpdateFailedAuthAttempt(account, false);

        var claims = new Dictionary<string, string>() {
            { Claims.AccountId, account.Id.ToString() }
        };

        var tokenData = _tokenService.CreateTokenData(claims);

        account.RefreshTokens.Add(new RefreshTokenEntity(){
            Token = tokenData.RefreshToken,
            ExpiresAt = tokenData.ExpirationTime
        });

        _authHelper.CleanupOldTokens(account);

        await _dbContext.SaveChangesAsync();
        
        return tokenData;
    }
}