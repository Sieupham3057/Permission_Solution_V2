using Customer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Customer.Infrastructure.Seed;

using Customer.Domain.Entities;

public static class CustomerSeed
{
    public static async Task SeedAsync(CustomerDbContext db)
    {
        if (await db.Customers.AnyAsync()) return;

        db.Customers.Add(Customer.Create(
        code: "CUST-001",
        name: "Default Customer",
        email: "default@company.com",
        createdBy: Guid.Empty));

        await db.SaveChangesAsync();
    }
}