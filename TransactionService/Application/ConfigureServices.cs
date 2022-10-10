using System.Text;
using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace TransactionService.Application;

public static class ConfigureServices {
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration){
        services.AddMediatR(System.Reflection.Assembly.GetExecutingAssembly());
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        // services.AddTransient<ITokenService, TokenService>(s => new TokenService(configuration));
        
        // services
        // .AddAuthentication(x => {
        //     x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //     x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        // })
        // .AddJwtBearer(x => {
        //     x.RequireHttpsMetadata = false;
        //     x.SaveToken = true;
        //     x.IncludeErrorDetails = true;
        //     x.TokenValidationParameters = GetTokenValidationParameters(configuration);
        //     x.Events = new JwtBearerEvents  
        //     {  
        //         OnAuthenticationFailed = context =>  
        //         {  
        //             // debug "invalid token" scenarios here
        //             var exType = context.Exception.GetType();
        //             return Task.CompletedTask;  
        //         }
        //     };  
        // });
    }

    private static TokenValidationParameters GetTokenValidationParameters(IConfiguration configuration)
    {          
        return new TokenValidationParameters
        {
            RequireSignedTokens = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Auth:ServerSigningPassword"])),
            ValidAudience = configuration["Auth:Audience"],
            ValidateAudience = true,
            ValidIssuer = configuration["Auth:Issuer"],
            ValidateIssuer = true,
            ValidateLifetime = true
        };
    }
}