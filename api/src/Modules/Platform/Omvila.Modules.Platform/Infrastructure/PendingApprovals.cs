using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Platform.Contracts;

namespace Omvila.Modules.Platform.Infrastructure;

/// <summary>
/// <see cref="IPendingApprovalCounts"/>: SMS gönderen adı başvuruları Notifications'tan. Havale bildirimi (PaymentNotice) tablosu
/// abonelik ve faturalama PR'ında gelir; o zamana kadar bekleyen bildirim 0 sayılır (acik-sorular.md T-119).
/// </summary>
internal sealed class PendingApprovals(IPlatformCredits credits) : IPendingApprovalCounts
{
    public async Task<PendingApprovalCounts> GetAsync(CancellationToken ct) =>
        new(PaymentNotices: 0, SmsSenders: await credits.CountPendingSmsSendersAsync(ct));
}
