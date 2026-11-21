using Omvila.Modules.Scheduling.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Deposits;

/// <summary>Sözleşme DepositRefundRequest ("Salon iade etti" gövdesi; yalnız kayıt, para hareketi yok).</summary>
/// <param name="Amount">İade edilen tutar (sözleşme Money).</param>
/// <param name="Note">Salon içi not (≤ 300; yalnız panel geçmişinde).</param>
internal sealed record DepositRefundRequest(DepositRefundAmount? Amount, string? Note)
{
    /// <summary>
    /// Biçim denetimi: tutar zorunlu, para birimi randevununki, not ≤ 300. Tutar aralığı (0 &lt; tutar ≤ alınan) randevu
    /// kuralındadır (<see cref="Appointment.MarkDepositRefunded"/>).
    /// </summary>
    public Dictionary<string, string[]> Validate(Currency currency)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (Amount is not { AmountMinor: not null, Currency: not null })
        {
            errors["amount"] = ["required"];
        }
        else if (!string.Equals(Amount.Currency, currency.ToString(), StringComparison.Ordinal))
        {
            errors["amount.currency"] = ["invalid_format"];
        }

        if (Note is { Length: > Appointment.DepositRefundNoteMaxLength })
        {
            errors["note"] = ["too_long"];
        }

        return errors;
    }
}
