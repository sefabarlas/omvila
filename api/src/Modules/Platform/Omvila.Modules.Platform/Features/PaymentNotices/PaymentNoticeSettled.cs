namespace Omvila.Modules.Platform.Features.PaymentNotices;

/// <summary>Eşleştirme sonucu.</summary>
/// <param name="PaymentNoticeId">Bildirim.</param>
/// <param name="BrandId">Marka.</param>
/// <param name="InvoiceId">Ödenen fatura (abonelikte, fiyat tanımlıysa).</param>
/// <param name="CreditsAdded">Eklenen kontör (kontörde).</param>
internal sealed record PaymentNoticeSettled(Guid PaymentNoticeId, Guid BrandId, Guid? InvoiceId, int? CreditsAdded);
