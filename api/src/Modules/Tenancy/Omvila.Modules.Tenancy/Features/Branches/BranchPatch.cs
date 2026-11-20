using Omvila.Modules.Tenancy.Domain;
using Omvila.Modules.Tenancy.Features.Brand;
using static Omvila.Modules.Tenancy.Features.ManageEndpointSupport;

namespace Omvila.Modules.Tenancy.Features.Branches;

/// <summary>Sözleşme BranchUpdate: gelmeyen alan değişmez, null temizler (ad ve adres alanları boşaltılamaz).</summary>
internal static class BranchPatch
{
    public static BranchDetails Apply(JsonBody input, Domain.Branch branch)
    {
        var errors = input.Errors;
        var name = Required(input, "name", branch.Name, 120, minLength: 2);
        var address = Required(input, "addressLine", branch.AddressLine, 300, minLength: 0);
        var district = Required(input, "district", branch.District, 80, minLength: 0);
        var city = Required(input, "city", branch.City, 80, minLength: 0);
        var postalCode = MaxLength(input.String("postalCode", branch.PostalCode), "postalCode", 10, errors);
        var latitude = input.Decimal("latitude", branch.Latitude);
        if (latitude is < -90 or > 90)
        {
            errors["latitude"] = ["out_of_range"];
        }

        var longitude = input.Decimal("longitude", branch.Longitude);
        if (longitude is < -180 or > 180)
        {
            errors["longitude"] = ["out_of_range"];
        }

        var phone = input.Has("phone") ? NormalizePhone(input.String("phone", null), "phone", errors) : branch.Phone;
        var email = input.String("email", branch.Email);
        if (input.Has("email"))
        {
            CheckEmail(email, "email", errors);
        }

        var slot = input.Int("slotIntervalMinutes", branch.SlotIntervalMinutes);
        if (!BranchEndpoints.IsValidSlotInterval(slot))
        {
            errors["slotIntervalMinutes"] = ["out_of_range"];
        }

        return new BranchDetails(
            name, address, district, city, postalCode, latitude, longitude, phone, email, slot, input.Bool("isActive", branch.IsActive));
    }

    private static string Required(JsonBody input, string field, string current, int max, int minLength)
    {
        if (!input.Has(field))
        {
            return current;
        }

        var value = input.String(field, current);
        if (value is null || value.Length < Math.Max(minLength, 1))
        {
            input.Errors[field] = [value is null ? "required" : "too_short"];
            return current;
        }

        return MaxLength(value, field, max, input.Errors) ?? current;
    }
}
