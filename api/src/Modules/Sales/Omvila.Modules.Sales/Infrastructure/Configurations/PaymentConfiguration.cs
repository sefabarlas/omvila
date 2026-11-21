using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Sales.Domain;

namespace Omvila.Modules.Sales.Infrastructure.Configurations;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments", table =>
        {
            table.HasCheckConstraint("ck_payments_method", EnumCheck.In<PaymentMethod>("method"));
            table.HasCheckConstraint("ck_payments_status", EnumCheck.In<PaymentStatus>("status"));
            table.HasCheckConstraint("ck_payments_amounts", "amount_minor > 0 AND refunded_amount_minor >= 0 AND refunded_amount_minor <= amount_minor");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Method).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsFixedLength();
        builder.Property(x => x.Reference).HasMaxLength(Payment.ReferenceMaxLength);
        builder.Property(x => x.Version).IsRowVersion();

        builder.HasMany(x => x.Allocations).WithOne().HasForeignKey(a => a.PaymentId).OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(x => x.Allocations).UsePropertyAccessMode(PropertyAccessMode.Field);

        // Gün kapatma ve raporlar şube + gün; müşterinin ödemeleri.
        builder.HasIndex(x => new { x.BrandId, x.BranchId, x.PaidUtc });
        builder.HasIndex(x => new { x.BrandId, x.BrandCustomerId });
    }
}
