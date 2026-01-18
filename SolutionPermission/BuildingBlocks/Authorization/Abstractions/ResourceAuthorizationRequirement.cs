using Microsoft.AspNetCore.Authorization;

namespace Authorization.Abstractions;

public sealed class ResourceAuthorizationRequirement : IAuthorizationRequirement
{
    public string Operation { get; }

    public ResourceAuthorizationRequirement(string operation)
        => Operation = operation;
}