namespace Customer.Application.Abstractions;

using Customer.Domain.Entities;

public interface ICustomerService
{
    Task<IEnumerable<Customer>> GetAllAsync();

    Task<Customer?> GetByIdAsync(Guid id);

    Task<Guid> CreateAsync(string code, string name, string email, Guid userId);

    Task UpdateAsync(Guid id, string name, string email);

    Task DeleteAsync(Guid id);
}