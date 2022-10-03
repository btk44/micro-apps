using TransactionService.Domain.Common;

namespace TransactionService.Domain.Entities;

public class TransactionEntity: BaseEntity {
    public int OwnerId { get; set; }
    public DateTime Date { get; set; }
    public int AccountId { get; set; }
    public AccountEntity Account { get; set; }
    public double Amount { get; set; }
    public string Payee { get; set; }
    public int CategoryId { get; set; }
    public CategoryEntity Category { get; set; }
    public string Comment { get; set; }
}