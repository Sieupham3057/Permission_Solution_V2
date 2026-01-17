using Microsoft.AspNetCore.Authorization;
using Permission.Domain.Constances;
using Permission.Domain.Permissions;
using System.Security.Claims;

namespace Permission.API.Authorization.Requirements;

public class UserAccountAuthorizationRequirement(string operationName) : IAuthorizationRequirement
{
	public string OperationName { get; private set; } = operationName;
}

public class ViewUserAuthorizationHandler : AuthorizationHandler<UserAccountAuthorizationRequirement, Guid>
{
	protected override Task HandleRequirementAsync(
		AuthorizationHandlerContext context, UserAccountAuthorizationRequirement requirement, Guid targetUserId)
	{
		if (context.User == null || requirement.OperationName != UserAccountManagementOperations.ReadOperationName)
			return Task.CompletedTask;

		if (context.User.HasClaim(CustomClaims.Permission, ApplicationPermissions.ViewUsers)
			|| GetIsSameUser(context.User, targetUserId))
			context.Succeed(requirement);

		return Task.CompletedTask;
	}

	private static bool GetIsSameUser(ClaimsPrincipal user, Guid targetUserId)
	{
		// Attempt to get the claim for NameIdentifier
		var userClaimId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

		// Check if we got a valid userClaimId and if it can be parsed to a Guid
		return Guid.TryParse(userClaimId, out Guid userId) && userId == targetUserId;
	}
}

public class ManageUserAuthorizationHandler : AuthorizationHandler<UserAccountAuthorizationRequirement, Guid>
{
	protected override Task HandleRequirementAsync(
		AuthorizationHandlerContext context, UserAccountAuthorizationRequirement requirement, Guid targetUserId)
	{
		if (context.User == null ||
			(requirement.OperationName != UserAccountManagementOperations.CreateOperationName &&
			 requirement.OperationName != UserAccountManagementOperations.UpdateOperationName &&
			 requirement.OperationName != UserAccountManagementOperations.DeleteOperationName))
			return Task.CompletedTask;

		if (context.User.HasClaim(CustomClaims.Permission, ApplicationPermissions.ManageUsers)
			|| GetIsSameUser(context.User, targetUserId))
			context.Succeed(requirement);

		return Task.CompletedTask;
	}

	private static bool GetIsSameUser(ClaimsPrincipal user, Guid targetUserId)
	{
		// Attempt to get the claim for NameIdentifier
		var userClaimId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

		// Check if we got a valid userClaimId and if it can be parsed to a Guid
		return Guid.TryParse(userClaimId, out Guid userId) && userId == targetUserId;
	}
}