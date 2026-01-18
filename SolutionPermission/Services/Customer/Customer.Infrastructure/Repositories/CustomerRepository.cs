using Customer.Domain.Repositories;
using Customer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Customer.Infrastructure.Repositories;

using Domain.Entities;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly CustomerDbContext _db;

    public CustomerRepository(CustomerDbContext db)
    {
        _db = db;
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
    => await _db.Customers.FindAsync(id);

    public async Task<IEnumerable<Customer>> GetAllAsync()
    => await _db.Customers.AsNoTracking().ToListAsync();

    public async Task AddAsync(Customer customer)
    {
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Customer customer)
    {
        _db.Customers.Update(customer);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Customer customer)
    {
        _db.Customers.Remove(customer);
        await _db.SaveChangesAsync();
    }
}