using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Platform.Domain;

namespace Omvila.Modules.Platform.Infrastructure.Configurations;

internal sealed class PaymentNoticeConfiguration : IEntityTypeConfiguration<PaymentNotice>
{
    /// <summary>Aynı amaç (kontörde aynı paket) için tek bekleyen bildirim.</summary>
    public const string PendingIndex = "ix_payment_notices_pending";

    public void Configure(EntityTypeBuilder<PaymentNotice> builder)
    {
        builder.ToTable("payment_notices", table =>
        {
            table.HasCheckConstraint("ck_payment_notices_purpose", "purpose IN ('Subscription', 'Credits')");
            table.HasCheckConstraint("ck_payment_notices_status", "status IN ('Pending', 'Matched', 'NotFound', 'Rejected')");
            table.HasCheckConstraint("ck_payment_notices_channel", "channel IN ('Panel', 'App')");
            table.HasCheckConstraint(
                "ck_payment_notices_credit_package", "(purpose = 'Credits') = (credit_package_id IS NOT NULL AND credits IS NOT NULL)");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Purpose).HasConversion<string>().HasMaxLength(15);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(10);
        builder.Property(x => x.Channel).HasConversion<string>().HasMaxLength(10);
        builder.Property(x => x.TransferCode).HasMaxLength(Subscription.TransferCodeMaxLength);
        builder.Property(x => x.SenderName).HasMaxLength(PaymentNotice.SenderNameMaxLength);
        builder.Property(x => x.ReceiptFileKey).HasMaxLength(PaymentNotice.ReceiptKeyMaxLength);
        builder.Property(x => x.ReceiptContentType).HasMaxLength(PaymentNotice.ReceiptContentTypeMaxLength);
        builder.HasIndex(x => new { x.BrandId, x.NotifiedUtc });
        builder.HasIndex(x => new { x.Status, x.NotifiedUtc });
        builder.HasIndex(x => new { x.BrandId, x.Purpose, x.CreditPackageId })
            .IsUnique()
            .AreNullsDistinct(false)
            .HasFilter("status = 'Pending'")
            .HasDatabaseName(PendingIndex);
        builder.HasOne<CreditPackage>().WithMany().HasForeignKey(x => x.CreditPackageId).OnDelete(DeleteBehavior.Restrict);
    }
}
