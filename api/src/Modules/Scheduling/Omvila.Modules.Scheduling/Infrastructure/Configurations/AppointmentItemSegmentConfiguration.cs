using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Scheduling.Domain;

namespace Omvila.Modules.Scheduling.Infrastructure.Configurations;

internal sealed class AppointmentItemSegmentConfiguration : IEntityTypeConfiguration<AppointmentItemSegment>
{
    /// <summary>
    /// Çakışma engelinin son garantisi (musaitlik.md § Katman 1, ADR-0004): aynı personelin aktif, personel-meşgul iki
    /// dilimi örtüşemez. Kısıt şubeyle ve markayla filtrelenmez. EF exclusion constraint modellemediği için migration'da
    /// SQL ile kurulur (ConflictGuard). Kısıtın GiST indeksi müsaitlik motorunun meşguliyet sorgusuna da hizmet eder;
    /// aynı koşullu eski indeks (ix_appointment_item_segments_staff_during) bu yüzden kaldırıldı.
    /// </summary>
    public const string StaffOverlapConstraint = "ex_segment_staff_overlap";

    public void Configure(EntityTypeBuilder<AppointmentItemSegment> builder)
    {
        builder.ToTable("appointment_item_segments", table =>
        {
            table.HasCheckConstraint("ck_appointment_item_segments_kind", EnumCheck.In<SegmentKind>("kind"));
            // [Start, End) yarı açık ve boş olmayan aralık (musaitlik.md § Kavramlar).
            table.HasCheckConstraint(
                "ck_appointment_item_segments_during",
                "NOT isempty(during) AND lower_inc(during) AND NOT upper_inc(during) AND NOT lower_inf(during) AND NOT upper_inf(during)");
            table.HasCheckConstraint("ck_appointment_item_segments_superseded", "superseded_utc IS NULL OR NOT is_active");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Kind).HasConversion<string>().HasMaxLength(15);
        builder.Property(x => x.During).HasColumnType("tstzrange");

        // Saat değişikliğinde yeni dilimler aynı sıra numaralarını alır; tekillik yalnız güncel dilimlerde.
        builder.HasIndex(x => new { x.BrandId, x.AppointmentItemId, x.SeqNo }).IsUnique().HasFilter("superseded_utc IS NULL").HasDatabaseName("ix_appointment_item_segments_brand_id_item_id_seq_no");
    }
}
