using Contracts.Constance;
using Contracts.Filters;
using Contracts.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Contracts.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwaggerContract(this IServiceCollection services, IConfiguration configuration, string serviceName = "Service API")
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = serviceName,
                Version = "v1",
                Description = serviceName + " – Authorization Platform Demo"
            });

            // 🔐 JWT Bearer
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Input JWT token: {access_token} | 'Don't need include keyword Bearer!'"
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

        return services;
    }

    public static IApplicationBuilder UseSwaggerIfDevelopmentContract(this IApplicationBuilder app, string serviceName = "Service API")
    {
        var env = app.ApplicationServices
                     .GetRequiredService<IHostEnvironment>();

        if (env.IsDevelopment())
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", serviceName);
                c.DisplayRequestDuration();
            });
        }

        return app;
    }

    public static IServiceCollection AddCorsContract(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CorsOptions>(
        configuration.GetSection(CorsOptions.SectionName));

        services.AddCors(options =>
        {
            options.AddPolicy(SystemConst.SpaCors, policy =>
            {
                var cors = configuration
                    .GetSection(CorsOptions.SectionName)
                    .Get<CorsOptions>()!;

                policy
                    .WithOrigins(cors.AllowedOrigins)
                    .WithMethods(cors.AllowedMethods)
                    .WithHeaders(cors.AllowedHeaders);

                if (cors.AllowCredentials)
                {
                    policy.AllowCredentials();
                }
                else
                {
                    policy.DisallowCredentials();
                }
            });
        });

        return services;
    }
}