using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransactionService.Application.Common.Interfaces;
using TransactionService.Domain.Common.Entities;
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

        Build(modelBuilder.Entity<AccountEntity>());
        Build(modelBuilder.Entity<CurrencyEntity>());
        Build(modelBuilder.Entity<CategoryEntity>());
        Build(modelBuilder.Entity<TransactionEntity>());

        modelBuilder.Entity<CategoryEntity>().HasOne(x => x.VisualProperties);
        modelBuilder.Entity<CurrencyEntity>().HasOne(x => x.VisualProperties);
        modelBuilder.Entity<AccountEntity>().HasMany(x => x.Transactions).WithOne(x => x.Account);        
        modelBuilder.Entity<AccountEntity>().HasOne(x => x.Currency);
        modelBuilder.Entity<AccountEntity>().HasOne(x => x.VisualProperties);
        modelBuilder.Entity<AccountAdditionalInfoEntity>().HasOne(x => x.Account);
        modelBuilder.Entity<TransactionEntity>().HasOne(x => x.Account).WithMany(x => x.Transactions);
        modelBuilder.Entity<TransactionEntity>().HasOne(x => x.Category);
        modelBuilder.Entity<TransactionAdditionalInfoEntity>().HasOne(x => x.Transaction);
    }    

    private void Build<T>(EntityTypeBuilder<T> entity) where T : BaseEntity
    {
        entity.Property(e => e.Id).UseIdentityColumn();
        entity.Property(e => e.Active).IsRequired().HasDefaultValue(true);
        entity.Property(e => e.Created).IsRequired().HasDefaultValueSql("getdate()");
        entity.Property(e => e.CreatedBy).IsRequired().HasDefaultValue(-1);
        entity.Property(e => e.Modified).IsRequired().HasDefaultValueSql("getdate()").ValueGeneratedOnAddOrUpdate();
        entity.Property(e => e.ModifiedBy).IsRequired().HasDefaultValue(-1);
    }
}

