using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Omvila.Infrastructure.Persistence;
using NodaTime;

#nullable disable

namespace Omvila.Modules.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UsersAndMemberships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    phone_e164 = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    full_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    language_code = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    last_login_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_memberships",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    branch_ids = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    staff_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    activated_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_memberships", x => x.id);
                    table.CheckConstraint("ck_user_memberships_role", "role IN ('Owner', 'BranchManager', 'Staff')");
                    table.CheckConstraint("ck_user_memberships_status", "status IN ('Invited', 'Active', 'Disabled')");
                    table.ForeignKey(
                        name: "fk_user_memberships_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_memberships_brand_id_user_id",
                schema: "identity",
                table: "user_memberships",
                columns: new[] { "brand_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_memberships_user_id",
                schema: "identity",
                table: "user_memberships",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_phone_e164",
                schema: "identity",
                table: "users",
                column: "phone_e164",
                unique: true);

            // Üyelik markaya aittir (ADR-0006). Giriş akışı kullanıcı kimliğiyle süzülmüş okumada platform bağlamı kullanır.
            migrationBuilder.EnableTenantIsolation("identity", "user_memberships");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_memberships",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "users",
                schema: "identity");
        }
    }
}
