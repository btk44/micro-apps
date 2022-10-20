using Domain.Entities.Common;

namespace IdentityService.Domain.Entities;

public class FailedAuthInfoEntity : BaseEntity {
    public int FailureCounter { get; set; }
    public DateTime LastAttempt { get; set; }


    public int AccountId { get; set; }
    public AccountEntity Account { get; set; }
}