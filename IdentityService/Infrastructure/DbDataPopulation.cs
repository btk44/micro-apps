
using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Infrastructure;

public static class DbDataPopulation{
    public static void PopulateWithData(IApplicationBuilder app){
        using (var serviceScope = app.ApplicationServices.CreateScope()){
            InsertData(serviceScope.ServiceProvider.GetService<DatabaseContext>());
        }
    }

    private static void InsertData(DatabaseContext dbContext){
        if(!dbContext.Accounts.Any()){
            Console.WriteLine("=== Inserting sample data ===");

            var passwordHasher = new PasswordHasher<string>();

            dbContext.Accounts.AddRange(
                new AccountEntity(){
                    Email = "joe@test.com",
                    Password = passwordHasher.HashPassword("joe@test.com", "joe")
                },
                new AccountEntity(){
                    Email = "jane2@test.com",
                    Password = passwordHasher.HashPassword("jane@test.com", "jane")
                }
            );

            dbContext.SaveChanges();
            return;
        }

        Console.WriteLine("=== Data already inserted ===");
    }
}