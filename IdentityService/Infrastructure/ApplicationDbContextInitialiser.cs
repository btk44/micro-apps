
using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure;

public class ApplicationDbContextInitialiser{
    private ApplicationDbContext _dbContext;

    public ApplicationDbContextInitialiser(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Migrate(){
        await _dbContext.Database.MigrateAsync();
        await InsertData();  // remove it later?
    }

    private async Task InsertData(){
        if(!_dbContext.Accounts.Any()){
            Console.WriteLine("=== Inserting sample data ===");

            var passwordHasher = new PasswordHasher<string>();

            _dbContext.Accounts.AddRange(
                new AccountEntity(){
                    Email = "joe@test.com",
                    Password = passwordHasher.HashPassword("joe@test.com", "joe")
                },
                new AccountEntity(){
                    Email = "jane2@test.com",
                    Password = passwordHasher.HashPassword("jane@test.com", "jane")
                }
            );

            await _dbContext.SaveChangesAsync();
            return;
        }

        Console.WriteLine("=== Data already inserted ===");
    }
}