using Microsoft.AspNetCore.Identity;
using SnackFlow.Domain.Entities;

namespace SnackFlow.Infrastructure.Persistence.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string Name { get; set; } = string.Empty;
    public Guid UserDomainId { get; set; }
    public User UserDomain { get; set; } = null!;
    public bool IsDeleted { get; set; } = false;
}