using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SnackFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabase : Migration
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

            migrationBuilder.CreateTable(
                name: "aspnet_role_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: false),
                    claim_value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnet_role_claim_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "aspnet_roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "text", nullable: false),
                    normalized_name = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "varchar(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnet_role_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "aspnet_user_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: false),
                    claim_value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnet_user_claim_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "aspnet_user_logins",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnet_user_login_user_id_login_provider_provider_key", x => new { x.user_id, x.login_provider, x.provider_key });
                });

            migrationBuilder.CreateTable(
                name: "aspnet_user_roles",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnet_user_role_user_id_role_id", x => new { x.user_id, x.role_id });
                });

            migrationBuilder.CreateTable(
                name: "aspnet_user_tokens",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnet_user_token_user_id_login_provider_name", x => new { x.user_id, x.login_provider, x.name });
                });

            migrationBuilder.CreateTable(
                name: "aspnet_users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "text", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    user_name = table.Column<string>(type: "text", nullable: false),
                    normalized_user_name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    normalized_email = table.Column<string>(type: "text", nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    security_stamp = table.Column<string>(type: "varchar(36)", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "varchar(36)", nullable: false),
                    phone_number = table.Column<string>(type: "varchar(15)", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnet_user_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "companies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    corporate_name = table.Column<string>(type: "text", maxLength: 150, nullable: false),
                    fantasy_name = table.Column<string>(type: "text", maxLength: 100, nullable: false),
                    tax_id_value = table.Column<string>(type: "text", maxLength: 14, nullable: false),
                    tax_id_type = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "text", maxLength: 15, nullable: false),
                    public_token = table.Column<string>(type: "text", maxLength: 64, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_company_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "invitations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "text", maxLength: 255, nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    position = table.Column<string>(type: "text", maxLength: 50, nullable: false),
                    role = table.Column<string>(type: "text", maxLength: 30, nullable: false),
                    permissions = table.Column<string>(type: "text", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_used = table.Column<bool>(type: "boolean", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    invite_token = table.Column<string>(type: "text", maxLength: 64, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invitations_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "text", maxLength: 255, nullable: false),
                    tax_id_value = table.Column<string>(type: "text", maxLength: 14, nullable: false),
                    tax_id_type = table.Column<string>(type: "text", nullable: false),
                    position = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    public_token = table.Column<string>(type: "text", maxLength: 64, nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_application_user_id",
                        column: x => x.ApplicationUserId,
                        principalTable: "aspnet_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
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

            migrationBuilder.InsertData(
                table: "aspnet_roles",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878"), null, "SuperAdmin", "SUPERADMIN" },
                    { new Guid("5b20150c-817c-4020-bb91-59d29f732a32"), null, "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "uq_application_permission_name",
                table: "application_permissions",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_aspnet_role_claim_role_id",
                table: "aspnet_role_claims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_aspnet_role_normalized_name",
                table: "aspnet_roles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_aspnet_user_claim_user_id",
                table: "aspnet_user_claims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_aspnet_user_role_role_id",
                table: "aspnet_user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_aspnet_user_role_user_id",
                table: "aspnet_user_roles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "uq_asp_net_user_email",
                table: "aspnet_users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_asp_net_user_normalized_email",
                table: "aspnet_users",
                column: "normalized_email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_aspnet_user_normalized_user_name",
                table: "aspnet_users",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_aspnet_user_phone_number",
                table: "aspnet_users",
                column: "phone_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_aspnet_user_user_name",
                table: "aspnet_users",
                column: "user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_company_corporate_name",
                table: "companies",
                column: "corporate_name");

            migrationBuilder.CreateIndex(
                name: "uq_company_email",
                table: "companies",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_company_phone",
                table: "companies",
                column: "phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_company_public_token",
                table: "companies",
                column: "public_token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_company_tax_id",
                table: "companies",
                column: "tax_id_value",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_invitations_company_id",
                table: "invitations",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "uq_company_email1",
                table: "invitations",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_invitations_invite_token",
                table: "invitations",
                column: "invite_token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_ApplicationUserId",
                table: "users",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_company_id",
                table: "users",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "uq_user_public_token",
                table: "users",
                column: "public_token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_user_tax_id",
                table: "users",
                column: "tax_id_value",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_permissions");

            migrationBuilder.DropTable(
                name: "aspnet_role_claims");

            migrationBuilder.DropTable(
                name: "aspnet_roles");

            migrationBuilder.DropTable(
                name: "aspnet_user_claims");

            migrationBuilder.DropTable(
                name: "aspnet_user_logins");

            migrationBuilder.DropTable(
                name: "aspnet_user_roles");

            migrationBuilder.DropTable(
                name: "aspnet_user_tokens");

            migrationBuilder.DropTable(
                name: "invitations");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "aspnet_users");

            migrationBuilder.DropTable(
                name: "companies");
        }
    }
}
