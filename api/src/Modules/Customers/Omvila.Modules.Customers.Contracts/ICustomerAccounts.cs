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
}
