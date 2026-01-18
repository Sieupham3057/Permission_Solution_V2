using Authorization.Claims;
using Contracts.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Authorization.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKeycloakJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind options
        services.Configure<AuthenticationOptions>(
            configuration.GetSection("Authentication"));

        services.Configure<KeycloakOptions>(
            configuration.GetSection("Keycloak"));

        // Register authentication scheme
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        // Configure JwtBearerOptions bằng DI (chuẩn production)
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<AuthenticationOptions>, IOptions<KeycloakOptions>>(
            (options, authOpt, kcOpt) =>
            {
                var auth = authOpt.Value;
                var kc = kcOpt.Value;

                options.Authority = auth.Authority;
                options.RequireHttpsMetadata = auth.RequireHttpsMetadata;
                options.MapInboundClaims = auth.MapInboundClaims;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = auth.Authority,

                    ValidateAudience = !string.IsNullOrWhiteSpace(auth.Audience),
                    ValidAudience = auth.Audience,

                    ValidateLifetime = true,

                    NameClaimType = "preferred_username",
                    RoleClaimType = ClaimTypes.Role,

                    ClockSkew = TimeSpan.FromSeconds(auth.ClockSkewSeconds)
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        if (context.Principal?.Identity is not ClaimsIdentity identity)
                            return Task.CompletedTask;

                        var resourceAccess =
                            context.Principal.FindFirst("resource_access")?.Value;

                        if (string.IsNullOrWhiteSpace(resourceAccess))
                            return Task.CompletedTask;

                        // resource_access =>> Audience: 'customer-api' =>> roles
                        var permissionClientId = auth.Audience ?? "customer-api"; // "customer-api"

                        var roles = KeycloakRoleExtractor.GetClientRolesFromResourceAccessJson(resourceAccess, permissionClientId);

                        foreach (var r in PermissionNormalizer.Normalize(
                                     roles, kc.NormalizePermissions))
                        {
                            identity.AddClaim(new Claim(CustomClaims.Permission, r));
                            identity.AddClaim(new Claim(ClaimTypes.Role, r));
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}