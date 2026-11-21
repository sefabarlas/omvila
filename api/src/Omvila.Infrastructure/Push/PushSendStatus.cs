namespace Omvila.Infrastructure.Push;

public enum PushSendStatus
{
    /// <summary>Sağlayıcı kabul etti.</summary>
    Sent,

    /// <summary>Geçici hata (ağ, kota, sağlayıcı arızası): tekrar denenebilir.</summary>
    Failed,

    /// <summary>Jeton artık geçerli değil: cihaz kaydı pasifleşir (alan-modeli.md § Device.IsActive).</summary>
    InvalidToken,

    /// <summary>Push kapalı (sağlayıcı yapılandırılmamış).</summary>
    Disabled,
}
