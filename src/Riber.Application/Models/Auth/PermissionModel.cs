namespace Riber.Application.Models.Auth;

public sealed record PermissionModel(
    string Name,
    string Description,
    bool IsActive = true
);