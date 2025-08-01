using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnackFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakePhoneNumberUniqueInAspNetUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "UQ_aspnet_user_phone_number",
                table: "aspnet_user",
                column: "phone_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ_aspnet_user_phone_number",
                table: "aspnet_user");
        }
    }
}
