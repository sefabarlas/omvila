using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Omvila.Infrastructure.Persistence;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Tenancy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StaffHoursAndBio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "bio",
                schema: "tenancy",
                table: "staff",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "staff_working_hours",
                schema: "tenancy",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    staff_branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    day_of_week = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<LocalTime>(type: "time", nullable: false),
                    end_time = table.Column<LocalTime>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_staff_working_hours", x => x.id);
                    table.CheckConstraint("ck_staff_working_hours_day", "day_of_week BETWEEN 1 AND 7");
                    table.CheckConstraint("ck_staff_working_hours_range", "start_time < end_time");
                    table.ForeignKey(
                        name: "fk_staff_working_hours_staff_branches_staff_branch_id",
                        column: x => x.staff_branch_id,
                        principalSchema: "tenancy",
                        principalTable: "staff_branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_staff_working_hours_brand_id_staff_branch_id_day_of_week",
                schema: "tenancy",
                table: "staff_working_hours",
                columns: new[] { "brand_id", "staff_branch_id", "day_of_week" });

            migrationBuilder.CreateIndex(
                name: "ix_staff_working_hours_staff_branch_id",
                schema: "tenancy",
                table: "staff_working_hours",
                column: "staff_branch_id");

            migrationBuilder.EnableTenantIsolation("tenancy", "staff_working_hours");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "staff_working_hours",
                schema: "tenancy");

            migrationBuilder.DropColumn(
                name: "bio",
                schema: "tenancy",
                table: "staff");
        }
    }
}
