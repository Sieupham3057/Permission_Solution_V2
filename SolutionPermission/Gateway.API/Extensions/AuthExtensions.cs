using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Gateway.API.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddGatewayAuthentication(this IServiceCollection services, IConfiguration config)
    {
        var authSection = config.GetSection("Authentication");

        var authority = authSection["Authority"]!;
        var audience = authSection["Audience"]; // gateway-api
        var requireHttps = authSection.GetValue("RequireHttpsMetadata", true);
        var clockSkew = authSection.GetValue("ClockSkewSeconds", 0);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.RequireHttpsMetadata = requireHttps;
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authority,

                    // ✅ PROD: bật audience
                    ValidateAudience = false, // ✅ gateway: accept any aud (service will enforce)
                    ValidAudience = audience,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(clockSkew),

                    NameClaimType = "preferred_username",
                    RoleClaimType = ClaimTypes.Role
                };
            });

        return services;
    }

    public static IServiceCollection AddGatewayAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Policy mặc định: yêu cầu login
            options.AddPolicy("GatewayAuthenticated", p => p.RequireAuthenticatedUser());

            // Ví dụ: swagger docs cũng yêu cầu login (hoặc AllowAnonymous nếu bạn muốn mở)
            // options.AddPolicy("SwaggerDocs", p => p.RequireAuthenticatedUser());

            // Ví dụ: policy theo permission (nếu bạn muốn enforce tại gateway)
            // options.AddPolicy("Customers.Read", p => p.RequireClaim("permission", "customers.read"));
        });

        return services;
    }
}