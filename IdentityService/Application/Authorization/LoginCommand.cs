using IdentityService.Application.Common.Tools;
using IdentityService.Application.Common.Exceptions;
using IdentityService.Application.Common.Interfaces;
using IdentityService.Application.Common.Models;
using IdentityService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using IdentityService.Application.Common.Constants;

namespace IdentityService.Application.Authorization;

public class LoginCommand: IRequest<Result<TokenDataDto>> {
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginCommandHandler: IRequestHandler<LoginCommand, Result<TokenDataDto>> {
    private IApplicationDbContext _dbContext;
    private AuthValidator _authValidator;
    private ITokenService _tokenService;

    public LoginCommandHandler(IApplicationDbContext dbContext, ITokenService tokenService){
        _dbContext = dbContext;
        _authValidator = new AuthValidator();
        _tokenService = tokenService;
    }

    public async Task<Result<TokenDataDto>> Handle(LoginCommand command, CancellationToken cancellationToken){
        var account = await _dbContext.Accounts
                                .Include(x => x.FailedAuthInfo)
                                .FirstOrDefaultAsync(x => x.Active && x.Email == command.Email);

        if(account == null){
            return new Result<TokenDataDto>(new AppException("Invalid email or password"));
        }

        if(_authValidator.IsAccountBlocked(account))  {
            await UpdateFailedAuthAttempt(account, true);
            return new Result<TokenDataDto>(new AppException("Account is blocked, try again in 5 minutes"));
        }     
        
        if(!_authValidator.IsPasswordValid(account, command.Password)){
            await UpdateFailedAuthAttempt(account, true);
            return new Result<TokenDataDto>(new AppException("Invalid email or password"));
        }

        await UpdateFailedAuthAttempt(account, false);

        var claims = new Dictionary<string, string>() {
            { Claims.UserName, account.Email },
            { Claims.UserId, account.Id.ToString() }
        };

        // save refresh token
        
        return new Result<TokenDataDto>(_tokenService.CreateTokenData(claims));
    }

    private async Task UpdateFailedAuthAttempt(AccountEntity account, bool isFail){  // to do: move it swhere else
        if (account.FailedAuthInfo == null){
            account.FailedAuthInfo = new FailedAuthInfoEntity();
        }

        account.FailedAuthInfo.FailureCounter = isFail ? account.FailedAuthInfo.FailureCounter+1 : 0;
        account.FailedAuthInfo.LastAttempt = DateTime.Now;

        await _dbContext.SaveChangesAsync();
    }
}