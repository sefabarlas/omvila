using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Omvila.Infrastructure.Persistence;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Tenancy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BrandOnboarding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "brand_onboarding",
                schema: "tenancy",
                columns: table => new
                {
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    solo_confirmed_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    booking_link_shared_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_onboarding", x => x.brand_id);
                    table.ForeignKey(
                        name: "fk_brand_onboarding_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "tenancy",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.EnableTenantIsolation("tenancy", "brand_onboarding");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "brand_onboarding",
                schema: "tenancy");
        }
    }
}
