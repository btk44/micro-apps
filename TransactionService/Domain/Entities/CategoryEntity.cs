using TransactionService.Domain.Common.Entities;

namespace TransactionService.Domain.Entities;

public class CategoryEntity: BaseEntity {
    public int OwnerId { get; set; }
    public string Name { get; set; }
    public int CategoryGroupId { get; set; }
    public CategoryGroupEntity CategoryGroup { get; set; }
}