using NodaTime;

namespace Omvila.Modules.Notifications.Domain;

/// <summary>
/// Push cihaz kaydı (alan-modeli.md § Device; sözleşme manageRegisterDevice). Doğal anahtar push jetonudur: aynı jeton
/// tekrar gelirse kayıt yeni sahibe, platforma ve sürüme güncellenir ve etkinleşir. Aktif kayıtlar arasında jeton tektir.
/// Sağlayıcı "geçersiz jeton" derse pasifleşir. D3'te yalnız işletme uygulaması (User, Business); müşteri cihazı D5'te.
/// </summary>
internal sealed class Device
{
    public const string UserOwner = "User";

    public const string CustomerOwner = "Customer";

    public const string BusinessApp = "Business";

    private Device()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public string OwnerType { get; private set; } = UserOwner;

    public Guid OwnerId { get; private set; }

    public string App { get; private set; } = BusinessApp;

    public DevicePlatform Platform { get; private set; }

    public string PushToken { get; private set; } = string.Empty;

    public string AppVersion { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public Instant LastSeenUtc { get; private set; }

    public Instant CreatedUtc { get; private set; }

    /// <summary>İşletme uygulaması kullanıcısının cihazı (markanın kiracı bağlamında).</summary>
    public static Device RegisterForUser(Guid id, Guid brandId, Guid userId, DevicePlatform platform, string pushToken, string appVersion, Instant now) => new()
    {
        Id = id,
        BrandId = brandId,
        OwnerId = userId,
        Platform = platform,
        PushToken = pushToken,
        AppVersion = appVersion,
        IsActive = true,
        LastSeenUtc = now,
        CreatedUtc = now,
    };

    /// <summary>Aynı jeton yeniden kaydedildi: sahip (aynı telefonda hesap değişimi), platform ve sürüm güncellenir, etkinleşir.</summary>
    public void Refresh(Guid userId, DevicePlatform platform, string appVersion, Instant now)
    {
        OwnerType = UserOwner;
        OwnerId = userId;
        Platform = platform;
        AppVersion = appVersion;
        IsActive = true;
        LastSeenUtc = now;
    }

    /// <summary>Sağlayıcı jetonu geçersiz saydı veya jeton başka kullanıcıya geçti.</summary>
    public void Deactivate() => IsActive = false;
}
