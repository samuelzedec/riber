namespace Riber.Application.Models.Shared;

public sealed record PermissionModel(
    string Name,
    string Description,
    bool IsActive = true
);