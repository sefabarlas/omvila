using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Infrastructure.Configurations;

internal sealed class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("branches");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Name).HasMaxLength(120);
        builder.Property(x => x.Slug).HasMaxLength(60);
        builder.Property(x => x.AddressLine).HasMaxLength(300);
        builder.Property(x => x.District).HasMaxLength(80);
        builder.Property(x => x.City).HasMaxLength(80);
        builder.Property(x => x.PostalCode).HasMaxLength(10);
        builder.Property(x => x.CountryCode).HasMaxLength(2).IsFixedLength();
        builder.Property(x => x.Latitude).HasPrecision(9, 6);
        builder.Property(x => x.Longitude).HasPrecision(9, 6);
        builder.Property(x => x.Phone).HasMaxLength(16);
        builder.Property(x => x.Email).HasMaxLength(254);
        builder.Property(x => x.TimeZoneId).HasMaxLength(64);
        builder.Property(x => x.Version).IsRowVersion();
        builder.HasIndex(x => new { x.BrandId, x.Slug }).IsUnique();
        builder.HasOne<Brand>().WithMany().HasForeignKey(x => x.BrandId).OnDelete(DeleteBehavior.Restrict);
    }
}
