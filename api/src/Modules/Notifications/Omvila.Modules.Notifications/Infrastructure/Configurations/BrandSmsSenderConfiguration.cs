using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Notifications.Domain;

namespace Omvila.Modules.Notifications.Infrastructure.Configurations;

internal sealed class BrandSmsSenderConfiguration : IEntityTypeConfiguration<BrandSmsSender>
{
    public void Configure(EntityTypeBuilder<BrandSmsSender> builder)
    {
        builder.ToTable("brand_sms_senders", table =>
        {
            // None satır olarak saklanmaz (başvuru yoksa satır yok).
            table.HasCheckConstraint("ck_brand_sms_senders_status", "status IN ('Pending', 'Approved', 'Rejected')");
            table.HasCheckConstraint("ck_brand_sms_senders_name", "name ~ '^[A-Z0-9]{3,11}$'");
        });
        builder.HasKey(x => x.BrandId);
        builder.Property(x => x.BrandId).ValueGeneratedNever();
        builder.Property(x => x.Name).HasMaxLength(11);
        builder.Property(x => x.ApprovedName).HasMaxLength(11);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.RejectionReason).HasMaxLength(500);
    }
}
