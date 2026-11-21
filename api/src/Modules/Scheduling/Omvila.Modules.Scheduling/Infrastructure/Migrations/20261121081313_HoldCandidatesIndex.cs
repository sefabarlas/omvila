using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Omvila.Modules.Scheduling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HoldCandidatesIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_appointments_hold_candidates",
                schema: "scheduling",
                table: "appointments",
                column: "hold_expires_utc",
                filter: "status = 'PendingApproval'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_appointments_hold_candidates",
                schema: "scheduling",
                table: "appointments");
        }
    }
}
