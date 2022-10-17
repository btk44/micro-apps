namespace TransactionService.Application.Common.Models;

public class CategoryDto {
    public int OwnerId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public int ParentCategoryId { get; set; }
    public List<CategoryDto> SubCategories { get; set; }
}