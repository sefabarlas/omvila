using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Infrastructure.Configurations;

internal sealed class BrandBankAccountConfiguration : IEntityTypeConfiguration<BrandBankAccount>
{
    public void Configure(EntityTypeBuilder<BrandBankAccount> builder)
    {
        // Markada tek hesap: BrandId birincil anahtar (sözleşme manageGetBankAccount "tekil kaynak").
        builder.ToTable("brand_bank_accounts", table =>
        {
            table.HasCheckConstraint("ck_brand_bank_accounts_transfer_window", "transfer_window_minutes IN (60, 120, 240)");
            table.HasCheckConstraint("ck_brand_bank_accounts_transfer_code_prefix", "transfer_code_prefix ~ '^[A-Z]{2,5}$'");
            table.HasCheckConstraint("ck_brand_bank_accounts_iban_last4", "iban_last4 ~ '^[0-9]{4}$'");
        });
        builder.HasKey(x => x.BrandId);
        builder.Property(x => x.BrandId).ValueGeneratedNever();
        builder.Property(x => x.AccountHolderName).HasMaxLength(BankAccountRules.AccountHolderNameMaxLength);
        builder.Property(x => x.BankName).HasMaxLength(BankAccountRules.BankNameMaxLength);
        builder.Property(x => x.IbanLast4).HasMaxLength(4);
        builder.Property(x => x.TransferCodePrefix).HasMaxLength(5);
        builder.Property(x => x.Version).IsRowVersion();
        builder.HasOne<Brand>().WithOne().HasForeignKey<BrandBankAccount>(x => x.BrandId).OnDelete(DeleteBehavior.Restrict);
    }
}
