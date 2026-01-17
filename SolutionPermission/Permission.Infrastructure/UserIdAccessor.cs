using Microsoft.AspNetCore.Http;
using Permission.Domain.Abstractions.Services;
using System.Security.Claims;

namespace Permission.Infrastructure;

public class UserIdAccessor(IHttpContextAccessor httpContextAccessor) : IUserIdAccessor
{
	private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

	public Guid? GetCurrentUserId() 
		=> Guid.TryParse(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId)
		? userId : null;

}