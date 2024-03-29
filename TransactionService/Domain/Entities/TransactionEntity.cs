using TransactionService.Domain.Common.Entities;

namespace TransactionService.Domain.Entities;

public class TransactionEntity: BaseEntity {
    public int OwnerId { get; set; }
    public DateTime Date { get; set; }
    public int AccountId { get; set; }
    public double Amount { get; set; }
    public int CategoryId { get; set; }
    public string GroupKey { get; set; }
    public string Payee { get; set; }
    public string Comment { get; set; }

    // navigation props
    public AccountEntity Account { get; set; }
    public CategoryEntity Category { get; set; }
}