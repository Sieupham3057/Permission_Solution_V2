using Microsoft.AspNetCore.Authorization;
using Permission.Domain.Constances;
using Permission.Domain.Permissions;
using System.Security.Claims;

namespace Permission.API.Authorization.Requirements;

public class AssignRolesAuthorizationRequirement : IAuthorizationRequirement
{ }

public class AssignRolesAuthorizationHandler :
    AuthorizationHandler<AssignRolesAuthorizationRequirement, (string[] newRoles, string[] currentRoles)>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        AssignRolesAuthorizationRequirement requirement, (string[] newRoles, string[] currentRoles) roles)
    {
        if (!GetIsRolesChanged(roles.newRoles, roles.currentRoles))
        {
            context.Succeed(requirement);
        }
        else if (context.User.HasClaim(CustomClaims.Permission, ApplicationPermissions.AssignRoles))
        {
            // If user has ReadRoles permission, then he can assign any roles
            if (context.User.HasClaim(CustomClaims.Permission, ApplicationPermissions.ReadRoles))
                context.Succeed(requirement);

            // Else user can only assign roles they're part of
            /**
			 * => On Frontend if user have permission ViewRoles then get all USERS and all ROLES (Support assignRole)
			 * => Else: Get USERS and role of CURRENT_USER (Support AssignRole): Use can assign role which role current_user have
			 */
            else if (GetIsUserInAllAddedRoles(context.User, roles.newRoles, roles.currentRoles))
                context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    private static bool GetIsRolesChanged(string[] newRoles, string[] currentRoles)
    {
        newRoles ??= [];
        currentRoles ??= [];

        var roleAdded = newRoles.Except(currentRoles).Any();
        var roleRemoved = currentRoles.Except(newRoles).Any();

        return roleAdded || roleRemoved;
    }

    private static bool GetIsUserInAllAddedRoles(ClaimsPrincipal contextUser, string[] newRoles, string[] currentRoles)
    {
        newRoles ??= [];
        currentRoles ??= [];

        var addedRoles = newRoles.Except(currentRoles);

        return addedRoles.All(contextUser.IsInRole);
    }
}