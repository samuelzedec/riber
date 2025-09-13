using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Riber.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionTableWithSeeders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "application_permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "varchar", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    category = table.Column<string>(type: "varchar", maxLength: 100, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_permissions_id", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "application_permissions",
                columns: new[] { "id", "category", "Description", "name" },
                values: new object[,]
                {
                    { 101, "Companies", "Criar empresas", "companies.create" },
                    { 102, "Companies", "Ver empresas", "companies.read" },
                    { 103, "Companies", "Editar empresas", "companies.update" },
                    { 104, "Companies", "Excluir empresas", "companies.delete" },
                    { 105, "Companies", "Gerenciar usuários da empresa", "companies.manage_users" },
                    { 201, "Orders", "Criar pedidos", "orders.create" },
                    { 202, "Orders", "Ver pedidos", "orders.read" },
                    { 203, "Orders", "Editar pedidos", "orders.update" },
                    { 204, "Orders", "Excluir pedidos", "orders.delete" },
                    { 205, "Orders", "Aprovar pedidos", "orders.approve" },
                    { 301, "Products", "Cadastrar produtos", "products.create" },
                    { 302, "Products", "Visualizar produtos", "products.read" },
                    { 303, "Products", "Editar produtos", "products.update" },
                    { 304, "Products", "Remover produtos", "products.delete" },
                    { 305, "Products", "Importar produtos", "products.import" },
                    { 401, "Users", "Criar usuários", "users.create" },
                    { 402, "Users", "Visualizar usuários", "users.read" },
                    { 403, "Users", "Editar usuários", "users.update" },
                    { 404, "Users", "Remover usuários", "users.delete" },
                    { 405, "Users", "Atribuir funções aos usuários", "users.assign_roles" },
                    { 501, "Reports", "Visualizar relatórios", "reports.view" },
                    { 502, "Reports", "Exportar relatórios", "reports.export" },
                    { 503, "Reports", "Agendar relatórios", "reports.schedule" },
                    { 601, "Settings", "Visualizar configurações", "settings.view" },
                    { 602, "Settings", "Alterar configurações", "settings.update" },
                    { 701, "Roles", "Criar perfis de acesso", "roles.create" },
                    { 702, "Roles", "Visualizar perfis de acesso", "roles.read" },
                    { 703, "Roles", "Editar perfis de acesso", "roles.update" },
                    { 704, "Roles", "Excluir perfis de acesso", "roles.delete" },
                    { 705, "Roles", "Atribuir permissões aos perfis", "roles.assign_permissions" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_permissions");
        }
    }
}
