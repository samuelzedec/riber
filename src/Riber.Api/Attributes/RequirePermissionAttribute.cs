using Microsoft.AspNetCore.Authorization;

namespace Riber.Api.Attributes;

/// <summary>
/// Atributo personalizado para autorização baseada em permissões.
/// IMPORTANTE: Herda de AuthorizeAttribute para que o ASP.NET reconheça como atributo de autorização.
/// </summary>
/// <remarks>
/// Fluxo: [RequirePermission] → Formata Policy → PermissionPolicyProvider → PermissionAuthorizationHandler → Autoriza/Nega
/// </remarks>
public sealed class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(params string[] permissions)
        => Policy = $"RequirePermissions:{string.Join(",", permissions)}";
}