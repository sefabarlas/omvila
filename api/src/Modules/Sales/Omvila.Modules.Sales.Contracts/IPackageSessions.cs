using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Contracts;

/// <summary>
/// Paket seansı defteri (alan-modeli.md § CustomerPackage, PackageSessionUsage; durum-makinesi.md A1, A4–A8). Seans randevu
/// oluşurken veya tamamlamada ("Paketten") ayrılır, randevu iptal olunca geri döner (tarihli), gelmedi olunca dönmez. Çağıranın
/// isteğiyle aynı bağlantı ve transaction'da çalışır; kiracı bağlamı kurulmuş olmalıdır.
/// </summary>
public interface IPackageSessions
{
    /// <summary>409 package_session_unavailable hatasının meta anahtarı (sözleşme manageCompleteAppointment).</summary>
    public const string CustomerPackageItemMeta = "customerPackageItemId";

    /// <summary>
    /// Her talep için bir seans kullanımı yazar. Paket kalemi bu müşterinin değilse, paket Active değilse, randevu günü son
    /// kullanım gününü geçiyorsa, şubede kullanılamıyorsa, hizmet uyuşmuyorsa veya kalan seans yetmiyorsa hiçbir şey yazılmaz ve
    /// 409 <c>package_session_unavailable</c> (<c>meta.customerPackageItemId</c>) döner. Eşzamanlı ayırmalar müşteri paketi
    /// satırında kilitlenir.
    /// </summary>
    /// <param name="reservation">Talep.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<Result> ReserveAsync(PackageSessionReservation reservation, CancellationToken ct);

    /// <summary>
    /// Randevuların geri dönmemiş seans kullanımlarına <c>RevertedUtc = şimdi</c> yazar (iptal: A4–A7). Kullanım yoksa bir şey
    /// yapmaz. Gelmedi'de (A8) çağrılmaz.
    /// </summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="appointmentIds">İptal edilen randevular.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task RevertAsync(Guid brandId, IReadOnlyCollection<Guid> appointmentIds, CancellationToken ct);

    /// <summary>
    /// Müşterinin bu şubede <paramref name="today"/> günü kullanabileceği, kalan seansı olan paket kalemleri (rezervasyon
    /// sayfası "Fön paketinizden düşülsün"; sözleşme publicListMyPackages), son kullanım günü yakın olan önce.
    /// </summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="branchId">Şube.</param>
    /// <param name="brandCustomerId">Müşteri kartı.</param>
    /// <param name="today">Şubenin yerel günü.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyList<UsablePackageItem>> ListUsableAsync(Guid brandId, Guid branchId, Guid brandCustomerId, LocalDate today, CancellationToken ct);
}
