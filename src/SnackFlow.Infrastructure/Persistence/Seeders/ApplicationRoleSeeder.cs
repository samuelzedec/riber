using Microsoft.EntityFrameworkCore;
using SnackFlow.Infrastructure.Persistence.Identity;

namespace SnackFlow.Infrastructure.Persistence.Seeders;

public static class ApplicationRoleSeeder
{
    public static void ApplyRoleSeeder(this ModelBuilder builder)
    {
        List<ApplicationRole> roles =
        [
            new()
            {
                Id = new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d"), 
                Name = "Admin", 
                NormalizedName = "ADMIN"
            },
            new()
            {
                Id = new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878"), 
                Name = "Manager", 
                NormalizedName = "MANAGER"
            },
            new()
            {
                Id = new Guid("5b20150c-817c-4020-bb91-59d29f732a32"),
                Name = "Employee",
                NormalizedName = "EMPLOYEE"
            },
            new()
            {
                Id = new Guid("f9bb36fe-9ac3-4cad-9a37-b90eab601cf5"),
                Name = "Viewer",
                NormalizedName = "VIEWER"
            }
        ];
        
        builder
            .Entity<ApplicationRole>()
            .HasData(roles);
    }
}