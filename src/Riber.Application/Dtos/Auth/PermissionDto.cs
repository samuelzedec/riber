namespace Riber.Application.Dtos.Auth;

public sealed record PermissionDto(
    string Name,
    string Description,
    bool IsActive = true
);