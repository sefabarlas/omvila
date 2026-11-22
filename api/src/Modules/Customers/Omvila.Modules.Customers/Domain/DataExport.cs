using NodaTime;

namespace Omvila.Modules.Customers.Domain;

/// <summary>
/// "Tüm verileri dışa aktar" isteği (sözleşme manageRequestDataExport, DataExportRequestResult; T-121). Queued → Processing →
/// Ready (arşiv hazır, indirme bağlantısı üretilebilir) → Sent (bağlantı e-postayla gitti); hata olursa Failed. Arşiv
/// (CSV'lerin ZIP'i) satırda saklanır ve bağlantı süresi dolunca silinir.
/// </summary>
internal sealed class DataExport
{
    public const int EmailMaxLength = 160;

    private DataExport()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid? RequestedByUserId { get; private set; }

    /// <summary>Bağlantının gideceği adres (kişisel veri; günlüğe yazılmaz).</summary>
    public string Email { get; private set; } = string.Empty;

    public DataExportStatus Status { get; private set; }

    public Instant RequestedUtc { get; private set; }

    public Instant? CompletedUtc { get; private set; }

    public string? FileName { get; private set; }

    public byte[]? Content { get; private set; }

    /// <summary>İndirme bağlantısının geçerlilik sonu (DataExportLinkValidityDays).</summary>
    public Instant? ExpiresUtc { get; private set; }

    public string? FailureReason { get; private set; }

    public bool IsOpen => Status is DataExportStatus.Queued or DataExportStatus.Processing;

    public static DataExport Request(Guid id, Guid brandId, Guid? userId, string email, Instant now) => new()
    {
        Id = id,
        BrandId = brandId,
        RequestedByUserId = userId,
        Email = email,
        Status = DataExportStatus.Queued,
        RequestedUtc = now,
    };

    public void Start() => Status = DataExportStatus.Processing;

    public void Complete(string fileName, byte[] content, Instant now, Duration validity)
    {
        Status = DataExportStatus.Ready;
        FileName = fileName;
        Content = content;
        CompletedUtc = now;
        ExpiresUtc = now + validity;
    }

    /// <summary>Bağlantı e-postayla gönderildi.</summary>
    public void MarkSent() => Status = DataExportStatus.Sent;

    public void Fail(string reason, Instant now)
    {
        Status = DataExportStatus.Failed;
        FailureReason = reason;
        CompletedUtc = now;
    }

    /// <summary>Süresi dolan arşiv silinir; kayıt (kim, ne zaman) kalır.</summary>
    public void Purge() => Content = null;

    /// <summary>Sözleşmedeki durum: arşiv hazır ama bağlantı henüz gönderilmediyse "Processing".</summary>
    public string ContractStatus => Status switch
    {
        DataExportStatus.Ready => nameof(DataExportStatus.Processing),
        _ => Status.ToString(),
    };
}
