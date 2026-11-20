using NodaTime;

namespace Omvila.Modules.Customers.Domain;

/// <summary>
/// Rıza kaydı (alan-modeli.md § ConsentRecord). Yalnız eklenir: güncellenmez, geri çekme yeni satırdır.
/// BrandId null ise platform geneli rızadır (yalnız platform bağlamı okur).
/// </summary>
internal sealed class ConsentRecord
{
    public const int DocumentVersionMaxLength = 20;

    private ConsentRecord()
    {
    }

    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid? BrandId { get; private set; }

    public ConsentKind ConsentType { get; private set; }

    public bool IsGranted { get; private set; }

    public string DocumentVersion { get; private set; } = string.Empty;

    public ConsentSource Channel { get; private set; }

    public string? IpAddress { get; private set; }

    public Instant OccurredUtc { get; private set; }

    public static ConsentRecord Create(
        Guid id, Guid customerId, Guid? brandId, ConsentKind type, bool isGranted, string documentVersion, ConsentSource channel, string? ipAddress, Instant occurredUtc)
    {
        if (documentVersion.Length > DocumentVersionMaxLength)
        {
            throw new SharedKernel.DomainException("Belge sürümü 20 karakteri aşamaz.");
        }

        return new ConsentRecord
        {
            Id = id,
            CustomerId = customerId,
            BrandId = brandId,
            ConsentType = type,
            IsGranted = isGranted,
            DocumentVersion = documentVersion,
            Channel = channel,
            IpAddress = ipAddress,
            OccurredUtc = occurredUtc,
        };
    }
}
