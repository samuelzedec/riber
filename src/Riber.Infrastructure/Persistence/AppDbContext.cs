using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Riber.Infrastructure.Persistence.Identity;
using Riber.Infrastructure.Persistence.Seeders;

namespace Riber.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        Guid,
        ApplicationUserClaim,
        ApplicationUserRole,
        ApplicationUserLogin,
        ApplicationRoleClaim,
        ApplicationUserToken
    >(options)
{

    /// <summary>
    /// Procura por todas as classes que implementam a interface <see cref="IEntityTypeConfiguration{T}"/>
    /// e aplica os mapeamentos para o banco!
    /// </summary>
    /// <param name="builder"></param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        builder.HasPostgresExtension("vector");
        builder.ApplyPermissionsSeed();
        builder.ApplyRoleSeeder();
        builder.ApplyRoleClaimSeeder();
    }
}