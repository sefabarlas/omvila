using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Omvila.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OutboxAndJobs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "infra");

            migrationBuilder.CreateTable(
                name: "job_runs",
                schema: "infra",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    started_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    finished_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    error = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_runs", x => x.id);
                    table.CheckConstraint("ck_job_runs_status", "status IN ('Succeeded', 'Failed')");
                });

            migrationBuilder.CreateTable(
                name: "outbox_events",
                schema: "infra",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    occurred_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    available_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    attempts = table.Column<int>(type: "integer", nullable: false),
                    processed_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    last_error = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "processed_events",
                schema: "infra",
                columns: table => new
                {
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    handler = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    processed_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_processed_events", x => new { x.event_id, x.handler });
                });

            migrationBuilder.CreateIndex(
                name: "ix_job_runs_job_key_started_utc",
                schema: "infra",
                table: "job_runs",
                columns: new[] { "job_key", "started_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_outbox_events_available_utc",
                schema: "infra",
                table: "outbox_events",
                column: "available_utc",
                filter: "processed_utc IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "job_runs",
                schema: "infra");

            migrationBuilder.DropTable(
                name: "outbox_events",
                schema: "infra");

            migrationBuilder.DropTable(
                name: "processed_events",
                schema: "infra");
        }
    }
}
