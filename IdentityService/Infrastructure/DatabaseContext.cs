
using IdentityService.Domain.Common;
using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityService.Infrastructure;
public class DatabaseContext : DbContext{
    public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options){ }
    public DbSet<AccountEntity> Accounts { get; set;} 
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    public DbSet<FailedAuthInfoEntity> FailedAuthInfos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) { 
        base.OnModelCreating(modelBuilder);

        BaseBuilder(modelBuilder.Entity<AccountEntity>());
        BaseBuilder(modelBuilder.Entity<RefreshTokenEntity>());
        BaseBuilder(modelBuilder.Entity<FailedAuthInfoEntity>());

        modelBuilder.Entity<AccountEntity>().HasMany(x => x.RefreshTokens).WithOne(x => x.Account);        
        modelBuilder.Entity<AccountEntity>().HasOne(x => x.FailedAuthInfo).WithOne(x => x.Account).HasForeignKey<FailedAuthInfoEntity>(x=>x.AccountId);        
    }

    protected void BaseBuilder<T>(EntityTypeBuilder<T> entity) where T : BaseEntity
    {
        entity.Property(e => e.Id).UseIdentityColumn();
        entity.Property(e => e.Active).IsRequired().HasDefaultValue(true);
        entity.Property(e => e.Created).IsRequired().HasDefaultValueSql("getdate()");
        entity.Property(e => e.CreatedBy).IsRequired().HasDefaultValue(-1);
        entity.Property(e => e.Modified).IsRequired().HasDefaultValueSql("getdate()").ValueGeneratedOnAddOrUpdate();
        entity.Property(e => e.ModifiedBy).IsRequired().HasDefaultValue(-1);
    }
}