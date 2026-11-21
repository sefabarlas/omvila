using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PublicBookingTrustedDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "branch_id",
                schema: "identity",
                table: "otp_challenges",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "trusted_devices",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    phone_e164 = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    token_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    verified_otp_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    expires_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    last_used_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trusted_devices", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_trusted_devices_phone_e164_expires_utc",
                schema: "identity",
                table: "trusted_devices",
                columns: new[] { "phone_e164", "expires_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_trusted_devices_token_hash",
                schema: "identity",
                table: "trusted_devices",
                column: "token_hash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trusted_devices",
                schema: "identity");

            migrationBuilder.DropColumn(
                name: "branch_id",
                schema: "identity",
                table: "otp_challenges");
        }
    }
}
