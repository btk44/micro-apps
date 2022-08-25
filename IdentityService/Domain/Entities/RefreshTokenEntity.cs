using System.ComponentModel.DataAnnotations.Schema;
using IdentityService.Domain.Common;

namespace IdentityService.Domain.Entities;

[Table("RefreshToken")]
public class RefreshTokenEntity : BaseEntity {
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }

    // Navigation properties
    public int AccountId { get; set; }
    public AccountEntity Account { get; set; }
}