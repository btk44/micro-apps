namespace IdentityService.Application.Dtos;

public class TokenDataDto{
    public string Token { get; set; } 
    public string RefreshToken { get; set; }
    public DateTime ExpirationTime { get; set; }
    public DateTime RefreshExpirationTime { get; set; }
}