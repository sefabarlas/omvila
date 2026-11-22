using NodaTime;

namespace Omvila.Modules.Notifications.Domain;

/// <summary>
/// Push cihaz kaydı (alan-modeli.md § Device; sözleşme manageRegisterDevice). Doğal anahtar push jetonudur: aynı jeton
/// tekrar gelirse kayıt yeni sahibe, platforma ve sürüme güncellenir ve etkinleşir. Aktif kayıtlar arasında jeton tektir.
/// Sağlayıcı "geçersiz jeton" derse pasifleşir. İşletme cihazı (User, Business) markanın kiracı bağlamındadır; müşteri cihazı
/// (Customer, Customer) markasızdır, müşterinin kendi bağlamında (RLS: owner_id = app.customer_id) görünür (T-103).
/// </summary>
internal sealed class Device
{
    public const string UserOwner = "User";

    public const string CustomerOwner = "Customer";

    public const string BusinessApp = "Business";

    public const string CustomerApp = "Customer";

    private Device()
    {
    }

    public Guid Id { get; private set; }

    /// <summary>İşletme cihazında kaydedildiği marka; müşteri cihazında boş.</summary>
    public Guid? BrandId { get; private set; }

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

    /// <summary>Müşteri uygulaması cihazı (sözleşme meRegisterDevice): markasız, sahibi global müşteri kaydı.</summary>
    public static Device RegisterForCustomer(Guid id, Guid customerId, DevicePlatform platform, string pushToken, string appVersion, Instant now) => new()
    {
        Id = id,
        OwnerType = CustomerOwner,
        OwnerId = customerId,
        App = CustomerApp,
        Platform = platform,
        PushToken = pushToken,
        AppVersion = appVersion,
        IsActive = true,
        LastSeenUtc = now,
        CreatedUtc = now,
    };

    /// <summary>Müşteri cihazının jetonu yeniden kaydedildi: sahip (aynı telefonda hesap değişimi), platform ve sürüm güncellenir, etkinleşir.</summary>
    public void RefreshForCustomer(Guid customerId, DevicePlatform platform, string appVersion, Instant now)
    {
        OwnerId = customerId;
        Platform = platform;
        AppVersion = appVersion;
        IsActive = true;
        LastSeenUtc = now;
    }

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

    /// <summary>Sağlayıcı jetonu geçersiz saydı, jeton başka kullanıcıya geçti veya müşteri çıkış yaptı.</summary>
    public void Deactivate() => IsActive = false;
}
