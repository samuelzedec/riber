using Microsoft.AspNetCore.Authorization;

namespace Riber.Api.Authorizations.Permissions;

/* --------------------------------------------------------------------------
 * A interface IAuthorizationRequirement serve para informar ao ASP.NET Core
 * que aquela classe pode ser usada como um requisito de autorização.
 * -------------------------------------------------------------------------- */
public sealed record PermissionAuthorization(params string[] Permissions) 
    : IAuthorizationRequirement;