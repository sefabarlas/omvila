using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Notifications.Domain;

namespace Omvila.Modules.Notifications.Infrastructure.Configurations;

internal sealed class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("devices", table =>
        {
            table.HasCheckConstraint("ck_devices_owner_type", "owner_type IN ('User', 'Customer')");
            table.HasCheckConstraint("ck_devices_app", "app IN ('Business', 'Customer')");
            table.HasCheckConstraint("ck_devices_platform", "platform IN ('iOS', 'Android')");
            // İşletme cihazı markalı, müşteri cihazı markasız (T-103).
            table.HasCheckConstraint(
                "ck_devices_owner_brand",
                "(owner_type = 'User' AND app = 'Business' AND brand_id IS NOT NULL) OR (owner_type = 'Customer' AND app = 'Customer' AND brand_id IS NULL)");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.OwnerType).HasMaxLength(20);
        builder.Property(x => x.App).HasMaxLength(20);
        builder.Property(x => x.Platform).HasConversion<string>().HasMaxLength(10);
        builder.Property(x => x.PushToken).HasMaxLength(4096);
        builder.Property(x => x.AppVersion).HasMaxLength(20);
        // Upsert anahtarı (marka bağlamında) ve alan modeli: aktif kayıtlar arasında jeton tek.
        builder.HasIndex(x => new { x.BrandId, x.PushToken }).IsUnique();
        // Müşteri cihazının upsert anahtarı (markasız satırlarda jeton tek).
        builder.HasIndex(x => x.PushToken, "ix_devices_customer_push_token").IsUnique().HasFilter("brand_id IS NULL").HasDatabaseName("ix_devices_customer_push_token");
        builder.HasIndex(x => x.PushToken).IsUnique().HasFilter("is_active");
        // Gönderimde alıcının aktif cihazları.
        builder.HasIndex(x => new { x.BrandId, x.OwnerType, x.OwnerId }).HasFilter("is_active");
        builder.HasIndex(x => x.OwnerId, "ix_devices_customer_owner").HasFilter("is_active AND brand_id IS NULL").HasDatabaseName("ix_devices_customer_owner");
    }
}
