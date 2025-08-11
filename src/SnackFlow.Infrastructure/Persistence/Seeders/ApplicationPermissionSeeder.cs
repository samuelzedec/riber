using Microsoft.EntityFrameworkCore;
using SnackFlow.Infrastructure.Persistence.Identity;
using SnackFlow.Infrastructure.Settings;

namespace SnackFlow.Infrastructure.Persistence.Seeders;

public static class ApplicationPermissionSeeder
{
    public static void ApplyPermissionsSeed(this ModelBuilder builder)
    {
        List<ApplicationPermission> permissions =
        [
            // Companies - 1xx
            new() { Id = 101, Name = PermissionsSettings.Companies.Read, Description = "Ver empresa", Category = "Companies" },
            new() { Id = 102, Name = PermissionsSettings.Companies.Update, Description = "Editar empresas", Category = "Companies" },
            new() { Id = 103, Name = PermissionsSettings.Companies.Delete, Description = "Excluir empresas", Category = "Companies" },
            new() { Id = 104, Name = PermissionsSettings.Companies.ManageUsers, Description = "Gerenciar usuários da empresa", Category = "Companies" },

            // Orders - 2xx  
            new() { Id = 201, Name = PermissionsSettings.Orders.Create, Description = "Criar pedidos", Category = "Orders" },
            new() { Id = 202, Name = PermissionsSettings.Orders.Read, Description = "Ver pedidos", Category = "Orders" },
            new() { Id = 203, Name = PermissionsSettings.Orders.Update, Description = "Editar pedidos", Category = "Orders" },
            new() { Id = 204, Name = PermissionsSettings.Orders.Delete, Description = "Excluir pedidos", Category = "Orders" },

            // Products - 3xx
            new() { Id = 301, Name = PermissionsSettings.Products.Create, Description = "Cadastrar produtos", Category = "Products" },
            new() { Id = 302, Name = PermissionsSettings.Products.Read, Description = "Visualizar produtos", Category = "Products" },
            new() { Id = 303, Name = PermissionsSettings.Products.Update, Description = "Editar produtos", Category = "Products" },
            new() { Id = 304, Name = PermissionsSettings.Products.Delete, Description = "Remover produtos", Category = "Products" },
            new() { Id = 305, Name = PermissionsSettings.Products.Import, Description = "Importar produtos", Category = "Products" },

            // Users - 4xx
            new() { Id = 401, Name = PermissionsSettings.Users.Create, Description = "Criar usuários", Category = "Users" },
            new() { Id = 402, Name = PermissionsSettings.Users.Read, Description = "Visualizar usuários", Category = "Users" },
            new() { Id = 403, Name = PermissionsSettings.Users.Update, Description = "Editar usuários", Category = "Users" },
            new() { Id = 404, Name = PermissionsSettings.Users.Delete, Description = "Remover usuários", Category = "Users" },
            new() { Id = 405, Name = PermissionsSettings.Users.AssignRoles, Description = "Atribuir funções aos usuários", Category = "Users" },

            // Reports - 5xx
            new() { Id = 501, Name = PermissionsSettings.Reports.View, Description = "Visualizar relatórios", Category = "Reports" },
            new() { Id = 502, Name = PermissionsSettings.Reports.Export, Description = "Exportar relatórios", Category = "Reports" },
            new() { Id = 503, Name = PermissionsSettings.Reports.Schedule, Description = "Agendar relatórios", Category = "Reports" },

            // Settings - 6xx
            new() { Id = 601, Name = PermissionsSettings.Settings.View, Description = "Visualizar configurações", Category = "Settings" },
            new() { Id = 602, Name = PermissionsSettings.Settings.Update, Description = "Editar configurações", Category = "Settings" },

            // Roles - 7xx
            new() { Id = 701, Name = PermissionsSettings.Roles.Read, Description = "Visualizar funções", Category = "Roles" },
            new() { Id = 702, Name = PermissionsSettings.Roles.Update, Description = "Editar funções", Category = "Roles" },
        ];
        
        builder
            .Entity<ApplicationPermission>()
            .HasData(permissions);
    }
}