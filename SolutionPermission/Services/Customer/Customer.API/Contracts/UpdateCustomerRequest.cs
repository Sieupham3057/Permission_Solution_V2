namespace Customer.API.Contracts;

public sealed class UpdateCustomerRequest
{
    public string Name { get; init; } = null!;
    public string Email { get; init; } = null!;
}