using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Omvila.Infrastructure.Persistence;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Tenancy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TimeOffs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "time_offs",
                schema: "tenancy",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    staff_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    start_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    end_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    is_all_day = table.Column<bool>(type: "boolean", nullable: false),
                    note = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_time_offs", x => x.id);
                    table.CheckConstraint("ck_time_offs_range", "start_utc < end_utc");
                    table.CheckConstraint("ck_time_offs_type", "type IN ('Leave', 'Break', 'Training', 'Sick', 'Other')");
                    table.ForeignKey(
                        name: "fk_time_offs_staff_staff_id",
                        column: x => x.staff_id,
                        principalSchema: "tenancy",
                        principalTable: "staff",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_time_offs_brand_id_staff_id_start_utc",
                schema: "tenancy",
                table: "time_offs",
                columns: new[] { "brand_id", "staff_id", "start_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_time_offs_staff_id",
                schema: "tenancy",
                table: "time_offs",
                column: "staff_id");

            migrationBuilder.EnableTenantIsolation("tenancy", "time_offs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "time_offs",
                schema: "tenancy");
        }
    }
}
