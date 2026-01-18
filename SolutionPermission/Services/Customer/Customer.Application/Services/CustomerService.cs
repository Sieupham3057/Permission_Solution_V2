namespace Customer.Application.Services;

using Customer.Application.Abstractions;
using Customer.Domain.Repositories;
using Customer.Domain.Entities;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repo;

    public CustomerService(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<Customer>> GetAllAsync()
        => _repo.GetAllAsync();

    public Task<Customer?> GetByIdAsync(Guid id)
        => _repo.GetByIdAsync(id);

    public async Task<Guid> CreateAsync(
        string code, string name, string email, Guid userId)
    {
        var customer = Customer.Create(code, name, email, userId);
        await _repo.AddAsync(customer);
        return customer.Id;
    }

    public async Task UpdateAsync(Guid id, string name, string email)
    {
        var customer = await _repo.GetByIdAsync(id)
            ?? throw new Exception("Customer not found");

        customer.Update(name, email);
        await _repo.UpdateAsync(customer);
    }

    public async Task DeleteAsync(Guid id)
    {
        var customer = await _repo.GetByIdAsync(id)
            ?? throw new Exception("Customer not found");

        await _repo.DeleteAsync(customer);
    }
}