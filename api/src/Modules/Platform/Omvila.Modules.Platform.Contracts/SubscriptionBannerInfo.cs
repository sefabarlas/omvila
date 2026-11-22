using NodaTime;

namespace Omvila.Modules.Platform.Contracts;

/// <summary>Sözleşme SubscriptionBanner.</summary>
/// <param name="Status">Trialing, Active, PastDue, ReadOnly, Cancelled (<see cref="SubscriptionStatusNames"/>).</param>
/// <param name="TrialEndsOn">Deneme bitişi.</param>
/// <param name="PastDueSince">Ödeme gecikmesinin başladığı gün.</param>
/// <param name="ReadOnlyOn">PastDue ise salt okunura geçeceği gün (PastDueSince + PastDueGraceDays).</param>
public sealed record SubscriptionBannerInfo(string Status, LocalDate? TrialEndsOn, LocalDate? PastDueSince, LocalDate? ReadOnlyOn)
{
    /// <summary>Salt okunur veya iptal: yeni randevu, online rezervasyon ve salonun başlattığı SMS kapalı (K5).</summary>
    public bool BlocksNewWork => SubscriptionStatusNames.BlocksNewWork(Status);
}
