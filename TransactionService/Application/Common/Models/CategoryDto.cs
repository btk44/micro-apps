namespace TransactionService.Application.Common.Models;

public class CategoryDto {
    public int OwnerId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Active { get; set; }
}