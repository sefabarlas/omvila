using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Platform.Domain;

namespace Omvila.Modules.Platform.Infrastructure.Configurations;

internal sealed class PlatformBankTransactionConfiguration : IEntityTypeConfiguration<PlatformBankTransaction>
{
    /// <summary>Banka referansı bütün platformda benzersizdir (409 bank_transaction_duplicate).</summary>
    public const string ReferenceIndex = "ix_bank_transactions_bank_reference";

    /// <summary>Bir bildirime tek hareket bağlanır.</summary>
    public const string MatchedNoticeIndex = "ix_bank_transactions_matched_notice_id";

    public void Configure(EntityTypeBuilder<PlatformBankTransaction> builder)
    {
        builder.ToTable("bank_transactions", table =>
        {
            table.HasCheckConstraint("ck_bank_transactions_amount", "amount_minor > 0");
            table.HasCheckConstraint("ck_bank_transactions_source", "source IN ('Import', 'Manual')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.SenderName).HasMaxLength(PlatformBankTransaction.SenderNameMaxLength);
        builder.Property(x => x.Description).HasMaxLength(PlatformBankTransaction.DescriptionMaxLength);
        builder.Property(x => x.BankReference).HasMaxLength(PlatformBankTransaction.BankReferenceMaxLength);
        builder.Property(x => x.Source).HasConversion<string>().HasMaxLength(10);
        builder.HasIndex(x => x.BankReference).IsUnique().HasDatabaseName(ReferenceIndex);
        builder.HasIndex(x => x.MatchedNoticeId).IsUnique().HasDatabaseName(MatchedNoticeIndex);
        builder.HasIndex(x => new { x.ValueDate, x.Id });
    }
}
