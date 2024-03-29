namespace TransactionService.Application.Common.Models;

public class AccountDto {
    public int OwnerId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public double Amount { get; set; }
    public int CurrencyId { get; set; }
    public bool Active { get; set; }
}