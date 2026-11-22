using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Omvila.Infrastructure.Persistence;

#nullable disable

namespace Omvila.Modules.Notifications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CustomerAppNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "brand_id",
                schema: "notifications",
                table: "devices",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "customer_notifications",
                schema: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: true),
                    event_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    title = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    body = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    target_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    target_appointment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    target_access_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    created_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    read_utc = table.Column<Instant>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customer_notifications", x => x.id);
                    table.CheckConstraint("ck_customer_notifications_target_type", "target_type IN ('Appointment', 'WaitlistOffer', 'Review', 'Booking')");
                });

            migrationBuilder.CreateIndex(
                name: "ix_devices_customer_owner",
                schema: "notifications",
                table: "devices",
                column: "owner_id",
                filter: "is_active AND brand_id IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_devices_customer_push_token",
                schema: "notifications",
                table: "devices",
                column: "push_token",
                unique: true,
                filter: "brand_id IS NULL");

            migrationBuilder.AddCheckConstraint(
                name: "ck_devices_owner_brand",
                schema: "notifications",
                table: "devices",
                sql: "(owner_type = 'User' AND app = 'Business' AND brand_id IS NOT NULL) OR (owner_type = 'Customer' AND app = 'Customer' AND brand_id IS NULL)");

            // Müşteri bildirimi: müşterinin kendi bağlamında (me yüzeyi) görünür; gönderim işi platform bağlamında yazar (T-103).
            migrationBuilder.EnableTenantIsolation("notifications", "customer_notifications", customerColumn: "customer_id");

            // Müşteri cihazı markasızdır: marka veya platform bağlamına ek olarak müşterinin kendi bağlamında görünür (T-103).
            migrationBuilder.Sql("""
                DROP POLICY tenant_isolation ON notifications.devices;
                CREATE POLICY tenant_isolation ON notifications.devices
                  USING (
                    brand_id = nullif(current_setting('app.brand_id', true), '')::uuid
                    OR current_setting('app.is_platform', true) = 'true'
                    OR (owner_type = 'Customer' AND brand_id IS NULL AND owner_id = nullif(current_setting('app.customer_id', true), '')::uuid)
                  );
                """);

            migrationBuilder.CreateIndex(
                name: "ix_customer_notifications_brand_id",
                schema: "notifications",
                table: "customer_notifications",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_notifications_customer_id_created_utc_id",
                schema: "notifications",
                table: "customer_notifications",
                columns: new[] { "customer_id", "created_utc", "id" },
                descending: new[] { false, true, true });

            migrationBuilder.CreateIndex(
                name: "ix_customer_notifications_unread",
                schema: "notifications",
                table: "customer_notifications",
                column: "customer_id",
                filter: "read_utc IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM notifications.devices WHERE brand_id IS NULL;
                DROP POLICY tenant_isolation ON notifications.devices;
                CREATE POLICY tenant_isolation ON notifications.devices
                  USING (
                    brand_id = nullif(current_setting('app.brand_id', true), '')::uuid
                    OR current_setting('app.is_platform', true) = 'true'
                  );
                """);

            migrationBuilder.DropTable(
                name: "customer_notifications",
                schema: "notifications");

            migrationBuilder.DropIndex(
                name: "ix_devices_customer_owner",
                schema: "notifications",
                table: "devices");

            migrationBuilder.DropIndex(
                name: "ix_devices_customer_push_token",
                schema: "notifications",
                table: "devices");

            migrationBuilder.DropCheckConstraint(
                name: "ck_devices_owner_brand",
                schema: "notifications",
                table: "devices");

            migrationBuilder.AlterColumn<Guid>(
                name: "brand_id",
                schema: "notifications",
                table: "devices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
