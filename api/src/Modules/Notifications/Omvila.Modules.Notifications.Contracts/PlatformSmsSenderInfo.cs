using NodaTime;

namespace Omvila.Modules.Notifications.Contracts;

/// <summary>Gönderen adı (sözleşme SmsSender alanları).</summary>
/// <param name="Name">Son başvurulan ad; başvuru yoksa null.</param>
/// <param name="Status">None, Pending, Approved, Rejected.</param>
/// <param name="EffectiveName">SMS'lerin şu an gittiği ad.</param>
/// <param name="SubmittedUtc">Başvuru anı.</param>
/// <param name="DecidedUtc">Karar anı.</param>
/// <param name="RejectionReason">Ret nedeni.</param>
public sealed record PlatformSmsSenderInfo(
    string? Name, string Status, string EffectiveName, Instant? SubmittedUtc, Instant? DecidedUtc, string? RejectionReason);
