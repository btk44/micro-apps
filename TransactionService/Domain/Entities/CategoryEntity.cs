using TransactionService.Domain.Common;

namespace TransactionService.Domain.Entities;

public class CategoryEntity: BaseEntity {
    public string Name { get; set; }
    public int ParentCategoryId { get; set; }
    public List<CategoryEntity> SubCategories { get; set; }
}