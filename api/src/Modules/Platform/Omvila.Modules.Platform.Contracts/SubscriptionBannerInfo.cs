using NodaTime;

namespace Omvila.Modules.Platform.Contracts;

/// <summary>Sözleşme SubscriptionBanner.</summary>
/// <param name="Status">Trialing, Active, PastDue, ReadOnly, Cancelled.</param>
/// <param name="TrialEndsOn">Deneme bitişi.</param>
/// <param name="PastDueSince">Ödeme gecikmesinin başladığı gün.</param>
/// <param name="ReadOnlyOn">PastDue ise salt okunura geçeceği gün (PastDueSince + PastDueGraceDays).</param>
public sealed record SubscriptionBannerInfo(string Status, LocalDate? TrialEndsOn, LocalDate? PastDueSince, LocalDate? ReadOnlyOn);
