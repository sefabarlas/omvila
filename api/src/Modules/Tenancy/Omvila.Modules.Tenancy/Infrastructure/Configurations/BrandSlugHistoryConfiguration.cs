using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Infrastructure.Configurations;

internal sealed class BrandSlugHistoryConfiguration : IEntityTypeConfiguration<BrandSlugHistory>
{
    public void Configure(EntityTypeBuilder<BrandSlugHistory> builder)
    {
        builder.ToTable("brand_slug_history");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Slug).HasMaxLength(60);
        builder.HasIndex(x => new { x.BrandId, x.Slug });
        // Bağlantı çözümü ve uygunluk denetimi markalar arası adla arar.
        builder.HasIndex(x => x.Slug);
        builder.HasOne<Brand>().WithMany().HasForeignKey(x => x.BrandId).OnDelete(DeleteBehavior.Restrict);
    }
}
