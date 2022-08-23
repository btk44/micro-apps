using System.Globalization;
using System.Text.RegularExpressions;
using IdentityService.Database.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Application;

public class AccountValidator {
    private PasswordHasher<string> _passwordHasher;

    public AccountValidator()
    {
        _passwordHasher = new PasswordHasher<string>();
    }

    public bool IsDataProvided(string email, string password){
        return !string.IsNullOrEmpty(email) && 
               !string.IsNullOrEmpty(password);
    }

    public bool IsEmailValid(string email){
        // code from: https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
        try
        {
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));
            string DomainMapper(Match match)
            {
                var idn = new IdnMapping();
                string domainName = idn.GetAscii(match.Groups[2].Value);
                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
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