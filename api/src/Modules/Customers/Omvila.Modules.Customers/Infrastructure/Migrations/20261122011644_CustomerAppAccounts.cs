using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Omvila.Infrastructure.Persistence;

#nullable disable

namespace Omvila.Modules.Customers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CustomerAppAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "email",
                schema: "customers",
                table: "customers",
                type: "character varying(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "has_app_account",
                schema: "customers",
                table: "customers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // xmin sistem sütunudur (iyimser kilit, sürüm = xmin); eklenmez.
            migrationBuilder.CreateTable(
                name: "customer_saved_brands",
                schema: "customers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    added_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    source = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customer_saved_brands", x => x.id);
                    table.CheckConstraint("ck_customer_saved_brands_source", "source IN ('Qr', 'Link', 'Booking')");
                });

            // Marka (silme, demo sıfırlama) veya müşterinin kendisi (me yüzeyi) görür; salon paneli bu tabloyu okumaz (KK1).
            migrationBuilder.EnableTenantIsolation("customers", "customer_saved_brands", customerColumn: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_saved_brands_brand_id",
                schema: "customers",
                table: "customer_saved_brands",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_saved_brands_customer_id_brand_id",
                schema: "customers",
                table: "customer_saved_brands",
                columns: new[] { "customer_id", "brand_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customer_saved_brands",
                schema: "customers");

            migrationBuilder.DropColumn(
                name: "email",
                schema: "customers",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "has_app_account",
                schema: "customers",
                table: "customers");
        }
    }
}
