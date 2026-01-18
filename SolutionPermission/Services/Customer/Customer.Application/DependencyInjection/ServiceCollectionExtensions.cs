using Customer.Application.Abstractions;
using Customer.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Customer.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomerApplication(
        this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();

        return services;
    }
}