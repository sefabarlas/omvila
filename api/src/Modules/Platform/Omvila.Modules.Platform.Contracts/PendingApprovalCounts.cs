namespace Omvila.Modules.Platform.Contracts;

/// <summary>Bekleyen onaylar.</summary>
/// <param name="PaymentNotices">Bekleyen havale bildirimi.</param>
/// <param name="SmsSenders">Bekleyen SMS gönderen adı.</param>
public sealed record PendingApprovalCounts(int PaymentNotices, int SmsSenders);
