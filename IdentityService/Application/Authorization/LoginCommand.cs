using IdentityService.Application.Common;
using IdentityService.Application.Dtos;
using IdentityService.Application.Exceptions;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Authorization;

public class LoginCommand: IRequest<Result<TokenDataDto>> {
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginCommandHandler: IRequestHandler<LoginCommand, Result<TokenDataDto>> {
    private IApplicationDbContext _dbContext;
    private AuthValidator _authValidator;

    public LoginCommandHandler(IApplicationDbContext dbContext){
        _dbContext = dbContext;
        _authValidator = new AuthValidator();
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
        return new Result<TokenDataDto>(new TokenDataDto());
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