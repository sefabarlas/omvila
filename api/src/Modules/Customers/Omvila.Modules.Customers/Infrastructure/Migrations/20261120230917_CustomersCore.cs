using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Omvila.Infrastructure.Persistence;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Customers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CustomersCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "customers");

            migrationBuilder.GrantSchemaUsageToApp("customers");

            migrationBuilder.CreateTable(
                name: "customers",
                schema: "customers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    phone_e164 = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    full_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    birth_year = table.Column<int>(type: "integer", nullable: true),
                    language_code = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    anonymized_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customers", x => x.id);
                    table.CheckConstraint("ck_customers_birth_year", "birth_year IS NULL OR birth_year >= 1900");
                });

            migrationBuilder.CreateTable(
                name: "brand_customers",
                schema: "customers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    birth_year = table.Column<int>(type: "integer", nullable: true),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    source = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    preferred_staff_id = table.Column<Guid>(type: "uuid", nullable: true),
                    first_visit_on = table.Column<LocalDate>(type: "date", nullable: true),
                    last_visit_on = table.Column<LocalDate>(type: "date", nullable: true),
                    visit_count = table.Column<int>(type: "integer", nullable: false),
                    cancel_count = table.Column<int>(type: "integer", nullable: false),
                    no_show_count = table.Column<int>(type: "integer", nullable: false),
                    total_spent_minor = table.Column<long>(type: "bigint", nullable: false),
                    is_blocked = table.Column<bool>(type: "boolean", nullable: false),
                    block_reason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    appointment_sms_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_customers", x => x.id);
                    table.CheckConstraint("ck_brand_customers_birth_year", "birth_year IS NULL OR birth_year >= 1900");
                    table.CheckConstraint("ck_brand_customers_counters", "visit_count >= 0 AND cancel_count >= 0 AND no_show_count >= 0 AND total_spent_minor >= 0");
                    table.CheckConstraint("ck_brand_customers_source", "source IN ('Online', 'Panel', 'Phone', 'WalkIn', 'Import')");
                    table.ForeignKey(
                        name: "fk_brand_customers_customers_customer_id",
                        column: x => x.customer_id,
                        principalSchema: "customers",
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "consent_records",
                schema: "customers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: true),
                    consent_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    is_granted = table.Column<bool>(type: "boolean", nullable: false),
                    document_version = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    channel = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    ip_address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    occurred_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_consent_records", x => x.id);
                    table.CheckConstraint("ck_consent_records_channel", "channel IN ('Web', 'CustomerApp', 'BusinessApp', 'Panel', 'Import')");
                    table.CheckConstraint("ck_consent_records_consent_type", "consent_type IN ('PrivacyNoticeAcknowledged', 'MarketingSms', 'MarketingEmail', 'MarketingPush')");
                    table.ForeignKey(
                        name: "fk_consent_records_customers_customer_id",
                        column: x => x.customer_id,
                        principalSchema: "customers",
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_brand_customers_brand_id_customer_id",
                schema: "customers",
                table: "brand_customers",
                columns: new[] { "brand_id", "customer_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_brand_customers_brand_id_last_visit_on",
                schema: "customers",
                table: "brand_customers",
                columns: new[] { "brand_id", "last_visit_on" });

            migrationBuilder.CreateIndex(
                name: "ix_brand_customers_customer_id",
                schema: "customers",
                table: "brand_customers",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_consent_records_brand_customer_type_occurred",
                schema: "customers",
                table: "consent_records",
                columns: new[] { "brand_id", "customer_id", "consent_type", "occurred_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_consent_records_customer_id",
                schema: "customers",
                table: "consent_records",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_customers_phone_e164",
                schema: "customers",
                table: "customers",
                column: "phone_e164",
                unique: true);

            // customers.customers global kişidir (identity.users gibi): brand_id yok, RLS yok; uygulama rolü varsayılan
            // tablo izinleriyle okur ve yazar. Markalar yalnız kendi kartları üzerinden erişir (alan-modeli.md § Customer).
            // Kart ve rıza markaya aittir (ADR-0006). Platform geneli rıza (brand_id boş) her bağlamdan eklenir, yalnız
            // platform okur.
            migrationBuilder.EnableTenantIsolation("customers", "brand_customers");
            migrationBuilder.EnableTenantIsolation("customers", "consent_records", allowPlatformRowInsert: true);

            // Rıza kaydı yalnız eklenir (alan-modeli.md § ConsentRecord): uygulama rolü güncelleyemez ve silemez.
            migrationBuilder.Sql($"REVOKE UPDATE, DELETE ON customers.consent_records FROM {MigrationBuilderExtensions.AppRole};");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "brand_customers",
                schema: "customers");

            migrationBuilder.DropTable(
                name: "consent_records",
                schema: "customers");

            migrationBuilder.DropTable(
                name: "customers",
                schema: "customers");
        }
    }
}
