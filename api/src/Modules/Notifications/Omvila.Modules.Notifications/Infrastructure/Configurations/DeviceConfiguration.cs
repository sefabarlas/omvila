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
        builder.HasIndex(x => x.PushToken).IsUnique().HasFilter("is_active");
        // Gönderimde alıcının aktif cihazları.
        builder.HasIndex(x => new { x.BrandId, x.OwnerType, x.OwnerId }).HasFilter("is_active");
    }
}
