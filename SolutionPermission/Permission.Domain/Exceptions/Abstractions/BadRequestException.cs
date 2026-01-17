namespace Permission.Domain.Exceptions.Abstractions;

public abstract class BadRequestException : DomainException
{
	protected BadRequestException(string message)
		: base("Bad Request", message)
	{
	}
}
