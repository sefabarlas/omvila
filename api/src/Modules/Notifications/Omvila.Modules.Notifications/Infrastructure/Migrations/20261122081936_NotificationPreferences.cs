using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Omvila.Infrastructure.Persistence;

#nullable disable

namespace Omvila.Modules.Notifications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NotificationPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notification_preferences",
                schema: "notifications",
                columns: table => new
                {
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    new_online_appointment = table.Column<bool>(type: "boolean", nullable: false),
                    cancellation_and_reschedule = table.Column<bool>(type: "boolean", nullable: false),
                    customer_checked_in = table.Column<bool>(type: "boolean", nullable: false),
                    walk_in_queue = table.Column<bool>(type: "boolean", nullable: false),
                    new_review = table.Column<bool>(type: "boolean", nullable: false),
                    daily_summary = table.Column<bool>(type: "boolean", nullable: false),
                    quiet_hours_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    updated_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_preferences", x => new { x.brand_id, x.user_id });
                });

            migrationBuilder.EnableTenantIsolation("notifications", "notification_preferences");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notification_preferences",
                schema: "notifications");
        }
    }
}
