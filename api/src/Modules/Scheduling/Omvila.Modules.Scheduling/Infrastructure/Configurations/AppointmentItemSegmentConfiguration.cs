using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Scheduling.Domain;

namespace Omvila.Modules.Scheduling.Infrastructure.Configurations;

internal sealed class AppointmentItemSegmentConfiguration : IEntityTypeConfiguration<AppointmentItemSegment>
{
    /// <summary>
    /// Personelin aktif meşguliyeti (GiST, btree_gist). Müsaitlik motorunun meşguliyet sorgusu içindir; exclusion
    /// constraint (ex_segment_staff_overlap, musaitlik.md § Katman 1) çakışma engeli parçasında eklenir. Kısıt gibi
    /// şubeyle ve markayla filtrelenmez; bu yüzden brand_id ile başlamaz.
    /// </summary>
    public const string StaffDuringIndex = "ix_appointment_item_segments_staff_during";

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
        builder.HasIndex(x => new { x.StaffId, x.During })
            .HasDatabaseName(StaffDuringIndex)
            .HasMethod("gist")
            .HasFilter("staff_busy AND is_active AND staff_id IS NOT NULL");
    }
}
