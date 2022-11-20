using IdentityService.Application;
using IdentityService.Infrastructure;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.OpenApi.Models;
using IdentityService.Api.Middleware;
using IdentityService.Domain.Messages;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Configuration["Auth:ServerSigningPassword"] = DockerSecretReader.GetSecretOrEnvVar("private_key", builder.Configuration);

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

builder.Host.UseNServiceBus(context =>
{
    var endpointConfiguration = new EndpointConfiguration("IdentityService");
    var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
    transport.UseConventionalRoutingTopology(QueueType.Quorum);

    transport.Routing().RouteToEndpoint(
            assembly: typeof(AccountCreatedMessage).Assembly,
            destination: "DontKnowYet");

    endpointConfiguration.SendOnly();

    return endpointConfiguration;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<ErrorHandlerMiddleware>();

using(var scope = app.Services.CreateScope()){
    var dbContextInitialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
    await dbContextInitialiser.Migrate();
}

app.Run();
