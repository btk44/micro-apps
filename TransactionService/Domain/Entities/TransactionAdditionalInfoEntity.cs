using TransactionService.Domain.Common.Entities;

public class TransactionAdditionalInfoEntity: BaseEntity {
    public string Payee { get; set; }
    public string Comment { get; set; }
}