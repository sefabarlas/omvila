using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Omvila.Infrastructure.Persistence;

#nullable disable

namespace Omvila.Modules.Sales.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Packages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_payments_amounts",
                schema: "sales",
                table: "payments");

            migrationBuilder.DropCheckConstraint(
                name: "ck_payment_allocations_amount",
                schema: "sales",
                table: "payment_allocations");

            migrationBuilder.CreateTable(
                name: "customer_packages",
                schema: "sales",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    package_id = table.Column<Guid>(type: "uuid", nullable: false),
                    package_name_snapshot = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    price_paid_minor = table.Column<long>(type: "bigint", nullable: false),
                    currency_code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    is_available_all_branches = table.Column<bool>(type: "boolean", nullable: false),
                    purchased_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    expires_on = table.Column<LocalDate>(type: "date", nullable: false),
                    sold_by_staff_id = table.Column<Guid>(type: "uuid", nullable: true),
                    sold_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    transferred_to_brand_customer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    refunded_amount_minor = table.Column<long>(type: "bigint", nullable: true),
                    refunded_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    refund_note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    refunded_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    expired_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customer_packages", x => x.id);
                    table.CheckConstraint("ck_customer_packages_price", "price_paid_minor > 0");
                    table.CheckConstraint("ck_customer_packages_refund", "(status = 'Refunded') = (refunded_amount_minor IS NOT NULL) AND (refunded_amount_minor IS NULL OR (refunded_amount_minor > 0 AND refunded_amount_minor <= price_paid_minor))");
                    table.CheckConstraint("ck_customer_packages_status", "status IN ('Active', 'Expired', 'Refunded', 'Transferred')");
                });

            migrationBuilder.CreateTable(
                name: "packages",
                schema: "sales",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    price_minor = table.Column<long>(type: "bigint", nullable: false),
                    currency_code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    validity_days = table.Column<int>(type: "integer", nullable: false),
                    sale_commission_percent_override = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    is_available_all_branches = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    updated_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_packages", x => x.id);
                    table.CheckConstraint("ck_packages_price", "price_minor > 0");
                    table.CheckConstraint("ck_packages_sale_commission_percent", "sale_commission_percent_override IS NULL OR (sale_commission_percent_override >= 0 AND sale_commission_percent_override <= 100)");
                    table.CheckConstraint("ck_packages_validity_days", "validity_days BETWEEN 1 AND 1095");
                });

            migrationBuilder.CreateTable(
                name: "customer_package_items",
                schema: "sales",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_package_id = table.Column<Guid>(type: "uuid", nullable: false),
                    seq_no = table.Column<int>(type: "integer", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    total_sessions = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customer_package_items", x => x.id);
                    table.CheckConstraint("ck_customer_package_items_total_sessions", "total_sessions > 0");
                    table.ForeignKey(
                        name: "fk_customer_package_items_customer_packages_customer_package_id",
                        column: x => x.customer_package_id,
                        principalSchema: "sales",
                        principalTable: "customer_packages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "package_items",
                schema: "sales",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    package_id = table.Column<Guid>(type: "uuid", nullable: false),
                    seq_no = table.Column<int>(type: "integer", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_package_items", x => x.id);
                    table.CheckConstraint("ck_package_items_session_count", "session_count BETWEEN 1 AND 100");
                    table.ForeignKey(
                        name: "fk_package_items_packages_package_id",
                        column: x => x.package_id,
                        principalSchema: "sales",
                        principalTable: "packages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "package_session_usages",
                schema: "sales",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_package_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    appointment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    appointment_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    used_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    reverted_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_package_session_usages", x => x.id);
                    table.ForeignKey(
                        name: "fk_package_session_usages_customer_package_items_customer_pack",
                        column: x => x.customer_package_item_id,
                        principalSchema: "sales",
                        principalTable: "customer_package_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "ck_payments_amounts",
                schema: "sales",
                table: "payments",
                sql: "(amount_minor > 0 OR (method = 'PackageRedemption' AND amount_minor = 0)) AND refunded_amount_minor >= 0 AND refunded_amount_minor <= amount_minor");

            migrationBuilder.AddCheckConstraint(
                name: "ck_payment_allocations_amount",
                schema: "sales",
                table: "payment_allocations",
                sql: "amount_minor >= 0");

            migrationBuilder.CreateIndex(
                name: "ix_commission_entries_brand_id_customer_package_id",
                schema: "sales",
                table: "commission_entries",
                columns: new[] { "brand_id", "customer_package_id" },
                unique: true,
                filter: "type = 'Sale' AND reverses_entry_id IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_customer_package_items_brand_id_customer_package_id",
                schema: "sales",
                table: "customer_package_items",
                columns: new[] { "brand_id", "customer_package_id" });

            migrationBuilder.CreateIndex(
                name: "ix_customer_package_items_customer_package_id",
                schema: "sales",
                table: "customer_package_items",
                column: "customer_package_id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_packages_brand_id_brand_customer_id",
                schema: "sales",
                table: "customer_packages",
                columns: new[] { "brand_id", "brand_customer_id" });

            migrationBuilder.CreateIndex(
                name: "ix_customer_packages_brand_id_package_id",
                schema: "sales",
                table: "customer_packages",
                columns: new[] { "brand_id", "package_id" });

            migrationBuilder.CreateIndex(
                name: "ix_customer_packages_status_expires_on",
                schema: "sales",
                table: "customer_packages",
                columns: new[] { "status", "expires_on" });

            migrationBuilder.CreateIndex(
                name: "ix_package_items_brand_id_package_id_service_id",
                schema: "sales",
                table: "package_items",
                columns: new[] { "brand_id", "package_id", "service_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_package_items_package_id",
                schema: "sales",
                table: "package_items",
                column: "package_id");

            migrationBuilder.CreateIndex(
                name: "ix_package_session_usages_brand_id_appointment_id",
                schema: "sales",
                table: "package_session_usages",
                columns: new[] { "brand_id", "appointment_id" });

            migrationBuilder.CreateIndex(
                name: "ix_package_session_usages_brand_id_appointment_item_id",
                schema: "sales",
                table: "package_session_usages",
                columns: new[] { "brand_id", "appointment_item_id" },
                unique: true,
                filter: "reverted_utc IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_package_session_usages_brand_id_customer_package_item_id",
                schema: "sales",
                table: "package_session_usages",
                columns: new[] { "brand_id", "customer_package_item_id" });

            migrationBuilder.CreateIndex(
                name: "ix_package_session_usages_customer_package_item_id",
                schema: "sales",
                table: "package_session_usages",
                column: "customer_package_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_packages_brand_id_is_active",
                schema: "sales",
                table: "packages",
                columns: new[] { "brand_id", "is_active" });

            migrationBuilder.EnableTenantIsolation("sales", "packages");
            migrationBuilder.EnableTenantIsolation("sales", "package_items");
            migrationBuilder.EnableTenantIsolation("sales", "customer_packages");
            migrationBuilder.EnableTenantIsolation("sales", "customer_package_items");
            migrationBuilder.EnableTenantIsolation("sales", "package_session_usages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "package_items",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "package_session_usages",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "packages",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "customer_package_items",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "customer_packages",
                schema: "sales");

            migrationBuilder.DropCheckConstraint(
                name: "ck_payments_amounts",
                schema: "sales",
                table: "payments");

            migrationBuilder.DropCheckConstraint(
                name: "ck_payment_allocations_amount",
                schema: "sales",
                table: "payment_allocations");

            migrationBuilder.DropIndex(
                name: "ix_commission_entries_brand_id_customer_package_id",
                schema: "sales",
                table: "commission_entries");

            migrationBuilder.AddCheckConstraint(
                name: "ck_payments_amounts",
                schema: "sales",
                table: "payments",
                sql: "amount_minor > 0 AND refunded_amount_minor >= 0 AND refunded_amount_minor <= amount_minor");

            migrationBuilder.AddCheckConstraint(
                name: "ck_payment_allocations_amount",
                schema: "sales",
                table: "payment_allocations",
                sql: "amount_minor > 0");
        }
    }
}
