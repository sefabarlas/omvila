namespace Omvila.Modules.Sales.Contracts;

/// <summary>
/// Kişisel prim okuması (işletme uygulaması "Bugün" prim kartı; sözleşme TodayCommission). Markanın kiracı bağlamında çağrılır.
/// </summary>
public interface ICommissionQueries
{
    /// <summary>
    /// Personelin bir aydaki (PeriodYearMonth, şubenin yerel takvimi) prim kayıtlarının günlük toplamları; gün kaydın oluştuğu anın
    /// <paramref name="timeZoneId"/> dilimindeki yerel günüdür. Ters kayıtlar eksi tutarıyla dahildir; kaydı olmayan gün listede yoktur.
    /// </summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="staffId">Personel.</param>
    /// <param name="year">Yıl.</param>
    /// <param name="month">Ay (1–12).</param>
    /// <param name="timeZoneId">Günlerin saat dilimi.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyList<CommissionDayTotal>> GetDailyTotalsAsync(Guid brandId, Guid staffId, int year, int month, string timeZoneId, CancellationToken ct);
}
