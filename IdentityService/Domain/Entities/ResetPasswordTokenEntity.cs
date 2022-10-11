using Shared.Entities;

namespace IdentityService.Domain.Entities;

public class ResetPasswordTokenEntity : BaseEntity {
    public string Token { get; set; }
    public int AccountId { get; set; }
    public AccountEntity Account { get; set; }
}