using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SnackFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabaseWithCompanyAndUserDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "aspnet_role",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "text", nullable: false),
                    normalized_name = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "varchar(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspnet_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "aspnet_role_claim",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: false),
                    claim_value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnet_role_claim_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "aspnet_user_claim",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: false),
                    claim_value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspnet_user_claim", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "aspnet_user_login",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnet_user_login_user_id_login_provider_provider_key", x => new { x.user_id, x.login_provider, x.provider_key });
                });

            migrationBuilder.CreateTable(
                name: "aspnet_user_role",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnet_user_role_user_id_role_id", x => new { x.user_id, x.role_id });
                });

            migrationBuilder.CreateTable(
                name: "aspnet_user_token",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnet_user_token_user_id_login_provider_name", x => new { x.user_id, x.login_provider, x.name });
                });

            migrationBuilder.CreateTable(
                name: "company",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    corporate_name = table.Column<string>(type: "text", maxLength: 150, nullable: false),
                    fantasy_name = table.Column<string>(type: "text", maxLength: 100, nullable: false),
                    tax_id_value = table.Column<string>(type: "text", maxLength: 14, nullable: false),
                    tax_id_type = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "text", maxLength: 15, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_company_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "text", maxLength: 255, nullable: false),
                    tax_id_value = table.Column<string>(type: "text", maxLength: 14, nullable: false),
                    tax_id_type = table.Column<string>(type: "text", nullable: false),
                    position = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_id", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_company_company_id",
                        column: x => x.company_id,
                        principalTable: "company",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "aspnet_user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "text", nullable: false),
                    user_domain_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    user_name = table.Column<string>(type: "text", nullable: false),
                    normalized_user_name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    normalized_email = table.Column<string>(type: "text", nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    security_stamp = table.Column<string>(type: "varchar(36)", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "varchar(36)", nullable: true),
                    phone_number = table.Column<string>(type: "varchar(15)", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspnet_user", x => x.id);
                    table.ForeignKey(
                        name: "FK_aspnet_user_user_user_domain_id",
                        column: x => x.user_domain_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_aspnet_role_normalized_name",
                table: "aspnet_role",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_aspnet_role_claim_role_id",
                table: "aspnet_role_claim",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_email",
                table: "aspnet_user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_aspnet_user_normalized_user_name",
                table: "aspnet_user",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_aspnet_user_user_domain_id",
                table: "aspnet_user",
                column: "user_domain_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_aspnet_user_claim_user_id",
                table: "aspnet_user_claim",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_aspnet_user_role_role_id",
                table: "aspnet_user_role",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_aspnet_user_role_user_id",
                table: "aspnet_user_role",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "uq_company_corporate_name",
                table: "company",
                column: "corporate_name");

            migrationBuilder.CreateIndex(
                name: "uq_company_email",
                table: "company",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_company_phone",
                table: "company",
                column: "phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_company_tax_id",
                table: "company",
                column: "tax_id_value",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_company_id",
                table: "user",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "uq_user_tax_id",
                table: "user",
                column: "tax_id_value",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "aspnet_role");

            migrationBuilder.DropTable(
                name: "aspnet_role_claim");

            migrationBuilder.DropTable(
                name: "aspnet_user");

            migrationBuilder.DropTable(
                name: "aspnet_user_claim");

            migrationBuilder.DropTable(
                name: "aspnet_user_login");

            migrationBuilder.DropTable(
                name: "aspnet_user_role");

            migrationBuilder.DropTable(
                name: "aspnet_user_token");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "company");
        }
    }
}
