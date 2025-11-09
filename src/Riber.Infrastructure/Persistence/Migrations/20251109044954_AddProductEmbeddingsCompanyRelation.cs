using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riber.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductEmbeddingsCompanyRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                table: "product_embeddings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_product_embeddings_company_id",
                table: "product_embeddings",
                column: "company_id");

            migrationBuilder.AddForeignKey(
                name: "fk_product_embeddings_company_id",
                table: "product_embeddings",
                column: "company_id",
                principalTable: "company",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_product_embeddings_company_id",
                table: "product_embeddings");

            migrationBuilder.DropIndex(
                name: "IX_product_embeddings_company_id",
                table: "product_embeddings");

            migrationBuilder.DropColumn(
                name: "company_id",
                table: "product_embeddings");
        }
    }
}
