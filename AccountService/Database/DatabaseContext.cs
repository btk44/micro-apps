using Microsoft.EntityFrameworkCore;

namespace AccountService.Database;
public class DatabaseContext : DbContext{
    public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options){ }
    public DbSet<AccountEntity> Accounts { get; set;} 

    protected override void OnModelCreating(ModelBuilder modelBuilder) { }
}