using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SnackFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedersForRoleClaimAndRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
