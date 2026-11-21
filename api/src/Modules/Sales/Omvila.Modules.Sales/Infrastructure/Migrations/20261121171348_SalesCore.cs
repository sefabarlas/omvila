using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Omvila.Infrastructure.Persistence;

#nullable disable

namespace Omvila.Modules.Sales.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SalesCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sales");

            migrationBuilder.GrantSchemaUsageToApp("sales");

            migrationBuilder.CreateTable(
                name: "payments",
                schema: "sales",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_customer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    method = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    amount_minor = table.Column<long>(type: "bigint", nullable: false),
                    currency_code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    is_deposit = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    refunded_amount_minor = table.Column<long>(type: "bigint", nullable: false),
                    paid_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    collected_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payments", x => x.id);
                    table.CheckConstraint("ck_payments_amounts", "amount_minor > 0 AND refunded_amount_minor >= 0 AND refunded_amount_minor <= amount_minor");
                    table.CheckConstraint("ck_payments_method", "method IN ('Cash', 'Card', 'Transfer', 'PackageRedemption')");
                    table.CheckConstraint("ck_payments_status", "status IN ('Recorded', 'Refunded', 'PartiallyRefunded', 'Voided')");
                });

            migrationBuilder.CreateTable(
                name: "payment_allocations",
                schema: "sales",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    target_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount_minor = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_allocations", x => x.id);
                    table.CheckConstraint("ck_payment_allocations_amount", "amount_minor > 0");
                    table.CheckConstraint("ck_payment_allocations_target_type", "target_type IN ('Appointment', 'CustomerPackage')");
                    table.ForeignKey(
                        name: "fk_payment_allocations_payments_payment_id",
                        column: x => x.payment_id,
                        principalSchema: "sales",
                        principalTable: "payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_payment_allocations_brand_id_target_type_target_id",
                schema: "sales",
                table: "payment_allocations",
                columns: new[] { "brand_id", "target_type", "target_id" });

            migrationBuilder.CreateIndex(
                name: "ix_payment_allocations_payment_id",
                schema: "sales",
                table: "payment_allocations",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "ix_payments_brand_id_branch_id_paid_utc",
                schema: "sales",
                table: "payments",
                columns: new[] { "brand_id", "branch_id", "paid_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_payments_brand_id_brand_customer_id",
                schema: "sales",
                table: "payments",
                columns: new[] { "brand_id", "brand_customer_id" });

            migrationBuilder.EnableTenantIsolation("sales", "payments");
            migrationBuilder.EnableTenantIsolation("sales", "payment_allocations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payment_allocations",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "payments",
                schema: "sales");
        }
    }
}
