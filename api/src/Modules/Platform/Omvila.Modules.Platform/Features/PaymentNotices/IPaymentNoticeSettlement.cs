using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.PaymentNotices;

/// <summary>
/// Havale bildiriminin banka hareketiyle eşleşmesinin sonuçları (dilim-06 kabul kriteri 2; alan-modeli.md § PaymentNotice
/// "Eşleştirme sonucu"). Platform paneli uçları (adminMatchPaymentNotice) banka hareketini işaretledikten sonra, aynı
/// transaction'da çağırır: bildirim Matched; abonelikte dönem faturası Paid (yoksa ve fiyat tanımlıysa kesilip Paid) ve abonelik
/// Active (salt okunurdan da); kontörde paketin kontörü <c>ICreditLedger.PurchaseAsync</c> ile (ADR-0006 K4); denetim kaydı
/// (Match) ve <c>PaymentNoticeMatched</c> olayı. Platform modülü içidir.
/// </summary>
internal interface IPaymentNoticeSettlement
{
    /// <summary>
    /// Bildirimi eşleştirir. Bildirim yoksa 404 not_found; Pending değilse veya abonelik iptal edilmişse 409 invalid_state_transition.
    /// </summary>
    /// <param name="paymentNoticeId">Bildirim.</param>
    /// <param name="bankTransactionId">Eşleşen banka hareketi (PlatformBankTransaction).</param>
    /// <param name="platformUserId">Eşleştiren platform kullanıcısı.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<Result<PaymentNoticeSettled>> MatchAsync(Guid paymentNoticeId, Guid bankTransactionId, Guid platformUserId, CancellationToken ct);
}
