using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Platform.Domain;

namespace Omvila.Modules.Platform.Infrastructure.Configurations;

internal sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public const string TransferCodeIndex = "ix_subscriptions_transfer_code";
    public const string CreditTransferCodeIndex = "ix_subscriptions_credit_transfer_code";

    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("subscriptions", table =>
        {
            table.HasCheckConstraint("ck_subscriptions_status", "status IN ('Trialing', 'Active', 'PastDue', 'ReadOnly', 'Cancelled')");
            table.HasCheckConstraint("ck_subscriptions_billing_cycle", "billing_cycle IN ('Monthly', 'Yearly')");
            table.HasCheckConstraint("ck_subscriptions_payment_method", "payment_method IS NULL OR payment_method IN ('Card', 'BankTransfer')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.BillingCycle).HasConversion<string>().HasMaxLength(10);
        builder.Property(x => x.PaymentMethod).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.TransferCode).HasMaxLength(Subscription.TransferCodeMaxLength);
        builder.Property(x => x.CreditTransferCode).HasMaxLength(Subscription.TransferCodeMaxLength);
        builder.HasIndex(x => x.BrandId).IsUnique();
        builder.HasIndex(x => x.TransferCode).IsUnique().HasDatabaseName(TransferCodeIndex);
        builder.HasIndex(x => x.CreditTransferCode).IsUnique().HasDatabaseName(CreditTransferCodeIndex);
        builder.HasIndex(x => x.Status);
        builder.HasOne<Plan>().WithMany().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Restrict);
    }
}
