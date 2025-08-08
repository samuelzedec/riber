using Microsoft.EntityFrameworkCore;
using SnackFlow.Infrastructure.Persistence.Identity;

namespace SnackFlow.Infrastructure.Persistence.Seeders;

/// <summary>
/// Fornece funcionalidade para semear dados de permissões do aplicativo no construtor do modelo.
/// </summary>
public static class ApplicationPermissionSeeder
{
    public static void ApplyPermissionsSeed(this ModelBuilder builder)
    {
        List<ApplicationPermission> permissions =
        [
            // Companies - 1xx
            new() { Id = 101, Name = "companies.create", Description = "Criar empresas", Category = "Companies" },
            new() { Id = 102, Name = "companies.read", Description = "Ver empresas", Category = "Companies" },
            new() { Id = 103, Name = "companies.update", Description = "Editar empresas", Category = "Companies" },
            new() { Id = 104, Name = "companies.delete", Description = "Excluir empresas", Category = "Companies" },
            new() { Id = 105, Name = "companies.manage_users", Description = "Gerenciar usuários da empresa", Category = "Companies" },

            // Orders - 2xx  
            new() { Id = 201, Name = "orders.create", Description = "Criar pedidos", Category = "Orders" },
            new() { Id = 202, Name = "orders.read", Description = "Ver pedidos", Category = "Orders" },
            new() { Id = 203, Name = "orders.update", Description = "Editar pedidos", Category = "Orders" },
            new() { Id = 204, Name = "orders.delete", Description = "Excluir pedidos", Category = "Orders" },
            new() { Id = 205, Name = "orders.approve", Description = "Aprovar pedidos", Category = "Orders" },

            // Products - 3xx
            new() { Id = 301, Name = "products.create", Description = "Cadastrar produtos", Category = "Products" },
            new() { Id = 302, Name = "products.read", Description = "Visualizar produtos", Category = "Products" },
            new() { Id = 303, Name = "products.update", Description = "Editar produtos", Category = "Products" },
            new() { Id = 304, Name = "products.delete", Description = "Remover produtos", Category = "Products" },
            new() { Id = 305, Name = "products.import", Description = "Importar produtos", Category = "Products" },

            // Users - 4xx
            new() { Id = 401, Name = "users.create", Description = "Criar usuários", Category = "Users" },
            new() { Id = 402, Name = "users.read", Description = "Visualizar usuários", Category = "Users" },
            new() { Id = 403, Name = "users.update", Description = "Editar usuários", Category = "Users" },
            new() { Id = 404, Name = "users.delete", Description = "Remover usuários", Category = "Users" },
            new() { Id = 405, Name = "users.assign_roles", Description = "Atribuir funções aos usuários", Category = "Users" },

            // Reports - 5xx
            new() { Id = 501, Name = "reports.view", Description = "Visualizar relatórios", Category = "Reports" },
            new() { Id = 502, Name = "reports.export", Description = "Exportar relatórios", Category = "Reports" },
            new() { Id = 503, Name = "reports.schedule", Description = "Agendar relatórios", Category = "Reports" },

            // Settings - 6xx
            new() { Id = 601, Name = "settings.view", Description = "Visualizar configurações", Category = "Settings" },
            new() { Id = 602, Name = "settings.update", Description = "Alterar configurações", Category = "Settings" },

            // Roles - 7xx
            new() { Id = 701, Name = "roles.create", Description = "Criar perfis de acesso", Category = "Roles" },
            new() { Id = 702, Name = "roles.read", Description = "Visualizar perfis de acesso", Category = "Roles" },
            new() { Id = 703, Name = "roles.update", Description = "Editar perfis de acesso", Category = "Roles" },
            new() { Id = 704, Name = "roles.delete", Description = "Excluir perfis de acesso", Category = "Roles" },
            new() { Id = 705, Name = "roles.assign_permissions", Description = "Atribuir permissões aos perfis", Category = "Roles" }
        ];
        
        builder
            .Entity<ApplicationPermission>()
            .HasData(permissions);
    }
}