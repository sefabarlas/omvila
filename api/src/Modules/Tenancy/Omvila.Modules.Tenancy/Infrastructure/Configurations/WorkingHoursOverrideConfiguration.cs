using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Infrastructure.Configurations;

internal sealed class WorkingHoursOverrideConfiguration : IEntityTypeConfiguration<WorkingHoursOverride>
{
    public void Configure(EntityTypeBuilder<WorkingHoursOverride> builder)
    {
        builder.ToTable("working_hours_overrides", table =>
        {
            table.HasCheckConstraint("ck_working_hours_overrides_kind", "kind IN ('Holiday', 'Closure', 'ExtraOpening', 'ShortDay')");
            // Kapalı günde aralık yok, açık günde en az bir aralık (DayOverrides).
            table.HasCheckConstraint("ck_working_hours_overrides_intervals", "(is_closed AND cardinality(intervals) = 0) OR (NOT is_closed AND cardinality(intervals) > 0)");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Kind).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Note).HasMaxLength(200);
        builder.Ignore(x => x.OpeningIntervals);
        builder.HasIndex(x => new { x.BrandId, x.BranchId, x.Date }).IsUnique();
        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
    }
}
