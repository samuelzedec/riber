namespace SnackFlow.Application.DTOs;

public sealed class UserDetailsDTO
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; } = false;
    public string PhoneNumber { get; set; } = string.Empty;
    public string SecurityStamp { get; set; } = string.Empty;
    public Guid UserDomainId { get; set; }
    public Domain.Entities.User UserDomain { get; set; } = null!;
    
    public string Role { get; set; } = string.Empty;
    public ICollection<ClaimDTO> Claims { get; set; } = [];
}