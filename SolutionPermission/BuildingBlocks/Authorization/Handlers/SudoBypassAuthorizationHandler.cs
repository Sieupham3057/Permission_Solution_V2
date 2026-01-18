using Contracts.Constance;
using Microsoft.AspNetCore.Authorization;

namespace Authorization.Handlers;

public sealed class SudoBypassAuthorizationHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (context.User?.Identity?.IsAuthenticated == true &&
            context.User.IsInRole(SystemConst.SudoRole))
        {
            // Pass ALL requirements (ClaimRequirement, PolicyRequirement, ResourceRequirement,...)
            foreach (var req in context.Requirements)
                context.Succeed(req);
        }

        return Task.CompletedTask;
    }
}