using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Identity.Domain;

namespace Omvila.Modules.Identity.Infrastructure.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.PhoneE164).HasMaxLength(16);
        builder.HasIndex(x => x.PhoneE164).IsUnique();
        builder.Property(x => x.FullName).HasMaxLength(120);
        builder.Property(x => x.LanguageCode).HasMaxLength(5);
    }
}
