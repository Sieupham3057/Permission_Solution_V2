namespace Permission.Domain.Exceptions.Abstractions;

public abstract class NotFoundException : DomainException
{
	protected NotFoundException(string message)
		: base("Not Found", message)
	{
	}
}
