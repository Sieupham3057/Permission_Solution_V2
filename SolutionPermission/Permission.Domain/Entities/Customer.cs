using Permission.Domain.Abstractions;
using Permission.Domain.Enums;

namespace Permission.Domain.Entities;

public class Customer : BaseEntity<Guid>
{
	public required string Name { get; set; }
	public required string Email { get; set; }
	public string? PhoneNumber { get; set; }
	public string? Address { get; set; }
	public string? City { get; set; }
	public Gender Gender { get; set; }

	public ICollection<Order> Orders { get; } = [];
}