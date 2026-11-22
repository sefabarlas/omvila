using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Infrastructure.Configurations;

internal sealed class BrandSettingsConfiguration : IEntityTypeConfiguration<BrandSettings>
{
    public void Configure(EntityTypeBuilder<BrandSettings> builder)
    {
        builder.ToTable("brand_settings", table =>
        {
            table.HasCheckConstraint("ck_brand_settings_deposit_policy", "deposit_policy IN ('None', 'RiskyCustomersOnly', 'Always')");
            table.HasCheckConstraint("ck_brand_settings_privacy_notice_mode", "privacy_notice_mode IN ('Standard', 'Custom')");
            table.HasCheckConstraint("ck_brand_settings_transaction_retention_years", "transaction_retention_years IN (3, 5, 10)");
        });
        builder.HasKey(x => x.BrandId);
        builder.Property(x => x.BrandId).ValueGeneratedNever();
        builder.Property(x => x.DepositPolicy).HasMaxLength(30);
        builder.Property(x => x.PrivacyNoticeMode).HasMaxLength(20);
        builder.Property(x => x.CustomPrivacyNoticeText).HasMaxLength(PrivacySettingsRules.CustomTextMaxLength);
        builder.Property(x => x.Version).IsRowVersion();
        builder.HasOne<Brand>().WithOne().HasForeignKey<BrandSettings>(x => x.BrandId).OnDelete(DeleteBehavior.Restrict);
    }
}
