using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Reviews.Domain;

namespace Omvila.Modules.Reviews.Infrastructure.Configurations;

internal sealed class ReviewInvitationConfiguration : IEntityTypeConfiguration<ReviewInvitation>
{
    public void Configure(EntityTypeBuilder<ReviewInvitation> builder)
    {
        builder.ToTable("review_invitations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.HasIndex(x => new { x.BrandId, x.AppointmentId }).IsUnique();
        // "Bu ay davet edilenler" (şube + gönderim zamanı).
        builder.HasIndex(x => new { x.BrandId, x.BranchId, x.SendAtUtc });
    }
}
