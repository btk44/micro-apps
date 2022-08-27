using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Application.Authorization;

public class AuthValidator {
    private PasswordHasher<string> _passwordHasher;

    public AuthValidator()
    {
        _passwordHasher = new PasswordHasher<string>();
    }

    public bool IsPasswordValid(AccountEntity account, string providedPassword){
        return _passwordHasher.VerifyHashedPassword(account.Email, account.Password, providedPassword) == PasswordVerificationResult.Success;
    }

    public bool IsAccountBlocked(AccountEntity account){
        if(account.FailedAuthInfo == null){
            return false;
        }

        if (account.FailedAuthInfo.FailureCounter > 5){
            if(account.FailedAuthInfo.LastAttempt.AddMinutes(5) > DateTime.Now){
                return true;
            }
        }

        return false;
    }
}