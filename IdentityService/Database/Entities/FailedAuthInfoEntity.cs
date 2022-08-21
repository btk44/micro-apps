using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityService.Database.Entities;

[Table("FailedAuthInfo")]
public class FailedAuthInfoEntity : BaseEntity {
    public int FailureCounter { get; set; }
    public DateTime LastAttempt { get; set; }

    // Navigation properties
    public int AccountId { get; set; }
    public AccountEntity Account { get; set; }
}