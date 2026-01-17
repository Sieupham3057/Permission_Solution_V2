namespace Permission.Domain.Abstractions;

public interface IAuditableEntity
{
	Guid? CreatedBy { get; set; }
	Guid? UpdatedBy { get; set; }
	DateTime CreatedDate { get; set; }
	DateTime UpdatedDate { get; set; }
}