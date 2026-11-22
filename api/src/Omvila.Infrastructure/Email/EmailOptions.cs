namespace Omvila.Infrastructure.Email;

/// <summary>
/// appsettings "Email". <see cref="Provider"/> boşsa e-posta kapalıdır; "Fake" yalnız Development, Testing ve izinli Staging
/// (bellekte tutar); "Smtp" test sunucusunda ve yerelde Mailpit'e (SMTP 1025) gönderir. Sağlayıcı kararı T-05. SMTP parolası
/// ortamdan (<c>Email__Smtp__Password</c>), depoya girmez.
/// </summary>
public sealed class EmailOptions
{
    public const string Section = "Email";

    public const string FakeProvider = "Fake";

    public const string SmtpProvider = "Smtp";

    public string Provider { get; set; } = string.Empty;

    /// <summary>Gönderen adresi (bildirim@omvila.com).</summary>
    public string FromAddress { get; set; } = string.Empty;

    /// <summary>Gönderen adı (OMVILA).</summary>
    public string FromName { get; set; } = string.Empty;

    /// <summary>E-postadaki panel bağlantılarının kökü (ör. https://salon.omvila.com/).</summary>
    public string PanelBaseUrl { get; set; } = string.Empty;

    public SmtpOptions Smtp { get; set; } = new();

    internal bool IsValid() =>
        Provider switch
        {
            "" => true,
            FakeProvider => HasSender,
            SmtpProvider => HasSender && !string.IsNullOrWhiteSpace(Smtp.Host) && Smtp.Port is > 0 and < 65536,
            _ => false,
        };

    private bool HasSender =>
        !string.IsNullOrWhiteSpace(FromAddress) && Uri.TryCreate(PanelBaseUrl, UriKind.Absolute, out _);
}
