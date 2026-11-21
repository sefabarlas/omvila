using System.Globalization;
using NodaTime;
using Omvila.Modules.Tenancy.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Features.BankAccount;

/// <summary>Sözleşme BankAccount (okuma). Tam IBAN içermez.</summary>
internal sealed record BankAccountResponse(
    bool Configured,
    bool IsActive,
    string? AccountHolderName,
    string? BankName,
    string? IbanMasked,
    string? IbanLast4,
    string TransferCodePrefix,
    int TransferWindowMinutes,
    Instant? UpdatedUtc,
    string Version)
{
    /// <summary>Hesap tanımlı değil: form varsayılanlarla açılır ("Kapora alınamaz · IBAN ekle").</summary>
    public static BankAccountResponse Missing(uint version) => new(
        false, false, null, null, null, null, PlatformDefaults.TransferCodePrefix, PlatformDefaults.TransferWindowMinutes, null,
        version.ToString(CultureInfo.InvariantCulture));

    public static BankAccountResponse From(BrandBankAccount account, Iban iban) => new(
        true,
        account.IsActive,
        account.AccountHolderName,
        account.BankName,
        iban.Masked,
        account.IbanLast4,
        account.TransferCodePrefix,
        account.TransferWindowMinutes,
        account.UpdatedUtc,
        account.Version.ToString(CultureInfo.InvariantCulture));
}
