using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Scheduling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DepositReminderAndRefund : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "deposit_refund_note",
                schema: "scheduling",
                table: "appointments",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "deposit_refunded_amount_minor",
                schema: "scheduling",
                table: "appointments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "deposit_refunded_by_user_id",
                schema: "scheduling",
                table: "appointments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Instant>(
                name: "deposit_refunded_utc",
                schema: "scheduling",
                table: "appointments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Instant>(
                name: "deposit_reminded_utc",
                schema: "scheduling",
                table: "appointments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_appointments_deposit_refund",
                schema: "scheduling",
                table: "appointments",
                sql: "(deposit_refunded_utc IS NULL) = (deposit_refunded_amount_minor IS NULL) AND (deposit_refunded_utc IS NULL OR (deposit_received_utc IS NOT NULL AND deposit_refunded_amount_minor > 0 AND deposit_refunded_amount_minor <= deposit_required_minor))");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_appointments_deposit_refund",
                schema: "scheduling",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "deposit_refund_note",
                schema: "scheduling",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "deposit_refunded_amount_minor",
                schema: "scheduling",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "deposit_refunded_by_user_id",
                schema: "scheduling",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "deposit_refunded_utc",
                schema: "scheduling",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "deposit_reminded_utc",
                schema: "scheduling",
                table: "appointments");
        }
    }
}
