using Microsoft.EntityFrameworkCore;

namespace IdentityService.Database;
public class DatabaseContext : DbContext{
    public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options){ }
    public DbSet<AccountEntity> Accounts { get; set;} 

    protected override void OnModelCreating(ModelBuilder modelBuilder) { }
}