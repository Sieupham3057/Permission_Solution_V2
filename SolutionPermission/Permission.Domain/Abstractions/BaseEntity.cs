namespace Permission.Domain.Abstractions;

public abstract class BaseEntity<Tkey> : IAuditableEntity
{
	public Tkey Id { get; set; }

	public Guid? CreatedBy { get; set; }

	public Guid? UpdatedBy { get; set; }

	public DateTime UpdatedDate { get; set; }

	public DateTime CreatedDate { get; set; }
}