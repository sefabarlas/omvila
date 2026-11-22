namespace Omvila.Modules.Identity.Contracts;

/// <summary>
/// İşletme bildirimlerinin alıcıları (bildirimler.md § İşletme olayları; roller-yetkiler.md § İzin kataloğu). Markanın
/// kiracı bağlamında çağrılır.
/// </summary>
public interface IBrandUserDirectory
{
    /// <summary>
    /// Markada etkin üyeliği olan ve rolü <paramref name="permission"/> iznini taşıyan kullanıcılar. <paramref name="branchId"/>
    /// verilirse yalnız kapsamı o şubeyi içerenler (Owner ve boş kapsam bütün şubeler); marka düzeyindeki olayda null.
    /// </summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="permission">İzin kodu, ör. <see cref="Permissions.Appointment.Approve"/>.</param>
    /// <param name="branchId">Olayın şubesi.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyList<Guid>> GetUserIdsWithPermissionAsync(Guid brandId, string permission, Guid? branchId, CancellationToken ct);

    /// <summary>Personel kayıtlarına bağlı etkin kullanıcılar ("İlgili personel"); kullanıcısı olmayan personel yer almaz.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="staffIds">Personel kayıtları.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyList<Guid>> GetUserIdsForStaffAsync(Guid brandId, IReadOnlyCollection<Guid> staffIds, CancellationToken ct);
}
