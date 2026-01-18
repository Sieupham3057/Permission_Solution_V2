namespace Customer.API.Contracts;

public sealed class CreateCustomerRequest
{
    public string Code { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Email { get; init; } = null!;
}