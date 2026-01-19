using Authorization.Claims;
using Authorization.Extensions;
using Authorization.Handlers;
using Contracts.Constance;
using Contracts.DependencyInjection;
using Contracts.Options;
using Customer.API.Authorization;
using Customer.Application.DependencyInjection;
using Customer.Infrastructure.DependencyInjection;
using Customer.Infrastructure.Persistence;
using Customer.Infrastructure.Seed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// BINDING ...
builder.Services.Configure<AuthenticationOptions>(builder.Configuration.GetSection("Authentication"));

builder.Services.Configure<KeycloakOptions>(builder.Configuration.GetSection("Keycloak"));

// ===== CORS from appsettings =====

builder.Services.AddCorsContract(builder.Configuration);

// Add services to the container.

// ================== MVC ==================
builder.Services.AddControllers();

// ================== INFRA ==================
builder.Services.AddCustomerInfrastructure(builder.Configuration);
builder.Services.AddCustomerApplication();

// ================== AUTHENTICATION ==================

builder.Services.AddKeycloakJwtAuthentication(builder.Configuration);

// ================== AUTHORIZATION ==================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(CustomerPolicies.ViewList,
    p => p.RequireClaim(CustomClaims.Permission, CustomerPermissions.Read));

    options.AddPolicy(CustomerPolicies.Create,
    p => p.RequireClaim(CustomClaims.Permission, CustomerPermissions.Create));
});

builder.Services.AddSingleton<IAuthorizationHandler, CustomerAuthorizationHandler>();

builder.Services.AddSingleton<IAuthorizationHandler, SudoBypassAuthorizationHandler>();

// ================== SWAGGER ==================

builder.Services.AddSwaggerContract(builder.Configuration, serviceName: "Customer API");

var app = builder.Build();

// ================== PIPELINE ==================

app.UseSwaggerIfDevelopmentContract();

// Seed DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
    await db.Database.MigrateAsync();
    await CustomerSeed.SeedAsync(db);
}

app.UseHttpsRedirection();
// ❗ THỨ TỰ BẮT BUỘC
// ✅ CORS phải đứng TRƯỚC auth nếu bạn muốn preflight đi qua an toàn
app.UseCors(SystemConst.SpaCors);
app.UseAuthentication();   // ❗ PHẢI CÓ
app.UseAuthorization();

app.MapControllers();

app.Run();