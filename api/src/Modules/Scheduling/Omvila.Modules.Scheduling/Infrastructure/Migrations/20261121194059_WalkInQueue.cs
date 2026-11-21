using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Omvila.Infrastructure.Persistence;

#nullable disable

namespace Omvila.Modules.Scheduling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WalkInQueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "branch_queue_counters",
                schema: "scheduling",
                columns: table => new
                {
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    queue_date = table.Column<LocalDate>(type: "date", nullable: false),
                    last_number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_branch_queue_counters", x => new { x.brand_id, x.branch_id, x.queue_date });
                    table.CheckConstraint("ck_branch_queue_counters_last_number", "last_number >= 1");
                });

            migrationBuilder.CreateTable(
                name: "walk_in_tickets",
                schema: "scheduling",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    queue_date = table.Column<LocalDate>(type: "date", nullable: false),
                    queue_number = table.Column<int>(type: "integer", nullable: false),
                    brand_customer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    customer_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    phone_e164 = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    service_id = table.Column<Guid>(type: "uuid", nullable: true),
                    service_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    service_minutes = table.Column<int>(type: "integer", nullable: true),
                    preferred_staff_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    joined_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    called_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    closed_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    appointment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    almost_turn_notified_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_walk_in_tickets", x => x.id);
                    table.CheckConstraint("ck_walk_in_tickets_queue_number", "queue_number >= 1");
                    table.CheckConstraint("ck_walk_in_tickets_served", "status <> 'Served' OR appointment_id IS NOT NULL");
                    table.CheckConstraint("ck_walk_in_tickets_status", "status IN ('Waiting', 'Called', 'Served', 'Left')");
                });

            migrationBuilder.CreateIndex(
                name: "ix_walk_in_tickets_brand_id_branch_id_queue_date_queue_number",
                schema: "scheduling",
                table: "walk_in_tickets",
                columns: new[] { "brand_id", "branch_id", "queue_date", "queue_number" },
                unique: true);

            migrationBuilder.EnableTenantIsolation("scheduling", "branch_queue_counters");
            migrationBuilder.EnableTenantIsolation("scheduling", "walk_in_tickets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "branch_queue_counters",
                schema: "scheduling");

            migrationBuilder.DropTable(
                name: "walk_in_tickets",
                schema: "scheduling");
        }
    }
}
