using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Customers.Features.Me;

/// <summary>Sözleşme MeBrand ("Salonlarım" satırı).</summary>
/// <param name="Id">Marka.</param>
/// <param name="Name">Marka adı.</param>
/// <param name="Slug">Güncel bağlantı adı.</param>
/// <param name="LogoUrl">Logo.</param>
/// <param name="CoverColor">Rozet zemini.</param>
/// <param name="BookingEnabled">Online randevu açık mı.</param>
/// <param name="HasBooked">Bu markada müşteri kaydı var.</param>
/// <param name="SavedUtc">QR veya bağlantıyla eklendiği an.</param>
/// <param name="LastVisitOn">Son ziyaret (şubenin yerel günü).</param>
/// <param name="Branches">Şubeler.</param>
/// <param name="ActivePackages">"Fön paketiniz var" rozeti.</param>
internal sealed record MeBrand(
    Guid Id,
    string Name,
    string Slug,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? LogoUrl,
    string CoverColor,
    bool BookingEnabled,
    bool HasBooked,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Instant? SavedUtc,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] LocalDate? LastVisitOn,
    IReadOnlyList<MeBranchRef> Branches,
    IReadOnlyList<MeBrandPackageBadge> ActivePackages);
