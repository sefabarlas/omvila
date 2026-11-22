using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Identity.Domain;

namespace Omvila.Modules.Identity.Infrastructure.Configurations;

internal sealed class OtpChallengeConfiguration : IEntityTypeConfiguration<OtpChallenge>
{
    public void Configure(EntityTypeBuilder<OtpChallenge> builder)
    {
        builder.ToTable("otp_challenges", table => table.HasCheckConstraint(
            "ck_otp_challenges_purpose",
            "purpose IN ('ManageLogin', 'ManageSignup', 'CustomerLogin', 'PublicBooking', 'AccountDeletion', 'AdminLogin')"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.PhoneE164).HasMaxLength(16);
        builder.Property(x => x.Purpose).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.CodeHash).HasMaxLength(64);
        // Hız sınırı, bekleme ve kilit sorguları telefona ve zamana göredir.
        builder.HasIndex(x => new { x.PhoneE164, x.CreatedUtc });
    }
}
