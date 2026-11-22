using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Omvila.Infrastructure.Persistence;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Customers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DataExports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "data_exports",
                schema: "customers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requested_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    email = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    status = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    requested_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    completed_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    file_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    content = table.Column<byte[]>(type: "bytea", nullable: true),
                    expires_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    failure_reason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_data_exports", x => x.id);
                    table.CheckConstraint("ck_data_exports_status", "status IN ('Queued', 'Processing', 'Ready', 'Sent', 'Failed')");
                });

            migrationBuilder.CreateIndex(
                name: "ix_data_exports_brand_id_requested_utc",
                schema: "customers",
                table: "data_exports",
                columns: new[] { "brand_id", "requested_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_data_exports_status",
                schema: "customers",
                table: "data_exports",
                column: "status",
                filter: "status IN ('Queued', 'Processing')");

            migrationBuilder.EnableTenantIsolation("customers", "data_exports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "data_exports",
                schema: "customers");
        }
    }
}
