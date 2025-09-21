using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riber.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdjusmentLengthProdcutCategoryCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_invitations_id",
                table: "invitations");

            migrationBuilder.RenameTable(
                name: "invitations",
                newName: "invitation");

            migrationBuilder.RenameTable(
                name: "application_permissions",
                newName: "application_permission");

            migrationBuilder.RenameIndex(
                name: "uq_invitations_invite_token",
                table: "invitation",
                newName: "uq_invitation_invite_token");

            migrationBuilder.AddPrimaryKey(
                name: "pk_invitation_id",
                table: "invitation",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_invitation_id",
                table: "invitation");

            migrationBuilder.RenameTable(
                name: "invitation",
                newName: "invitations");

            migrationBuilder.RenameTable(
                name: "application_permission",
                newName: "application_permissions");

            migrationBuilder.RenameIndex(
                name: "uq_invitation_invite_token",
                table: "invitations",
                newName: "uq_invitations_invite_token");

            migrationBuilder.AddPrimaryKey(
                name: "pk_invitations_id",
                table: "invitations",
                column: "id");
        }
    }
}
