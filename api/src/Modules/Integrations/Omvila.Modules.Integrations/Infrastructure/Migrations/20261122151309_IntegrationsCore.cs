using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Omvila.Infrastructure.Persistence;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Integrations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IntegrationsCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "integrations");

            migrationBuilder.GrantSchemaUsageToApp("integrations");

            migrationBuilder.CreateTable(
                name: "calendar_links",
                schema: "integrations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    staff_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    external_calendar_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    account_email_masked = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    access_token_encrypted = table.Column<byte[]>(type: "bytea", nullable: true),
                    refresh_token_encrypted = table.Column<byte[]>(type: "bytea", nullable: true),
                    token_expires_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    connected_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    last_synced_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    status_changed_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    connect_nonce = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_calendar_links", x => x.id);
                    table.CheckConstraint("ck_calendar_links_provider", "provider IN ('Google')");
                    table.CheckConstraint("ck_calendar_links_status", "status IN ('Active', 'TokenExpired', 'Revoked')");
                });

            migrationBuilder.CreateTable(
                name: "appointment_calendar_syncs",
                schema: "integrations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    calendar_link_id = table.Column<Guid>(type: "uuid", nullable: false),
                    appointment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_event_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    sync_status = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    content_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    event_start_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    last_synced_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_appointment_calendar_syncs", x => x.id);
                    table.CheckConstraint("ck_appointment_calendar_syncs_sync_status", "sync_status IN ('Synced', 'Deleted', 'Failed')");
                    table.ForeignKey(
                        name: "fk_appointment_calendar_syncs_calendar_links_calendar_link_id",
                        column: x => x.calendar_link_id,
                        principalSchema: "integrations",
                        principalTable: "calendar_links",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_appointment_calendar_syncs_brand_id_calendar_link_id_appoin",
                schema: "integrations",
                table: "appointment_calendar_syncs",
                columns: new[] { "brand_id", "calendar_link_id", "appointment_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_appointment_calendar_syncs_calendar_link_id",
                schema: "integrations",
                table: "appointment_calendar_syncs",
                column: "calendar_link_id");

            migrationBuilder.CreateIndex(
                name: "ix_calendar_links_brand_id_staff_id",
                schema: "integrations",
                table: "calendar_links",
                columns: new[] { "brand_id", "staff_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_calendar_links_status",
                schema: "integrations",
                table: "calendar_links",
                column: "status",
                filter: "status = 'Active'");

            migrationBuilder.EnableTenantIsolation("integrations", "calendar_links");
            migrationBuilder.EnableTenantIsolation("integrations", "appointment_calendar_syncs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointment_calendar_syncs",
                schema: "integrations");

            migrationBuilder.DropTable(
                name: "calendar_links",
                schema: "integrations");
        }
    }
}
