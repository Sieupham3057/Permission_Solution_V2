namespace Contracts.Options;

public sealed class KeycloakOptions
{
    public string Realm { get; init; } = default!;
    public string ClientId { get; init; } = default!;
    public bool NormalizePermissions { get; init; } = true;
}