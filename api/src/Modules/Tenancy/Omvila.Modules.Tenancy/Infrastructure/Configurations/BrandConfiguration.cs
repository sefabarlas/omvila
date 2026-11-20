using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Infrastructure.Configurations;

internal sealed class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("brands", table =>
        {
            table.HasCheckConstraint("ck_brands_status", "status IN ('Onboarding', 'Active', 'Suspended', 'Archived')");
            table.HasCheckConstraint(
                "ck_brands_industry_code", "industry_code IN ('barber', 'hair_salon', 'beauty', 'spa', 'nail', 'dietitian', 'other')");
            table.HasCheckConstraint("ck_brands_slug", "slug ~ '^[a-z0-9-]{3,60}$'");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Name).HasMaxLength(160);
        builder.Property(x => x.Slug).HasMaxLength(60);
        builder.Property(x => x.IndustryCode).HasMaxLength(20);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.ShortDescription).HasMaxLength(160);
        builder.Property(x => x.LogoUrl).HasMaxLength(500);
        builder.Property(x => x.Phone).HasMaxLength(16);
        builder.Property(x => x.Email).HasMaxLength(254);
        builder.Property(x => x.Instagram).HasMaxLength(60);
        builder.Property(x => x.Website).HasMaxLength(200);
        builder.Property(x => x.LegalName).HasMaxLength(200);
        builder.Property(x => x.TaxOffice).HasMaxLength(100);
        builder.Property(x => x.TaxNumber).HasMaxLength(20);
        builder.Property(x => x.DefaultCurrencyCode).HasMaxLength(3).IsFixedLength();
        builder.Property(x => x.Version).IsRowVersion();
        // Güncel bağlantı adı bütün markalar arasında tektir (yönlendirmesi süren eski adlar SlugAvailability'de).
        builder.HasIndex(x => x.Slug).IsUnique();
    }
}
