using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Infrastructure.Configurations;

internal sealed class TimeOffConfiguration : IEntityTypeConfiguration<TimeOff>
{
    public void Configure(EntityTypeBuilder<TimeOff> builder)
    {
        builder.ToTable("time_offs", table =>
        {
            table.HasCheckConstraint("ck_time_offs_range", "start_utc < end_utc");
            table.HasCheckConstraint("ck_time_offs_type", "type IN ('Leave', 'Break', 'Training', 'Sick', 'Other')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Note).HasMaxLength(200);
        builder.HasIndex(x => new { x.BrandId, x.StaffId, x.StartUtc });
        builder.HasOne<Staff>().WithMany().HasForeignKey(x => x.StaffId).OnDelete(DeleteBehavior.Restrict);
    }
}
