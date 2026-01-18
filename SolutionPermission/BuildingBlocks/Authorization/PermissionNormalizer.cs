namespace Authorization;

public static class PermissionNormalizer
{
    public static IEnumerable<string> Normalize(IEnumerable<string> roles, bool enable)
    {
        if (!enable) return roles;

        return roles
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Select(r => r.Trim().ToLowerInvariant())
            .Distinct();
    }
}