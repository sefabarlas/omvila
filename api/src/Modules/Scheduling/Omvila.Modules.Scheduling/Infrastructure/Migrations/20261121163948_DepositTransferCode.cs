using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Omvila.Modules.Scheduling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DepositTransferCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "deposit_transfer_code",
                schema: "scheduling",
                table: "appointments",
                type: "character varying(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_appointments_brand_id_deposit_transfer_code",
                schema: "scheduling",
                table: "appointments",
                columns: new[] { "brand_id", "deposit_transfer_code" },
                unique: true,
                filter: "deposit_transfer_code IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "ck_appointments_deposit_transfer_code",
                schema: "scheduling",
                table: "appointments",
                sql: "deposit_transfer_code IS NULL OR deposit_required_minor IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_appointments_brand_id_deposit_transfer_code",
                schema: "scheduling",
                table: "appointments");

            migrationBuilder.DropCheckConstraint(
                name: "ck_appointments_deposit_transfer_code",
                schema: "scheduling",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "deposit_transfer_code",
                schema: "scheduling",
                table: "appointments");
        }
    }
}
