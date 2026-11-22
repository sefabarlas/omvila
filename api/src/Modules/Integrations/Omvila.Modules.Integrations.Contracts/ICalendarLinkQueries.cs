namespace Omvila.Modules.Integrations.Contracts;

/// <summary>
/// Personelin Google Takvim bağlantı durumu (alan-modeli.md § CalendarLink; sözleşme StaffDetail.googleCalendarStatus,
/// AdminBrandDetail.googleCalendarConnectedStaffCount). Markanın kiracı bağlamında (veya platform bağlamında) çağrılır.
/// </summary>
public interface ICalendarLinkQueries
{
    /// <summary>Personel başına bağlantı durumu (Active, TokenExpired, Revoked); bağlantısı olmayan personel sonuçta yer almaz.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="staffIds">Personel kimlikleri.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyDictionary<Guid, string>> GetStatusesAsync(Guid brandId, IReadOnlyCollection<Guid> staffIds, CancellationToken ct);

    /// <summary>Bağlantısı etkin (Active) personel sayısı.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<int> CountActiveAsync(Guid brandId, CancellationToken ct);
}
