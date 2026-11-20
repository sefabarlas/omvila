using Omvila.Modules.Tenancy.Domain;
using static Omvila.Modules.Tenancy.Features.ManageEndpointSupport;

namespace Omvila.Modules.Tenancy.Features.Branches;

/// <summary>Sözleşme BranchCreate.</summary>
internal sealed record BranchCreateRequest(
    string? Name,
    string? AddressLine,
    string? District,
    string? City,
    string? Phone,
    Guid? CopyWorkingHoursFromBranchId,
    Guid? CopyServicePricesFromBranchId)
{
    public BranchDetails? ToDetails(Dictionary<string, string[]> errors)
    {
        var name = Text(Name, "name", 120, errors, minLength: 2);
        var address = Text(AddressLine, "addressLine", 300, errors);
        var district = Text(District, "district", 80, errors);
        var city = Text(City, "city", 80, errors);
        var phone = NormalizePhone(string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim(), "phone", errors);
        return errors.Count > 0
            ? null
            : new BranchDetails(name!, address!, district!, city!, null, null, null, phone, null, null, IsActive: true);
    }

    private static string? Text(string? value, string field, int max, Dictionary<string, string[]> errors, int minLength = 1)
    {
        var text = value?.Trim();
        if (string.IsNullOrEmpty(text))
        {
            errors[field] = ["required"];
            return null;
        }

        if (text.Length < minLength)
        {
            errors[field] = ["too_short"];
            return null;
        }

        return MaxLength(text, field, max, errors);
    }
}
