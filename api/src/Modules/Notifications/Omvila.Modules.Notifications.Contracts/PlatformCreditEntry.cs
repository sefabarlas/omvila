using NodaTime;

namespace Omvila.Modules.Notifications.Contracts;

/// <summary>Kontör hareketi (sözleşme AdminCreditTransaction alanları; not Platform'un denetim kaydından gelir).</summary>
/// <param name="Id">Hareket (toplanmış gönderim satırında günün ilk hareketi).</param>
/// <param name="OccurredUtc">An (toplanmış satırda günün son hareketi).</param>
/// <param name="Type">Purchase, Usage, TrialGift, Grant, Refund.</param>
/// <param name="Credits">Miktar (artı ekleme, eksi tüketim).</param>
/// <param name="BalanceAfter">Sonraki bakiye.</param>
public sealed record PlatformCreditEntry(Guid Id, Instant OccurredUtc, string Type, int Credits, int BalanceAfter);
