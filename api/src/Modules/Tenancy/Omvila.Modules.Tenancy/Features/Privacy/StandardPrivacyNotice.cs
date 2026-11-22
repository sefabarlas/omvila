using NodaTime;
using NodaTime.Text;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Features.Privacy;

/// <summary>
/// OMVILA standart aydınlatma metni (guvenlik-kvkk.md § Roller: salon veri sorumlusu, OMVILA veri işleyen). Metin salon adına
/// gösterilir; veri sorumlusunun adı ve iletişim bilgisi sunucuda eklenir. Hukuk onaylı metin gelince (H-04) yalnız burası değişir.
/// </summary>
internal static class StandardPrivacyNotice
{
    public static string Render(string brandName, string? legalName, string? email, string? phoneE164)
    {
        var controller = string.IsNullOrWhiteSpace(legalName) ? brandName : $"{brandName} ({legalName})";
        var contact = email ?? (phoneE164 is not null && PhoneNumber.TryParse(phoneE164, out var phone) ? phone.Display : null);
        var reach = contact is null ? "salonumuza doğrudan başvurabilirsiniz" : $"bize {contact} üzerinden ulaşabilirsiniz";
        return $"{controller} olarak kişisel verilerinizi (ad, telefon numarası, randevu ve ödeme kayıtları) randevu hizmeti sunmak, "
            + "randevunuzu hatırlatmak ve yasal yükümlülüklerimizi yerine getirmek amacıyla işliyoruz. Verileriniz OMVILA randevu altyapısında "
            + "saklanır; OMVILA veri işleyen olarak verilerinizi yalnız bizim talimatımızla işler. Verileriniz izniniz olmadan pazarlama amacıyla "
            + "kullanılmaz ve üçüncü kişilere satılmaz. İşlem kayıtları yasal saklama süresi boyunca tutulur, süre dolunca anonimleştirilir. "
            + $"KVKK'nın 11. maddesindeki haklarınız (bilgi alma, düzeltme, silme, itiraz) için {reach}.";
    }
}
