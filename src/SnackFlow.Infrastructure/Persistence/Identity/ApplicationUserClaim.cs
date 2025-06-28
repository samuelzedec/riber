using Microsoft.AspNetCore.Identity;

namespace SnackFlow.Infrastructure.Persistence.Identity;

public sealed class ApplicationUserClaim : IdentityUserClaim<Guid>;