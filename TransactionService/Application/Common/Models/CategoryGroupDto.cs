namespace TransactionService.Application.Common.Models;

public class CategoryGroupDto {
    public int OwnerId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Active { get; set; }
    public List<CategoryDto> Categories { get; set; }
}