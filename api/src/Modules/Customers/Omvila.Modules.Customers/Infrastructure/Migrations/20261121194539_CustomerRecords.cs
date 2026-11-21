using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Omvila.Infrastructure.Persistence;

#nullable disable

namespace Omvila.Modules.Customers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CustomerRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Instant>(
                name: "anonymized_utc",
                schema: "customers",
                table: "brand_customers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "merged_into_id",
                schema: "customers",
                table: "brand_customers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Instant>(
                name: "merged_utc",
                schema: "customers",
                table: "brand_customers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "customer_imports",
                schema: "customers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    total_rows = table.Column<int>(type: "integer", nullable: false),
                    to_create = table.Column<int>(type: "integer", nullable: false),
                    to_merge = table.Column<int>(type: "integer", nullable: false),
                    invalid_rows = table.Column<int>(type: "integer", nullable: false),
                    mapped_columns = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    errors = table.Column<string>(type: "jsonb", nullable: false),
                    rows = table.Column<string>(type: "jsonb", nullable: true),
                    status = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    send_info_sms = table.Column<bool>(type: "boolean", nullable: false),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    expires_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    committed_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    created = table.Column<int>(type: "integer", nullable: false),
                    merged = table.Column<int>(type: "integer", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customer_imports", x => x.id);
                    table.CheckConstraint("ck_customer_imports_status", "status IN ('Previewed', 'Committed', 'Failed')");
                });

            migrationBuilder.CreateTable(
                name: "data_deletion_requests",
                schema: "customers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: true),
                    brand_customer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    customer_name_masked = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    requested_via = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    status = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    requested_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    due_on = table.Column<LocalDate>(type: "date", nullable: false),
                    completed_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_data_deletion_requests", x => x.id);
                    table.CheckConstraint("ck_data_deletion_requests_requested_via", "requested_via IN ('CustomerApp', 'Panel')");
                    table.CheckConstraint("ck_data_deletion_requests_status", "status IN ('Pending', 'Completed', 'Rejected')");
                    table.ForeignKey(
                        name: "fk_data_deletion_requests_customers_customer_id",
                        column: x => x.customer_id,
                        principalSchema: "customers",
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_customer_imports_brand_id_created_utc",
                schema: "customers",
                table: "customer_imports",
                columns: new[] { "brand_id", "created_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_data_deletion_requests_brand_id_requested_utc",
                schema: "customers",
                table: "data_deletion_requests",
                columns: new[] { "brand_id", "requested_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_data_deletion_requests_customer_id",
                schema: "customers",
                table: "data_deletion_requests",
                column: "customer_id");

            // İçe aktarma ve silme talebi markaya aittir (ADR-0006). Müşterinin kendi hesabını silme talebi (brand_id boş)
            // her bağlamdan eklenir, yalnız platform okur.
            migrationBuilder.EnableTenantIsolation("customers", "customer_imports");
            migrationBuilder.EnableTenantIsolation("customers", "data_deletion_requests", allowPlatformRowInsert: true);

            // Silme talebi kaydı silinmez (KVKK işlem kanıtı).
            migrationBuilder.Sql($"REVOKE DELETE ON customers.data_deletion_requests FROM {MigrationBuilderExtensions.AppRole};");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customer_imports",
                schema: "customers");

            migrationBuilder.DropTable(
                name: "data_deletion_requests",
                schema: "customers");

            migrationBuilder.DropColumn(
                name: "anonymized_utc",
                schema: "customers",
                table: "brand_customers");

            migrationBuilder.DropColumn(
                name: "merged_into_id",
                schema: "customers",
                table: "brand_customers");

            migrationBuilder.DropColumn(
                name: "merged_utc",
                schema: "customers",
                table: "brand_customers");
        }
    }
}
