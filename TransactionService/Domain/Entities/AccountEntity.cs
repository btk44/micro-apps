using TransactionService.Domain.Common;

namespace TransactionService.Domain.Entities;

public class AccountEntity: BaseEntity {
    public int OwnerId { get; set; }
    public double Amount { get; set; }
    public string Name { get; set; }
    public string CurrencyId { get; set; }
}