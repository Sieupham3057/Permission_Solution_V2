namespace Customer.Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Customer()
    { }

    public static Customer Create(
        string code,
        string name,
        string email,
        Guid createdBy)
    {
        return new Customer
        {
            Id = Guid.NewGuid(),
            Code = code,
            Name = name,
            Email = email,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string email)
    {
        Name = name;
        Email = email;
    }
}