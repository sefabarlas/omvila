using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Omvila.Modules.Scheduling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConflictGuard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Çakışma engeli katman 1 (musaitlik.md, ADR-0004): şubeyle filtrelenmez; bekleme dilimi ve pasif dilim kapsam
            // dışıdır. Kısıtın GiST indeksi eski koşullu indeksin yerini alır (aynı sütunlar, aynı koşul). Kaynak kısıtı
            // (ex_item_resource_overlap) kaynak tabloları gelince eklenir (T-48, T-49).
            migrationBuilder.Sql("""
                ALTER TABLE scheduling.appointment_item_segments
                  ADD CONSTRAINT ex_segment_staff_overlap
                  EXCLUDE USING gist (staff_id WITH =, during WITH &&)
                  WHERE (staff_busy AND is_active AND staff_id IS NOT NULL);
                """);

            migrationBuilder.DropIndex(
                name: "ix_appointment_item_segments_staff_during",
                schema: "scheduling",
                table: "appointment_item_segments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE scheduling.appointment_item_segments DROP CONSTRAINT ex_segment_staff_overlap;");

            migrationBuilder.CreateIndex(
                name: "ix_appointment_item_segments_staff_during",
                schema: "scheduling",
                table: "appointment_item_segments",
                columns: new[] { "staff_id", "during" },
                filter: "staff_busy AND is_active AND staff_id IS NOT NULL")
                .Annotation("Npgsql:IndexMethod", "gist");
        }
    }
}
