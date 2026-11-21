using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Omvila.Infrastructure.Persistence;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Notifications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PushDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "recipient_id",
                schema: "notifications",
                table: "notification_outbox",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "devices",
                schema: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    app = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    platform = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    push_token = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    app_version = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    last_seen_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_devices", x => x.id);
                    table.CheckConstraint("ck_devices_app", "app IN ('Business', 'Customer')");
                    table.CheckConstraint("ck_devices_owner_type", "owner_type IN ('User', 'Customer')");
                    table.CheckConstraint("ck_devices_platform", "platform IN ('iOS', 'Android')");
                });

            migrationBuilder.EnableTenantIsolation("notifications", "devices");

            migrationBuilder.CreateIndex(
                name: "ix_devices_brand_id_owner_type_owner_id",
                schema: "notifications",
                table: "devices",
                columns: new[] { "brand_id", "owner_type", "owner_id" },
                filter: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_devices_brand_id_push_token",
                schema: "notifications",
                table: "devices",
                columns: new[] { "brand_id", "push_token" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_devices_push_token",
                schema: "notifications",
                table: "devices",
                column: "push_token",
                unique: true,
                filter: "is_active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "devices",
                schema: "notifications");

            migrationBuilder.DropColumn(
                name: "recipient_id",
                schema: "notifications",
                table: "notification_outbox");
        }
    }
}
