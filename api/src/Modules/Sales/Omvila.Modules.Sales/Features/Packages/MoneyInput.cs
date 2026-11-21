using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Packages;

/// <summary>Sözleşme Money (istek gövdesi).</summary>
internal sealed record MoneyInput(long? AmountMinor, string? Currency)
{
    /// <summary>Zorunlu, markanın para biriminde ve sıfırdan büyük tutar; değilse alan hatası.</summary>
    public static Money? Read(MoneyInput? input, string field, Currency brandCurrency, Dictionary<string, string[]> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        if (input is not { AmountMinor: { } amount, Currency: { } currency })
        {
            errors[field] = ["required"];
            return null;
        }

        if (!string.Equals(currency, brandCurrency.ToString(), StringComparison.Ordinal))
        {
            errors[$"{field}.currency"] = ["invalid_format"];
            return null;
        }

        if (amount <= 0)
        {
            errors[field] = ["out_of_range"];
            return null;
        }

        return new Money(amount, brandCurrency);
    }
}
