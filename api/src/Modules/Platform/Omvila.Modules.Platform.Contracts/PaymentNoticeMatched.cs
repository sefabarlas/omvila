using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Contracts;

/// <summary>
/// Havale bildirimi banka hareketiyle eşleşti (bildirimler.md owner.payment_notice_matched; metin taslak, T-116). Abonelikte dönem
/// ödendi, kontörde paket bakiyeye eklendi.
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="PaymentNoticeId">Bildirim.</param>
/// <param name="Purpose">Subscription veya Credits.</param>
/// <param name="Credits">Kontörde eklenen kontör; abonelikte null.</param>
public sealed record PaymentNoticeMatched(Guid BrandId, Guid PaymentNoticeId, string Purpose, int? Credits) : IIntegrationEvent;
