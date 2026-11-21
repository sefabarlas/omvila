using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Omvila.Infrastructure.Persistence;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Tenancy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BrandBankAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "brand_bank_accounts",
                schema: "tenancy",
                columns: table => new
                {
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_holder_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    bank_name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    iban_encrypted = table.Column<byte[]>(type: "bytea", nullable: false),
                    iban_last4 = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    transfer_code_prefix = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    transfer_window_minutes = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    updated_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_bank_accounts", x => x.brand_id);
                    table.CheckConstraint("ck_brand_bank_accounts_iban_last4", "iban_last4 ~ '^[0-9]{4}$'");
                    table.CheckConstraint("ck_brand_bank_accounts_transfer_code_prefix", "transfer_code_prefix ~ '^[A-Z]{2,5}$'");
                    table.CheckConstraint("ck_brand_bank_accounts_transfer_window", "transfer_window_minutes IN (60, 120, 240)");
                    table.ForeignKey(
                        name: "fk_brand_bank_accounts_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "tenancy",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.EnableTenantIsolation("tenancy", "brand_bank_accounts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "brand_bank_accounts",
                schema: "tenancy");
        }
    }
}
