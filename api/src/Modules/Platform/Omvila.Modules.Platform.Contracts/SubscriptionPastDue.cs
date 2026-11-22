using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Contracts;

/// <summary>
/// Abonelik ödemesi alınamadı: deneme bitti veya dönem başladı, ödeme yok → PastDue (bildirimler.md owner.subscription_payment_failed).
/// Kart (T-06) olmadığından neden her zaman "havale bekleniyor"dur.
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="PastDueSince">Gecikmenin başladığı gün.</param>
/// <param name="ReadOnlyOn">Salt okunura geçeceği gün.</param>
/// <param name="PeriodStart">Ödenmeyen dönemin başı.</param>
/// <param name="PlanName">Planın ekrandaki adı.</param>
public sealed record SubscriptionPastDue(Guid BrandId, LocalDate PastDueSince, LocalDate ReadOnlyOn, LocalDate PeriodStart, string PlanName)
    : IIntegrationEvent;
