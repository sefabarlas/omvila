using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Features.Services;

/// <summary>Sözleşme Money (istek). Biçim uçta denetlenir; negatif tutar alan hatasıdır.</summary>
internal sealed record MoneyDto(long? AmountMinor, string? Currency)
{
    public Money? ToMoney(string field, Dictionary<string, string[]> errors)
    {
        if (AmountMinor is not { } amount || Currency is null)
        {
            errors[field] = ["required"];
            return null;
        }

        if (amount < 0)
        {
            errors[$"{field}.amountMinor"] = ["out_of_range"];
            return null;
        }

        if (!Enum.GetNames<SharedKernel.Currency>().Contains(Currency, StringComparer.Ordinal))
        {
            errors[$"{field}.currency"] = ["invalid_format"];
            return null;
        }

        return new Money(amount, Enum.Parse<SharedKernel.Currency>(Currency));
    }
}
