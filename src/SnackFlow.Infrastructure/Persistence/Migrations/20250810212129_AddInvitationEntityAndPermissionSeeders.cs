using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SnackFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInvitationEntityAndPermissionSeeders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_aspnet_user_user_user_domain_id",
                table: "aspnet_user");

            migrationBuilder.DropForeignKey(
                name: "FK_user_company_company_id",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "PK_aspnet_user_claim",
                table: "aspnet_user_claim");

            migrationBuilder.DropPrimaryKey(
                name: "PK_aspnet_user",
                table: "aspnet_user");

            migrationBuilder.DropPrimaryKey(
                name: "PK_aspnet_role",
                table: "aspnet_role");

            migrationBuilder.DeleteData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 205);

            migrationBuilder.DeleteData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 703);

            migrationBuilder.DeleteData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 704);

            migrationBuilder.DeleteData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 705);

            migrationBuilder.RenameIndex(
                name: "ix_aspnet_user_user_domain_id",
                table: "aspnet_user",
                newName: "uq_aspnet_user_user_domain_id");

            migrationBuilder.RenameIndex(
                name: "ix_aspnet_user_normalized_user_name",
                table: "aspnet_user",
                newName: "uq_aspnet_user_normalized_user_name");

            migrationBuilder.RenameIndex(
                name: "ix_asp_net_user_email",
                table: "aspnet_user",
                newName: "uq_asp_net_user_email");

            migrationBuilder.AlterColumn<string>(
                name: "security_stamp",
                table: "aspnet_user",
                type: "varchar(36)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "concurrency_stamp",
                table: "aspnet_user",
                type: "varchar(36)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_aspnet_user_claim_id",
                table: "aspnet_user_claim",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_aspnet_user_id",
                table: "aspnet_user",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_aspnet_role_id",
                table: "aspnet_role",
                column: "id");

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

            migrationBuilder.UpdateData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 101,
                columns: new[] { "Description", "name" },
                values: new object[] { "Ver empresa", "companies.read" });

            migrationBuilder.UpdateData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 102,
                columns: new[] { "Description", "name" },
                values: new object[] { "Editar empresas", "companies.update" });

            migrationBuilder.UpdateData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 103,
                columns: new[] { "Description", "name" },
                values: new object[] { "Excluir empresas", "companies.delete" });

            migrationBuilder.UpdateData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 104,
                columns: new[] { "Description", "name" },
                values: new object[] { "Gerenciar usuários da empresa", "companies.manage_users" });

            migrationBuilder.UpdateData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 602,
                column: "Description",
                value: "Editar configurações");

            migrationBuilder.UpdateData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 701,
                columns: new[] { "Description", "name" },
                values: new object[] { "Visualizar funções", "roles.read" });

            migrationBuilder.UpdateData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 702,
                columns: new[] { "Description", "name" },
                values: new object[] { "Editar funções", "roles.update" });

            migrationBuilder.InsertData(
                table: "aspnet_role",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878"), null, "Manager", "MANAGER" },
                    { new Guid("5b20150c-817c-4020-bb91-59d29f732a32"), null, "Employee", "EMPLOYEE" },
                    { new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d"), null, "Admin", "ADMIN" },
                    { new Guid("f9bb36fe-9ac3-4cad-9a37-b90eab601cf5"), null, "Viewer", "VIEWER" }
                });

            migrationBuilder.InsertData(
                table: "aspnet_role_claim",
                columns: new[] { "id", "claim_type", "claim_value", "role_id" },
                values: new object[,]
                {
                    { 1, "permission", "companies.create", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 2, "permission", "companies.read", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 3, "permission", "companies.update", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 4, "permission", "companies.delete", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 5, "permission", "companies.manage_users", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 6, "permission", "orders.create", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 7, "permission", "orders.read", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 8, "permission", "orders.update", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 9, "permission", "orders.delete", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 10, "permission", "orders.approve", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 11, "permission", "products.create", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 12, "permission", "products.read", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 13, "permission", "products.update", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 14, "permission", "products.delete", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 15, "permission", "products.import", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 16, "permission", "users.create", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 17, "permission", "users.read", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 18, "permission", "users.update", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 19, "permission", "users.delete", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 20, "permission", "users.assign_roles", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 21, "permission", "reports.view", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 22, "permission", "reports.export", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 23, "permission", "reports.schedule", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 24, "permission", "settings.view", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 25, "permission", "settings.update", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 26, "permission", "roles.create", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 27, "permission", "roles.read", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 28, "permission", "roles.update", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 29, "permission", "roles.delete", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 30, "permission", "roles.assign_permissions", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") },
                    { 31, "permission", "products.create", new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878") },
                    { 32, "permission", "products.read", new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878") },
                    { 33, "permission", "products.update", new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878") },
                    { 34, "permission", "products.import", new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878") },
                    { 35, "permission", "users.create", new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878") },
                    { 36, "permission", "users.read", new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878") },
                    { 37, "permission", "users.update", new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878") },
                    { 38, "permission", "users.assign_roles", new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878") },
                    { 39, "permission", "reports.view", new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878") },
                    { 40, "permission", "reports.export", new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878") },
                    { 41, "permission", "reports.schedule", new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878") },
                    { 42, "permission", "roles.read", new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878") },
                    { 43, "permission", "roles.update", new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878") },
                    { 44, "permission", "roles.assign_permissions", new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878") },
                    { 45, "permission", "orders.create", new Guid("5b20150c-817c-4020-bb91-59d29f732a32") },
                    { 46, "permission", "orders.read", new Guid("5b20150c-817c-4020-bb91-59d29f732a32") },
                    { 47, "permission", "orders.update", new Guid("5b20150c-817c-4020-bb91-59d29f732a32") },
                    { 48, "permission", "products.read", new Guid("5b20150c-817c-4020-bb91-59d29f732a32") },
                    { 49, "permission", "reports.view", new Guid("5b20150c-817c-4020-bb91-59d29f732a32") },
                    { 50, "permission", "orders.read", new Guid("f9bb36fe-9ac3-4cad-9a37-b90eab601cf5") },
                    { 51, "permission", "products.read", new Guid("f9bb36fe-9ac3-4cad-9a37-b90eab601cf5") }
                });

            migrationBuilder.CreateIndex(
                name: "uq_asp_net_user_normalized_email",
                table: "aspnet_user",
                column: "normalized_email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_aspnet_user_phone_number",
                table: "aspnet_user",
                column: "phone_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_aspnet_user_user_name",
                table: "aspnet_user",
                column: "user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_application_permission_name",
                table: "application_permissions",
                column: "name",
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

            migrationBuilder.AddForeignKey(
                name: "fk_aspnet_user_user_domain_id",
                table: "aspnet_user",
                column: "user_domain_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_company_id",
                table: "user",
                column: "company_id",
                principalTable: "company",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_aspnet_user_user_domain_id",
                table: "aspnet_user");

            migrationBuilder.DropForeignKey(
                name: "fk_user_company_id",
                table: "user");

            migrationBuilder.DropTable(
                name: "invitations");

            migrationBuilder.DropPrimaryKey(
                name: "pk_aspnet_user_claim_id",
                table: "aspnet_user_claim");

            migrationBuilder.DropPrimaryKey(
                name: "pk_aspnet_user_id",
                table: "aspnet_user");

            migrationBuilder.DropIndex(
                name: "uq_asp_net_user_normalized_email",
                table: "aspnet_user");

            migrationBuilder.DropIndex(
                name: "uq_aspnet_user_phone_number",
                table: "aspnet_user");

            migrationBuilder.DropIndex(
                name: "uq_aspnet_user_user_name",
                table: "aspnet_user");

            migrationBuilder.DropPrimaryKey(
                name: "pk_aspnet_role_id",
                table: "aspnet_role");

            migrationBuilder.DropIndex(
                name: "uq_application_permission_name",
                table: "application_permissions");

            migrationBuilder.DeleteData(
                table: "aspnet_role",
                keyColumn: "id",
                keyValue: new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878"));

            migrationBuilder.DeleteData(
                table: "aspnet_role",
                keyColumn: "id",
                keyValue: new Guid("5b20150c-817c-4020-bb91-59d29f732a32"));

            migrationBuilder.DeleteData(
                table: "aspnet_role",
                keyColumn: "id",
                keyValue: new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d"));

            migrationBuilder.DeleteData(
                table: "aspnet_role",
                keyColumn: "id",
                keyValue: new Guid("f9bb36fe-9ac3-4cad-9a37-b90eab601cf5"));

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 51);

            migrationBuilder.RenameIndex(
                name: "uq_aspnet_user_user_domain_id",
                table: "aspnet_user",
                newName: "ix_aspnet_user_user_domain_id");

            migrationBuilder.RenameIndex(
                name: "uq_aspnet_user_normalized_user_name",
                table: "aspnet_user",
                newName: "ix_aspnet_user_normalized_user_name");

            migrationBuilder.RenameIndex(
                name: "uq_asp_net_user_email",
                table: "aspnet_user",
                newName: "ix_asp_net_user_email");

            migrationBuilder.AlterColumn<string>(
                name: "security_stamp",
                table: "aspnet_user",
                type: "varchar(36)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(36)");

            migrationBuilder.AlterColumn<string>(
                name: "concurrency_stamp",
                table: "aspnet_user",
                type: "varchar(36)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(36)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_aspnet_user_claim",
                table: "aspnet_user_claim",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_aspnet_user",
                table: "aspnet_user",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_aspnet_role",
                table: "aspnet_role",
                column: "id");

            migrationBuilder.UpdateData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 101,
                columns: new[] { "Description", "name" },
                values: new object[] { "Criar empresas", "companies.create" });

            migrationBuilder.UpdateData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 102,
                columns: new[] { "Description", "name" },
                values: new object[] { "Ver empresas", "companies.read" });

            migrationBuilder.UpdateData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 103,
                columns: new[] { "Description", "name" },
                values: new object[] { "Editar empresas", "companies.update" });

            migrationBuilder.UpdateData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 104,
                columns: new[] { "Description", "name" },
                values: new object[] { "Excluir empresas", "companies.delete" });

            migrationBuilder.UpdateData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 602,
                column: "Description",
                value: "Alterar configurações");

            migrationBuilder.UpdateData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 701,
                columns: new[] { "Description", "name" },
                values: new object[] { "Criar perfis de acesso", "roles.create" });

            migrationBuilder.UpdateData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 702,
                columns: new[] { "Description", "name" },
                values: new object[] { "Visualizar perfis de acesso", "roles.read" });

            migrationBuilder.InsertData(
                table: "application_permissions",
                columns: new[] { "id", "category", "Description", "name" },
                values: new object[,]
                {
                    { 105, "Companies", "Gerenciar usuários da empresa", "companies.manage_users" },
                    { 205, "Orders", "Aprovar pedidos", "orders.approve" },
                    { 703, "Roles", "Editar perfis de acesso", "roles.update" },
                    { 704, "Roles", "Excluir perfis de acesso", "roles.delete" },
                    { 705, "Roles", "Atribuir permissões aos perfis", "roles.assign_permissions" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_aspnet_user_user_user_domain_id",
                table: "aspnet_user",
                column: "user_domain_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_company_company_id",
                table: "user",
                column: "company_id",
                principalTable: "company",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
