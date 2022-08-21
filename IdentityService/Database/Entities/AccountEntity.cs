using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityService.Database.Entities;

[Table("Account")]
public class AccountEntity: BaseEntity {
    public string Email { get; set; }
    public string Password { get; set; }

    // Navigation properties
    public int FailedAuthInfoId { get; set; }
    public FailedAuthInfoEntity FailedAuthInfo { get; set; }
    public List<RefreshTokenEntity> RefreshTokens { get; set; }
}