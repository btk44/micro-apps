using IdentityService.Domain.Common;

namespace IdentityService.Domain.Entities;

public class RefreshTokenEntity : BaseEntity {
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }


    public int AccountId { get; set; }
    public AccountEntity Account { get; set; }
}