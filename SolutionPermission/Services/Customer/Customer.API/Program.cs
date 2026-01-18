using Authorization.Claims;
using Authorization.Handlers;
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
using System.Text;

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
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["JwtOption:Issuer"],
            ValidAudience = builder.Configuration["JwtOption:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["JwtOption:SecretKey"]!
                )),

            ClockSkew = TimeSpan.Zero, // ❗ tránh lệch giờ

            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role // ✅ QUAN TRỌNG
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