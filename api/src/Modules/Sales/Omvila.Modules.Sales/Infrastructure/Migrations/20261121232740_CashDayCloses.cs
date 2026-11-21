using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Omvila.Infrastructure.Persistence;

#nullable disable

namespace Omvila.Modules.Sales.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CashDayCloses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Instant>(
                name: "refunded_utc",
                schema: "sales",
                table: "payments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "cash_day_closes",
                schema: "sales",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    business_date = table.Column<LocalDate>(type: "date", nullable: false),
                    expected_cash_minor = table.Column<long>(type: "bigint", nullable: false),
                    counted_cash_minor = table.Column<long>(type: "bigint", nullable: false),
                    difference_minor = table.Column<long>(type: "bigint", nullable: false),
                    currency_code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    difference_note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    closed_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    closed_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cash_day_closes", x => x.id);
                    table.CheckConstraint("ck_cash_day_closes_amounts", "expected_cash_minor >= 0 AND counted_cash_minor >= 0 AND difference_minor = counted_cash_minor - expected_cash_minor");
                    table.CheckConstraint("ck_cash_day_closes_note", "difference_minor = 0 OR difference_note IS NOT NULL");
                });

            migrationBuilder.CreateIndex(
                name: "ix_cash_day_closes_brand_id_branch_id_business_date",
                schema: "sales",
                table: "cash_day_closes",
                columns: new[] { "brand_id", "branch_id", "business_date" },
                unique: true);

            migrationBuilder.EnableTenantIsolation("sales", "cash_day_closes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cash_day_closes",
                schema: "sales");

            migrationBuilder.DropColumn(
                name: "refunded_utc",
                schema: "sales",
                table: "payments");
        }
    }
}
