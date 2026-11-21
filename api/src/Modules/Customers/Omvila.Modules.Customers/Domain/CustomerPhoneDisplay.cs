using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Domain;

/// <summary>
/// Telefon gösterimi (roller-yetkiler.md: customer.phone.view ve ** dipnotu). Tam numara, kullanıcının
/// customer.phone.view izni varsa veya marka MaskCustomerPhoneFromStaff ayarını kapattıysa döner.
/// </summary>
internal static class CustomerPhoneDisplay
{
    public static bool CanSeeFullPhone(bool hasPhoneViewPermission, bool maskCustomerPhoneFromStaff) =>
        hasPhoneViewPermission || !maskCustomerPhoneFromStaff;

    /// <summary>Anonimleştirilmiş numara (PhoneNumber olarak çözülmez) ve telefonsuz müşteri (kapıda sıra) numarasızdır.</summary>
    public static CustomerPhoneView For(string? phoneE164, bool canSeeFullPhone) =>
        PhoneNumber.TryParse(phoneE164, out var phone)
            ? new CustomerPhoneView(canSeeFullPhone ? phone.E164 : null, phone.Masked)
            : new CustomerPhoneView(null, null);
}
