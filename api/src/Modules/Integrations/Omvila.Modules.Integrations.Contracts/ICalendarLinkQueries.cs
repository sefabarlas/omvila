namespace Omvila.Modules.Integrations.Contracts;

/// <summary>
/// Personelin Google Takvim bağlantı durumu (alan-modeli.md § CalendarLink; sözleşme StaffDetail.googleCalendarStatus).
/// Markanın kiracı bağlamında çağrılır. Platform paneli sayıyı <see cref="IPlatformCalendarLinks"/> ile okur (ADR-0006 K4).
/// </summary>
public interface ICalendarLinkQueries
{
    /// <summary>Personel başına bağlantı durumu (Active, TokenExpired, Revoked); bağlantısı olmayan personel sonuçta yer almaz.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="staffIds">Personel kimlikleri.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyDictionary<Guid, string>> GetStatusesAsync(Guid brandId, IReadOnlyCollection<Guid> staffIds, CancellationToken ct);
}
