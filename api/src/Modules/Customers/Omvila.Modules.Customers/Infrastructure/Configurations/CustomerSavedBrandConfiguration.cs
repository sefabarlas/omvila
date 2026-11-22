using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Customers.Domain;

namespace Omvila.Modules.Customers.Infrastructure.Configurations;

internal sealed class CustomerSavedBrandConfiguration : IEntityTypeConfiguration<CustomerSavedBrand>
{
    public void Configure(EntityTypeBuilder<CustomerSavedBrand> builder)
    {
        builder.ToTable("customer_saved_brands", table =>
            table.HasCheckConstraint("ck_customer_saved_brands_source", "source IN ('Qr', 'Link', 'Booking')"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Source).HasMaxLength(10);
        // Kişi başına markada tek satır (meSaveBrand kendiliğinden idempotent); "Salonlarım" müşterinin satırlarını okur.
        builder.HasIndex(x => new { x.CustomerId, x.BrandId }).IsUnique();
        builder.HasIndex(x => x.BrandId);
    }
}
