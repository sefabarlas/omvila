using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Contracts;

/// <summary>
/// SMS gönderen adı başvurularının platform kararı (sözleşme adminListApprovalQueue, adminApproveSmsSender, adminRejectSmsSender;
/// ADR-0006 K4: platform Notifications'ın verisine bu arayüzle yazar). Platform servisidir: yalnız admin yüzeyinin platform
/// bağlamında çağrılır. Başvuru marka başına tek satırdır; başvuru kimliği marka kimliğidir (T-124).
/// </summary>
public interface IPlatformSmsSenders
{
    /// <summary>Bekleyen başvurular, en eskiden yeniye.</summary>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyList<PendingSmsSender>> ListPendingAsync(CancellationToken ct);

    /// <summary>
    /// Başvuruyu onaylar (<paramref name="reason"/> null) veya reddeder. Başvuru yoksa 404, sürüm tutmuyorsa 412, bekleyen değilse
    /// 409 invalid_state_transition. Karar markaya bildirilir (<see cref="SmsSenderDecided"/>).
    /// </summary>
    /// <param name="senderId">Başvuru (marka kimliği).</param>
    /// <param name="approve">Onay mı.</param>
    /// <param name="reason">Ret nedeni (retde zorunlu).</param>
    /// <param name="expectedVersion">If-Match sürümü.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<Result<SmsSenderDecision>> DecideAsync(Guid senderId, bool approve, string? reason, uint expectedVersion, CancellationToken ct);
}
