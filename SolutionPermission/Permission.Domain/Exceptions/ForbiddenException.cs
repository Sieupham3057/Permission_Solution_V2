using Permission.Domain.Exceptions.Abstractions;

namespace Permission.Domain.Exceptions;

public class ForbiddenException : DomainException
{
	public ForbiddenException() : base("Access denied.") { }
		
	public ForbiddenException(string message) : base(message)
	{
	}
}