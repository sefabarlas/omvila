using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Contracts;

/// <summary>Bekleyen gönderen adı başvurusu.</summary>
/// <param name="SenderId">Başvuru (marka kimliği).</param>
/// <param name="BrandId">Marka.</param>
/// <param name="RequestedName">Başvurulan ad.</param>
/// <param name="CurrentEffectiveName">SMS'lerin şu an gittiği ad.</param>
/// <param name="SubmittedUtc">Başvuru anı.</param>
/// <param name="Version">Satır sürümü (If-Match).</param>
public sealed record PendingSmsSender(Guid SenderId, Guid BrandId, string RequestedName, string CurrentEffectiveName, Instant SubmittedUtc, uint Version);
