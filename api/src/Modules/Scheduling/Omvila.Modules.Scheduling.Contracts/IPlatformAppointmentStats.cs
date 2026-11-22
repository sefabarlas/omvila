using NodaTime;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Platform paneli randevu sayıları (Markalar "Bu ay randevu", marka detayı online oranı, aktivasyon hunisi). Platform okuma
/// servisidir (ADR-0006): yalnız admin yüzeyinin platform bağlamında çağrılır, başka bağlamda istisna atar. Yalnız sayı döner.
/// </summary>
public interface IPlatformAppointmentStats
{
    /// <summary>Başlangıcı [from, to) aralığında olan, taslak ve iptal dışındaki randevuların marka başına sayısı.</summary>
    /// <param name="from">Aralık başı.</param>
    /// <param name="to">Aralık sonu (dahil değil).</param>
    /// <param name="brandIds">Markalar; null = bütün markalar.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyDictionary<Guid, PlatformAppointmentCount>> CountAsync(
        Instant from, Instant to, IReadOnlyCollection<Guid>? brandIds, CancellationToken ct);

    /// <summary>Verilen markalardan en az bir online randevu (taslak dışında) almış olanlar.</summary>
    /// <param name="brandIds">Markalar.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlySet<Guid>> GetBrandsWithOnlineAppointmentAsync(IReadOnlyCollection<Guid> brandIds, CancellationToken ct);
}
