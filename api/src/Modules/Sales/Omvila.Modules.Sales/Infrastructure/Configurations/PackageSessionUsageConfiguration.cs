using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Sales.Domain;

namespace Omvila.Modules.Sales.Infrastructure.Configurations;

internal sealed class PackageSessionUsageConfiguration : IEntityTypeConfiguration<PackageSessionUsage>
{
    public void Configure(EntityTypeBuilder<PackageSessionUsage> builder)
    {
        builder.ToTable("package_session_usages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Ignore(x => x.IsActive);
        builder.HasOne<CustomerPackageItem>().WithMany().HasForeignKey(x => x.CustomerPackageItemId).OnDelete(DeleteBehavior.Restrict);

        // Kalan seans defteri (paket kalemi başına geri dönmemiş kullanımlar); iptalde randevunun kullanımları.
        builder.HasIndex(x => new { x.BrandId, x.CustomerPackageItemId });
        builder.HasIndex(x => new { x.BrandId, x.AppointmentId });
        // Bir randevu kalemi aynı anda tek seans kullanır.
        builder.HasIndex(x => new { x.BrandId, x.AppointmentItemId }).IsUnique().HasFilter("reverted_utc IS NULL");
    }
}
