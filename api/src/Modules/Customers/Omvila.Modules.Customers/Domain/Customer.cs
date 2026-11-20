using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Domain;

/// <summary>
/// Telefonla tanınan gerçek kişi (alan-modeli.md § Customer). Bir markaya ait değildir; markalar bu kaydın tamamını
/// görmez, yalnız kendi <see cref="BrandCustomer"/> kartını görür. Telefon benzersizdir.
/// </summary>
internal sealed class Customer
{
    /// <summary>v1'de arayüz dili yalnız Türkçe (Identity.User ile aynı varsayılan).</summary>
    public const string DefaultLanguageCode = "tr";

    private Customer()
    {
    }

    public Guid Id { get; private set; }

    public string PhoneE164 { get; private set; } = string.Empty;

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

    /// <summary>Telefon OTP ile doğrulandı (online randevu). Doğrulama geri alınmaz.</summary>
    public void MarkVerified() => IsVerified = true;
}
