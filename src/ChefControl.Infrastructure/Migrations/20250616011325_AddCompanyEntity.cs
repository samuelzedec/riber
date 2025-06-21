using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChefControl.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "company",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", maxLength: 150, nullable: false),
                    trading_name = table.Column<string>(type: "text", maxLength: 100, nullable: false),
                    value = table.Column<string>(type: "text", maxLength: 14, nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "text", maxLength: 15, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    Deleted_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_company_id", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "uq_company_email",
                table: "company",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_company_name",
                table: "company",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "uq_company_phone",
                table: "company",
                column: "phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_company_tax_id",
                table: "company",
                column: "value",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "company");
        }
    }
}
