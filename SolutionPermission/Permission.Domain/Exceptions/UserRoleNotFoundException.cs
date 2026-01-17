using Permission.Domain.Exceptions.Abstractions;

namespace Permission.Domain.Exceptions;

public class UserRoleNotFoundException : NotFoundException
{
	public UserRoleNotFoundException(Guid id) : base($"User role with id: {id} was not found.")
	{
	}
	public UserRoleNotFoundException(string message) : base(message)
	{
	}
}