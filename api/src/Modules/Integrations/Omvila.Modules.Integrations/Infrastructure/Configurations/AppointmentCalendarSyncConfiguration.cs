using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Integrations.Domain;

namespace Omvila.Modules.Integrations.Infrastructure.Configurations;

internal sealed class AppointmentCalendarSyncConfiguration : IEntityTypeConfiguration<AppointmentCalendarSync>
{
    public void Configure(EntityTypeBuilder<AppointmentCalendarSync> builder)
    {
        builder.ToTable("appointment_calendar_syncs", table =>
            table.HasCheckConstraint("ck_appointment_calendar_syncs_sync_status", "sync_status IN ('Synced', 'Deleted', 'Failed')"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.ExternalEventId).HasMaxLength(256);
        builder.Property(x => x.SyncStatus).HasConversion<string>().HasMaxLength(15);
        builder.Property(x => x.ContentHash).HasMaxLength(64);
        builder.HasIndex(x => new { x.BrandId, x.CalendarLinkId, x.AppointmentId }).IsUnique();
        builder.HasOne<CalendarLink>().WithMany().HasForeignKey(x => x.CalendarLinkId).OnDelete(DeleteBehavior.Cascade);
    }
}
