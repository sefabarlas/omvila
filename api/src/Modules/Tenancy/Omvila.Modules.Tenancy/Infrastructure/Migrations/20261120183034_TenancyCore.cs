using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Omvila.Infrastructure.Persistence;

#nullable disable

namespace Omvila.Modules.Tenancy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TenancyCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tenancy");

            migrationBuilder.GrantSchemaUsageToApp("tenancy");

            migrationBuilder.CreateTable(
                name: "brands",
                schema: "tenancy",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    slug = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    industry_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    short_description = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    logo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    phone = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    instagram = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    website = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    legal_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    tax_office = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    tax_number = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    default_currency_code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brands", x => x.id);
                    table.CheckConstraint("ck_brands_industry_code", "industry_code IN ('barber', 'hair_salon', 'beauty', 'spa', 'nail', 'dietitian', 'other')");
                    table.CheckConstraint("ck_brands_slug", "slug ~ '^[a-z0-9-]{3,60}$'");
                    table.CheckConstraint("ck_brands_status", "status IN ('Onboarding', 'Active', 'Suspended', 'Archived')");
                });

            migrationBuilder.CreateTable(
                name: "booking_page_settings",
                schema: "tenancy",
                columns: table => new
                {
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cover_color_token = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    show_reviews = table.Column<bool>(type: "boolean", nullable: false),
                    show_prices = table.Column<bool>(type: "boolean", nullable: false),
                    pre_appointment_note = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_booking_page_settings", x => x.brand_id);
                    table.CheckConstraint("ck_booking_page_settings_cover_color", "cover_color_token IN ('petrol', 'mercan', 'kum')");
                    table.ForeignKey(
                        name: "fk_booking_page_settings_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "tenancy",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "branches",
                schema: "tenancy",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    slug = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    address_line = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    district = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    city = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    postal_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    country_code = table.Column<string>(type: "character(2)", fixedLength: true, maxLength: 2, nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    phone = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    time_zone_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    slot_interval_minutes = table.Column<int>(type: "integer", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_branches", x => x.id);
                    table.ForeignKey(
                        name: "fk_branches_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "tenancy",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "brand_settings",
                schema: "tenancy",
                columns: table => new
                {
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requires_approval = table.Column<bool>(type: "boolean", nullable: false),
                    approval_timeout_minutes = table.Column<int>(type: "integer", nullable: false),
                    allow_staff_selection = table.Column<bool>(type: "boolean", nullable: false),
                    allow_any_staff_option = table.Column<bool>(type: "boolean", nullable: false),
                    cancellation_cutoff_hours = table.Column<int>(type: "integer", nullable: false),
                    reschedule_cutoff_hours = table.Column<int>(type: "integer", nullable: false),
                    no_show_after_minutes = table.Column<int>(type: "integer", nullable: false),
                    min_notice_minutes = table.Column<int>(type: "integer", nullable: false),
                    max_advance_days = table.Column<int>(type: "integer", nullable: false),
                    slot_interval_minutes = table.Column<int>(type: "integer", nullable: false),
                    default_branch_transfer_minutes = table.Column<int>(type: "integer", nullable: false),
                    deposit_policy = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    risky_customer_no_show_threshold = table.Column<int>(type: "integer", nullable: false),
                    mask_customer_phone_from_staff = table.Column<bool>(type: "boolean", nullable: false),
                    reminder_offsets_minutes = table.Column<int[]>(type: "integer[]", nullable: false),
                    review_invite_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_settings", x => x.brand_id);
                    table.CheckConstraint("ck_brand_settings_deposit_policy", "deposit_policy IN ('None', 'RiskyCustomersOnly', 'Always')");
                    table.ForeignKey(
                        name: "fk_brand_settings_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "tenancy",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "brand_slug_history",
                schema: "tenancy",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    slug = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    changed_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    redirect_expires_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_slug_history", x => x.id);
                    table.ForeignKey(
                        name: "fk_brand_slug_history_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "tenancy",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "staff",
                schema: "tenancy",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    full_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    display_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    title = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    calendar_color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    service_commission_percent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    sale_commission_percent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    is_bookable_online = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_staff", x => x.id);
                    table.CheckConstraint("ck_staff_calendar_color", "calendar_color ~ '^#[0-9A-Fa-f]{6}$'");
                    table.ForeignKey(
                        name: "fk_staff_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "tenancy",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "staff_branches",
                schema: "tenancy",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    staff_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_staff_branches", x => x.id);
                    table.ForeignKey(
                        name: "fk_staff_branches_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "tenancy",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_staff_branches_staff_staff_id",
                        column: x => x.staff_id,
                        principalSchema: "tenancy",
                        principalTable: "staff",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_branches_brand_id_slug",
                schema: "tenancy",
                table: "branches",
                columns: new[] { "brand_id", "slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_brand_slug_history_brand_id_slug",
                schema: "tenancy",
                table: "brand_slug_history",
                columns: new[] { "brand_id", "slug" });

            migrationBuilder.CreateIndex(
                name: "ix_brand_slug_history_slug",
                schema: "tenancy",
                table: "brand_slug_history",
                column: "slug");

            migrationBuilder.CreateIndex(
                name: "ix_brands_slug",
                schema: "tenancy",
                table: "brands",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_staff_brand_id",
                schema: "tenancy",
                table: "staff",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_staff_brand_id_user_id",
                schema: "tenancy",
                table: "staff",
                columns: new[] { "brand_id", "user_id" },
                unique: true,
                filter: "user_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_staff_branches_branch_id",
                schema: "tenancy",
                table: "staff_branches",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "ix_staff_branches_brand_id_branch_id",
                schema: "tenancy",
                table: "staff_branches",
                columns: new[] { "brand_id", "branch_id" });

            migrationBuilder.CreateIndex(
                name: "ix_staff_branches_brand_id_staff_id_branch_id",
                schema: "tenancy",
                table: "staff_branches",
                columns: new[] { "brand_id", "staff_id", "branch_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_staff_branches_staff_id",
                schema: "tenancy",
                table: "staff_branches",
                column: "staff_id");

            // Hepsi markaya aittir (ADR-0006). Marka tablosunda kiracı sütunu kimliğin kendisidir. Bağlantı adı
            // uygunluğu ve slug çözümü markalar arası okumayı platform bağlamında yapar.
            migrationBuilder.EnableTenantIsolation("tenancy", "brands", tenantColumn: "id");
            migrationBuilder.EnableTenantIsolation("tenancy", "brand_settings");
            migrationBuilder.EnableTenantIsolation("tenancy", "booking_page_settings");
            migrationBuilder.EnableTenantIsolation("tenancy", "brand_slug_history");
            migrationBuilder.EnableTenantIsolation("tenancy", "branches");
            migrationBuilder.EnableTenantIsolation("tenancy", "staff");
            migrationBuilder.EnableTenantIsolation("tenancy", "staff_branches");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "booking_page_settings",
                schema: "tenancy");

            migrationBuilder.DropTable(
                name: "brand_settings",
                schema: "tenancy");

            migrationBuilder.DropTable(
                name: "brand_slug_history",
                schema: "tenancy");

            migrationBuilder.DropTable(
                name: "staff_branches",
                schema: "tenancy");

            migrationBuilder.DropTable(
                name: "branches",
                schema: "tenancy");

            migrationBuilder.DropTable(
                name: "staff",
                schema: "tenancy");

            migrationBuilder.DropTable(
                name: "brands",
                schema: "tenancy");
        }
    }
}
