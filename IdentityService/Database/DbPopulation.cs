using Microsoft.AspNetCore.Identity;

namespace IdentityService.Database;

public static class DbPopulation{
    public static void PopulateWithTestData(IApplicationBuilder app){
        using (var serviceScope = app.ApplicationServices.CreateScope()){
            InsertData(serviceScope.ServiceProvider.GetService<DatabaseContext>());
        }
    }

    private static void InsertData(DatabaseContext context){
        if(!context.Accounts.Any()){
            var passwordHasher = new PasswordHasher<string>();

            context.Accounts.AddRange(
                new AccountEntity(){
                    Email = "joe@test.com",
                    Password = passwordHasher.HashPassword("joe@test.com", "joe")
                },
                new AccountEntity(){
                    Email = "jane2@test.com",
                    Password = passwordHasher.HashPassword("jane@test.com", "jane")
                }
            );

            context.SaveChanges();
        }
    }
}