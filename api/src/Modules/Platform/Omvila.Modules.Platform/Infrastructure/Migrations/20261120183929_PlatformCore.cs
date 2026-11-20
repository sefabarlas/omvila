using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Omvila.Infrastructure.Persistence;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Platform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PlatformCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "platform");

            migrationBuilder.GrantSchemaUsageToApp("platform");

            migrationBuilder.CreateTable(
                name: "plans",
                schema: "platform",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    monthly_price_minor = table.Column<long>(type: "bigint", nullable: true),
                    yearly_price_minor = table.Column<long>(type: "bigint", nullable: true),
                    included_branch_count = table.Column<int>(type: "integer", nullable: true),
                    extra_branch_price_minor = table.Column<long>(type: "bigint", nullable: true),
                    staff_limit = table.Column<int>(type: "integer", nullable: true),
                    included_sms_credits = table.Column<int>(type: "integer", nullable: true),
                    is_public = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plans", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "subscriptions",
                schema: "platform",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    billing_cycle = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    trial_ends_on = table.Column<LocalDate>(type: "date", nullable: true),
                    current_period_start = table.Column<LocalDate>(type: "date", nullable: false),
                    current_period_end = table.Column<LocalDate>(type: "date", nullable: false),
                    past_due_since = table.Column<LocalDate>(type: "date", nullable: true),
                    read_only_since = table.Column<LocalDate>(type: "date", nullable: true),
                    cancel_at_period_end = table.Column<bool>(type: "boolean", nullable: false),
                    cancelled_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    branch_count = table.Column<int>(type: "integer", nullable: false),
                    staff_count = table.Column<int>(type: "integer", nullable: false),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscriptions", x => x.id);
                    table.CheckConstraint("ck_subscriptions_billing_cycle", "billing_cycle IN ('Monthly', 'Yearly')");
                    table.CheckConstraint("ck_subscriptions_status", "status IN ('Trialing', 'Active', 'PastDue', 'ReadOnly', 'Cancelled')");
                    table.ForeignKey(
                        name: "fk_subscriptions_plans_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "platform",
                        principalTable: "plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_plans_code",
                schema: "platform",
                table: "plans",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_brand_id",
                schema: "platform",
                table: "subscriptions",
                column: "brand_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_plan_id",
                schema: "platform",
                table: "subscriptions",
                column: "plan_id");

            // Abonelik markaya aittir (ADR-0006). Planlar platform verisidir; RLS yok, markalar okur.
            migrationBuilder.EnableTenantIsolation("platform", "subscriptions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "subscriptions",
                schema: "platform");

            migrationBuilder.DropTable(
                name: "plans",
                schema: "platform");
        }
    }
}
