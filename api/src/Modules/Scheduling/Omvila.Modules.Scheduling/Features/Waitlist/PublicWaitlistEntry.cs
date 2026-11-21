using System.Text.Json.Serialization;
using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Waitlist;

/// <summary>Sözleşme PublicWaitlistEntry.</summary>
internal sealed record PublicWaitlistEntry(
    Guid Id,
    string AccessCode,
    string Status,
    Guid BranchId,
    string TimeZone,
    IReadOnlyList<string> ServiceNames,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? PreferredStaffName,
    bool AcceptOtherStaff,
    IReadOnlyList<LocalDate> Dates,
    IReadOnlyList<string> TimeWindows);
