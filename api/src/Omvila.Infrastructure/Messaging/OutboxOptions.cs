namespace Omvila.Infrastructure.Messaging;

/// <summary>Outbox dağıtım ayarları (appsettings "Outbox" bölümü). Değerler kodda değil yapılandırmada.</summary>
public sealed class OutboxOptions
{
    public const string Section = "Outbox";

    /// <summary>Bir turda alınan en fazla olay.</summary>
    public int BatchSize { get; set; }

    /// <summary>Dağıtıcının iki tur arasındaki bekleme süresi.</summary>
    public int DispatchIntervalSeconds { get; set; }

    /// <summary>İlk hatadan sonra bekleme; her denemede ikiye katlanır.</summary>
    public int RetryBaseDelaySeconds { get; set; }

    /// <summary>Geri çekilmenin üst sınırı.</summary>
    public int RetryMaxDelaySeconds { get; set; }

    internal bool IsValid() =>
        BatchSize > 0 && DispatchIntervalSeconds > 0 && RetryBaseDelaySeconds > 0 && RetryMaxDelaySeconds >= RetryBaseDelaySeconds;
}
