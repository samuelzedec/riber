using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Riber.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNewPermissionsForAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 31,
                columns: new[] { "claim_value", "role_id" },
                values: new object[] { "categories.create", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") });

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 32,
                columns: new[] { "claim_value", "role_id" },
                values: new object[] { "categories.read", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") });

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 33,
                columns: new[] { "claim_value", "role_id" },
                values: new object[] { "categories.update", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") });

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 34,
                columns: new[] { "claim_value", "role_id" },
                values: new object[] { "categories.delete", new Guid("72bf32a9-69e8-4a57-936b-c6b23c47216d") });

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 35,
                column: "claim_value",
                value: "products.create");

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 36,
                column: "claim_value",
                value: "products.read");

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 37,
                column: "claim_value",
                value: "products.update");

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 38,
                column: "claim_value",
                value: "products.import");

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 39,
                column: "claim_value",
                value: "users.create");

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 40,
                column: "claim_value",
                value: "users.read");

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 41,
                column: "claim_value",
                value: "users.update");

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 42,
                column: "claim_value",
                value: "users.assign_roles");

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 43,
                column: "claim_value",
                value: "reports.view");

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 44,
                column: "claim_value",
                value: "reports.export");

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 45,
                columns: new[] { "claim_value", "role_id" },
                values: new object[] { "reports.schedule", new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878") });

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 46,
                columns: new[] { "claim_value", "role_id" },
                values: new object[] { "roles.read", new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878") });

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 47,
                columns: new[] { "claim_value", "role_id" },
                values: new object[] { "roles.update", new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878") });

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 48,
                columns: new[] { "claim_value", "role_id" },
                values: new object[] { "roles.assign_permissions", new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878") });

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 49,
                column: "claim_value",
                value: "orders.create");

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 50,
                column: "role_id",
                value: new Guid("5b20150c-817c-4020-bb91-59d29f732a32"));

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 51,
                columns: new[] { "claim_value", "role_id" },
                values: new object[] { "orders.update", new Guid("5b20150c-817c-4020-bb91-59d29f732a32") });

            migrationBuilder.InsertData(
                table: "aspnet_role_claim",
                columns: new[] { "id", "claim_type", "claim_value", "role_id" },
                values: new object[,]
                {
                    { 52, "permission", "products.read", new Guid("5b20150c-817c-4020-bb91-59d29f732a32") },
                    { 53, "permission", "reports.view", new Guid("5b20150c-817c-4020-bb91-59d29f732a32") },
                    { 54, "permission", "orders.read", new Guid("f9bb36fe-9ac3-4cad-9a37-b90eab601cf5") },
                    { 55, "permission", "products.read", new Guid("f9bb36fe-9ac3-4cad-9a37-b90eab601cf5") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 55);

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 31,
                columns: new[] { "claim_value", "role_id" },
                values: new object[] { "products.create", new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878") });

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 32,
                columns: new[] { "claim_value", "role_id" },
                values: new object[] { "products.read", new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878") });

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 33,
                columns: new[] { "claim_value", "role_id" },
                values: new object[] { "products.update", new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878") });

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 34,
                columns: new[] { "claim_value", "role_id" },
                values: new object[] { "products.import", new Guid("2a74bf8e-0be3-46cc-9310-fdd5f80bd878") });

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 35,
                column: "claim_value",
                value: "users.create");

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 36,
                column: "claim_value",
                value: "users.read");

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 37,
                column: "claim_value",
                value: "users.update");

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 38,
                column: "claim_value",
                value: "users.assign_roles");

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 39,
                column: "claim_value",
                value: "reports.view");

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 40,
                column: "claim_value",
                value: "reports.export");

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 41,
                column: "claim_value",
                value: "reports.schedule");

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 42,
                column: "claim_value",
                value: "roles.read");

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 43,
                column: "claim_value",
                value: "roles.update");

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 44,
                column: "claim_value",
                value: "roles.assign_permissions");

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 45,
                columns: new[] { "claim_value", "role_id" },
                values: new object[] { "orders.create", new Guid("5b20150c-817c-4020-bb91-59d29f732a32") });

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 46,
                columns: new[] { "claim_value", "role_id" },
                values: new object[] { "orders.read", new Guid("5b20150c-817c-4020-bb91-59d29f732a32") });

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 47,
                columns: new[] { "claim_value", "role_id" },
                values: new object[] { "orders.update", new Guid("5b20150c-817c-4020-bb91-59d29f732a32") });

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 48,
                columns: new[] { "claim_value", "role_id" },
                values: new object[] { "products.read", new Guid("5b20150c-817c-4020-bb91-59d29f732a32") });

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 49,
                column: "claim_value",
                value: "reports.view");

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 50,
                column: "role_id",
                value: new Guid("f9bb36fe-9ac3-4cad-9a37-b90eab601cf5"));

            migrationBuilder.UpdateData(
                table: "aspnet_role_claim",
                keyColumn: "id",
                keyValue: 51,
                columns: new[] { "claim_value", "role_id" },
                values: new object[] { "products.read", new Guid("f9bb36fe-9ac3-4cad-9a37-b90eab601cf5") });
        }
    }
}
