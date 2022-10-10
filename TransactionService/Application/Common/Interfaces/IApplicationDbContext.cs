using TransactionService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TransactionService.Application.Common.Interfaces;

public interface IApplicationDbContext{
    public DbSet<AccountEntity> Accounts { get; } 
    public DbSet<TransactionEntity> Transactions { get; }
    public DbSet<CategoryEntity> Categories { get; }
    public DbSet<CurrencyEntity> Currencies { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}