using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Catalog.Domain;

namespace Omvila.Modules.Catalog.Infrastructure.Configurations;

internal sealed class BranchServicePriceConfiguration : IEntityTypeConfiguration<BranchServicePrice>
{
    public void Configure(EntityTypeBuilder<BranchServicePrice> builder)
    {
        builder.ToTable("branch_service_prices", table => table.HasCheckConstraint("ck_branch_service_prices_price", "price_minor >= 0"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        // Şube Tenancy'dedir: modüller arası yabancı anahtar yok.
        builder.HasIndex(x => new { x.BrandId, x.BranchId, x.ServiceId }).IsUnique();
        builder.HasOne<Service>().WithMany().HasForeignKey(x => x.ServiceId).OnDelete(DeleteBehavior.Cascade);
    }
}
