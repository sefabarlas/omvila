using NodaTime;

namespace Omvila.Modules.Platform.DemoData;

/// <summary>
/// Demo aboneliği girdisi. Tutarlar örnek veridir, fiyat değildir: beklenen tutar (plan fiyatı) tanımlıysa kodu taşıyan hareket
/// o tutarla yazılır; değilse <paramref name="FallbackAmountMinor"/> ile ("Kontrol edin").
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="State">Hedef durum.</param>
/// <param name="DaysSinceTrialEnd">Denemenin kaç gün önce bittiği.</param>
/// <param name="SenderName">Havaleyi gönderen (bildirim ve banka hareketi).</param>
/// <param name="NotifiedByUserId">Bildirimi yapan salon kullanıcısı.</param>
/// <param name="PlatformUserId">Hareketi giren ve (Aktif'te) eşleştiren platform kullanıcısı.</param>
/// <param name="NoticeAge">Gecikti'de bekleyen bildirimin yaşı.</param>
/// <param name="FallbackAmountMinor">Beklenen tutar yokken hareket tutarı (kuruş).</param>
/// <param name="MismatchDeltaMinor">Kodu taşımayan hareketin tutar farkı (kuruş).</param>
public sealed record DemoSubscription(
    Guid BrandId,
    DemoSubscriptionState State,
    int DaysSinceTrialEnd,
    string SenderName,
    Guid? NotifiedByUserId,
    Guid PlatformUserId,
    Duration NoticeAge,
    long FallbackAmountMinor,
    long MismatchDeltaMinor);
