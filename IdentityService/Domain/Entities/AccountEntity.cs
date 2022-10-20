
using Domain.Entities.Common;

namespace IdentityService.Domain.Entities;

public class AccountEntity: BaseEntity {
    public string Email { get; set; }
    public string Password { get; set; }


    public int FailedAuthInfoId { get; set; }
    public FailedAuthInfoEntity FailedAuthInfo { get; set; }
    public List<RefreshTokenEntity> RefreshTokens { get; set; }
    public List<ResetPasswordTokenEntity> ResetPasswordTokens { get; set; }
}