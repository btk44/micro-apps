using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Application.Interfaces;

public interface IApplicationDbContext{
    public DbSet<AccountEntity> Accounts { get; } 
    public DbSet<RefreshTokenEntity> RefreshTokens { get; }
    public DbSet<FailedAuthInfoEntity> FailedAuthInfos { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}