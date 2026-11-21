namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Marka ve şube okuma (oturum özeti). Markanın kiracı bağlamında çağrılır.</summary>
public interface ITenantQueries
{
    /// <param name="brandId">Marka.</param>
    /// <param name="branchIds">Kapsam: null = bütün etkin şubeler (Owner), dolu = yalnız bu şubeler.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<TenantSummary?> GetSummaryAsync(Guid brandId, IReadOnlyCollection<Guid>? branchIds, CancellationToken ct);

    /// <summary>Verilen kimliklerden markanın etkin personeline ait olanlar.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="staffIds">Denetlenecek personel kimlikleri.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlySet<Guid>> GetActiveStaffIdsAsync(Guid brandId, IReadOnlyCollection<Guid> staffIds, CancellationToken ct);

    /// <summary>Şube markaya ait mi (etkin olmasa da).</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="branchId">Şube.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<bool> BranchExistsAsync(Guid brandId, Guid branchId, CancellationToken ct);

    /// <summary>Müşteri modülünün kullandığı marka ayarları; marka yoksa null.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<BrandCustomerSettings?> GetCustomerSettingsAsync(Guid brandId, CancellationToken ct);

    /// <summary>Şubenin IANA saat dilimi; şube markada yoksa null.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="branchId">Şube.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<string?> GetBranchTimeZoneAsync(Guid brandId, Guid branchId, CancellationToken ct);

    /// <summary>Randevu uçlarının marka ayarları; marka yoksa null.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<BrandAppointmentSettings?> GetAppointmentSettingsAsync(Guid brandId, CancellationToken ct);

    /// <summary>Markanın personel kartları (etkin olmasa da; geçmiş randevularda ad görünür). Bulunmayan kimlikler yer almaz.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="staffIds">Personel kimlikleri.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyDictionary<Guid, StaffCard>> GetStaffCardsAsync(Guid brandId, IReadOnlyCollection<Guid> staffIds, CancellationToken ct);

    /// <summary>Verilen personelin [<paramref name="fromUtc"/>, <paramref name="toUtc"/>) aralığına değen izinleri, başlangıca göre sıralı.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="staffIds">Personel kimlikleri.</param>
    /// <param name="fromUtc">Aralık başı.</param>
    /// <param name="toUtc">Aralık sonu (hariç).</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyList<StaffTimeOffDetail>> GetTimeOffsAsync(
        Guid brandId, IReadOnlyCollection<Guid> staffIds, NodaTime.Instant fromUtc, NodaTime.Instant toUtc, CancellationToken ct);
}
