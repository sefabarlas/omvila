using NodaTime;

namespace Omvila.Modules.Scheduling.DemoData;

/// <summary>
/// Demo kapıda sıra bileti (Waiting): bugünün şube yerel gününde, numara <c>BranchQueueNumbers</c> ile. Müşteri kartı verilirse
/// bilete bağlanır (telefonu kartta); verilmezse telefonsuz, adla.
/// </summary>
public sealed record DemoWalkInTicket(
    Guid BrandId,
    Guid BranchId,
    string CustomerName,
    Guid? BrandCustomerId,
    Guid ServiceId,
    string ServiceName,
    int ServiceMinutes,
    Guid? PreferredStaffId,
    Instant CreatedUtc);
