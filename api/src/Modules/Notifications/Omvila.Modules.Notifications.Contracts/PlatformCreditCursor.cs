using NodaTime;

namespace Omvila.Modules.Notifications.Contracts;

/// <summary>Kontör hareketleri sayfa sınırı (son satırın anı ve kimliği).</summary>
/// <param name="OccurredUtc">An.</param>
/// <param name="Id">Kimlik.</param>
public sealed record PlatformCreditCursor(Instant OccurredUtc, Guid Id);
