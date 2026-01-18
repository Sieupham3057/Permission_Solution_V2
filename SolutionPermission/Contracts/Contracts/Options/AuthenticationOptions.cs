namespace Contracts.Options;

public sealed class AuthenticationOptions
{
    public string Authority { get; init; } = default!;
    public string? Audience { get; init; }
    public bool RequireHttpsMetadata { get; init; }
    public bool MapInboundClaims { get; init; }
    public int ClockSkewSeconds { get; init; }
}