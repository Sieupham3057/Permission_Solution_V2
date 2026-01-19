using Customer.Domain.Repositories;
using Customer.Infrastructure.Persistence;
using Customer.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Customer.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomerInfrastructure(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        services.AddDbContext<CustomerDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionString")));

        services.AddScoped<ICustomerRepository, CustomerRepository>();

        return services;
    }
}