namespace Omvila.Infrastructure.Sms;

/// <summary>
/// appsettings "Sms:Providers:fake": sahte sağlayıcının teslim raporu imza gizlisi. Yalnız Development ve Testing
/// yapılandırmasında bulunur; üretimde sahte sağlayıcı seçilemez.
/// </summary>
public sealed class FakeSmsProviderOptions
{
    public const string Section = "Sms:Providers:fake";

    public string WebhookSecret { get; set; } = string.Empty;
}
