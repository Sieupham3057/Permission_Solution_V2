using Authorization;
using Authorization.Claims;
using Authorization.Handlers;
using Contracts;
using Customer.API.AttributeAndFilters;
using Customer.API.Authorization;
using Customer.Application.DependencyInjection;
using Customer.Infrastructure.DependencyInjection;
using Customer.Infrastructure.Persistence;
using Customer.Infrastructure.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// ================== MVC ==================
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// ================== INFRA ==================
builder.Services.AddCustomerInfrastructure(builder.Configuration);
builder.Services.AddCustomerApplication();

// ================== AUTHENTICATION ==================

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://172.28.225.131:8080/realms/company-dev";
        options.RequireHttpsMetadata = false; // DEV only (prod dùng https)

        // QUAN TRỌNG:
        // Keycloak token của bạn aud = "account", azp = "angular-spa"
        // Nhưng API nên có audience riêng (ví dụ: customer-api). Tạm thời để false để bạn chạy được trước,
        // rồi mình sẽ hướng dẫn set audience chuẩn ở Cách B phía dưới.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "http://172.28.225.131:8080/realms/company-dev",

            ValidateAudience = false, // tạm thời (prod nên bật và cấu hình audience đúng)
            ValidateLifetime = true,

            NameClaimType = "preferred_username",
            RoleClaimType = ClaimTypes.Role,

            ClockSkew = TimeSpan.Zero
        };

        // Tắt mapping mặc định để đỡ bị "http://schemas..." rối
        options.MapInboundClaims = false;

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                if (context.Principal?.Identity is not ClaimsIdentity identity)
                    return Task.CompletedTask;

                // resource_access thường là 1 claim dạng JSON string
                var resourceAccess = context.Principal.FindFirst("resource_access")?.Value;

                if (string.IsNullOrWhiteSpace(resourceAccess))
                    return Task.CompletedTask;

                var roles = KeycloakRoleExtractor.GetClientRolesFromResourceAccessJson(
                    resourceAccess,
                    clientId: "angular-spa"
                );

                // ✅ Case sensitivity
                //foreach (var r in roles.Distinct(StringComparer.OrdinalIgnoreCase))
                //{
                //    // Reuse toàn bộ code policy/handler hiện tại
                //    identity.AddClaim(new Claim(CustomClaims.Permission, r));
                //    identity.AddClaim(new Claim(ClaimTypes.Role, r));
                //}

                // Convert to normal "ex: Customers.Read => customer.read"
                foreach (var r in roles.Select(StringHelper.NormalizePermission).Distinct()) // ✅ normalize trước --> ✅ distinct sau
                {
                    identity.AddClaim(new Claim(CustomClaims.Permission, r));
                    identity.AddClaim(new Claim(ClaimTypes.Role, r));
                }

                return Task.CompletedTask;
            }
        };
    });

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

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Customer API",
        Version = "v1",
        Description = "Customer Service – Authorization Platform Demo"
    });

    // 🔐 JWT Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Input JWT token: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // 👉 Show permission info (optional nhưng RẤT HAY cho team)
    c.OperationFilter<SwaggerAuthorizationOperationFilter>();
});

var app = builder.Build();

// ================== PIPELINE ==================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer API v1");
        c.DisplayRequestDuration();
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Seed DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
    await db.Database.MigrateAsync();
    await CustomerSeed.SeedAsync(db);
}

app.UseHttpsRedirection();
// ❗ THỨ TỰ BẮT BUỘC
app.UseAuthentication();   // ❗ PHẢI CÓ
app.UseAuthorization();

app.MapControllers();

app.Run();