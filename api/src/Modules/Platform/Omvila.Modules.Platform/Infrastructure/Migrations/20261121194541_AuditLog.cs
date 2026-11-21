using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Omvila.Infrastructure.Persistence;

#nullable disable

namespace Omvila.Modules.Platform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "audit_logs",
                schema: "platform",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    actor_type = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    actor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: true),
                    action = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    entity_type = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    changes = table.Column<string>(type: "jsonb", nullable: true),
                    ip_address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    occurred_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_logs", x => x.id);
                    table.CheckConstraint("ck_audit_logs_action", "action IN ('Create', 'Update', 'Delete', 'View', 'Approve', 'Match', 'Export', 'Anonymize')");
                    table.CheckConstraint("ck_audit_logs_actor_type", "actor_type IN ('User', 'PlatformUser', 'Customer', 'System')");
                });

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_brand_id_occurred_utc",
                schema: "platform",
                table: "audit_logs",
                columns: new[] { "brand_id", "occurred_utc" });

            // Denetim kaydı markaya aittir (ADR-0006); platform geneli kayıt (brand_id boş) her bağlamdan eklenir, yalnız
            // platform okur. Kayıt değiştirilemez ve silinemez (alan-modeli.md § AuditLog: yalnız INSERT).
            migrationBuilder.EnableTenantIsolation("platform", "audit_logs", allowPlatformRowInsert: true);
            migrationBuilder.Sql($"REVOKE UPDATE, DELETE ON platform.audit_logs FROM {MigrationBuilderExtensions.AppRole};");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs",
                schema: "platform");
        }
    }
}
