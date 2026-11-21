using NodaTime;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features;

/// <summary>
/// Sözleşme DepositInstructions (Havale ekranı). V10: alıcı, banka ve IBAN randevuda kopyalanmaz, güncel aktif hesaptan
/// okunur; hesap yoksa veya kapora artık beklenmiyorsa talimat verilmez. Tam IBAN yalnız bu alanda, boşluklu döner.
/// </summary>
internal sealed record DepositInstructionsView(string AccountHolderName, string? BankName, string Iban, Money Amount, string TransferCode, Instant DueUtc)
{
    // Kural: dilim-04 kabul kriteri 3, durum-makinesi.md (Kapora bekleniyor = PendingApproval + kapora alınmamış)
    public static DepositInstructionsView? For(Appointment appointment, Currency currency, ActiveBankAccount? account)
    {
        ArgumentNullException.ThrowIfNull(appointment);
        return account is not null
            && appointment is
            {
                DisplayStatus: DisplayStatus.DepositPending,
                DepositRequiredMinor: { } amount,
                DepositTransferCode: { } code,
                HoldExpiresUtc: { } due,
            }
            ? new DepositInstructionsView(account.AccountHolderName, account.BankName, account.Iban.Grouped, new Money(amount, currency), code, due)
            : null;
    }
}
