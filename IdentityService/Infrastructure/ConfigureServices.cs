using IdentityService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure;

public static class ConfigureServices {
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration){
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"],
                    builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitialiser>();
    }
}