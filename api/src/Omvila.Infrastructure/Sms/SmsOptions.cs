namespace Omvila.Infrastructure.Sms;

/// <summary>appsettings "Sms" bölümü. Gerçek sağlayıcı seçimi açık soru T-03; şimdilik yalnız Fake.</summary>
public sealed class SmsOptions
{
    public const string Section = "Sms";

    public const string FakeProvider = "Fake";

    public string Provider { get; set; } = string.Empty;

    /// <summary>Gönderen başlığı varsayılanı ve giriş kodlarının gönderen adı (bildirimler.md: "OMVILA").</summary>
    public string DefaultSender { get; set; } = string.Empty;
}
