using System.Text.Json.Serialization;

namespace Omvila.Modules.Scheduling.Features.Me;

/// <summary>Sözleşme MeAppointmentPage.</summary>
internal sealed record MeAppointmentPage(
    IReadOnlyList<MeAppointmentSummary> Items,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? NextCursor);
