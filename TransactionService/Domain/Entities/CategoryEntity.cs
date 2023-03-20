using TransactionService.Domain.Common.Entities;

namespace TransactionService.Domain.Entities;

public class CategoryEntity: BaseEntity {
    public int OwnerId { get; set; }
    public string Name { get; set; }
    public string CategoryGroupName { get; set; }
}