using Microsoft.EntityFrameworkCore;
using SnackFlow.Infrastructure.Persistence.Identity;

namespace SnackFlow.Infrastructure.Persistence.Seeders;

/// <summary>
/// Fornece funcionalidade para semear funções predefinidas do aplicativo no banco de dados de identidade.
/// </summary>
/// <remarks>
/// Esta classe é responsável por adicionar funções padrão, como "SuperAdmin" e "User",
/// para garantir que as funções do aplicativo sejam inicializadas adequadamente no esquema do banco de dados.
/// As funções são semeadas durante o processo de construção do modelo no fluxo de trabalho do Entity Framework.
/// </remarks>
public static class ApplicationRoleSeeder
{
    public static void ApplyRoleSeeder(this ModelBuilder builder)
    {
        List<ApplicationRole> roles =
        [
            new()
            {
                Id = Guid.Parse("2a74bf8e-0be3-46cc-9310-fdd5f80bd878"),
                Name = "SuperAdmin",
                NormalizedName = "SUPERADMIN"
            },
            new()
            {
                Id = Guid.Parse("5b20150c-817c-4020-bb91-59d29f732a32"),
                Name = "User",
                NormalizedName = "USER"
            }
        ];
    
        builder
            .Entity<ApplicationRole>()
            .HasData(roles);
    }
}