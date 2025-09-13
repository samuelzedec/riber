namespace Riber.Application.DTOs;

public sealed record PermissionDTO(
    string Name,
    string Description,
    bool IsActive = true
);