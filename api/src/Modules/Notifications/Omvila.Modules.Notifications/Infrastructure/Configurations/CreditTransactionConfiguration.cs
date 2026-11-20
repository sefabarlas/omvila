using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Notifications.Domain;

namespace Omvila.Modules.Notifications.Infrastructure.Configurations;

internal sealed class CreditTransactionConfiguration : IEntityTypeConfiguration<CreditTransaction>
{
    public void Configure(EntityTypeBuilder<CreditTransaction> builder)
    {
        builder.ToTable("credit_transactions", table =>
        {
            table.HasCheckConstraint("ck_credit_transactions_balance", "balance_after >= 0");
            table.HasCheckConstraint("ck_credit_transactions_type", "type IN ('Purchase', 'Consume', 'Refund', 'Grant', 'Expire')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.ReferenceType).HasMaxLength(40);
        // Güncel bakiye: markanın son satırı.
        builder.HasIndex(x => new { x.BrandId, x.CreatedUtc });
    }
}
