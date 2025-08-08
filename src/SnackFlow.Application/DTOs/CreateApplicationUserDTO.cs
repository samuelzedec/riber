namespace SnackFlow.Application.DTOs;

public sealed class CreateApplicationUserDTO
{
    public string UserName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public IEnumerable<string> Roles = [];
    public IEnumerable<string> Permissions = [];
}