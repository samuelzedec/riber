using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnackFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateProductCatalogAndOrderSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "uq_company_email1",
                table: "invitations",
                newName: "uq_invitations_email");

            migrationBuilder.CreateTable(
                name: "order",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_token = table.Column<string>(type: "text", maxLength: 100, nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttendantId = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_order_attendant_id",
                        column: x => x.AttendantId,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_order_company_id",
                        column: x => x.CompanyId,
                        principalTable: "company",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_category",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", maxLength: 255, nullable: false),
                    code = table.Column<string>(type: "text", maxLength: 3, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_category_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_category_company_id",
                        column: x => x.company_id,
                        principalTable: "company",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", maxLength: 255, nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric", nullable: false),
                    unit_price_currency = table.Column<string>(type: "text", maxLength: 3, nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_category_id",
                        column: x => x.category_id,
                        principalTable: "product_category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_product_company_id",
                        column: x => x.company_id,
                        principalTable: "company",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_item",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    product_name = table.Column<string>(type: "text", maxLength: 255, nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric", nullable: false),
                    unit_price_currency = table.Column<string>(type: "text", maxLength: 3, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    discount_percentage = table.Column<decimal>(type: "numeric(4,2)", nullable: true, defaultValue: 0m),
                    discount_fixed_amount = table.Column<decimal>(type: "numeric", nullable: true, defaultValue: 0m),
                    discount_reason = table.Column<string>(type: "text", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_item_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_order_item_order_id",
                        column: x => x.order_id,
                        principalTable: "order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_order_item_product_id",
                        column: x => x.ProductId,
                        principalTable: "product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_order_AttendantId",
                table: "order",
                column: "AttendantId");

            migrationBuilder.CreateIndex(
                name: "IX_order_CompanyId",
                table: "order",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "uq_order_order_token",
                table: "order",
                column: "order_token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_order_item_order_id",
                table: "order_item",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_item_ProductId",
                table: "order_item",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_product_category_id",
                table: "product",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_company_id",
                table: "product",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_name",
                table: "product",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_product_category_name",
                table: "product_category",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "uq_product_category_company_code",
                table: "product_category",
                columns: new[] { "company_id", "code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_item");

            migrationBuilder.DropTable(
                name: "order");

            migrationBuilder.DropTable(
                name: "product");

            migrationBuilder.DropTable(
                name: "product_category");

            migrationBuilder.RenameIndex(
                name: "uq_invitations_email",
                table: "invitations",
                newName: "uq_company_email1");
        }
    }
}
