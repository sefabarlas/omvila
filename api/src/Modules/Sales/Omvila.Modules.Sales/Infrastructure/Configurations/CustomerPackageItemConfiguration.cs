using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Sales.Domain;

namespace Omvila.Modules.Sales.Infrastructure.Configurations;

internal sealed class CustomerPackageItemConfiguration : IEntityTypeConfiguration<CustomerPackageItem>
{
    public void Configure(EntityTypeBuilder<CustomerPackageItem> builder)
    {
        builder.ToTable("customer_package_items", table =>
            table.HasCheckConstraint("ck_customer_package_items_total_sessions", "total_sessions > 0"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.HasIndex(x => new { x.BrandId, x.CustomerPackageId });
    }
}
