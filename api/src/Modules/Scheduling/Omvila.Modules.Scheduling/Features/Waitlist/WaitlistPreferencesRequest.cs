using System.Text.Json.Serialization;
using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Waitlist;

/// <summary>Sözleşme WaitlistPreferences (istek gövdesi; biçim <see cref="WaitlistInput"/>'ta denetlenir).</summary>
internal sealed record WaitlistPreferencesRequest(
    IReadOnlyList<Guid>? ServiceIds,
    Guid? PreferredStaffId,
    bool? AcceptOtherStaff,
    IReadOnlyList<LocalDate>? Dates,
    IReadOnlyList<string>? TimeWindows);
