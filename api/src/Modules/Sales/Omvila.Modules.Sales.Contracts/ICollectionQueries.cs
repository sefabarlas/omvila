using NodaTime;

namespace Omvila.Modules.Sales.Contracts;

/// <summary>
/// Net tahsilat okuması (Genel bakış "BUGÜNKÜ CİRO", gün sonu özeti). Markanın kiracı bağlamında çağrılır.
/// </summary>
public interface ICollectionQueries
{
    /// <summary>
    /// Şubede [<paramref name="fromUtc"/>, <paramref name="toUtc"/>) aralığında kaydedilen tahsilatların toplamı (paket satışı ve
    /// kapora dahil; iptal edilen ve paketten düşüm kayıtları hariç) eksi aynı aralıkta işaretlenen iadeler (kuruş).
    /// </summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="branchId">Şube.</param>
    /// <param name="fromUtc">Aralık başı.</param>
    /// <param name="toUtc">Aralık sonu (hariç).</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<long> GetNetCollectedAsync(Guid brandId, Guid branchId, Instant fromUtc, Instant toUtc, CancellationToken ct);
}
