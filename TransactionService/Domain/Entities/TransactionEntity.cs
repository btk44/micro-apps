using TransactionService.Domain.Common.Entities;

namespace TransactionService.Domain.Entities;

public class TransactionEntity: BaseEntity {
    public int OwnerId { get; set; }
    public DateTime Date { get; set; }
    public int AccountId { get; set; }
    public double Amount { get; set; }
    public int CategoryId { get; set; }

    // navigation props
    public AccountEntity Account { get; set; }
    public CategoryEntity Category { get; set; }
    public TransactionAdditionalInfoEntity AdditionalInfo { get; set;}
    public ICollection<TransactionEntity> ConnectedTransactions { get; set; }
    public ICollection<TransactionEntity> TransactionsImConnectedTo { get; set; }

}