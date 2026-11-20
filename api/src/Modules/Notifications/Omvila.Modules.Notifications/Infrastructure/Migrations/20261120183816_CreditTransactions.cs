using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Omvila.Infrastructure.Persistence;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Notifications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreditTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "credit_transactions",
                schema: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    amount = table.Column<int>(type: "integer", nullable: false),
                    balance_after = table.Column<int>(type: "integer", nullable: false),
                    reference_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    reference_id = table.Column<Guid>(type: "uuid", nullable: true),
                    unit_price_minor = table.Column<long>(type: "bigint", nullable: true),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_credit_transactions", x => x.id);
                    table.CheckConstraint("ck_credit_transactions_balance", "balance_after >= 0");
                    table.CheckConstraint("ck_credit_transactions_type", "type IN ('Purchase', 'Consume', 'Refund', 'Grant', 'Expire')");
                });

            migrationBuilder.CreateIndex(
                name: "ix_credit_transactions_brand_id_created_utc",
                schema: "notifications",
                table: "credit_transactions",
                columns: new[] { "brand_id", "created_utc" });

            // Kontör defteri markaya aittir (ADR-0006).
            migrationBuilder.EnableTenantIsolation("notifications", "credit_transactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "credit_transactions",
                schema: "notifications");
        }
    }
}
