using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Omvila.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DevSmsLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dev_sms_log",
                schema: "infra",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    phone_e164 = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    sender = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    sent_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dev_sms_log", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_dev_sms_log_phone_e164_sent_utc",
                schema: "infra",
                table: "dev_sms_log",
                columns: new[] { "phone_e164", "sent_utc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dev_sms_log",
                schema: "infra");
        }
    }
}
