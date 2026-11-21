using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Bekleme listesindeki kişiye boşalan saat teklif edildi (durum-makinesi.md § 3, — → Pending; teklif motoru veya panel
/// "Saat öner"). Notifications müşteriye <c>waitlist.offer</c> yazar: push → SMS, sessiz saate kaydırılmaz
/// (bildirimler.md). Erişim kodu yalnız <c>/b/</c> bağlantısını kurmak içindir; günlüğe yazılmaz.
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="OfferId">Teklif.</param>
/// <param name="BranchId">Şube.</param>
/// <param name="CustomerId">Global kişi (müşteri uygulaması push'u).</param>
/// <param name="BrandCustomerId">Müşteri kartı (telefon).</param>
/// <param name="StartUtc">Teklif edilen saat.</param>
/// <param name="HoldMinutes">Tutma süresi (WaitlistOfferHoldMinutes).</param>
/// <param name="AccessCode">/b/ bağlantı kodu.</param>
public sealed record WaitlistOfferCreated(
    Guid BrandId,
    Guid OfferId,
    Guid BranchId,
    Guid CustomerId,
    Guid BrandCustomerId,
    Instant StartUtc,
    int HoldMinutes,
    string AccessCode) : IIntegrationEvent;
