using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Contracts;

/// <summary>
/// Personelin panel üyeliği (ADR-0007 § Personel daveti). Tenancy'deki personel kaydı ile Identity'deki kullanıcı ve
/// üyelik bu arayüzle bağlanır. Markanın kiracı bağlamında, istek transaction'ı içinde çağrılır.
/// </summary>
public interface IStaffMemberships
{
    /// <summary>
    /// Telefonla kullanıcı (yoksa) ve Invited üyelik oluşturur; <see cref="StaffInvitation.SendInvite"/> ise davet SMS'i
    /// kuyruğa yazılır. Numaranın zaten bir üyeliği varsa 409 phone_already_exists (v1: kullanıcı başına tek üyelik).
    /// </summary>
    /// <param name="invitation">Davet.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<Result<MembershipInfo>> InviteAsync(StaffInvitation invitation, CancellationToken ct);

    /// <summary>Personel kayıtlarına bağlı üyelikler (personel kimliğine göre).</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="staffIds">Personel kimlikleri.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyDictionary<Guid, MembershipInfo>> GetByStaffAsync(Guid brandId, IReadOnlyCollection<Guid> staffIds, CancellationToken ct);

    /// <summary>Rol, şube kapsamı ve etkinlik değişikliği; personelin üyeliği yoksa bir şey yapmaz. Owner değiştirilmez.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="staffId">Personel.</param>
    /// <param name="change">Değişiklik (boş alan değişmez).</param>
    /// <param name="ct">İptal belirteci.</param>
    Task UpdateAsync(Guid brandId, Guid staffId, MembershipChange change, CancellationToken ct);
}
