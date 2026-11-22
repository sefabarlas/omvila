using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Contracts;

/// <summary>Gönderen adı kararı (sözleşme AdminSmsSenderDecision).</summary>
/// <param name="SenderId">Başvuru.</param>
/// <param name="BrandId">Marka.</param>
/// <param name="Name">Başvurulan ad.</param>
/// <param name="Status">Approved veya Rejected.</param>
/// <param name="DecidedUtc">Karar anı.</param>
/// <param name="RejectionReason">Ret nedeni.</param>
public sealed record SmsSenderDecision(Guid SenderId, Guid BrandId, string Name, string Status, Instant DecidedUtc, string? RejectionReason);
