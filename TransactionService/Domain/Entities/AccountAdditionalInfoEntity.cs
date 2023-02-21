using TransactionService.Domain.Common.Entities;
using TransactionService.Domain.Entities;

public class AccountAdditionalInfoEntity: BaseEntity {
    public double Amount { get; set; }
    public int AccountId { get; set; }
    public AccountEntity Account { get; set; }
}