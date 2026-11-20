using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Omvila.Infrastructure.Persistence;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Tenancy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BranchHoursAndOverrides : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "branch_working_hours",
                schema: "tenancy",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    day_of_week = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<LocalTime>(type: "time", nullable: false),
                    end_time = table.Column<LocalTime>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_branch_working_hours", x => x.id);
                    table.CheckConstraint("ck_branch_working_hours_day", "day_of_week BETWEEN 1 AND 7");
                    table.CheckConstraint("ck_branch_working_hours_range", "start_time < end_time");
                    table.ForeignKey(
                        name: "fk_branch_working_hours_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "tenancy",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "working_hours_overrides",
                schema: "tenancy",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<LocalDate>(type: "date", nullable: false),
                    kind = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_closed = table.Column<bool>(type: "boolean", nullable: false),
                    intervals = table.Column<string[]>(type: "text[]", nullable: false),
                    note = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_working_hours_overrides", x => x.id);
                    table.CheckConstraint("ck_working_hours_overrides_intervals", "(is_closed AND cardinality(intervals) = 0) OR (NOT is_closed AND cardinality(intervals) > 0)");
                    table.CheckConstraint("ck_working_hours_overrides_kind", "kind IN ('Holiday', 'Closure', 'ExtraOpening', 'ShortDay')");
                    table.ForeignKey(
                        name: "fk_working_hours_overrides_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "tenancy",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_branch_working_hours_branch_id",
                schema: "tenancy",
                table: "branch_working_hours",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "ix_branch_working_hours_brand_id_branch_id_day_of_week",
                schema: "tenancy",
                table: "branch_working_hours",
                columns: new[] { "brand_id", "branch_id", "day_of_week" });

            migrationBuilder.CreateIndex(
                name: "ix_working_hours_overrides_branch_id",
                schema: "tenancy",
                table: "working_hours_overrides",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "ix_working_hours_overrides_brand_id_branch_id_date",
                schema: "tenancy",
                table: "working_hours_overrides",
                columns: new[] { "brand_id", "branch_id", "date" },
                unique: true);

            migrationBuilder.EnableTenantIsolation("tenancy", "branch_working_hours");
            migrationBuilder.EnableTenantIsolation("tenancy", "working_hours_overrides");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "branch_working_hours",
                schema: "tenancy");

            migrationBuilder.DropTable(
                name: "working_hours_overrides",
                schema: "tenancy");
        }
    }
}
