using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Domain;

/// <summary>
/// Telefonla tanınan gerçek kişi (alan-modeli.md § Customer). Bir markaya ait değildir; markalar bu kaydın tamamını
/// görmez, yalnız kendi <see cref="BrandCustomer"/> kartını görür. Telefon benzersizdir. Telefonu yalnız kapıda sıradan
/// telefonsuz başlatılan müşteride boştur (alan-modeli.md § WalkInTicket; T-86 W6): o kişi telefonla bulunamaz, yalnız
/// açıldığı markanın kartıyla görünür.
/// </summary>
internal sealed class Customer
{
    /// <summary>v1'de arayüz dili yalnız Türkçe (Identity.User ile aynı varsayılan).</summary>
    public const string DefaultLanguageCode = "tr";

    /// <summary>Anonimleştirilmiş kaydın adı (guvenlik-kvkk.md § Anonimleştirme).</summary>
    public const string AnonymizedName = "Silinmiş müşteri";

    public const string AnonymizedPhonePrefix = "+000";

    private Customer()
    {
    }

    public Guid Id { get; private set; }

    public string? PhoneE164 { get; private set; }

    public string FullName { get; private set; } = string.Empty;

    public int? BirthYear { get; private set; }

    public string LanguageCode { get; private set; } = DefaultLanguageCode;

    /// <summary>En az bir kez OTP ile doğrulandı.</summary>
    public bool IsVerified { get; private set; }

    public Instant CreatedUtc { get; private set; }

    /// <summary>KVKK silmesi yapıldıysa.</summary>
    public Instant? AnonymizedUtc { get; private set; }

    public static Customer Create(Guid id, PhoneNumber phone, string fullName, int? birthYear, bool isVerified, Instant now)
    {
        ArgumentNullException.ThrowIfNull(phone);
        return new Customer
        {
            Id = id,
            PhoneE164 = phone.E164,
            FullName = fullName.Trim(),
            BirthYear = birthYear,
            LanguageCode = DefaultLanguageCode,
            IsVerified = isVerified,
            CreatedUtc = now,
        };
    }

    /// <summary>Kapıda sıradan telefonsuz başlatılan müşteri (T-86 W6): telefonsuz, doğrulanmamış.</summary>
    public static Customer CreateWithoutPhone(Guid id, string fullName, Instant now)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        return new Customer
        {
            Id = id,
            FullName = fullName.Trim(),
            LanguageCode = DefaultLanguageCode,
            CreatedUtc = now,
        };
    }

    /// <summary>
    /// KVKK silmesinde markadaki kartın bağlandığı anonim kişi (guvenlik-kvkk.md § Anonimleştirme): ad "Silinmiş müşteri",
    /// telefon <c>+000</c> + kimlik özetinin 16 onaltılık hanesi (benzersizlik korunur, <see cref="PhoneNumber"/> olarak
    /// çözülmez, gösterilmez), <see cref="AnonymizedUtc"/> dolu. Gerçek kişinin kaydı ve başka markalardaki kartları değişmez.
    /// </summary>
    // Kural: guvenlik-kvkk.md § Anonimleştirme
    public static Customer CreateAnonymous(Guid id, Instant now)
    {
        var hash = Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(id.ToByteArray()));
        return new Customer
        {
            Id = id,
            PhoneE164 = AnonymizedPhonePrefix + hash[..16],
            FullName = AnonymizedName,
            LanguageCode = DefaultLanguageCode,
            IsVerified = false,
            CreatedUtc = now,
            AnonymizedUtc = now,
        };
    }

    /// <summary>Telefon OTP ile doğrulandı (online randevu). Doğrulama geri alınmaz.</summary>
    public void MarkVerified() => IsVerified = true;
}
