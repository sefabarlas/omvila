using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Contracts;

/// <summary>
/// Havale OMVILA hesabında bulunamadı ("Bulunamadı, markaya yaz"; bildirimler.md owner.payment_notice_not_found). Salon yeniden
/// bildirebilir; abonelik durumu değişmez.
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="PaymentNoticeId">Bildirim.</param>
/// <param name="Purpose">Subscription veya Credits.</param>
/// <param name="Message">Platform ekibinin notu (yoksa null).</param>
public sealed record PaymentNoticeNotFound(Guid BrandId, Guid PaymentNoticeId, string Purpose, string? Message) : IIntegrationEvent;
