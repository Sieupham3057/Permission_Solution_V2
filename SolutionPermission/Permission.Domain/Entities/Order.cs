using Permission.Domain.Abstractions;
using Permission.Domain.Entities.Account;

namespace Permission.Domain.Entities;

public class Order : BaseEntity<int>
{
	public decimal Discount { get; set; }
	public string? Comments { get; set; }

	public Guid? CashierId { get; set; }
	public ApplicationUser? Cashier { get; set; }

	public Guid CustomerId { get; set; }
	public required Customer Customer { get; set; }

	public ICollection<OrderDetail> OrderDetails { get; } = [];
}