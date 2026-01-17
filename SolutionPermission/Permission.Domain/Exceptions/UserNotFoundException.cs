using Permission.Domain.Exceptions.Abstractions;

namespace Permission.Domain.Exceptions;

public class UserNotFoundException : NotFoundException
{
	public UserNotFoundException() : base("Unable to find the requested User.")
	{

	}

	public UserNotFoundException(string message) : base(message)
	{
	}
}