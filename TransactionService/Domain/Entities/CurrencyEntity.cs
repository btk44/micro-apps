using TransactionService.Domain.Common;

namespace TransactionService.Domain.Entities;

public class CurrencyEntity: BaseEntity {
    public string Description { get; set; }
    public string Code { get; set; }
}