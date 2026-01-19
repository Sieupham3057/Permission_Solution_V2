using Contracts.Constance;

namespace Contracts.Options;

/**
 * ✔ sealed
 * ✔ init → immutable after bind
 * ✔ SectionName → tránh hard-code
 */

public sealed class CorsOptions
{
    public const string SectionName = SystemConst.SpaCors;

    public string[] AllowedOrigins { get; init; } = [];
    public string[] AllowedMethods { get; init; } = [];
    public string[] AllowedHeaders { get; init; } = [];
    public bool AllowCredentials { get; init; }
}