using System.Security.Claims;

namespace Authorization.Claims;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
        => Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

    public static bool HasPermission(this ClaimsPrincipal user, string permission)
        => user.HasClaim(CustomClaims.Permission, permission);
}