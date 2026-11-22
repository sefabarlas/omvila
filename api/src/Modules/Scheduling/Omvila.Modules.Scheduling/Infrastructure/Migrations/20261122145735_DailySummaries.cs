using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Omvila.Infrastructure.Persistence;

#nullable disable

namespace Omvila.Modules.Scheduling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DailySummaries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "branch_daily_summaries",
                schema: "scheduling",
                columns: table => new
                {
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    business_date = table.Column<LocalDate>(type: "date", nullable: false),
                    completed_count = table.Column<int>(type: "integer", nullable: false),
                    collected_minor = table.Column<long>(type: "bigint", nullable: false),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_branch_daily_summaries", x => new { x.brand_id, x.branch_id, x.business_date });
                    table.CheckConstraint("ck_branch_daily_summaries_completed_count", "completed_count >= 0");
                });

            migrationBuilder.EnableTenantIsolation("scheduling", "branch_daily_summaries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "branch_daily_summaries",
                schema: "scheduling");
        }
    }
}
