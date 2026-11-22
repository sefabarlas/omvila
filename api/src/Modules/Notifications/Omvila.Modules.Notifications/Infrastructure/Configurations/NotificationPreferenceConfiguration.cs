using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Notifications.Domain;

namespace Omvila.Modules.Notifications.Infrastructure.Configurations;

internal sealed class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
{
    public void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.ToTable("notification_preferences");
        // Marka + kullanıcı başına tek satır; gönderimde (marka, kullanıcı) ile okunur.
        builder.HasKey(x => new { x.BrandId, x.UserId });
        builder.Property(x => x.Version).IsRowVersion();
    }
}
