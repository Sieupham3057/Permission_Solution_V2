using System.Security.Claims;

namespace Authorization.Claims;

public static class ClaimsPrincipalExtensions
{
    // ============================== KeyCloak ==============================
    // Keycloak/OIDC: sub (string, thường là GUID nhưng không guarantee)
    private const string Sub = "sub";

    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        // 1) Ưu tiên OIDC standard
        var raw = user.FindFirst(Sub)?.Value
            // 2) Fallback cho hệ thống cũ của bạn
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            // 3) Một số hệ thống dùng "nameid" hoặc "userid"
            ?? user.FindFirst("nameid")?.Value
            ?? user.FindFirst("userId")?.Value;

        if (string.IsNullOrWhiteSpace(raw))
            throw new UnauthorizedAccessException("UserId claim not found (sub / nameidentifier).");

        // Nếu Keycloak sub là GUID -> parse được
        if (Guid.TryParse(raw, out var id))
            return id;

        // Nếu sub KHÔNG phải GUID (vài IdP có thể dùng string khác)
        throw new UnauthorizedAccessException($"UserId claim value is not a GUID: '{raw}'. Use GetUserSubject() instead.");
    }

    // Khuyến nghị: dùng string cho OIDC subject (bền nhất)
    public static string GetUserSubject(this ClaimsPrincipal user)
        => user.FindFirst(Sub)?.Value
           ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
           ?? throw new UnauthorizedAccessException("Subject claim not found (sub / nameidentifier).");

    public static string? GetUsername(this ClaimsPrincipal user)
        => user.FindFirst("preferred_username")?.Value
           ?? user.Identity?.Name;

    // ============================== KeyCloak ==============================

    // // ============================== JWT ==============================
    public static Guid GetUserIdJWT(this ClaimsPrincipal user)
        => Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

    public static bool HasPermission(this ClaimsPrincipal user, string permission)
        => user.HasClaim(CustomClaims.Permission, permission);

    // ============================== JWT ==============================
}