using IdentityService.Application.Common.Interfaces;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Authorization;

public class AuthHelper {
    private IApplicationDbContext _dbContext;

    public AuthHelper(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void UpdateFailedAuthAttempt(AccountEntity account, bool isFail){
        if (account.FailedAuthInfo == null){
            account.FailedAuthInfo = new FailedAuthInfoEntity();
        }

        account.FailedAuthInfo.FailureCounter = isFail ? account.FailedAuthInfo.FailureCounter+1 : 0;
        account.FailedAuthInfo.LastAttempt = DateTime.UtcNow;
    }

    public async Task UpdateFailedAuthAttemptAndSave(AccountEntity account, bool isFail){
        UpdateFailedAuthAttempt(account, isFail);
        await _dbContext.SaveChangesAsync();
    }

    public void CleanupOldTokens(AccountEntity account){
        var expiredTokens = account.RefreshTokens.Where(x => x.Active && x.ExpiresAt < DateTime.UtcNow);

        foreach(var token in expiredTokens){
            token.Active = false;
        }
    }
}