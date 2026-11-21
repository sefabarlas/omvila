using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Sales.Domain;

namespace Omvila.Modules.Sales.Infrastructure.Configurations;

internal sealed class CustomerPackageConfiguration : IEntityTypeConfiguration<CustomerPackage>
{
    public void Configure(EntityTypeBuilder<CustomerPackage> builder)
    {
        builder.ToTable("customer_packages", table =>
        {
            table.HasCheckConstraint("ck_customer_packages_status", EnumCheck.In<CustomerPackageStatus>("status"));
            table.HasCheckConstraint("ck_customer_packages_price", "price_paid_minor > 0");
            table.HasCheckConstraint(
                "ck_customer_packages_refund",
                "(status = 'Refunded') = (refunded_amount_minor IS NOT NULL) AND (refunded_amount_minor IS NULL OR (refunded_amount_minor > 0 AND refunded_amount_minor <= price_paid_minor))");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.PackageNameSnapshot).HasMaxLength(Package.NameMaxLength);
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsFixedLength();
        builder.Property(x => x.RefundNote).HasMaxLength(CustomerPackage.RefundNoteMaxLength);
        builder.Property(x => x.Version).IsRowVersion();
        builder.HasMany(x => x.Items).WithOne().HasForeignKey(i => i.CustomerPackageId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Items).UsePropertyAccessMode(PropertyAccessMode.Field);

        // Müşteri kartı Paketler sekmesi; paket tanımının satış sayıları; süre dolumu işi (Active + son gün).
        builder.HasIndex(x => new { x.BrandId, x.BrandCustomerId });
        builder.HasIndex(x => new { x.BrandId, x.PackageId });
        builder.HasIndex(x => new { x.Status, x.ExpiresOn });
    }
}
