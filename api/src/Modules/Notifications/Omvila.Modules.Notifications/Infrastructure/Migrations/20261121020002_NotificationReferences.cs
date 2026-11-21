using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Omvila.Modules.Notifications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NotificationReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "reference_id",
                schema: "notifications",
                table: "notification_outbox",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reference_type",
                schema: "notifications",
                table: "notification_outbox",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "template_variant",
                schema: "notifications",
                table: "notification_outbox",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_notification_outbox_brand_id_reference_id",
                schema: "notifications",
                table: "notification_outbox",
                columns: new[] { "brand_id", "reference_id" },
                filter: "status = 'Pending' AND reference_id IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_notification_outbox_brand_id_reference_id",
                schema: "notifications",
                table: "notification_outbox");

            migrationBuilder.DropColumn(
                name: "reference_id",
                schema: "notifications",
                table: "notification_outbox");

            migrationBuilder.DropColumn(
                name: "reference_type",
                schema: "notifications",
                table: "notification_outbox");

            migrationBuilder.DropColumn(
                name: "template_variant",
                schema: "notifications",
                table: "notification_outbox");
        }
    }
}
