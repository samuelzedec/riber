using Microsoft.AspNetCore.Authorization;

namespace Riber.Api.Authorizations.Permissions;

/* --------------------------------------------------------------------------
 * A classe AuthorizationHandler<PermissionAuthorization> informa ao ASP.NET
 * Core que ela é o manipulador (handler) responsável por avaliar o requisito
 * personalizado PermissionAuthorization.
 * -------------------------------------------------------------------------- */
public sealed class PermissionAuthorizationHandler 
    : AuthorizationHandler<PermissionAuthorization>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, // Contexto de autorização
        PermissionAuthorization requirement) // Permissões necessárias
    {
        // Permissões do usuário autenticado
        var userPermissions = context.User.FindAll("permission")
            .Select(c => c.Value)
            .ToList();

        // Verifica se o usuário possui todas as permissões necessárias
        var hasAllPermissions = requirement.Permissions.All(p => userPermissions.Contains(p));

        if (hasAllPermissions)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}