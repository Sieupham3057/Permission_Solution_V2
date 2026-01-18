using Authorization.Abstractions;
using Authorization.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Authorization.Handlers;

public abstract class BaseCrudAuthorizationHandler<T>
    : AuthorizationHandler<ResourceAuthorizationRequirement, T>
{
    protected abstract string CreatePermission { get; }
    protected abstract string ReadPermission { get; }
    protected abstract string UpdatePermission { get; }
    protected abstract string DeletePermission { get; }

    protected abstract bool IsOwner(ClaimsPrincipal user, T resource);

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ResourceAuthorizationRequirement requirement,
        T resource)
    {
        // Get current user
        var user = context.User;

        // If user still not login yet! or dont have login info
        if (!user.Identity?.IsAuthenticated ?? false)
            return Task.CompletedTask;

        // OWNER ALWAYS ALLOW => WHO CREATED THAT RESOURCE
        if (IsOwner(user, resource))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // CRUD → Permission
        var permission = requirement.Operation switch
        {
            CrudOperations.Create => CreatePermission,
            CrudOperations.Read => ReadPermission,
            CrudOperations.Update => UpdatePermission,
            CrudOperations.Delete => DeletePermission,
            _ => null
        };

        // Check claim, CLAIM WITH NAME: 'CustomClaims.Permission' HAVE VALUE 'permission'
        if (permission != null && user.HasClaim(CustomClaims.Permission, permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}