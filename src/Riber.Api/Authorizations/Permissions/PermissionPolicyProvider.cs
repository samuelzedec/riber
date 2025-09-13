using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Riber.Api.Authorizations.Permissions;

/// <summary>
/// Provider responsável por criar políticas de autorização dinamicamente baseadas em permissões.
/// Intercepta policies no formato "RequirePermissions:perm1,perm2" e cria automaticamente.
/// </summary>
/// <remarks>
/// Evita ter que registrar manualmente cada combinação de permissões no Program.cs.
/// Para policies que não seguem o padrão, delega para o DefaultAuthorizationPolicyProvider.
/// </remarks>
/// <param name="options">Opções de autorização do ASP.NET Core</param>
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