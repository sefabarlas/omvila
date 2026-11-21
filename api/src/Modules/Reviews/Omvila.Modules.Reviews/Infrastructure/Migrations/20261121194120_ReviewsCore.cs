using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Omvila.Infrastructure.Persistence;

#nullable disable

namespace Omvila.Modules.Reviews.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReviewsCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "reviews");

            migrationBuilder.GrantSchemaUsageToApp("reviews");

            migrationBuilder.CreateTable(
                name: "review_invitations",
                schema: "reviews",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    appointment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    send_at_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_review_invitations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "reviews",
                schema: "reviews",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    appointment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    staff_id = table.Column<Guid>(type: "uuid", nullable: true),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    service_names = table.Column<List<string>>(type: "text[]", nullable: false),
                    is_published = table.Column<bool>(type: "boolean", nullable: false),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    reply_text = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    replied_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    replied_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    replied_by_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reviews", x => x.id);
                    table.CheckConstraint("ck_reviews_rating", "rating BETWEEN 1 AND 5");
                    table.CheckConstraint("ck_reviews_reply", "(reply_text IS NULL) = (replied_utc IS NULL)");
                });

            migrationBuilder.CreateIndex(
                name: "ix_review_invitations_brand_id_appointment_id",
                schema: "reviews",
                table: "review_invitations",
                columns: new[] { "brand_id", "appointment_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_review_invitations_brand_id_branch_id_send_at_utc",
                schema: "reviews",
                table: "review_invitations",
                columns: new[] { "brand_id", "branch_id", "send_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_reviews_brand_id_appointment_id",
                schema: "reviews",
                table: "reviews",
                columns: new[] { "brand_id", "appointment_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reviews_brand_id_branch_id_created_utc",
                schema: "reviews",
                table: "reviews",
                columns: new[] { "brand_id", "branch_id", "created_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_reviews_brand_id_brand_customer_id_created_utc",
                schema: "reviews",
                table: "reviews",
                columns: new[] { "brand_id", "brand_customer_id", "created_utc" });

            migrationBuilder.EnableTenantIsolation("reviews", "reviews");
            migrationBuilder.EnableTenantIsolation("reviews", "review_invitations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "review_invitations",
                schema: "reviews");

            migrationBuilder.DropTable(
                name: "reviews",
                schema: "reviews");
        }
    }
}
