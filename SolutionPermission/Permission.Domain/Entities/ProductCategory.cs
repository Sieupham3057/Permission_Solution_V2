using Permission.Domain.Abstractions;

namespace Permission.Domain.Entities;

public class ProductCategory : BaseEntity<int>
{
	public required string Name { get; set; }
	public string? Description { get; set; }
	public string? Icon { get; set; }

	public ICollection<Product> Products { get; } = [];
}