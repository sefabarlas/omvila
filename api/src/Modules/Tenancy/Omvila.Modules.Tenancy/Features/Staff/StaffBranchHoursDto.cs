using Omvila.Modules.Tenancy.Features.Branches;

namespace Omvila.Modules.Tenancy.Features.Staff;

internal sealed record StaffBranchHoursDto(Guid BranchId, WeeklyHoursDto Hours);
