using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Customers.Domain;

namespace Omvila.Modules.Customers.Infrastructure.Configurations;

internal sealed class ConsentRecordConfiguration : IEntityTypeConfiguration<ConsentRecord>
{
    public void Configure(EntityTypeBuilder<ConsentRecord> builder)
    {
        builder.ToTable("consent_records", table =>
        {
            table.HasCheckConstraint(
                "ck_consent_records_consent_type", "consent_type IN ('PrivacyNoticeAcknowledged', 'MarketingSms', 'MarketingEmail', 'MarketingPush')");
            table.HasCheckConstraint(
                "ck_consent_records_channel", "channel IN ('Web', 'CustomerApp', 'BusinessApp', 'Panel', 'Import')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.ConsentType).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.Channel).HasConversion<string>().HasMaxLength(15);
        builder.Property(x => x.DocumentVersion).HasMaxLength(ConsentRecord.DocumentVersionMaxLength);
        builder.Property(x => x.IpAddress).HasMaxLength(45);
        builder.HasIndex(x => new { x.BrandId, x.CustomerId, x.ConsentType, x.OccurredUtc }).HasDatabaseName("ix_consent_records_brand_customer_type_occurred");
        builder.HasIndex(x => x.CustomerId);
        builder.HasOne<Customer>().WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
    }
}
