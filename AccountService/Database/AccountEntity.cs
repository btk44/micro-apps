using System.ComponentModel.DataAnnotations.Schema;

namespace AccountService.Database;

[Table("Account")]
public class AccountEntity: BaseEntity {
    public string Email { get; set; }
    public string Password { get; set; }
}