namespace Omvila.Modules.Platform.Contracts;

/// <summary>
/// Platform onay kuyruklarındaki bekleyenler (adminGetSession rozeti, adminGetOverview "ONAY BEKLEYEN"). Platform bağlamında
/// (admin yüzeyi) çağrılır.
/// </summary>
public interface IPendingApprovalCounts
{
    /// <param name="ct">İptal belirteci.</param>
    Task<PendingApprovalCounts> GetAsync(CancellationToken ct);
}
