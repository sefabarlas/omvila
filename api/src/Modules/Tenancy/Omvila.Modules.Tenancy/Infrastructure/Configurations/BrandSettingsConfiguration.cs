using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Infrastructure.Configurations;

internal sealed class BrandSettingsConfiguration : IEntityTypeConfiguration<BrandSettings>
{
    public void Configure(EntityTypeBuilder<BrandSettings> builder)
    {
        builder.ToTable("brand_settings", table =>
            table.HasCheckConstraint("ck_brand_settings_deposit_policy", "deposit_policy IN ('None', 'RiskyCustomersOnly', 'Always')"));
        builder.HasKey(x => x.BrandId);
        builder.Property(x => x.BrandId).ValueGeneratedNever();
        builder.Property(x => x.DepositPolicy).HasMaxLength(30);
        builder.Property(x => x.Version).IsRowVersion();
        builder.HasOne<Brand>().WithOne().HasForeignKey<BrandSettings>(x => x.BrandId).OnDelete(DeleteBehavior.Restrict);
    }
}
