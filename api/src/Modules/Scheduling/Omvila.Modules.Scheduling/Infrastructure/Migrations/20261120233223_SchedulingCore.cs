using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Omvila.Infrastructure.Persistence;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Scheduling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SchedulingCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "scheduling");

            migrationBuilder.GrantSchemaUsageToApp("scheduling");

            // GiST indeksinde uuid için (ADR-0004). Yerelde ve testte api/db/init betiği kurar; burada yoksa kurulur.
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS btree_gist;");

            migrationBuilder.CreateTable(
                name: "appointment_series",
                schema: "scheduling",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recurrence_rule = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    starts_on = table.Column<LocalDate>(type: "date", nullable: false),
                    local_start_time = table.Column<LocalTime>(type: "time", nullable: false),
                    ends_on = table.Column<LocalDate>(type: "date", nullable: true),
                    status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    generated_until = table.Column<LocalDate>(type: "date", nullable: false),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    updated_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    template_items = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_appointment_series", x => x.id);
                    table.CheckConstraint("ck_appointment_series_rule", "recurrence_rule IN ('FREQ=WEEKLY;INTERVAL=1', 'FREQ=WEEKLY;INTERVAL=2', 'FREQ=WEEKLY;INTERVAL=4')");
                    table.CheckConstraint("ck_appointment_series_status", "status IN ('Active', 'Paused', 'Ended')");
                });

            migrationBuilder.CreateTable(
                name: "appointments",
                schema: "scheduling",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    channel = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    client_app = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    start_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    end_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    total_amount_minor = table.Column<long>(type: "bigint", nullable: false),
                    currency_code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    customer_note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    staff_note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    hold_expires_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    hold_reason = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    series_id = table.Column<Guid>(type: "uuid", nullable: true),
                    confirmed_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    checked_in_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    completed_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    no_show_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    cancelled_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    cancelled_by = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    cancelled_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cancellation_reason = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    cancellation_note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    deposit_required_minor = table.Column<long>(type: "bigint", nullable: true),
                    deposit_customer_reported_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    deposit_received_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_appointments", x => x.id);
                    table.CheckConstraint("ck_appointments_amounts", "total_amount_minor >= 0 AND (deposit_required_minor IS NULL OR deposit_required_minor >= 0)");
                    table.CheckConstraint("ck_appointments_cancellation", "status <> 'Cancelled' OR (cancelled_utc IS NOT NULL AND cancelled_by IS NOT NULL AND cancellation_reason IS NOT NULL)");
                    table.CheckConstraint("ck_appointments_cancellation_reason", "cancellation_reason IS NULL OR cancellation_reason IN ('PlanChanged', 'Sick', 'WantsDifferentTime', 'Other', 'CustomerRequested', 'StaffUnavailable', 'DeclinedBySalon', 'ApprovalExpired', 'DepositNotReceived', 'AccountDeleted', 'BrandReadOnly')");
                    table.CheckConstraint("ck_appointments_cancelled_by", "cancelled_by IS NULL OR cancelled_by IN ('Customer', 'Salon', 'System')");
                    table.CheckConstraint("ck_appointments_channel", "channel IN ('Online', 'Panel', 'Phone', 'WalkIn')");
                    table.CheckConstraint("ck_appointments_client_app", "client_app IS NULL OR client_app IN ('BookingPage', 'CustomerApp', 'BusinessApp', 'Panel')");
                    table.CheckConstraint("ck_appointments_hold", "hold_expires_utc IS NULL OR status = 'PendingApproval'");
                    table.CheckConstraint("ck_appointments_hold_reason", "hold_reason IS NULL OR hold_reason IN ('Approval', 'Deposit', 'ApprovalAndDeposit')");
                    table.CheckConstraint("ck_appointments_status", "status IN ('Draft', 'PendingApproval', 'Confirmed', 'CheckedIn', 'Completed', 'Cancelled', 'NoShow')");
                    table.CheckConstraint("ck_appointments_time", "end_utc > start_utc");
                    table.ForeignKey(
                        name: "fk_appointments_appointment_series_series_id",
                        column: x => x.series_id,
                        principalSchema: "scheduling",
                        principalTable: "appointment_series",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "appointment_access_codes",
                schema: "scheduling",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    appointment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    purpose = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    expires_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    revoked_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_appointment_access_codes", x => x.id);
                    table.CheckConstraint("ck_appointment_access_codes_code", "code ~ '^[0-9A-Za-z]{10}$'");
                    table.CheckConstraint("ck_appointment_access_codes_purpose", "purpose IN ('Manage', 'WaitlistOffer', 'Review')");
                    table.ForeignKey(
                        name: "fk_appointment_access_codes_appointments_appointment_id",
                        column: x => x.appointment_id,
                        principalSchema: "scheduling",
                        principalTable: "appointments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "appointment_items",
                schema: "scheduling",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    appointment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    seq_no = table.Column<int>(type: "integer", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    staff_id = table.Column<Guid>(type: "uuid", nullable: true),
                    start_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    end_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    price_minor = table.Column<long>(type: "bigint", nullable: false),
                    staff_surcharge_minor = table.Column<long>(type: "bigint", nullable: false),
                    discount_minor = table.Column<long>(type: "bigint", nullable: false),
                    customer_package_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    service_name_snapshot = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_appointment_items", x => x.id);
                    table.CheckConstraint("ck_appointment_items_amounts", "price_minor >= 0 AND staff_surcharge_minor >= 0 AND staff_surcharge_minor <= price_minor AND discount_minor >= 0 AND discount_minor <= price_minor");
                    table.CheckConstraint("ck_appointment_items_status", "status IN ('Active', 'Completed', 'Cancelled')");
                    table.CheckConstraint("ck_appointment_items_time", "end_utc > start_utc");
                    table.ForeignKey(
                        name: "fk_appointment_items_appointments_appointment_id",
                        column: x => x.appointment_id,
                        principalSchema: "scheduling",
                        principalTable: "appointments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "appointment_item_segments",
                schema: "scheduling",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    appointment_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    seq_no = table.Column<int>(type: "integer", nullable: false),
                    kind = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    staff_id = table.Column<Guid>(type: "uuid", nullable: true),
                    during = table.Column<Interval>(type: "tstzrange", nullable: false),
                    staff_busy = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    superseded_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_appointment_item_segments", x => x.id);
                    table.CheckConstraint("ck_appointment_item_segments_during", "NOT isempty(during) AND lower_inc(during) AND NOT upper_inc(during) AND NOT lower_inf(during) AND NOT upper_inf(during)");
                    table.CheckConstraint("ck_appointment_item_segments_kind", "kind IN ('BufferBefore', 'Stage', 'BufferAfter')");
                    table.CheckConstraint("ck_appointment_item_segments_superseded", "superseded_utc IS NULL OR NOT is_active");
                    table.ForeignKey(
                        name: "fk_appointment_item_segments_appointment_items_appointment_ite",
                        column: x => x.appointment_item_id,
                        principalSchema: "scheduling",
                        principalTable: "appointment_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_appointment_access_codes_appointment_id",
                schema: "scheduling",
                table: "appointment_access_codes",
                column: "appointment_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointment_access_codes_brand_id_appointment_id",
                schema: "scheduling",
                table: "appointment_access_codes",
                columns: new[] { "brand_id", "appointment_id" });

            migrationBuilder.CreateIndex(
                name: "ix_appointment_access_codes_code",
                schema: "scheduling",
                table: "appointment_access_codes",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_appointment_item_segments_appointment_item_id",
                schema: "scheduling",
                table: "appointment_item_segments",
                column: "appointment_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointment_item_segments_brand_id_item_id_seq_no",
                schema: "scheduling",
                table: "appointment_item_segments",
                columns: new[] { "brand_id", "appointment_item_id", "seq_no" },
                unique: true,
                filter: "superseded_utc IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_appointment_item_segments_staff_during",
                schema: "scheduling",
                table: "appointment_item_segments",
                columns: new[] { "staff_id", "during" },
                filter: "staff_busy AND is_active AND staff_id IS NOT NULL")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "ix_appointment_items_appointment_id",
                schema: "scheduling",
                table: "appointment_items",
                column: "appointment_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointment_items_brand_id_appointment_id_seq_no",
                schema: "scheduling",
                table: "appointment_items",
                columns: new[] { "brand_id", "appointment_id", "seq_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_appointment_items_brand_id_staff_id_start_utc",
                schema: "scheduling",
                table: "appointment_items",
                columns: new[] { "brand_id", "staff_id", "start_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_appointment_series_brand_id_branch_id_status",
                schema: "scheduling",
                table: "appointment_series",
                columns: new[] { "brand_id", "branch_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_appointments_brand_id_branch_id_start_utc",
                schema: "scheduling",
                table: "appointments",
                columns: new[] { "brand_id", "branch_id", "start_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_appointments_brand_id_brand_customer_id_start_utc",
                schema: "scheduling",
                table: "appointments",
                columns: new[] { "brand_id", "brand_customer_id", "start_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_appointments_brand_id_series_id",
                schema: "scheduling",
                table: "appointments",
                columns: new[] { "brand_id", "series_id" });

            migrationBuilder.CreateIndex(
                name: "ix_appointments_series_id",
                schema: "scheduling",
                table: "appointments",
                column: "series_id");

            // Bütün randevu tabloları markaya aittir (ADR-0006). Müşteri yüzeyi (me) ve markasız erişim kodu araması
            // (/r/{code}) sonraki parçalarda ele alınır; şimdilik yalnız marka ve platform bağlamı okur.
            migrationBuilder.EnableTenantIsolation("scheduling", "appointment_series");
            migrationBuilder.EnableTenantIsolation("scheduling", "appointments");
            migrationBuilder.EnableTenantIsolation("scheduling", "appointment_items");
            migrationBuilder.EnableTenantIsolation("scheduling", "appointment_item_segments");
            migrationBuilder.EnableTenantIsolation("scheduling", "appointment_access_codes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointment_access_codes",
                schema: "scheduling");

            migrationBuilder.DropTable(
                name: "appointment_item_segments",
                schema: "scheduling");

            migrationBuilder.DropTable(
                name: "appointment_items",
                schema: "scheduling");

            migrationBuilder.DropTable(
                name: "appointments",
                schema: "scheduling");

            migrationBuilder.DropTable(
                name: "appointment_series",
                schema: "scheduling");
        }
    }
}
