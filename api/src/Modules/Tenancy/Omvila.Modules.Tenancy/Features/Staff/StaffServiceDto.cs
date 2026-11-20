using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Features.Staff;

/// <summary>Sözleşme StaffServiceAssignment.</summary>
internal sealed record StaffServiceDto(Guid? ServiceId, int? DurationOverrideMinutes, Money? Surcharge, decimal? CommissionPercentOverride);
