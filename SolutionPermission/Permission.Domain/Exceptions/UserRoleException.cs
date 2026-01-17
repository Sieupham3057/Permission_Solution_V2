using Permission.Domain.Exceptions.Abstractions;

namespace Permission.Domain.Exceptions;

public class UserRoleException : DomainException
{
	public UserRoleException() : base("A User Role Exception has occurred.")
	{

	}

	public UserRoleException(string message) : base(nameof(UserRoleException),message)
	{
	}
}