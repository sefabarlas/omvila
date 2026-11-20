using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Omvila.Infrastructure.Persistence;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CatalogCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "catalog");

            migrationBuilder.GrantSchemaUsageToApp("catalog");

            migrationBuilder.CreateTable(
                name: "service_categories",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_service_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "services",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    duration_minutes = table.Column<int>(type: "integer", nullable: false),
                    base_price_minor = table.Column<long>(type: "bigint", nullable: false),
                    currency_code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    price_display = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    buffer_before_minutes = table.Column<int>(type: "integer", nullable: false),
                    buffer_after_minutes = table.Column<int>(type: "integer", nullable: false),
                    deposit_mode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    deposit_amount_minor = table.Column<long>(type: "bigint", nullable: true),
                    deposit_percent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    requires_approval = table.Column<bool>(type: "boolean", nullable: true),
                    is_online_bookable = table.Column<bool>(type: "boolean", nullable: false),
                    min_notice_minutes = table.Column<int>(type: "integer", nullable: true),
                    max_advance_days = table.Column<int>(type: "integer", nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_services", x => x.id);
                    table.CheckConstraint("ck_services_deposit_mode", "deposit_mode IN ('Inherit', 'Always', 'Never')");
                    table.CheckConstraint("ck_services_deposit_xor", "deposit_amount_minor IS NULL OR deposit_percent IS NULL");
                    table.CheckConstraint("ck_services_duration", "duration_minutes BETWEEN 5 AND 720");
                    table.CheckConstraint("ck_services_price", "base_price_minor >= 0");
                    table.CheckConstraint("ck_services_price_display", "price_display IN ('Fixed', 'StartingFrom')");
                    table.ForeignKey(
                        name: "fk_services_service_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "catalog",
                        principalTable: "service_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "branch_service_prices",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    price_minor = table.Column<long>(type: "bigint", nullable: false),
                    duration_override_minutes = table.Column<int>(type: "integer", nullable: true),
                    is_available = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_branch_service_prices", x => x.id);
                    table.CheckConstraint("ck_branch_service_prices_price", "price_minor >= 0");
                    table.ForeignKey(
                        name: "fk_branch_service_prices_services_service_id",
                        column: x => x.service_id,
                        principalSchema: "catalog",
                        principalTable: "services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "service_staff",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    staff_id = table.Column<Guid>(type: "uuid", nullable: false),
                    duration_override_minutes = table.Column<int>(type: "integer", nullable: true),
                    surcharge_minor = table.Column<long>(type: "bigint", nullable: false),
                    commission_percent_override = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_service_staff", x => x.id);
                    table.CheckConstraint("ck_service_staff_surcharge", "surcharge_minor >= 0");
                    table.ForeignKey(
                        name: "fk_service_staff_services_service_id",
                        column: x => x.service_id,
                        principalSchema: "catalog",
                        principalTable: "services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "service_stages",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    seq_no = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    duration_minutes = table.Column<int>(type: "integer", nullable: false),
                    staff_busy = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_service_stages", x => x.id);
                    table.CheckConstraint("ck_service_stages_duration", "duration_minutes BETWEEN 5 AND 600");
                    table.ForeignKey(
                        name: "fk_service_stages_services_service_id",
                        column: x => x.service_id,
                        principalSchema: "catalog",
                        principalTable: "services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_branch_service_prices_brand_id_branch_id_service_id",
                schema: "catalog",
                table: "branch_service_prices",
                columns: new[] { "brand_id", "branch_id", "service_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_branch_service_prices_service_id",
                schema: "catalog",
                table: "branch_service_prices",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_categories_brand_id_name",
                schema: "catalog",
                table: "service_categories",
                columns: new[] { "brand_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_service_staff_brand_id_service_id_staff_id",
                schema: "catalog",
                table: "service_staff",
                columns: new[] { "brand_id", "service_id", "staff_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_service_staff_brand_id_staff_id",
                schema: "catalog",
                table: "service_staff",
                columns: new[] { "brand_id", "staff_id" });

            migrationBuilder.CreateIndex(
                name: "ix_service_staff_service_id",
                schema: "catalog",
                table: "service_staff",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_stages_brand_id_service_id_seq_no",
                schema: "catalog",
                table: "service_stages",
                columns: new[] { "brand_id", "service_id", "seq_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_service_stages_service_id",
                schema: "catalog",
                table: "service_stages",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "ix_services_brand_id_category_id",
                schema: "catalog",
                table: "services",
                columns: new[] { "brand_id", "category_id" });

            migrationBuilder.CreateIndex(
                name: "ix_services_brand_id_name",
                schema: "catalog",
                table: "services",
                columns: new[] { "brand_id", "name" },
                unique: true,
                filter: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_services_category_id",
                schema: "catalog",
                table: "services",
                column: "category_id");

            // Katalog markaya aittir (ADR-0006).
            migrationBuilder.EnableTenantIsolation("catalog", "service_categories");
            migrationBuilder.EnableTenantIsolation("catalog", "services");
            migrationBuilder.EnableTenantIsolation("catalog", "service_stages");
            migrationBuilder.EnableTenantIsolation("catalog", "service_staff");
            migrationBuilder.EnableTenantIsolation("catalog", "branch_service_prices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "branch_service_prices",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "service_staff",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "service_stages",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "services",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "service_categories",
                schema: "catalog");
        }
    }
}
