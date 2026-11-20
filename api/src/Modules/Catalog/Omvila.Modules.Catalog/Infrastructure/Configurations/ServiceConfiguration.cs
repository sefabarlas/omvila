using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Catalog.Domain;

namespace Omvila.Modules.Catalog.Infrastructure.Configurations;

internal sealed class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("services", table =>
        {
            table.HasCheckConstraint("ck_services_duration", "duration_minutes BETWEEN 5 AND 720");
            table.HasCheckConstraint("ck_services_price", "base_price_minor >= 0");
            table.HasCheckConstraint("ck_services_deposit_xor", "deposit_amount_minor IS NULL OR deposit_percent IS NULL");
            table.HasCheckConstraint("ck_services_deposit_mode", "deposit_mode IN ('Inherit', 'Always', 'Never')");
            table.HasCheckConstraint("ck_services_price_display", "price_display IN ('Fixed', 'StartingFrom')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Name).HasMaxLength(120);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsFixedLength();
        builder.Property(x => x.PriceDisplay).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.DepositMode).HasConversion<string>().HasMaxLength(10);
        builder.Property(x => x.DepositPercent).HasPrecision(5, 2);
        builder.Property(x => x.Version).IsRowVersion();
        builder.Ignore(x => x.BasePrice);
        builder.Ignore(x => x.Deposit);
        // Ad marka içinde aktif hizmetler arasında tektir (alan-modeli.md § Service).
        builder.HasIndex(x => new { x.BrandId, x.Name }).IsUnique().HasFilter("is_active");
        builder.HasIndex(x => new { x.BrandId, x.CategoryId });
        builder.HasOne<ServiceCategory>().WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
    }
}
