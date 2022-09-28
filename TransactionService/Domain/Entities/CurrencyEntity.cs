using TransactionService.Domain.Common;

namespace TransactionService.Domain.Entities;

public class CurrencyEntity: BaseEntity {
    public string Code { get; set; }
}