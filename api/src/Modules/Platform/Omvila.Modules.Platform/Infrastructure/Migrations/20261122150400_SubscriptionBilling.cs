using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Omvila.Infrastructure.Persistence;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Platform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SubscriptionBilling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Instant>(
                name: "brand_archived_utc",
                schema: "platform",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<LocalDate>(
                name: "cancelled_on",
                schema: "platform",
                table: "subscriptions",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credit_transfer_code",
                schema: "platform",
                table: "subscriptions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "payment_method",
                schema: "platform",
                table: "subscriptions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "transfer_code",
                schema: "platform",
                table: "subscriptions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Instant>(
                name: "trial_ending_notified_utc",
                schema: "platform",
                table: "subscriptions",
                type: "timestamp with time zone",
                nullable: true);

            // Var olan aboneliklere sabit havale kodu: UUID'nin rastgele son 8 hanesi (benzersiz indeks aşağıda).
            migrationBuilder.Sql("""
                UPDATE platform.subscriptions
                SET transfer_code = 'ABN-' || upper(right(replace(id::text, '-', ''), 8)),
                    credit_transfer_code = 'KNT-' || upper(right(replace(id::text, '-', ''), 8))
                WHERE transfer_code = '';
                """);

            migrationBuilder.CreateTable(
                name: "credit_packages",
                schema: "platform",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    credits = table.Column<int>(type: "integer", nullable: false),
                    price_minor = table.Column<long>(type: "bigint", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_credit_packages", x => x.id);
                    table.CheckConstraint("ck_credit_packages_credits", "credits > 0");
                    table.CheckConstraint("ck_credit_packages_price", "price_minor IS NULL OR price_minor >= 0");
                });

            migrationBuilder.CreateTable(
                name: "invoice_number_counters",
                schema: "platform",
                columns: table => new
                {
                    year = table.Column<int>(type: "integer", nullable: false),
                    last_value = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoice_number_counters", x => x.year);
                    table.CheckConstraint("ck_invoice_number_counters_last_value", "last_value > 0");
                });

            migrationBuilder.CreateTable(
                name: "subscription_invoices",
                schema: "platform",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    period_start = table.Column<LocalDate>(type: "date", nullable: false),
                    period_end = table.Column<LocalDate>(type: "date", nullable: false),
                    amount_minor = table.Column<long>(type: "bigint", nullable: false),
                    tax_minor = table.Column<long>(type: "bigint", nullable: false),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    issued_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    due_on = table.Column<LocalDate>(type: "date", nullable: false),
                    paid_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    paid_via = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscription_invoices", x => x.id);
                    table.CheckConstraint("ck_subscription_invoices_amounts", "amount_minor >= 0 AND tax_minor >= 0");
                    table.CheckConstraint("ck_subscription_invoices_paid_via", "paid_via IS NULL OR paid_via IN ('Card', 'BankTransfer')");
                    table.CheckConstraint("ck_subscription_invoices_period", "period_end >= period_start");
                    table.CheckConstraint("ck_subscription_invoices_status", "status IN ('Issued', 'Paid', 'Failed', 'Void')");
                    table.ForeignKey(
                        name: "fk_subscription_invoices_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalSchema: "platform",
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payment_notices",
                schema: "platform",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    purpose = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    credit_package_id = table.Column<Guid>(type: "uuid", nullable: true),
                    credits = table.Column<int>(type: "integer", nullable: true),
                    period_start = table.Column<LocalDate>(type: "date", nullable: true),
                    expected_amount_minor = table.Column<long>(type: "bigint", nullable: true),
                    channel = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    transfer_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    sender_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    receipt_file_key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    receipt_content_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    notified_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    notified_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    matched_bank_transaction_id = table.Column<Guid>(type: "uuid", nullable: true),
                    resolved_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    resolved_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_notices", x => x.id);
                    table.CheckConstraint("ck_payment_notices_channel", "channel IN ('Panel', 'App')");
                    table.CheckConstraint("ck_payment_notices_credit_package", "(purpose = 'Credits') = (credit_package_id IS NOT NULL AND credits IS NOT NULL)");
                    table.CheckConstraint("ck_payment_notices_purpose", "purpose IN ('Subscription', 'Credits')");
                    table.CheckConstraint("ck_payment_notices_status", "status IN ('Pending', 'Matched', 'NotFound', 'Rejected')");
                    table.ForeignKey(
                        name: "fk_payment_notices_credit_packages_credit_package_id",
                        column: x => x.credit_package_id,
                        principalSchema: "platform",
                        principalTable: "credit_packages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_credit_transfer_code",
                schema: "platform",
                table: "subscriptions",
                column: "credit_transfer_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_status",
                schema: "platform",
                table: "subscriptions",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_transfer_code",
                schema: "platform",
                table: "subscriptions",
                column: "transfer_code",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_subscriptions_payment_method",
                schema: "platform",
                table: "subscriptions",
                sql: "payment_method IS NULL OR payment_method IN ('Card', 'BankTransfer')");

            migrationBuilder.CreateIndex(
                name: "ix_credit_packages_code",
                schema: "platform",
                table: "credit_packages",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_payment_notices_brand_id_notified_utc",
                schema: "platform",
                table: "payment_notices",
                columns: new[] { "brand_id", "notified_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_payment_notices_credit_package_id",
                schema: "platform",
                table: "payment_notices",
                column: "credit_package_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_notices_pending",
                schema: "platform",
                table: "payment_notices",
                columns: new[] { "brand_id", "purpose", "credit_package_id" },
                unique: true,
                filter: "status = 'Pending'")
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "ix_payment_notices_status_notified_utc",
                schema: "platform",
                table: "payment_notices",
                columns: new[] { "status", "notified_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_subscription_invoices_brand_id_issued_utc",
                schema: "platform",
                table: "subscription_invoices",
                columns: new[] { "brand_id", "issued_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_subscription_invoices_invoice_number",
                schema: "platform",
                table: "subscription_invoices",
                column: "invoice_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_subscription_invoices_subscription_id_period_start",
                schema: "platform",
                table: "subscription_invoices",
                columns: new[] { "subscription_id", "period_start" },
                unique: true);
            // Fatura ve havale bildirimi markaya aittir (ADR-0006); sayaç ve paketler platform verisidir (RLS yok).
            migrationBuilder.EnableTenantIsolation("platform", "subscription_invoices");
            migrationBuilder.EnableTenantIsolation("platform", "payment_notices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "invoice_number_counters",
                schema: "platform");

            migrationBuilder.DropTable(
                name: "payment_notices",
                schema: "platform");

            migrationBuilder.DropTable(
                name: "subscription_invoices",
                schema: "platform");

            migrationBuilder.DropTable(
                name: "credit_packages",
                schema: "platform");

            migrationBuilder.DropIndex(
                name: "ix_subscriptions_credit_transfer_code",
                schema: "platform",
                table: "subscriptions");

            migrationBuilder.DropIndex(
                name: "ix_subscriptions_status",
                schema: "platform",
                table: "subscriptions");

            migrationBuilder.DropIndex(
                name: "ix_subscriptions_transfer_code",
                schema: "platform",
                table: "subscriptions");

            migrationBuilder.DropCheckConstraint(
                name: "ck_subscriptions_payment_method",
                schema: "platform",
                table: "subscriptions");

            migrationBuilder.DropColumn(
                name: "brand_archived_utc",
                schema: "platform",
                table: "subscriptions");

            migrationBuilder.DropColumn(
                name: "cancelled_on",
                schema: "platform",
                table: "subscriptions");

            migrationBuilder.DropColumn(
                name: "credit_transfer_code",
                schema: "platform",
                table: "subscriptions");

            migrationBuilder.DropColumn(
                name: "payment_method",
                schema: "platform",
                table: "subscriptions");

            migrationBuilder.DropColumn(
                name: "transfer_code",
                schema: "platform",
                table: "subscriptions");

            migrationBuilder.DropColumn(
                name: "trial_ending_notified_utc",
                schema: "platform",
                table: "subscriptions");
        }
    }
}
