using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Infrastructure.Configurations;

internal sealed class BrandOnboardingConfiguration : IEntityTypeConfiguration<BrandOnboarding>
{
    public void Configure(EntityTypeBuilder<BrandOnboarding> builder)
    {
        builder.ToTable("brand_onboarding");
        builder.HasKey(x => x.BrandId);
        builder.Property(x => x.BrandId).ValueGeneratedNever();
        builder.HasOne<Brand>().WithOne().HasForeignKey<BrandOnboarding>(x => x.BrandId).OnDelete(DeleteBehavior.Restrict);
    }
}
