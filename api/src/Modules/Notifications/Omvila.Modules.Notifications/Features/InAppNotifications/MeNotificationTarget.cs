using System.Text.Json.Serialization;

namespace Omvila.Modules.Notifications.Features.InAppNotifications;

/// <summary>Sözleşme MeNotificationTarget.</summary>
/// <param name="Type">Appointment, WaitlistOffer, Review veya Booking.</param>
/// <param name="AppointmentId">Randevu (Appointment, Review).</param>
/// <param name="AccessCode">Bekleme teklifi kodu.</param>
/// <param name="BrandSlug">Markanın güncel bağlantı adı.</param>
internal sealed record MeNotificationTarget(
    string Type,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Guid? AppointmentId,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? AccessCode,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? BrandSlug);
