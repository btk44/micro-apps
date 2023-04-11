using TransactionService.Domain.Common.Entities;

namespace TransactionService.Domain.Entities;

public class CategoryGroupEntity: BaseEntity {
    public int OwnerId { get; set; }
    public string Name { get; set; }
    public ICollection<CategoryEntity> Categories { get; set; }    
}