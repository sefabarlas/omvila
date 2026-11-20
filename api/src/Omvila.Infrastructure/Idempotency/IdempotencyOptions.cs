namespace Omvila.Infrastructure.Idempotency;

/// <summary>appsettings "Idempotency" bölümü. api-kurallari.md: anahtarlar 24 saat saklanır.</summary>
public sealed class IdempotencyOptions
{
    public const string Section = "Idempotency";

    public int RetentionHours { get; set; }

    public int PurgeIntervalMinutes { get; set; }

    internal bool IsValid() => RetentionHours > 0 && PurgeIntervalMinutes > 0;
}
