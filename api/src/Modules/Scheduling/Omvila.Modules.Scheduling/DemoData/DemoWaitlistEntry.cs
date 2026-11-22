using NodaTime;

namespace Omvila.Modules.Scheduling.DemoData;

/// <summary>
/// Demo bekleme kaydı (Waiting). <paramref name="TimeWindows"/>: <c>Morning</c>, <c>Afternoon</c>, <c>Evening</c> (sözleşme
/// WaitlistTimeWindow); boşsa her saat. <paramref name="Online"/>: rezervasyon sayfasından mı (değilse panelden).
/// </summary>
public sealed record DemoWaitlistEntry(
    Guid BrandId,
    Guid BranchId,
    Guid CustomerId,
    Guid BrandCustomerId,
    IReadOnlyList<Guid> ServiceIds,
    Guid? PreferredStaffId,
    bool AcceptOtherStaff,
    IReadOnlyList<LocalDate> Dates,
    IReadOnlyList<string> TimeWindows,
    string? Note,
    bool Online,
    Instant JoinedUtc);
