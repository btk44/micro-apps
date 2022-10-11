
using IdentityService.Application.Common.Interfaces;
using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure;

namespace IdentityService.Infrastructure;
public class ApplicationDbContext : DbContext, IApplicationDbContext{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options){ }
    public DbSet<AccountEntity> Accounts { get; set;} 
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    public DbSet<FailedAuthInfoEntity> FailedAuthInfos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) { 
        base.OnModelCreating(modelBuilder);

        BaseEntityBuilder.Build(modelBuilder.Entity<AccountEntity>());
        BaseEntityBuilder.Build(modelBuilder.Entity<RefreshTokenEntity>());
        BaseEntityBuilder.Build(modelBuilder.Entity<FailedAuthInfoEntity>());
        BaseEntityBuilder.Build(modelBuilder.Entity<ResetPasswordTokenEntity>());

        modelBuilder.Entity<AccountEntity>().HasMany(x => x.RefreshTokens).WithOne(x => x.Account);        
        modelBuilder.Entity<AccountEntity>().HasMany(x => x.ResetPasswordTokens).WithOne(x => x.Account);        
        modelBuilder.Entity<AccountEntity>().HasOne(x => x.FailedAuthInfo).WithOne(x => x.Account).HasForeignKey<FailedAuthInfoEntity>(x=>x.AccountId);        
    }    
}