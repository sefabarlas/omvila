using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Notifications.Domain;

namespace Omvila.Modules.Notifications.Infrastructure.Configurations;

internal sealed class CustomerNotificationConfiguration : IEntityTypeConfiguration<CustomerNotification>
{
    public void Configure(EntityTypeBuilder<CustomerNotification> builder)
    {
        builder.ToTable("customer_notifications", table => table.HasCheckConstraint(
            "ck_customer_notifications_target_type", "target_type IN ('Appointment', 'WaitlistOffer', 'Review', 'Booking')"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.EventCode).HasMaxLength(50);
        builder.Property(x => x.Title).HasMaxLength(CustomerNotification.TitleMaxLength);
        builder.Property(x => x.Body).HasMaxLength(CustomerNotification.BodyMaxLength);
        builder.Property(x => x.TargetType).HasMaxLength(20);
        builder.Property(x => x.TargetAccessCode).HasMaxLength(20);
        // Liste: müşterinin bildirimleri yeniden eskiye; zil: okunmamış sayısı.
        builder.HasIndex(x => new { x.CustomerId, x.CreatedUtc, x.Id }).IsDescending(false, true, true);
        builder.HasIndex(x => x.CustomerId).HasFilter("read_utc IS NULL").HasDatabaseName("ix_customer_notifications_unread");
        builder.HasIndex(x => x.BrandId);
    }
}
