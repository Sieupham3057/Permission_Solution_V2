using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace Authorization;

public static class KeycloakRoleExtractor
{
    public static IEnumerable<string> GetClientRoles(JwtSecurityToken jwt, string clientId)
    {
        // Payload["resource_access"] có thể là JsonElement / Dictionary / object
        if (!jwt.Payload.TryGetValue("resource_access", out var raObj) || raObj == null)
            return Array.Empty<string>();

        try
        {
            JsonElement raEl = raObj switch
            {
                JsonElement je => je,
                _ => JsonSerializer.SerializeToElement(raObj)
            };

            if (!raEl.TryGetProperty(clientId, out var clientEl))
                return Array.Empty<string>();

            if (!clientEl.TryGetProperty("roles", out var rolesEl) || rolesEl.ValueKind != JsonValueKind.Array)
                return Array.Empty<string>();

            return rolesEl.EnumerateArray()
                .Where(x => x.ValueKind == JsonValueKind.String)
                .Select(x => x.GetString()!)
                .ToArray();
        }
        catch (Exception ex)
        {
            Console.WriteLine("GetClientRoles failed: " + ex);
            return Array.Empty<string>();
        }
    }

    public static IEnumerable<string> GetClientRolesFromResourceAccessJson(string resourceAccessJson, string clientId)
    {
        try
        {
            using var doc = JsonDocument.Parse(resourceAccessJson);
            var root = doc.RootElement;

            if (!root.TryGetProperty(clientId, out var clientEl))
                return Array.Empty<string>();

            if (!clientEl.TryGetProperty("roles", out var rolesEl) || rolesEl.ValueKind != JsonValueKind.Array)
                return Array.Empty<string>();

            return rolesEl.EnumerateArray()
                .Where(x => x.ValueKind == JsonValueKind.String)
                .Select(x => x.GetString()!)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }
}