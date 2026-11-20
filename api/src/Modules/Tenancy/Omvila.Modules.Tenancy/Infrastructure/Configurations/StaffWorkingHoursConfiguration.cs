using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Infrastructure.Configurations;

internal sealed class StaffWorkingHoursConfiguration : IEntityTypeConfiguration<StaffWorkingHours>
{
    public void Configure(EntityTypeBuilder<StaffWorkingHours> builder)
    {
        builder.ToTable("staff_working_hours", table =>
        {
            table.HasCheckConstraint("ck_staff_working_hours_range", "start_time < end_time");
            table.HasCheckConstraint("ck_staff_working_hours_day", "day_of_week BETWEEN 1 AND 7");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.DayOfWeek).HasConversion<int>();
        builder.HasIndex(x => new { x.BrandId, x.StaffBranchId, x.DayOfWeek });
        builder.HasOne<StaffBranch>().WithMany().HasForeignKey(x => x.StaffBranchId).OnDelete(DeleteBehavior.Cascade);
    }
}
