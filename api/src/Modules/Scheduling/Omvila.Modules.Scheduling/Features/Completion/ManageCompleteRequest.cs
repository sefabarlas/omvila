using Omvila.Modules.Sales.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Completion;

/// <summary>Sözleşme ManageCompleteRequest ("Tamamla ve ödeme al" gövdesi).</summary>
/// <param name="Payments">Ödeme satırları (en fazla 3; alınacak 0 ise boş).</param>
/// <param name="PackageRedemptions">"Paketten" düşülecek kalemler (en fazla 5).</param>
internal sealed record ManageCompleteRequest(IReadOnlyList<CompletionPayment>? Payments, IReadOnlyList<PackageRedemptionRequest>? PackageRedemptions)
{
    public const int MaxPayments = 3;
    public const int MaxPackageRedemptions = 5;

    /// <summary>
    /// Biçim denetimi: payments zorunlu ve ≤ 3; her satırda yöntem Cash/Card/Transfer, tutar &gt; 0 ve randevunun para biriminde;
    /// packageRedemptions ≤ 5 ve iki kimlik dolu. Toplam ve durum randevu kuralındadır.
    /// </summary>
    public Dictionary<string, string[]> Validate(Currency currency)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (Payments is null)
        {
            errors["payments"] = ["required"];
        }
        else if (Payments.Count > MaxPayments)
        {
            errors["payments"] = ["out_of_range"];
        }
        else
        {
            for (var i = 0; i < Payments.Count; i++)
            {
                if (Payments[i] is { } payment)
                {
                    payment.Validate($"payments[{i}]", currency, errors);
                }
                else
                {
                    errors[$"payments[{i}]"] = ["required"];
                }
            }
        }

        if (PackageRedemptions is { Count: > MaxPackageRedemptions })
        {
            errors["packageRedemptions"] = ["out_of_range"];
        }
        else if (PackageRedemptions is not null)
        {
            for (var i = 0; i < PackageRedemptions.Count; i++)
            {
                if (PackageRedemptions[i] is not { AppointmentItemId: not null, CustomerPackageItemId: not null })
                {
                    errors[$"packageRedemptions[{i}]"] = ["required"];
                }
            }
        }

        return errors;
    }

    /// <summary>Doğrulanmış satırlar.</summary>
    public IReadOnlyList<CompletionPaymentLine> Lines(Currency currency) =>
        [.. (Payments ?? []).Select(p => new CompletionPaymentLine(Enum.Parse<CollectedPaymentMethod>(p.Method!), new Money(p.Amount!.AmountMinor!.Value, currency)))];
}
