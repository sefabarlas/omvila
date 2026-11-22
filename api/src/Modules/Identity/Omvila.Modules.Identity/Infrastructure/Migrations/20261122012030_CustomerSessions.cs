using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Omvila.Modules.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CustomerSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "audience",
                schema: "identity",
                table: "refresh_tokens",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "manage");

            migrationBuilder.AddCheckConstraint(
                name: "ck_refresh_tokens_audience",
                schema: "identity",
                table: "refresh_tokens",
                sql: "audience IN ('manage', 'me')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_refresh_tokens_audience",
                schema: "identity",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "audience",
                schema: "identity",
                table: "refresh_tokens");
        }
    }
}
