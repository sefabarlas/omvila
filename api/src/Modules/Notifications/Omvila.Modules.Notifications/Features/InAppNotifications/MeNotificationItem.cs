using System.Text.Json.Serialization;
using NodaTime;
using Omvila.Modules.Notifications.Domain;
using Omvila.Modules.Tenancy.Contracts;

namespace Omvila.Modules.Notifications.Features.InAppNotifications;

/// <summary>Sözleşme MeNotification.</summary>
/// <param name="Id">Bildirim.</param>
/// <param name="Event">Olay kodu (MeNotificationEvent).</param>
/// <param name="Title">Başlık (push başlığı).</param>
/// <param name="Body">Metin (push metni).</param>
/// <param name="CreatedUtc">Oluşma anı.</param>
/// <param name="ReadUtc">Okunma anı.</param>
/// <param name="Brand">Marka özeti; marka arşivlendiyse null.</param>
/// <param name="Target">Dokununca açılacak ekran.</param>
internal sealed record MeNotificationItem(
    Guid Id,
    string Event,
    string Title,
    string Body,
    Instant CreatedUtc,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Instant? ReadUtc,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] MeBrandRef? Brand,
    MeNotificationTarget Target)
{
    public static MeNotificationItem From(CustomerNotification n, CustomerBrandCard? brand) => new(
        n.Id,
        n.EventCode,
        n.Title,
        n.Body,
        n.CreatedUtc,
        n.ReadUtc,
        brand is null ? null : new MeBrandRef(brand.Id, brand.Name, brand.Slug, brand.LogoUrl),
        new MeNotificationTarget(n.TargetType, n.TargetAppointmentId, n.TargetAccessCode, brand?.Slug));
}

