using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace Permission.API.Attributes;

public class CustomJwtBearerEvents : JwtBearerEvents
{
	//private readonly UserManager<ApplicationUser> _userManager;
	//private readonly ILogger<CustomJwtBearerEvents> _logger;

	public CustomJwtBearerEvents(//UserManager<ApplicationUser> userManager, 
								 //ILogger<CustomJwtBearerEvents> logger
		)
	{
		//_userManager = userManager;
		//_logger = logger;
	}

	public override async Task TokenValidated(TokenValidatedContext context)
	{
		if (context.SecurityToken is JsonWebToken accessToken)
		{
			var userId = accessToken.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;

			// Check for missing userId
			if (string.IsNullOrEmpty(userId))
			{
				//_logger.LogWarning("Authentication failed: Missing user ID in token.");
				context.Fail("Authentication fail.");
				return;
			}

			//var user = await _userManager.FindByIdAsync(userId);
			//if (user == null)
			//{
			//    _logger.LogWarning("Authentication failed: User not found for ID {UserId}.", userId);
			//    context.Fail("User not found.");
			//}
			//else if (await _userManager.IsLockedOutAsync(user))
			//{
			//    _logger.LogWarning("Authentication failed: User {UserId} is locked out.", userId);
			//    context.Fail("User is locked out.");
			//}
			//else
			//{
			//    context.Success();
			//}
		}
		else
		{
			//_logger.LogWarning("Authentication failed: Invalid token.");
			context.Fail("Authentication fail.");
		}
	}
}