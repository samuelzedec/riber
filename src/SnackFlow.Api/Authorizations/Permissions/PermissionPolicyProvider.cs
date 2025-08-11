using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SnackFlow.Api.Authorizations.Permissions;

public sealed class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) 
    : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider = new(options);

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!policyName.StartsWith("RequirePermissions:", StringComparison.OrdinalIgnoreCase))
            return _fallbackPolicyProvider.GetPolicyAsync(policyName);
        
        var permissions = policyName["RequirePermissions:".Length..] // Substring para remover o prefixo
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionAuthorization(permissions))
            .Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        => _fallbackPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        => _fallbackPolicyProvider.GetFallbackPolicyAsync();
}