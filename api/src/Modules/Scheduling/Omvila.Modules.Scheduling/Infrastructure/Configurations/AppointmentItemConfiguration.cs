using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Scheduling.Domain;

namespace Omvila.Modules.Scheduling.Infrastructure.Configurations;

internal sealed class AppointmentItemConfiguration : IEntityTypeConfiguration<AppointmentItem>
{
    public void Configure(EntityTypeBuilder<AppointmentItem> builder)
    {
        builder.ToTable("appointment_items", table =>
        {
            table.HasCheckConstraint("ck_appointment_items_status", EnumCheck.In<AppointmentItemStatus>("status"));
            table.HasCheckConstraint("ck_appointment_items_time", "end_utc > start_utc");
            table.HasCheckConstraint(
                "ck_appointment_items_amounts",
                "price_minor >= 0 AND staff_surcharge_minor >= 0 AND staff_surcharge_minor <= price_minor AND discount_minor >= 0 AND discount_minor <= price_minor");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(10);
        builder.Property(x => x.ServiceNameSnapshot).HasMaxLength(120);
        builder.Ignore(x => x.NetAmountMinor);
        builder.Ignore(x => x.IsFromPackage);
        builder.Ignore(x => x.CurrentSegments);

        builder.HasMany(x => x.Segments).WithOne().HasForeignKey(s => s.AppointmentItemId).OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(x => x.Segments).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => new { x.BrandId, x.AppointmentId, x.SeqNo }).IsUnique();
        // Takvim personel kolonları.
        builder.HasIndex(x => new { x.BrandId, x.StaffId, x.StartUtc });
    }
}
