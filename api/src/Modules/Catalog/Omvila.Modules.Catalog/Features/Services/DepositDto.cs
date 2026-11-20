using Omvila.Modules.Catalog.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Features.Services;

/// <summary>Sözleşme DepositRule.</summary>
internal sealed record DepositDto(string? Mode, MoneyDto? Amount, decimal? Percent)
{
    public DepositRule? ToRule(Dictionary<string, string[]> errors)
    {
        if (Mode is null || !Enum.GetNames<DepositMode>().Contains(Mode, StringComparer.Ordinal))
        {
            errors["deposit.mode"] = [Mode is null ? "required" : "invalid_format"];
            return null;
        }

        Money? amount = Amount is null ? null : Amount.ToMoney("deposit.amount", errors);
        return new DepositRule(Enum.Parse<DepositMode>(Mode), amount, Percent);
    }

    public static DepositDto From(DepositRule rule) =>
        new(rule.Mode.ToString(), rule.Amount is { } a ? new MoneyDto(a.AmountMinor, a.Currency.ToString()) : null, rule.Percent);
}
