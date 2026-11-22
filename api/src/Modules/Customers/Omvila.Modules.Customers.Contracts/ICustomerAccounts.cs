using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Contracts;

/// <summary>
/// Müşteri uygulaması hesabı (alan-modeli.md § Customer.HasAppAccount; sözleşme meVerifyOtp). Global kişi kaydı üzerinde
/// çalışır; hiçbir markada kart (BrandCustomer) açmaz. Çağıranın transaction'ında çalışır.
/// </summary>
public interface ICustomerAccounts
{
    /// <summary>Doğrulanmış telefonla uygulamaya giriş: kişi yoksa adsız açılır; doğrulanmış ve uygulama hesabı açık olur.</summary>
    /// <param name="phone">OTP ile doğrulanmış telefon.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<CustomerAppSignIn> SignInAsync(PhoneNumber phone, CancellationToken ct);

    /// <summary>Oturum yenilenebilir mi: kişi var, uygulama hesabı açık ve anonimleştirilmemiş.</summary>
    /// <param name="customerId">Global kişi.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<bool> CanSignInAsync(Guid customerId, CancellationToken ct);

    /// <summary>
    /// Uygulama hesabının telefonu ve profildeki adı (müşteri jetonuyla randevu, hesap silme kodu). Kişi yoksa, anonimleştirilmişse
    /// veya uygulama hesabı yoksa null; ad verilmemişse <see cref="CustomerAppIdentity.FullName"/> null.
    /// </summary>
    /// <param name="customerId">Global kişi (me jetonunun sub'ı).</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<CustomerAppIdentity?> GetIdentityAsync(Guid customerId, CancellationToken ct);

    /// <summary>
    /// Kişinin bütün markalardaki etkin (arşivlenmemiş, anonimleştirilmemiş) kartları. Kartlar marka RLS'inde olduğundan okuma platform
    /// bağlamında, yalnız kişinin kimliğiyle süzülerek yapılır; oturumun önceki bağlamı geri yüklenir.
    /// </summary>
    /// <param name="customerId">Global kişi.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyList<CustomerBrandCardRef>> ListBrandCardsAsync(Guid customerId, CancellationToken ct);

    /// <summary>
    /// Uygulamadan randevu alınan markayı "Salonlarım"a ekler (CustomerSavedBrand, Source Booking; alan-modeli.md). Zaten kayıtlıysa
    /// bir şey yapmaz. Markanın kiracı bağlamında çağrılır.
    /// </summary>
    /// <param name="customerId">Global kişi.</param>
    /// <param name="brandId">Marka.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task SaveBookedBrandAsync(Guid customerId, Guid brandId, CancellationToken ct);

    /// <summary>
    /// Müşterinin kendi hesabını silmesi (KK9, W7; guvenlik-kvkk.md § Anonimleştirme): bütün markalardaki kartlar anonimleşir
    /// (katılımcılar müşteri adına çalışır: yaklaşan randevular iptal, seanslar geri döner), global kayıt yerinde anonimleşir, kampanya
    /// izinleri geri çekilir, kayıtlı markalar silinir, DataDeletionRequest (CustomerApp, Completed) ve AuditLog(Anonymize) yazılır.
    /// Müşteri bağlamında çağrılır; dönüşte bağlam geri yüklenir. Kişi yoksa veya zaten silinmişse null.
    /// </summary>
    /// <param name="customerId">Global kişi.</param>
    /// <param name="ipAddress">İstemci IP'si (denetim).</param>
    /// <param name="userAgent">İstemci (denetim).</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<CustomerAccountDeletion?> DeleteAccountAsync(Guid customerId, string? ipAddress, string? userAgent, CancellationToken ct);
}
