using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Omvila.Modules.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PlatformUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_refresh_tokens_audience",
                schema: "identity",
                table: "refresh_tokens");

            migrationBuilder.DropCheckConstraint(
                name: "ck_otp_challenges_purpose",
                schema: "identity",
                table: "otp_challenges");

            migrationBuilder.AddColumn<string>(
                name: "platform_role",
                schema: "identity",
                table: "users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_users_platform_role",
                schema: "identity",
                table: "users",
                sql: "platform_role IS NULL OR platform_role IN ('PlatformAdmin', 'PlatformSupport', 'PlatformFinance')");

            migrationBuilder.AddCheckConstraint(
                name: "ck_refresh_tokens_audience",
                schema: "identity",
                table: "refresh_tokens",
                sql: "audience IN ('manage', 'me', 'admin')");

            migrationBuilder.AddCheckConstraint(
                name: "ck_otp_challenges_purpose",
                schema: "identity",
                table: "otp_challenges",
                sql: "purpose IN ('ManageLogin', 'ManageSignup', 'CustomerLogin', 'PublicBooking', 'AccountDeletion', 'AdminLogin')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_users_platform_role",
                schema: "identity",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "ck_refresh_tokens_audience",
                schema: "identity",
                table: "refresh_tokens");

            migrationBuilder.DropCheckConstraint(
                name: "ck_otp_challenges_purpose",
                schema: "identity",
                table: "otp_challenges");

            migrationBuilder.DropColumn(
                name: "platform_role",
                schema: "identity",
                table: "users");

            migrationBuilder.AddCheckConstraint(
                name: "ck_refresh_tokens_audience",
                schema: "identity",
                table: "refresh_tokens",
                sql: "audience IN ('manage', 'me')");

            migrationBuilder.AddCheckConstraint(
                name: "ck_otp_challenges_purpose",
                schema: "identity",
                table: "otp_challenges",
                sql: "purpose IN ('ManageLogin', 'ManageSignup', 'CustomerLogin', 'PublicBooking', 'AccountDeletion')");
        }
    }
}
