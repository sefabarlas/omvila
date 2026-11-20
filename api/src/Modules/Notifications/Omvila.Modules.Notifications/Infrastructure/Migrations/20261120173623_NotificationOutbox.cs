using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Omvila.Infrastructure.Persistence;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Notifications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NotificationOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "notifications");

            migrationBuilder.GrantSchemaUsageToApp("notifications");

            migrationBuilder.CreateTable(
                name: "notification_outbox",
                schema: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: true),
                    event_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    category = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    recipient_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    recipient_phone = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    channel_plan = table.Column<string[]>(type: "text[]", nullable: false),
                    channel_used = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    rendered_body = table.Column<string>(type: "text", nullable: true),
                    scheduled_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    attempt_count = table.Column<int>(type: "integer", nullable: false),
                    credits_consumed = table.Column<int>(type: "integer", nullable: false),
                    provider_message_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    failure_reason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_outbox", x => x.id);
                    table.CheckConstraint("ck_notification_outbox_category", "category IN ('Transactional', 'Marketing')");
                    table.CheckConstraint("ck_notification_outbox_recipient_type", "recipient_type IN ('Customer', 'User', 'Phone')");
                    table.CheckConstraint("ck_notification_outbox_status", "status IN ('Pending', 'Sent', 'Failed', 'Cancelled', 'Skipped')");
                });

            migrationBuilder.CreateIndex(
                name: "ix_notification_outbox_brand_id_created_utc",
                schema: "notifications",
                table: "notification_outbox",
                columns: new[] { "brand_id", "created_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_notification_outbox_scheduled_utc",
                schema: "notifications",
                table: "notification_outbox",
                column: "scheduled_utc",
                filter: "status = 'Pending'");

            // Platform mesajı (OTP, brand_id boş) her bağlamdan eklenebilir, yalnız platform okur (ADR-0006).
            migrationBuilder.EnableTenantIsolation("notifications", "notification_outbox", allowPlatformRowInsert: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notification_outbox",
                schema: "notifications");
        }
    }
}
