using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Sales.Domain;

namespace Omvila.Modules.Sales.Infrastructure.Configurations;

internal sealed class CashDayCloseConfiguration : IEntityTypeConfiguration<CashDayClose>
{
    /// <summary>Şubenin bir günü bir kez kapanır; eşzamanlı ikinci kapanış 409 cash_day_already_closed.</summary>
    public const string BranchDayIndex = "ix_cash_day_closes_brand_id_branch_id_business_date";

    public void Configure(EntityTypeBuilder<CashDayClose> builder)
    {
        builder.ToTable("cash_day_closes", table =>
        {
            table.HasCheckConstraint(
                "ck_cash_day_closes_amounts",
                "expected_cash_minor >= 0 AND counted_cash_minor >= 0 AND difference_minor = counted_cash_minor - expected_cash_minor");
            table.HasCheckConstraint("ck_cash_day_closes_note", "difference_minor = 0 OR difference_note IS NOT NULL");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsFixedLength();
        builder.Property(x => x.DifferenceNote).HasMaxLength(CashDayClose.DifferenceNoteMaxLength);
        builder.Ignore(x => x.Difference);
        builder.HasIndex(x => new { x.BrandId, x.BranchId, x.BusinessDate }).IsUnique().HasDatabaseName(BranchDayIndex);
    }
}
