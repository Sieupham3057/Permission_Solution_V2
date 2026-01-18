using Microsoft.Extensions.DependencyInjection;

namespace Authorization.Extensions;

public static class AuthorizationServiceExtensions
{
    public static IServiceCollection AddAuthorizationBuildingBlocks(
        this IServiceCollection services)
    {
        // future shared registrations
        return services;
    }
}