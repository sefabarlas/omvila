using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Reviews.Domain;

namespace Omvila.Modules.Reviews.Infrastructure.Configurations;

internal sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    /// <summary>Bir randevu bir kez değerlendirilir; eşzamanlı ikinci gönderim 409 already_reviewed.</summary>
    public const string AppointmentIndex = "ix_reviews_brand_id_appointment_id";

    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews", table =>
        {
            table.HasCheckConstraint("ck_reviews_rating", $"rating BETWEEN {Review.MinRating} AND {Review.MaxRating}");
            table.HasCheckConstraint("ck_reviews_reply", "(reply_text IS NULL) = (replied_utc IS NULL)");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Comment).HasMaxLength(Review.CommentMaxLength);
        builder.Property(x => x.ReplyText).HasMaxLength(Review.ReplyMaxLength);
        builder.Property(x => x.RepliedByName).HasMaxLength(Review.RepliedByNameMaxLength);
        builder.Property(x => x.ServiceNames).HasColumnType("text[]");
        builder.Property(x => x.Version).IsRowVersion();

        builder.HasIndex(x => new { x.BrandId, x.AppointmentId }).IsUnique().HasDatabaseName(AppointmentIndex);
        // Liste (şube + yeniden eskiye), müşteri kartı sekmesi.
        builder.HasIndex(x => new { x.BrandId, x.BranchId, x.CreatedUtc });
        builder.HasIndex(x => new { x.BrandId, x.BrandCustomerId, x.CreatedUtc });
    }
}
