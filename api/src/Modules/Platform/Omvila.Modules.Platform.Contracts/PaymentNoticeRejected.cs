using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Contracts;

/// <summary>Havale bildirimi reddedildi (yinelenen veya geçersiz; bildirimler.md owner.payment_notice_rejected).</summary>
/// <param name="BrandId">Marka.</param>
/// <param name="PaymentNoticeId">Bildirim.</param>
/// <param name="Purpose">Subscription veya Credits.</param>
/// <param name="Message">Platform ekibinin notu.</param>
public sealed record PaymentNoticeRejected(Guid BrandId, Guid PaymentNoticeId, string Purpose, string Message) : IIntegrationEvent;
