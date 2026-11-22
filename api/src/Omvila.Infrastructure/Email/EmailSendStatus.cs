namespace Omvila.Infrastructure.Email;

/// <summary>Gönderim sonucu.</summary>
public enum EmailSendStatus
{
    /// <summary>Sağlayıcı kabul etti.</summary>
    Sent,

    /// <summary>E-posta yapılandırılmamış.</summary>
    Disabled,
}
