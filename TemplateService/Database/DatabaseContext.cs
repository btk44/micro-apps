using Microsoft.EntityFrameworkCore;

namespace TemplateService.Database;
public class DatabaseContext : DbContext{
    public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options){ }
    public DbSet<SomeEntity> SomeEntities { get; set;} 

    protected override void OnModelCreating(ModelBuilder modelBuilder) { }
}