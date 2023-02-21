using TransactionService.Domain.Common.Entities;

namespace TransactionService.Domain.Entities;

public class AccountEntity: BaseEntity {
    public int OwnerId { get; set; }
    public string Name { get; set; }
    public int CurrencyId { get; set; }

    // navigation props
    public CurrencyEntity Currency { get; set; }
    public ICollection<TransactionEntity> Transactions { get; set; }
    public AccountAdditionalInfoEntity AdditionalInfo { get; set; }
}