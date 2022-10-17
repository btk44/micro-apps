using Shared.Entities;

namespace TransactionService.Domain.Entities;

public class CategoryEntity: BaseEntity {
    public int OwnerId { get; set; }
    public string Name { get; set; }
    public int? ParentCategoryId { get; set; }
    public CategoryEntity ParentCategory { get; set; }
    public ICollection<CategoryEntity> SubCategories { get; } = new List<CategoryEntity>();
}