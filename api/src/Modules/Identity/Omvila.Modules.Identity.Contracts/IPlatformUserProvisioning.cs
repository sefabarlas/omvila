using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Contracts;

/// <summary>
/// Platform kullanıcısı oluşturma (K2: web'den kayıt yok, sunucuda komutla). İdempotent: numara zaten platform kullanıcısıysa
/// rolü ve adı güncellenir. Numara marka üyesiyse de olur; salondaki adı değişmez (T-128).
/// Platform bağlamında, açık transaction içinde çağrılır.
/// </summary>
public interface IPlatformUserProvisioning
{
    /// <param name="phone">Cep telefonu.</param>
    /// <param name="fullName">Ad.</param>
    /// <param name="platformRole"><see cref="PlatformRoles"/> değerlerinden biri.</param>
    /// <param name="ct">İptal belirteci.</param>
    /// <returns>Kullanıcı ve yeni oluşturulup oluşturulmadığı; numaranın marka üyeliği varsa 409 phone_already_exists.</returns>
    Task<Result<ProvisionedPlatformUser>> EnsureAsync(PhoneNumber phone, string fullName, string platformRole, CancellationToken ct);
}
