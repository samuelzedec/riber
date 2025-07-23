namespace SnackFlow.Application.DTOs;

public sealed class PermissionDTO
{
    public required string Name { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
    public required bool IsActive { get; set; } = true;
}