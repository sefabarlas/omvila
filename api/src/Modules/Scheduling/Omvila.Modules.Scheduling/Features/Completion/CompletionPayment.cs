using Omvila.Modules.Sales.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Completion;

/// <summary>Sözleşme CompletionPayment.</summary>
/// <param name="Method">Cash, Card veya Transfer.</param>
/// <param name="Amount">Tutar (sözleşme Money, 0'dan büyük).</param>
internal sealed record CompletionPayment(string? Method, CompletionAmount? Amount)
{
    public void Validate(string path, Currency currency, Dictionary<string, string[]> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        if (Method is null)
        {
            errors[$"{path}.method"] = ["required"];
        }
        else if (!Enum.GetNames<CollectedPaymentMethod>().Contains(Method, StringComparer.Ordinal))
        {
            errors[$"{path}.method"] = ["invalid_format"];
        }

        if (Amount is not { AmountMinor: not null, Currency: not null })
        {
            errors[$"{path}.amount"] = ["required"];
        }
        else if (!string.Equals(Amount.Currency, currency.ToString(), StringComparison.Ordinal))
        {
            errors[$"{path}.amount.currency"] = ["invalid_format"];
        }
        else if (Amount.AmountMinor <= 0)
        {
            errors[$"{path}.amount"] = ["out_of_range"];
        }
    }
}
