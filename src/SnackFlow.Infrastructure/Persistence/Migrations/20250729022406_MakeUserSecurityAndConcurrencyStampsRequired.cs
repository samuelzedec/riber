using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnackFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeUserSecurityAndConcurrencyStampsRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
