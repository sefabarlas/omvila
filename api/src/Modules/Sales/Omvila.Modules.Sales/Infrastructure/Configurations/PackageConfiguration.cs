using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Sales.Domain;

namespace Omvila.Modules.Sales.Infrastructure.Configurations;

internal sealed class PackageConfiguration : IEntityTypeConfiguration<Package>
{
    public void Configure(EntityTypeBuilder<Package> builder)
    {
        builder.ToTable("packages", table =>
        {
            table.HasCheckConstraint("ck_packages_price", "price_minor > 0");
            table.HasCheckConstraint("ck_packages_validity_days", $"validity_days BETWEEN 1 AND {Package.MaxValidityDays}");
            table.HasCheckConstraint(
                "ck_packages_sale_commission_percent",
                "sale_commission_percent_override IS NULL OR (sale_commission_percent_override >= 0 AND sale_commission_percent_override <= 100)");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Name).HasMaxLength(Package.NameMaxLength);
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsFixedLength();
        builder.Property(x => x.SaleCommissionPercentOverride).HasPrecision(5, 2);
        builder.Property(x => x.Version).IsRowVersion();
        builder.HasMany(x => x.Items).WithOne().HasForeignKey(i => i.PackageId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(x => new { x.BrandId, x.IsActive });
    }
}
