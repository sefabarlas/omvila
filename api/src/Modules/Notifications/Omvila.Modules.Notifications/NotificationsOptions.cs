namespace Omvila.Modules.Notifications;

/// <summary>appsettings "Notifications" bölümü (gönderim işinin teknik ayarları).</summary>
public sealed class NotificationsOptions
{
    public const string Section = "Notifications";

    public int DispatchIntervalSeconds { get; set; }

    public int BatchSize { get; set; }

    public int MaxAttempts { get; set; }

    public int RetryDelaySeconds { get; set; }

    internal bool IsValid() => DispatchIntervalSeconds > 0 && BatchSize > 0 && MaxAttempts > 0 && RetryDelaySeconds > 0;
}
