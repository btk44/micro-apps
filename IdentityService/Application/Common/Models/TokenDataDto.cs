namespace IdentityService.Application.Common.Models;

public class TokenDataDto{
    public string Token { get; set; } 
    public string RefreshToken { get; set; }
    public DateTime ExpirationTime { get; set; }
    public DateTime RefreshExpirationTime { get; set; }
}