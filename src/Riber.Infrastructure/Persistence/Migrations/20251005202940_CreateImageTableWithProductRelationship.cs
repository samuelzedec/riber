using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riber.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateImageTableWithProductRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image_url",
                table: "product");

            migrationBuilder.AddColumn<Guid>(
                name: "image_id",
                table: "product",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "product",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateTable(
                name: "image",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    should_delete = table.Column<bool>(type: "boolean", nullable: false),
                    length = table.Column<long>(type: "bigint", nullable: false),
                    content_type = table.Column<string>(type: "text", nullable: false),
                    marked_for_deletion_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    original_name = table.Column<string>(type: "text", nullable: false),
                    key = table.Column<string>(type: "text", nullable: false),
                    extension = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_image_id", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_product_image_id",
                table: "product",
                column: "image_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_product_image_id",
                table: "product",
                column: "image_id",
                principalTable: "image",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_product_image_id",
                table: "product");

            migrationBuilder.DropTable(
                name: "image");

            migrationBuilder.DropIndex(
                name: "IX_product_image_id",
                table: "product");

            migrationBuilder.DropColumn(
                name: "image_id",
                table: "product");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "product");

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "product",
                type: "text",
                nullable: true);
        }
    }
}
