using Microsoft.OpenApi.Models;
using Shared.Api.Middleware;
using Swashbuckle.AspNetCore.Filters;
using TransactionService.Application;
using TransactionService.Infrastructure;

var CorsOrigins = "CorsOriginsAllowed";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
//builder.Configuration["Auth:ServerSigningPassword"] = DockerSecretReader.GetSecretOrEnvVar("private_key", builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsOrigins, policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
 
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRouting();
app.UseCors(CorsOrigins);
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<ErrorHandlerMiddleware>();

using(var scope = app.Services.CreateScope()){
    var dbContextInitialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
    await dbContextInitialiser.Migrate();
}

app.Run();
