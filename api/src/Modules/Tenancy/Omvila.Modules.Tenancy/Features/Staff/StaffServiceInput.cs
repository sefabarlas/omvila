using Omvila.Modules.Catalog.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Features.Staff;

/// <summary>Sözleşme StaffServiceAssignment (istek; para biçimi uçta denetlenir).</summary>
internal sealed record StaffServiceInput(Guid? ServiceId, int? DurationOverrideMinutes, MoneyInput? Surcharge, decimal? CommissionPercentOverride)
{
    public static IReadOnlyList<StaffServiceAssignment>? ToAssignments(IReadOnlyList<StaffServiceInput>? items, string prefix, Dictionary<string, string[]> errors)
    {
        var result = new List<StaffServiceAssignment>();
        for (var i = 0; i < (items?.Count ?? 0); i++)
        {
            var item = items![i];
            if (item.ServiceId is null)
            {
                errors[$"{prefix}[{i}].serviceId"] = ["required"];
                continue;
            }

            Money? surcharge = null;
            if (item.Surcharge is { } s)
            {
                if (s.AmountMinor is not { } amount || amount < 0 || !Enum.TryParse<Currency>(s.Currency, out var currency) || !Enum.IsDefined(currency))
                {
                    errors[$"{prefix}[{i}].surcharge"] = ["invalid_format"];
                    continue;
                }

                surcharge = new Money(amount, currency);
            }

            result.Add(new StaffServiceAssignment(item.ServiceId.Value, item.DurationOverrideMinutes, surcharge, item.CommissionPercentOverride));
        }

        return errors.Count > 0 ? null : result;
    }
}
