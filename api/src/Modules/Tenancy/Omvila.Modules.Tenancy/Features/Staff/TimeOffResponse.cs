using NodaTime;
using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Features.Staff;

/// <summary>Sözleşme TimeOff.</summary>
internal sealed record TimeOffResponse(Guid Id, Guid StaffId, string Type, Instant StartUtc, Instant EndUtc, bool IsAllDay, string? Note)
{
    public static TimeOffResponse From(TimeOff t) => new(t.Id, t.StaffId, t.Type.ToString(), t.StartUtc, t.EndUtc, t.IsAllDay, t.Note);
}
