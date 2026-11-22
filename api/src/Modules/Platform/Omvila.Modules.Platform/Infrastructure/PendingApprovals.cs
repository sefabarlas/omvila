using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Platform.Domain;

namespace Omvila.Modules.Platform.Infrastructure;

/// <summary>
/// <see cref="IPendingApprovalCounts"/>: bekleyen havale bildirimleri Platform'un kendi tablosundan (T-118), SMS gönderen adı
/// başvuruları Notifications'tan. Platform bağlamında çağrılır (bütün markalar).
/// </summary>
internal sealed class PendingApprovals(PlatformDbContext db, IPlatformCredits credits) : IPendingApprovalCounts
{
    public async Task<PendingApprovalCounts> GetAsync(CancellationToken ct) =>
        new(
            PaymentNotices: await db.PaymentNotices.CountAsync(n => n.Status == PaymentNoticeStatus.Pending, ct),
            SmsSenders: await credits.CountPendingSmsSendersAsync(ct));
}
