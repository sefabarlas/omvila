namespace Omvila.Modules.Tenancy.Features.Brand;

/// <summary>Sözleşme BookingPageSettingsUpdate. Slug biçimi ve ayrılmış ad <see cref="Domain.Slug"/>'ta denetlenir.</summary>
internal sealed record BookingPageRequest(string? Slug, string? CoverColor, bool? ShowReviews, bool? ShowPrices, string? PreAppointmentNote)
{
    private static readonly HashSet<string> CoverColors = new(["petrol", "mercan", "kum"], StringComparer.Ordinal);

    public Dictionary<string, string[]> Validate()
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (Slug is null)
        {
            errors["slug"] = ["required"];
        }

        if (CoverColor is null)
        {
            errors["coverColor"] = ["required"];
        }
        else if (!CoverColors.Contains(CoverColor))
        {
            errors["coverColor"] = ["invalid_format"];
        }

        if (ShowReviews is null)
        {
            errors["showReviews"] = ["required"];
        }

        if (ShowPrices is null)
        {
            errors["showPrices"] = ["required"];
        }

        if (PreAppointmentNote?.Trim().Length > 200)
        {
            errors["preAppointmentNote"] = ["too_long"];
        }

        return errors;
    }
}
