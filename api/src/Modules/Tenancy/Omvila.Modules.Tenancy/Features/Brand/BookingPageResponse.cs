using System.Globalization;
using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Features.Brand;

/// <summary>Sözleşme BookingPageSettings.</summary>
internal sealed record BookingPageResponse(
    string Slug, Uri Url, string CoverColor, bool ShowReviews, bool ShowPrices, string? PreAppointmentNote, string Version)
{
    public static BookingPageResponse From(string slug, BookingPageSettings page, Uri baseUrl) => new(
        slug,
        new Uri(baseUrl, slug),
        page.CoverColorToken,
        page.ShowReviews,
        page.ShowPrices,
        page.PreAppointmentNote,
        page.Version.ToString(CultureInfo.InvariantCulture));
}
