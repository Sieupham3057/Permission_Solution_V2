namespace Permission.Domain.Abstractions.Services;

public interface IUserIdAccessor
{
	Guid? GetCurrentUserId();
}