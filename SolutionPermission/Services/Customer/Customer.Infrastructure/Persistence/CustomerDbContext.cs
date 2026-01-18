using Microsoft.EntityFrameworkCore;

namespace Customer.Infrastructure.Persistence;

using Domain.Entities;

public sealed class CustomerDbContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();

    public CustomerDbContext(DbContextOptions<CustomerDbContext> options)
    : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(CustomerDbContext).Assembly);
    }
}