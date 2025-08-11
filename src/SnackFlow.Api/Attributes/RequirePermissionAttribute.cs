using Microsoft.AspNetCore.Authorization;

namespace SnackFlow.Api.Attributes;

public sealed class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(params string[] permissions)
        => Policy = $"RequirePermissions:{string.Join(",", permissions)}";
}