using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Omvila.Infrastructure.Persistence;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Notifications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SmsDispatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Instant>(
                name: "delivered_utc",
                schema: "notifications",
                table: "notification_outbox",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "brand_sms_senders",
                schema: "notifications",
                columns: table => new
                {
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    approved_name = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    submitted_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    decided_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    rejection_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_sms_senders", x => x.brand_id);
                    table.CheckConstraint("ck_brand_sms_senders_name", "name ~ '^[A-Z0-9]{3,11}$'");
                    table.CheckConstraint("ck_brand_sms_senders_status", "status IN ('Pending', 'Approved', 'Rejected')");
                });

            migrationBuilder.CreateIndex(
                name: "ix_notification_outbox_provider_message_id",
                schema: "notifications",
                table: "notification_outbox",
                column: "provider_message_id",
                filter: "provider_message_id IS NOT NULL");

            // Gönderen adı markaya aittir (ADR-0006).
            migrationBuilder.EnableTenantIsolation("notifications", "brand_sms_senders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "brand_sms_senders",
                schema: "notifications");

            migrationBuilder.DropIndex(
                name: "ix_notification_outbox_provider_message_id",
                schema: "notifications",
                table: "notification_outbox");

            migrationBuilder.DropColumn(
                name: "delivered_utc",
                schema: "notifications",
                table: "notification_outbox");
        }
    }
}
