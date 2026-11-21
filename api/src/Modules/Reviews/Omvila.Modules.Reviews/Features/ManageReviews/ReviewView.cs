using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Reviews.Features.ManageReviews;

/// <summary>Sözleşme ReviewView.</summary>
internal sealed record ReviewView(
    Guid Id,
    Guid AppointmentId,
    Guid BranchId,
    int Rating,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? Comment,
    ReviewCustomerRef Customer,
    IReadOnlyList<string> ServiceNames,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Guid? StaffId,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? StaffName,
    Instant CreatedUtc,
    bool IsPublished,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] ReviewReplyView? Reply);
