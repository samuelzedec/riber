namespace Riber.Application.Models;

public sealed record PermissionModel(
    string Name,
    string Description,
    bool IsActive = true
);