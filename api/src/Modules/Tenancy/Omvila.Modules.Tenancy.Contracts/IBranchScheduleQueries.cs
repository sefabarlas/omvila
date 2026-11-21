using NodaTime;

namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>
/// Müsaitlik motorunun Tenancy girdileri (musaitlik.md § Performans hedefi: tek sorgu turu, döngüde sorgu yok). Markanın
/// kiracı bağlamında çağrılır. Sorgu sayısı gün ve personel sayısından bağımsızdır.
/// </summary>
public interface IBranchScheduleQueries
{
    /// <summary>
    /// Şubenin saat dilimi, randevu kuralları, haftalık saatleri, <paramref name="from"/>–<paramref name="to"/> arasındaki
    /// özel günleri; şubede etkin çalışan personelin bu şubedeki saatleri ve aralığa değen bütün izinleri. Şube markada
    /// yoksa null.
    /// </summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="branchId">Şube.</param>
    /// <param name="from">İlk yerel gün.</param>
    /// <param name="to">Son yerel gün (dahil).</param>
    /// <param name="staffIds">Aday personel; null = şubenin bütün etkin personeli.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<BranchSchedule?> GetAsync(Guid brandId, Guid branchId, LocalDate from, LocalDate to, IReadOnlyCollection<Guid>? staffIds, CancellationToken ct);
}
