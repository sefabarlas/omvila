using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Platform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdminApprovalsAndBankTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "platform",
                table: "plans",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "features",
                schema: "platform",
                table: "plans",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'{\"walkInQueue\":false,\"waitlist\":false,\"packages\":false,\"crossBranchReports\":false,\"prioritySupport\":false}'::jsonb");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "platform",
                table: "plans",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<string>(
                name: "receipt_file_name",
                schema: "platform",
                table: "payment_notices",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "receipt_size_bytes",
                schema: "platform",
                table: "payment_notices",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "resolution_message",
                schema: "platform",
                table: "payment_notices",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "platform",
                table: "payment_notices",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "platform",
                table: "credit_packages",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateTable(
                name: "bank_transactions",
                schema: "platform",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    value_date = table.Column<LocalDate>(type: "date", nullable: false),
                    received_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    amount_minor = table.Column<long>(type: "bigint", nullable: false),
                    sender_name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    bank_reference = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    source = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    matched_notice_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bank_transactions", x => x.id);
                    table.CheckConstraint("ck_bank_transactions_amount", "amount_minor > 0");
                    table.CheckConstraint("ck_bank_transactions_source", "source IN ('Import', 'Manual')");
                });

            migrationBuilder.CreateIndex(
                name: "ix_bank_transactions_bank_reference",
                schema: "platform",
                table: "bank_transactions",
                column: "bank_reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_bank_transactions_matched_notice_id",
                schema: "platform",
                table: "bank_transactions",
                column: "matched_notice_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_bank_transactions_value_date_id",
                schema: "platform",
                table: "bank_transactions",
                columns: new[] { "value_date", "id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bank_transactions",
                schema: "platform");

            migrationBuilder.DropColumn(
                name: "description",
                schema: "platform",
                table: "plans");

            migrationBuilder.DropColumn(
                name: "features",
                schema: "platform",
                table: "plans");

            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "platform",
                table: "plans");

            migrationBuilder.DropColumn(
                name: "receipt_file_name",
                schema: "platform",
                table: "payment_notices");

            migrationBuilder.DropColumn(
                name: "receipt_size_bytes",
                schema: "platform",
                table: "payment_notices");

            migrationBuilder.DropColumn(
                name: "resolution_message",
                schema: "platform",
                table: "payment_notices");

            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "platform",
                table: "payment_notices");

            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "platform",
                table: "credit_packages");
        }
    }
}
