namespace Omvila.Modules.Notifications.Contracts;

/// <summary>
/// Müşterinin kendi hesabını silmesinde (sözleşme meRequestAccountDeletion; KK9) Notifications'ın payı: müşterinin uygulama cihazları ve
/// uygulama içi bildirim listesi silinir (push jetonu ve bildirim metni kişisel veridir; salonların SMS teslim kayıtları kalır).
/// Çağıranın transaction'ında, müşteri bağlamında çalışır.
/// </summary>
public interface ICustomerAppDataEraser
{
    /// <summary>Müşterinin cihazlarını ve bildirimlerini siler.</summary>
    /// <param name="customerId">Global müşteri.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task EraseAsync(Guid customerId, CancellationToken ct);
}
