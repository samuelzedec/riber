using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riber.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSeeders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            INSERT INTO company (id, corporate_name, fantasy_name, tax_id_value, tax_id_type,
                                 email, phone, created_at)
            VALUES ('ba99e7c2-f824-490d-a0fb-6dae78cc0163',
                    'ADMIN LTDA',
                    'Admin Company Test',
                    '55572258032',
                    'IndividualWithCpf',
                    'admin@company.com',
                    '92998648877',
                    NOW());

            INSERT INTO ""user"" (id, full_name, tax_id_value, tax_id_type, position,
                                is_active, company_id, created_at)
            VALUES ('d670c05b-461d-4f06-8b05-288eb4671758', 'Admin User', '55572258032', 'IndividualWithCpf', 'Owner', true,
                    'ba99e7c2-f824-490d-a0fb-6dae78cc0163', NOW()),
                   ('a1c4f6e2-2d6e-4f3b-8e3a-3c9e5f7b8a1b', 'Director User', '51972791095', 'IndividualWithCpf', 'Director', true,
                    NULL, NOW());

            INSERT INTO aspnet_user (id, name, user_name, normalized_user_name, email,
                                     normalized_email, email_confirmed, password_hash,
                                     security_stamp, concurrency_stamp, user_domain_id,
                                     is_deleted, phone_number, phone_number_confirmed,
                                     two_factor_enabled, lockout_enabled, access_failed_count)
            VALUES ('e6fba186-c1e7-4083-90a6-966c421720e5', 'Admin', 'admin123', 'ADMIN123',
                    'admin@user.com', 'ADMIN@USER.COM', true,
                    'AQAAAAIAAYagAAAAEAVkwbhIu3dtsIH8gzDrFSiicMjx3FXDmHd/NCQjBoXzG0CLXs9bbHqQnJdysHVYrw==',
                    gen_random_uuid()::text, gen_random_uuid()::text,
                    'd670c05b-461d-4f06-8b05-288eb4671758',
                    false, '92998648877', true, false, true, 0),
                   ('c819a3a8-37e1-46ab-94df-2186e0170bd1', 'Director', 'director123', 'DIRECTOR123',
                    'director@company.com', 'DIRECTOR@COMPANY.COM', true,
                    'AQAAAAIAAYagAAAAEOKMxE3G6AieoFWFpZZkohz+htIKtl8nhIUPzR2LPSw/iJA7rYGwIXHlz/Kw0P+g1A==',
                    gen_random_uuid()::text, gen_random_uuid()::text,
                    'a1c4f6e2-2d6e-4f3b-8e3a-3c9e5f7b8a1b',
                    false, '92998648878', true, false, true, 0);

            INSERT INTO aspnet_user_role (user_id, role_id)
            VALUES ('e6fba186-c1e7-4083-90a6-966c421720e5',
                    '72bf32a9-69e8-4a57-936b-c6b23c47216d'),
                   ('c819a3a8-37e1-46ab-94df-2186e0170bd1',
                    '2a74bf8e-0be3-46cc-9310-fdd5f80bd878');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM aspnet_user_role WHERE user_id IN ('e6fba186-c1e7-4083-90a6-966c421720e5', 'c819a3a8-37e1-46ab-94df-2186e0170bd1');
                DELETE FROM aspnet_user WHERE id IN ('e6fba186-c1e7-4083-90a6-966c421720e5', 'c819a3a8-37e1-46ab-94df-2186e0170bd1');
                DELETE FROM ""user"" WHERE id IN ('d670c05b-461d-4f06-8b05-288eb4671758', 'a1c4f6e2-2d6e-4f3b-8e3a-3c9e5f7b8a1b');
                DELETE FROM company WHERE id = 'ba99e7c2-f824-490d-a0fb-6dae78cc0163';
            ");
        }
    }
}
