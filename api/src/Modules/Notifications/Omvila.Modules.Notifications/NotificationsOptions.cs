namespace Omvila.Modules.Notifications;

/// <summary>appsettings "Notifications" bölümü (gönderim işinin teknik ayarları).</summary>
public sealed class NotificationsOptions
{
    public const string Section = "Notifications";

    public int DispatchIntervalSeconds { get; set; }

    public int BatchSize { get; set; }

    /// <summary>Toplam gönderim denemesi; bu sayıdaki başarısızlıkta mesaj Failed olur (dilim-03 kriter 10: 5).</summary>
    public int MaxAttempts { get; set; }

    /// <summary>İlk tekrar denemesinin gecikmesi; her başarısızlıkta iki katına çıkar.</summary>
    public int RetryBaseDelaySeconds { get; set; }

    /// <summary>Geri çekilme gecikmesinin üst sınırı.</summary>
    public int RetryMaxDelaySeconds { get; set; }

    internal bool IsValid() =>
        DispatchIntervalSeconds > 0 && BatchSize > 0 && MaxAttempts > 0
        && RetryBaseDelaySeconds > 0 && RetryMaxDelaySeconds >= RetryBaseDelaySeconds;
}
