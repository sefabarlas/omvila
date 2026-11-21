using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Omvila.Infrastructure.Persistence;

#nullable disable

namespace Omvila.Modules.Notifications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InAppNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "in_app_notifications",
                schema: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    title = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    body = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: true),
                    target_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    target_id = table.Column<Guid>(type: "uuid", nullable: true),
                    hold_expires_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    read_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_in_app_notifications", x => x.id);
                    table.CheckConstraint("ck_in_app_notifications_target_type", "target_type IN ('Appointment', 'Calendar', 'Today', 'Deposit', 'Reviews', 'WalkInQueue', 'SmsCredits', 'Subscription')");
                });

            migrationBuilder.EnableTenantIsolation("notifications", "in_app_notifications");

            migrationBuilder.CreateIndex(
                name: "ix_in_app_notifications_brand_id_user_id_created_utc_id",
                schema: "notifications",
                table: "in_app_notifications",
                columns: new[] { "brand_id", "user_id", "created_utc", "id" },
                descending: new[] { false, false, true, true });

            migrationBuilder.CreateIndex(
                name: "ix_in_app_notifications_holds",
                schema: "notifications",
                table: "in_app_notifications",
                columns: new[] { "brand_id", "target_id" },
                filter: "hold_expires_utc IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_in_app_notifications_unread",
                schema: "notifications",
                table: "in_app_notifications",
                columns: new[] { "brand_id", "user_id" },
                filter: "read_utc IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "in_app_notifications",
                schema: "notifications");
        }
    }
}
