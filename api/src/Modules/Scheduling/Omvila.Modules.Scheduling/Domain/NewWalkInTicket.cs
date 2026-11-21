using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>Bilet girdisi (biçim ve kapsam uçta denetlenir).</summary>
internal sealed record NewWalkInTicket(
    Guid BrandId,
    Guid BranchId,
    string CustomerName,
    PhoneNumber? Phone,
    Guid? BrandCustomerId,
    Guid? ServiceId,
    string? ServiceName,
    int? ServiceMinutes,
    Guid? PreferredStaffId,
    Guid? CreatedByUserId);
