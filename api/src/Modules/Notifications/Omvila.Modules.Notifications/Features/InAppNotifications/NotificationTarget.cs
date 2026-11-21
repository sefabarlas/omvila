using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Notifications.Features.InAppNotifications;

/// <summary>Sözleşme NotificationTarget: dokununca açılacak ekran; ekran düzeyinde id null, kalan süre yalnız onay talebinde.</summary>
internal sealed record NotificationTarget(
    string Type,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Guid? Id,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Instant? HoldExpiresUtc);
