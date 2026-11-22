using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Platform.Domain;

namespace Omvila.Modules.Platform.Infrastructure.Configurations;

internal sealed class SubscriptionInvoiceConfiguration : IEntityTypeConfiguration<SubscriptionInvoice>
{
    public void Configure(EntityTypeBuilder<SubscriptionInvoice> builder)
    {
        builder.ToTable("subscription_invoices", table =>
        {
            table.HasCheckConstraint("ck_subscription_invoices_status", "status IN ('Issued', 'Paid', 'Failed', 'Void')");
            table.HasCheckConstraint("ck_subscription_invoices_paid_via", "paid_via IS NULL OR paid_via IN ('Card', 'BankTransfer')");
            table.HasCheckConstraint("ck_subscription_invoices_amounts", "amount_minor >= 0 AND tax_minor >= 0");
            table.HasCheckConstraint("ck_subscription_invoices_period", "period_end >= period_start");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.InvoiceNumber).HasMaxLength(SubscriptionInvoice.InvoiceNumberMaxLength);
        builder.Property(x => x.Currency).HasConversion<string>().HasMaxLength(3).IsFixedLength();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(10);
        builder.Property(x => x.PaidVia).HasConversion<string>().HasMaxLength(20);
        builder.Ignore(x => x.TotalMinor);
        builder.HasIndex(x => x.InvoiceNumber).IsUnique();
        builder.HasIndex(x => new { x.BrandId, x.IssuedUtc });
        // Aynı dönem iki kez faturalanmaz (iş tekrar çalışsa da).
        builder.HasIndex(x => new { x.SubscriptionId, x.PeriodStart }).IsUnique();
        builder.HasOne<Subscription>().WithMany().HasForeignKey(x => x.SubscriptionId).OnDelete(DeleteBehavior.Restrict);
    }
}
