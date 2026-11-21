using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Scheduling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DepositOutcome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "deposit_payment_id",
                schema: "scheduling",
                table: "appointments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "deposit_received_by_user_id",
                schema: "scheduling",
                table: "appointments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "deposit_waived_by_user_id",
                schema: "scheduling",
                table: "appointments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Instant>(
                name: "deposit_waived_utc",
                schema: "scheduling",
                table: "appointments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_appointments_deposit_outcome",
                schema: "scheduling",
                table: "appointments",
                sql: "(deposit_received_utc IS NULL OR (deposit_required_minor IS NOT NULL AND deposit_payment_id IS NOT NULL AND deposit_waived_utc IS NULL)) AND (deposit_waived_utc IS NULL OR deposit_required_minor IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_appointments_deposit_outcome",
                schema: "scheduling",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "deposit_payment_id",
                schema: "scheduling",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "deposit_received_by_user_id",
                schema: "scheduling",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "deposit_waived_by_user_id",
                schema: "scheduling",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "deposit_waived_utc",
                schema: "scheduling",
                table: "appointments");
        }
    }
}
