using NodaTime;
using Omvila.Modules.Platform.Contracts;

namespace Omvila.Modules.Platform.Domain;

/// <summary>Denetim kaydı (alan-modeli.md § AuditLog). Yalnız eklenir; uygulama rolünün güncelleme ve silme izni yoktur.</summary>
internal sealed class AuditLogEntry
{
    public const int EntityTypeMaxLength = 60;
    public const int IpAddressMaxLength = 45;
    public const int UserAgentMaxLength = 300;

    private AuditLogEntry()
    {
    }

    public long Id { get; private set; }

    public AuditActorType ActorType { get; private set; }

    public Guid? ActorId { get; private set; }

    public Guid? BrandId { get; private set; }

    public AuditAction Action { get; private set; }

    public string EntityType { get; private set; } = string.Empty;

    public Guid? EntityId { get; private set; }

    /// <summary>JSON nesnesi (jsonb); kişisel veri taşımaz.</summary>
    public string? Changes { get; private set; }

    public string? IpAddress { get; private set; }

    public string? UserAgent { get; private set; }

    public Instant OccurredUtc { get; private set; }

    public static AuditLogEntry Create(AuditEntry entry, string? changesJson, Instant now)
    {
        ArgumentNullException.ThrowIfNull(entry);
        if (string.IsNullOrWhiteSpace(entry.EntityType) || entry.EntityType.Length > EntityTypeMaxLength)
        {
            throw new SharedKernel.DomainException("Denetim kaydının varlık adı boş veya çok uzun.");
        }

        return new AuditLogEntry
        {
            ActorType = entry.ActorType,
            ActorId = entry.ActorId,
            BrandId = entry.BrandId,
            Action = entry.Action,
            EntityType = entry.EntityType,
            EntityId = entry.EntityId,
            Changes = changesJson,
            IpAddress = Cut(entry.IpAddress, IpAddressMaxLength),
            UserAgent = Cut(entry.UserAgent, UserAgentMaxLength),
            OccurredUtc = now,
        };
    }

    private static string? Cut(string? value, int max) => value is { } v && v.Length > max ? v[..max] : value;
}
