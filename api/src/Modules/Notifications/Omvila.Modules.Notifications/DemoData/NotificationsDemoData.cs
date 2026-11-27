using NodaTime;
using Omvila.Modules.Notifications.Domain;
using Omvila.Modules.Notifications.Infrastructure;

namespace Omvila.Modules.Notifications.DemoData;

/// <summary>
/// Demo verisi (<c>--role=seed</c>; acik-sorular.md T-131) için Notifications yazma yolu: bekleyen SMS gönderen adı başvurusu
/// (platform panelinde onay kuyruğuna düşer). Çağıranın transaction'ında, markanın kiracı bağlamında çalışır.
/// </summary>
public sealed class NotificationsDemoData
{
    private readonly NotificationsDbContext _db;

    internal NotificationsDemoData(NotificationsDbContext db) => _db = db;

    /// <summary>İlk başvuru (BrandSmsSender.Submit kuralları: 3–11 büyük harf veya rakam).</summary>
    public async Task SubmitSmsSenderAsync(Guid brandId, string name, Instant submittedUtc, CancellationToken ct)
    {
        _db.SmsSenders.Add(BrandSmsSender.Submit(brandId, name, submittedUtc));
        await _db.SaveChangesAsync(ct);
    }
}
