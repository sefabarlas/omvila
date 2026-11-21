using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Notifications.Domain;

namespace Omvila.Modules.Notifications.Infrastructure.Configurations;

internal sealed class InAppNotificationConfiguration : IEntityTypeConfiguration<InAppNotification>
{
    public void Configure(EntityTypeBuilder<InAppNotification> builder)
    {
        builder.ToTable("in_app_notifications", table => table.HasCheckConstraint(
            "ck_in_app_notifications_target_type",
            "target_type IN ('Appointment', 'Calendar', 'Today', 'Deposit', 'Reviews', 'WalkInQueue', 'SmsCredits', 'Subscription')"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.EventCode).HasMaxLength(50);
        builder.Property(x => x.Title).HasMaxLength(InAppNotification.TitleMaxLength);
        builder.Property(x => x.Body).HasMaxLength(InAppNotification.BodyMaxLength);
        builder.Property(x => x.TargetType).HasMaxLength(20);
        // Panel: kullanıcının bildirimleri yeniden eskiye; zil: okunmamış sayısı.
        builder.HasIndex(x => new { x.BrandId, x.UserId, x.CreatedUtc, x.Id }).IsDescending(false, false, true, true);
        builder.HasIndex(x => new { x.BrandId, x.UserId }).HasFilter("read_utc IS NULL").HasDatabaseName("ix_in_app_notifications_unread");
        // Talep yanıtlanınca bekleyen satırların kalan süresi düşer.
        builder.HasIndex(x => new { x.BrandId, x.TargetId }).HasFilter("hold_expires_utc IS NOT NULL").HasDatabaseName("ix_in_app_notifications_holds");
    }
}
