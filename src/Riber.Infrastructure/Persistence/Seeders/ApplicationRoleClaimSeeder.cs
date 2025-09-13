using Microsoft.EntityFrameworkCore;
using Riber.Infrastructure.Persistence.Identity;

namespace Riber.Infrastructure.Persistence.Seeders;

public static class ApplicationRoleClaimSeeder
{
    public static void ApplyRoleClaimSeeder(this ModelBuilder builder)
    {
        var adminRoleId = new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d");
        var managerRoleId = new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878");
        var employeeRoleId = new Guid("5b20150c-817c-4020-bb91-59d29f732a32");
        var viewerRoleId = new Guid("f9bb36fe-9ac3-4cad-9a37-b90eab601cf5");

        var rolePermissions = new Dictionary<Guid, List<string>>
        {
            { adminRoleId, GetAdminPermissions() },
            { managerRoleId, GetManagerPermissions() },
            { employeeRoleId, GetEmployeePermissions() },
            { viewerRoleId, GetViewerPermissions() }
        };

        var roleClaims = rolePermissions
            .SelectMany(rp => rp.Value.Select(permission => new { RoleId = rp.Key, Permission = permission }))
            .Select((item, index) => new ApplicationRoleClaim
            {
                Id = index + 1,
                RoleId = item.RoleId,
                ClaimType = "permission",
                ClaimValue = item.Permission
            })
            .ToList();
        
        builder
            .Entity<ApplicationRoleClaim>()
            .HasData(roleClaims);
    }

    private static List<string> GetAdminPermissions()
        => [
            "companies.create", "companies.read", "companies.update", "companies.delete", "companies.manage_users",
            "orders.create", "orders.read", "orders.update", "orders.delete", "orders.approve",
            "products.create", "products.read", "products.update", "products.delete", "products.import",
            "users.create", "users.read", "users.update", "users.delete", "users.assign_roles",
            "reports.view", "reports.export", "reports.schedule",
            "settings.view", "settings.update",
            "roles.create", "roles.read", "roles.update", "roles.delete", "roles.assign_permissions"
        ];

    private static List<string> GetManagerPermissions()
        => [
            "products.create", "products.read", "products.update", "products.import",
            "users.create", "users.read", "users.update", "users.assign_roles",
            "reports.view", "reports.export", "reports.schedule",
            "roles.read", "roles.update", "roles.assign_permissions"
        ];

    private static List<string> GetEmployeePermissions()
        => [
            "orders.create", "orders.read", "orders.update",
            "products.read",
            "reports.view"
        ];

    private static List<string> GetViewerPermissions()
        => [ "orders.read", "products.read"];
}