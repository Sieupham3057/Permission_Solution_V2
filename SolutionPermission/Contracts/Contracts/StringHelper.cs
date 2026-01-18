namespace Contracts;

public static class StringHelper
{
    public static string NormalizePermission(this string permission)
        => permission.Trim().ToLowerInvariant();
}