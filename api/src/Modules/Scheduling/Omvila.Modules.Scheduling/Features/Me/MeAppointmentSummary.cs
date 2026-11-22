using System.Text.Json.Serialization;
using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Me;

/// <summary>Sözleşme MeAppointmentSummary (Randevularım / Geçmiş satırı). holdExpiresUtc ve depositStatus isteğe bağlıdır.</summary>
internal sealed record MeAppointmentSummary(
    Guid Id,
    MeAppointmentView.BrandRef Brand,
    MeAppointmentView.BranchRef Branch,
    string TimeZone,
    Instant StartUtc,
    Instant EndUtc,
    string Status,
    string DisplayStatus,
    Instant? HoldExpiresUtc,
    string? DepositStatus,
    IReadOnlyList<MeAppointmentView.Item> Items,
    Money Total,
    Money AmountDue,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] MeAppointmentView.ReviewRef? Review,
    bool CanReview,
    bool CanRebook);
