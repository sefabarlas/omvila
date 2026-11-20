using NodaTime;

namespace Omvila.Infrastructure.Messaging;

/// <summary>
/// infra.outbox_events satırı. brand_id sütunu yoktur: tablo altyapının iç kaydıdır ve hiçbir uca açılmaz;
/// marka kimliği olay gövdesindedir.
/// </summary>
internal sealed class OutboxMessage
{
    public Guid Id { get; init; }

    public required string Type { get; init; }

    public required string Payload { get; init; }

    public Instant OccurredUtc { get; init; }

    /// <summary>Bir sonraki deneme zamanı (hata sonrası geri çekilme).</summary>
    public Instant AvailableUtc { get; set; }

    public int Attempts { get; set; }

    public Instant? ProcessedUtc { get; set; }

    /// <summary>Yalnız istisna türü; mesaj kişisel veri içerebileceği için yazılmaz.</summary>
    public string? LastError { get; set; }
}
