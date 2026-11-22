namespace Omvila.Modules.Notifications.Contracts;

/// <summary>Müşteri uygulaması cihazları (alan-modeli.md § Device, OwnerType Customer). Çağıranın transaction'ında, müşteri bağlamında çalışır.</summary>
public interface ICustomerDevices
{
    /// <summary>Çıkış (meLogout): müşterinin bu jetonlu cihaz kaydı pasifleşir; kayıt yoksa bir şey yapmaz.</summary>
    /// <param name="customerId">Global müşteri.</param>
    /// <param name="pushToken">Telefonun push jetonu.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task DeactivateAsync(Guid customerId, string pushToken, CancellationToken ct);
}
