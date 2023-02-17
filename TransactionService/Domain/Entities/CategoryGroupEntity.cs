using TransactionService.Domain.Common.Entities;
using TransactionService.Domain.Entities;

public class CategoryGroup: BaseEntity{
    public string Name { get; set; }

    // navigation props
    public ICollection<CategoryEntity> Categories { get; set; }
}