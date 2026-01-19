using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Gateway.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddGatewaySwagger(this IServiceCollection services, IConfiguration config)
    {
        var title = config["Swagger:Title"] ?? "API Gateway";
        var version = config["Swagger:Version"] ?? "v1";

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(version, new OpenApiInfo { Title = title, Version = version });

            // ✅ Cách đơn giản nhất: Bearer auth (textbox)
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
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

            // Nếu bạn muốn OAuth2 PKCE thay vì Bearer textbox, mình sẽ đưa config ở dưới.
        });

        return services;
    }

    public static void ConfigureSwaggerEndpoints(this WebApplication app, SwaggerUIOptions options)
    {
        // Gateway doc của chính nó
        var version = app.Configuration["Swagger:Version"] ?? "v1";
        options.SwaggerEndpoint($"/swagger/{version}/swagger.json", "Gateway");

        // Downstream docs (dropdown)
        var docs = app.Configuration.GetSection("Swagger:DownstreamDocs").GetChildren();
        foreach (var doc in docs)
        {
            var name = doc["Name"]!;
            var path = doc["SwaggerJsonPathOnGateway"]!;
            options.SwaggerEndpoint(path, name);
        }
    }

    public static void ConfigureSwaggerOAuth(this WebApplication app, SwaggerUIOptions options)
    {
        var enabled = app.Configuration.GetValue("Swagger:OAuth:Enabled", false);
        if (!enabled) return;

        var clientId = app.Configuration["Swagger:OAuth:ClientId"]!;
        options.OAuthClientId(clientId);
        options.OAuthUsePkce();
        options.OAuthScopeSeparator(" ");
    }
}