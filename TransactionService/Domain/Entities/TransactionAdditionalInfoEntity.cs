using TransactionService.Domain.Common.Entities;
using TransactionService.Domain.Entities;

public class TransactionAdditionalInfoEntity: BaseEntity {
    public string Payee { get; set; }
    public string Comment { get; set; }
    public int TransactionId { get; set; }
    public TransactionEntity Transaction { get; set; }
}