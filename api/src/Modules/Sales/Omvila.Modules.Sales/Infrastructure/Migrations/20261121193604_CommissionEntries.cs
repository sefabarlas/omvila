using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Omvila.Infrastructure.Persistence;

#nullable disable

namespace Omvila.Modules.Sales.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CommissionEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "commission_entries",
                schema: "sales",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    staff_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    appointment_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    customer_package_id = table.Column<Guid>(type: "uuid", nullable: true),
                    basis_minor = table.Column<long>(type: "bigint", nullable: false),
                    percent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    amount_minor = table.Column<long>(type: "bigint", nullable: false),
                    currency_code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    period_year_month = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    reverses_entry_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_commission_entries", x => x.id);
                    table.CheckConstraint("ck_commission_entries_percent", "percent >= 0 AND percent <= 100");
                    table.CheckConstraint("ck_commission_entries_reversal", "(status = 'Reversed') = (reverses_entry_id IS NOT NULL) AND (amount_minor >= 0 OR status = 'Reversed')");
                    table.CheckConstraint("ck_commission_entries_status", "status IN ('Accrued', 'Approved', 'Paid', 'Reversed')");
                    table.CheckConstraint("ck_commission_entries_target", "(type = 'Service' AND appointment_item_id IS NOT NULL) OR (type = 'Sale' AND customer_package_id IS NOT NULL)");
                    table.CheckConstraint("ck_commission_entries_type", "type IN ('Service', 'Sale')");
                });

            migrationBuilder.CreateIndex(
                name: "ix_commission_entries_brand_id_appointment_item_id",
                schema: "sales",
                table: "commission_entries",
                columns: new[] { "brand_id", "appointment_item_id" },
                unique: true,
                filter: "type = 'Service' AND reverses_entry_id IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_commission_entries_brand_id_staff_id_period_year_month",
                schema: "sales",
                table: "commission_entries",
                columns: new[] { "brand_id", "staff_id", "period_year_month" });

            migrationBuilder.EnableTenantIsolation("sales", "commission_entries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "commission_entries",
                schema: "sales");
        }
    }
}
