using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Domain.Entities;

namespace TransactionService.Infrastructure;
public class ApplicationDbContext : DbContext, IApplicationDbContext{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options){ }
    public DbSet<AccountEntity> Accounts { get; set;} 
    public DbSet<CurrencyEntity> Currencies { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<TransactionEntity> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) { 
        base.OnModelCreating(modelBuilder);

        BaseEntityBuilder.Build(modelBuilder.Entity<AccountEntity>());
        BaseEntityBuilder.Build(modelBuilder.Entity<CurrencyEntity>());
        BaseEntityBuilder.Build(modelBuilder.Entity<CategoryEntity>());
        BaseEntityBuilder.Build(modelBuilder.Entity<TransactionEntity>());

        modelBuilder.Entity<AccountEntity>().HasMany(x => x.Transactions).WithOne(x => x.Account);        
        modelBuilder.Entity<AccountEntity>().HasOne(x => x.Currency);
        modelBuilder.Entity<TransactionEntity>().HasOne(x => x.Account).WithMany(x => x.Transactions);
        modelBuilder.Entity<TransactionEntity>().HasOne(x => x.Category);
        modelBuilder.Entity<CategoryEntity>().HasMany(x => x.SubCategories).WithOne(x => x.ParentCategory).OnDelete(DeleteBehavior.NoAction);
    }    
}

