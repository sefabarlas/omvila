using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Identity.Domain;

namespace Omvila.Modules.Identity.Infrastructure.Configurations;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens", table => table.HasCheckConstraint("ck_refresh_tokens_client_type", "client_type IN ('Web', 'Mobile')"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.TokenHash).HasMaxLength(64);
        builder.HasIndex(x => x.TokenHash).IsUnique();
        builder.HasIndex(x => x.FamilyId);
        builder.Property(x => x.ClientType).HasConversion<string>().HasMaxLength(10);
        builder.Property(x => x.RevokedReason).HasMaxLength(30);
    }
}
