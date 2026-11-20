using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TermsAcceptances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "terms_acceptances",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    document_version = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    accepted_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    ip_address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_terms_acceptances", x => x.id);
                    table.CheckConstraint("ck_terms_acceptances_document_type", "document_type IN ('TermsOfUse', 'PrivacyNotice')");
                    table.ForeignKey(
                        name: "fk_terms_acceptances_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_terms_acceptances_user_id_document_type_document_version",
                schema: "identity",
                table: "terms_acceptances",
                columns: new[] { "user_id", "document_type", "document_version" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "terms_acceptances",
                schema: "identity");
        }
    }
}
