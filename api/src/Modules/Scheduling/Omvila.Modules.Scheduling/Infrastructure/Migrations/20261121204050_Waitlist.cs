using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Omvila.Infrastructure.Persistence;

#nullable disable

namespace Omvila.Modules.Scheduling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Waitlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "waitlist_entries",
                schema: "scheduling",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_ids = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    preferred_staff_id = table.Column<Guid>(type: "uuid", nullable: true),
                    accept_other_staff = table.Column<bool>(type: "boolean", nullable: false),
                    dates = table.Column<List<LocalDate>>(type: "date[]", nullable: false),
                    time_windows = table.Column<string[]>(type: "text[]", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    source = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    access_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    joined_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    updated_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    closed_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_waitlist_entries", x => x.id);
                    table.CheckConstraint("ck_waitlist_entries_access_code", "access_code ~ '^[0-9A-Za-z]{10}$'");
                    table.CheckConstraint("ck_waitlist_entries_closed", "(status = 'Waiting') = (closed_utc IS NULL)");
                    table.CheckConstraint("ck_waitlist_entries_services", "cardinality(service_ids) BETWEEN 1 AND 5");
                    table.CheckConstraint("ck_waitlist_entries_source", "source IN ('Online', 'Panel')");
                    table.CheckConstraint("ck_waitlist_entries_status", "status IN ('Waiting', 'Converted', 'Cancelled', 'Expired')");
                });

            migrationBuilder.CreateTable(
                name: "waitlist_offers",
                schema: "scheduling",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    waitlist_entry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    offered_staff_id = table.Column<Guid>(type: "uuid", nullable: false),
                    offered_start_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    offered_end_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    hold_expires_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    access_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    offered_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    offered_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    responded_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    appointment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_waitlist_offers", x => x.id);
                    table.CheckConstraint("ck_waitlist_offers_access_code", "access_code ~ '^[0-9A-Za-z]{10}$'");
                    table.CheckConstraint("ck_waitlist_offers_appointment", "(status = 'Accepted') = (appointment_id IS NOT NULL)");
                    table.CheckConstraint("ck_waitlist_offers_status", "status IN ('Pending', 'Accepted', 'Declined', 'Expired', 'Withdrawn')");
                    table.CheckConstraint("ck_waitlist_offers_time", "offered_end_utc > offered_start_utc AND hold_expires_utc > offered_utc");
                    table.ForeignKey(
                        name: "fk_waitlist_offers_appointments_appointment_id",
                        column: x => x.appointment_id,
                        principalSchema: "scheduling",
                        principalTable: "appointments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_waitlist_offers_waitlist_entries_waitlist_entry_id",
                        column: x => x.waitlist_entry_id,
                        principalSchema: "scheduling",
                        principalTable: "waitlist_entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_waitlist_entries_access_code",
                schema: "scheduling",
                table: "waitlist_entries",
                column: "access_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_waitlist_entries_brand_id_branch_id_status_joined_utc",
                schema: "scheduling",
                table: "waitlist_entries",
                columns: new[] { "brand_id", "branch_id", "status", "joined_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_waitlist_entries_brand_id_brand_customer_id",
                schema: "scheduling",
                table: "waitlist_entries",
                columns: new[] { "brand_id", "brand_customer_id" });

            migrationBuilder.CreateIndex(
                name: "ix_waitlist_offers_access_code",
                schema: "scheduling",
                table: "waitlist_offers",
                column: "access_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_waitlist_offers_appointment_id",
                schema: "scheduling",
                table: "waitlist_offers",
                column: "appointment_id");

            migrationBuilder.CreateIndex(
                name: "ix_waitlist_offers_hold_candidates",
                schema: "scheduling",
                table: "waitlist_offers",
                column: "hold_expires_utc",
                filter: "status = 'Pending'");

            migrationBuilder.CreateIndex(
                name: "ix_waitlist_offers_one_pending",
                schema: "scheduling",
                table: "waitlist_offers",
                column: "waitlist_entry_id",
                unique: true,
                filter: "status = 'Pending'");

            migrationBuilder.CreateIndex(
                name: "ix_waitlist_offers_pending_staff",
                schema: "scheduling",
                table: "waitlist_offers",
                columns: new[] { "brand_id", "offered_staff_id", "offered_start_utc" },
                filter: "status = 'Pending'");

            migrationBuilder.EnableTenantIsolation("scheduling", "waitlist_entries");
            migrationBuilder.EnableTenantIsolation("scheduling", "waitlist_offers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "waitlist_offers",
                schema: "scheduling");

            migrationBuilder.DropTable(
                name: "waitlist_entries",
                schema: "scheduling");
        }
    }
}
