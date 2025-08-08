using Microsoft.AspNetCore.Identity;

namespace SnackFlow.Infrastructure.Persistence.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
}