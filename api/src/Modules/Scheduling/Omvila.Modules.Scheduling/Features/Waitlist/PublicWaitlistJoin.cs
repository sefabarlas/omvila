using System.Text.Json.Serialization;
using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Waitlist;

/// <summary>Sözleşme PublicWaitlistJoin ("Bekleme listesine katıl").</summary>
internal sealed record PublicWaitlistJoin(
    WaitlistPreferencesRequest? Preferences,
    string? FullName,
    bool? PrivacyNoticeAcknowledged,
    string? PrivacyNoticeVersion);
