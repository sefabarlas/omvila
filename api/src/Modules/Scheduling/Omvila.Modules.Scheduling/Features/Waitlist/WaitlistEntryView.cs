using System.Text.Json.Serialization;
using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Waitlist;

/// <summary>Sözleşme WaitlistEntryView (BeklemeListesi satırı).</summary>
internal sealed record WaitlistEntryView(
    Guid Id,
    Guid BranchId,
    string Status,
    WaitlistCustomerRef Customer,
    IReadOnlyList<Guid> ServiceIds,
    IReadOnlyList<string> ServiceNames,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Guid? PreferredStaffId,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? PreferredStaffName,
    bool AcceptOtherStaff,
    IReadOnlyList<LocalDate> Dates,
    IReadOnlyList<string> TimeWindows,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? Note,
    Instant JoinedUtc,
    string Source,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] WaitlistOfferView? PendingOffer,
    string Version);
