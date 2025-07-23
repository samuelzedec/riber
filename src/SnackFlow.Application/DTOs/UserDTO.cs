namespace SnackFlow.Application.DTOs;

public sealed class UserDTO
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; } = false;
    public string PhoneNumber { get; set; } = string.Empty;
    
    public Domain.Entities.User UserDomain { get; set; } = null!;
    
    public ICollection<string> Roles { get; set; } = [];
    public ICollection<ClaimDTO> Claims { get; set; } = [];
}