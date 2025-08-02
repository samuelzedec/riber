using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnackFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixApplicationUserIndexesAndConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_aspnet_user_user_user_domain_id",
                table: "aspnet_user");

            migrationBuilder.DropForeignKey(
                name: "FK_user_company_company_id",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "PK_aspnet_user_claim",
                table: "aspnet_user_claim");

            migrationBuilder.DropPrimaryKey(
                name: "PK_aspnet_user",
                table: "aspnet_user");

            migrationBuilder.DropPrimaryKey(
                name: "PK_aspnet_role",
                table: "aspnet_role");

            migrationBuilder.RenameIndex(
                name: "ix_aspnet_user_user_domain_id",
                table: "aspnet_user",
                newName: "uq_aspnet_user_user_domain_id");

            migrationBuilder.RenameIndex(
                name: "ix_aspnet_user_normalized_user_name",
                table: "aspnet_user",
                newName: "uq_aspnet_user_normalized_user_name");

            migrationBuilder.RenameIndex(
                name: "ix_asp_net_user_email",
                table: "aspnet_user",
                newName: "uq_asp_net_user_email");

            migrationBuilder.AddPrimaryKey(
                name: "pk_aspnet_user_claim_id",
                table: "aspnet_user_claim",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_aspnet_user_id",
                table: "aspnet_user",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_aspnet_role_id",
                table: "aspnet_role",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "uq_asp_net_user_normalized_email",
                table: "aspnet_user",
                column: "normalized_email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_aspnet_user_phone_number",
                table: "aspnet_user",
                column: "phone_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_aspnet_user_user_name",
                table: "aspnet_user",
                column: "user_name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_aspnet_user_user_domain_id",
                table: "aspnet_user",
                column: "user_domain_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_company_id",
                table: "user",
                column: "company_id",
                principalTable: "company",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_aspnet_user_user_domain_id",
                table: "aspnet_user");

            migrationBuilder.DropForeignKey(
                name: "fk_user_company_id",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "pk_aspnet_user_claim_id",
                table: "aspnet_user_claim");

            migrationBuilder.DropPrimaryKey(
                name: "pk_aspnet_user_id",
                table: "aspnet_user");

            migrationBuilder.DropIndex(
                name: "uq_asp_net_user_normalized_email",
                table: "aspnet_user");

            migrationBuilder.DropIndex(
                name: "uq_aspnet_user_phone_number",
                table: "aspnet_user");

            migrationBuilder.DropIndex(
                name: "uq_aspnet_user_user_name",
                table: "aspnet_user");

            migrationBuilder.DropPrimaryKey(
                name: "pk_aspnet_role_id",
                table: "aspnet_role");

            migrationBuilder.RenameIndex(
                name: "uq_aspnet_user_user_domain_id",
                table: "aspnet_user",
                newName: "ix_aspnet_user_user_domain_id");

            migrationBuilder.RenameIndex(
                name: "uq_aspnet_user_normalized_user_name",
                table: "aspnet_user",
                newName: "ix_aspnet_user_normalized_user_name");

            migrationBuilder.RenameIndex(
                name: "uq_asp_net_user_email",
                table: "aspnet_user",
                newName: "ix_asp_net_user_email");

            migrationBuilder.AddPrimaryKey(
                name: "PK_aspnet_user_claim",
                table: "aspnet_user_claim",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_aspnet_user",
                table: "aspnet_user",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_aspnet_role",
                table: "aspnet_role",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_aspnet_user_user_user_domain_id",
                table: "aspnet_user",
                column: "user_domain_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_company_company_id",
                table: "user",
                column: "company_id",
                principalTable: "company",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
