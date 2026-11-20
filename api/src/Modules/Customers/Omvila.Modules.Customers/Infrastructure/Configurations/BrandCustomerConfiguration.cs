using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Customers.Domain;

namespace Omvila.Modules.Customers.Infrastructure.Configurations;

internal sealed class BrandCustomerConfiguration : IEntityTypeConfiguration<BrandCustomer>
{
    public const string BrandCustomerIndex = "ix_brand_customers_brand_id_customer_id";

    public void Configure(EntityTypeBuilder<BrandCustomer> builder)
    {
        builder.ToTable("brand_customers", table =>
        {
            table.HasCheckConstraint("ck_brand_customers_source", "source IN ('Online', 'Panel', 'Phone', 'WalkIn', 'Import')");
            table.HasCheckConstraint(
                "ck_brand_customers_counters",
                "visit_count >= 0 AND cancel_count >= 0 AND no_show_count >= 0 AND total_spent_minor >= 0");
            table.HasCheckConstraint("ck_brand_customers_birth_year", "birth_year IS NULL OR birth_year >= 1900");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.DisplayName).HasMaxLength(120);
        builder.Property(x => x.Notes).HasMaxLength(2000);
        builder.Property(x => x.Source).HasConversion<string>().HasMaxLength(10);
        builder.Property(x => x.BlockReason).HasMaxLength(200);
        builder.Property(x => x.Version).IsRowVersion();
        // Bir kişi markada tek karttır (alan-modeli.md § BrandCustomer).
        builder.HasIndex(x => new { x.BrandId, x.CustomerId }).IsUnique().HasDatabaseName(BrandCustomerIndex);
        builder.HasIndex(x => new { x.BrandId, x.LastVisitOn });
        builder.HasIndex(x => x.CustomerId);
        builder.HasOne<Customer>().WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
    }
}
