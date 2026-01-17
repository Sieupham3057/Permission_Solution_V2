using Permission.Domain.Exceptions.Abstractions;

namespace Permission.Domain.Exceptions;

public class UserAccountException : DomainException
{
	public UserAccountException() : base("A User Account Exception has occurred.")
	{
		
	}
	public UserAccountException(string message) : base(nameof(UserAccountException), message)
	{
	}
}