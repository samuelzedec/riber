using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Riber.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriesSeeders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "application_permissions",
                columns: new[] { "id", "category", "Description", "name" },
                values: new object[,]
                {
                    { 306, "Categories", "Criar categorias", "categories.create" },
                    { 307, "Categories", "Visualizar categorias", "categories.read" },
                    { 308, "Categories", "Editar categorias", "categories.update" },
                    { 309, "Categories", "Remover categorias", "categories.delete" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 306);

            migrationBuilder.DeleteData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 307);

            migrationBuilder.DeleteData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 308);

            migrationBuilder.DeleteData(
                table: "application_permissions",
                keyColumn: "id",
                keyValue: 309);
        }
    }
}
