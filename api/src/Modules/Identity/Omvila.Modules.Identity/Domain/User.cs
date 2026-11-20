using NodaTime;

namespace Omvila.Modules.Identity.Domain;

/// <summary>
/// Salon çalışanı veya OMVILA ekibi (alan-modeli.md § User). Giriş yalnız telefon + SMS koduyla; şifre yok.
/// Küreseldir (brand_id yok); markadaki rolü <see cref="UserMembership"/>'tedir.
/// </summary>
internal sealed class User
{
    private User()
    {
    }

    public Guid Id { get; private set; }

    public string PhoneE164 { get; private set; } = string.Empty;

    /// <summary>Kayıtta sorulmaz (S-21); personel kaydından veya profilden gelir.</summary>
    public string? FullName { get; private set; }

    public string LanguageCode { get; private set; } = "tr";

    public bool IsActive { get; private set; } = true;

    public Instant CreatedUtc { get; private set; }

    public Instant? LastLoginUtc { get; private set; }

    public static User Create(Guid id, string phoneE164, string? fullName, Instant now) => new()
    {
        Id = id,
        PhoneE164 = phoneE164,
        FullName = string.IsNullOrWhiteSpace(fullName) ? null : fullName.Trim(),
        CreatedUtc = now,
    };

    public void RecordLogin(Instant now) => LastLoginUtc = now;
}
