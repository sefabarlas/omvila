using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Tenancy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PrivacySettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "custom_privacy_notice_text",
                schema: "tenancy",
                table: "brand_settings",
                type: "character varying(20000)",
                maxLength: 20000,
                nullable: true);

            migrationBuilder.AddColumn<Instant>(
                name: "custom_privacy_notice_updated_utc",
                schema: "tenancy",
                table: "brand_settings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "privacy_notice_mode",
                schema: "tenancy",
                table: "brand_settings",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Standard");

            migrationBuilder.AddColumn<int>(
                name: "transaction_retention_years",
                schema: "tenancy",
                table: "brand_settings",
                type: "integer",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddCheckConstraint(
                name: "ck_brand_settings_privacy_notice_mode",
                schema: "tenancy",
                table: "brand_settings",
                sql: "privacy_notice_mode IN ('Standard', 'Custom')");

            migrationBuilder.AddCheckConstraint(
                name: "ck_brand_settings_transaction_retention_years",
                schema: "tenancy",
                table: "brand_settings",
                sql: "transaction_retention_years IN (3, 5, 10)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_brand_settings_privacy_notice_mode",
                schema: "tenancy",
                table: "brand_settings");

            migrationBuilder.DropCheckConstraint(
                name: "ck_brand_settings_transaction_retention_years",
                schema: "tenancy",
                table: "brand_settings");

            migrationBuilder.DropColumn(
                name: "custom_privacy_notice_text",
                schema: "tenancy",
                table: "brand_settings");

            migrationBuilder.DropColumn(
                name: "custom_privacy_notice_updated_utc",
                schema: "tenancy",
                table: "brand_settings");

            migrationBuilder.DropColumn(
                name: "privacy_notice_mode",
                schema: "tenancy",
                table: "brand_settings");

            migrationBuilder.DropColumn(
                name: "transaction_retention_years",
                schema: "tenancy",
                table: "brand_settings");
        }
    }
}
