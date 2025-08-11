namespace SnackFlow.Application.DTOs;

public sealed record PermissionDTO(
    string Name,
    string Description,
    bool IsActive = true
);