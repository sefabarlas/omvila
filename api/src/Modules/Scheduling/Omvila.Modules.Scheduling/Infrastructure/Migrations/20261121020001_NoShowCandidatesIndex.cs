using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Omvila.Modules.Scheduling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NoShowCandidatesIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_appointments_no_show_candidates",
                schema: "scheduling",
                table: "appointments",
                column: "start_utc",
                filter: "status = 'Confirmed' AND checked_in_utc IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_appointments_no_show_candidates",
                schema: "scheduling",
                table: "appointments");
        }
    }
}
