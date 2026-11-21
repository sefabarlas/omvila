using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Notifications.Domain;

namespace Omvila.Modules.Notifications.Infrastructure.Configurations;

internal sealed class NotificationMessageConfiguration : IEntityTypeConfiguration<NotificationMessage>
{
    public void Configure(EntityTypeBuilder<NotificationMessage> builder)
    {
        builder.ToTable("notification_outbox", table =>
        {
            table.HasCheckConstraint("ck_notification_outbox_status", "status IN ('Pending', 'Sent', 'Failed', 'Cancelled', 'Skipped')");
            table.HasCheckConstraint("ck_notification_outbox_category", "category IN ('Transactional', 'Marketing')");
            table.HasCheckConstraint("ck_notification_outbox_recipient_type", "recipient_type IN ('Customer', 'User', 'Phone')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.EventCode).HasMaxLength(50);
        builder.Property(x => x.Category).HasMaxLength(20);
        builder.Property(x => x.RecipientType).HasMaxLength(20);
        builder.Property(x => x.RecipientPhone).HasMaxLength(16);
        builder.Property(x => x.ChannelUsed).HasMaxLength(10);
        builder.Property(x => x.Payload).HasColumnType("jsonb");
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.ProviderMessageId).HasMaxLength(100);
        builder.Property(x => x.FailureReason).HasMaxLength(200);
        builder.Property(x => x.ReferenceType).HasMaxLength(40);
        builder.Property(x => x.TemplateVariant).HasMaxLength(30);
        // RLS performansı: brand_id ilk sütun; dağıtıcının taradığı küme bekleyenler.
        builder.HasIndex(x => new { x.BrandId, x.CreatedUtc });
        builder.HasIndex(x => x.ScheduledUtc).HasFilter("status = 'Pending'");
        // Bekleyen hatırlatmaları kayda göre düşürme (randevu iptali / saat değişikliği).
        builder.HasIndex(x => new { x.BrandId, x.ReferenceId }).HasFilter("status = 'Pending' AND reference_id IS NOT NULL");
    }
}
