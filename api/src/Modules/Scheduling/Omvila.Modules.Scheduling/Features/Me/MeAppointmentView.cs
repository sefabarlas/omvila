using System.Text.Json.Serialization;
using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Me;

/// <summary>
/// Sözleşme MeAppointment (RandevuDetay, Iptal, RandevuErtele, YRHavale). Zorunlu ve boş olabilen alanlar null olarak yazılır.
/// </summary>
internal sealed record MeAppointmentView(
    Guid Id,
    MeAppointmentView.BrandRef Brand,
    MeAppointmentView.BranchDetail Branch,
    string TimeZone,
    Instant StartUtc,
    Instant EndUtc,
    string Status,
    string DisplayStatus,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Instant? HoldExpiresUtc,
    IReadOnlyList<MeAppointmentView.Item> Items,
    Money Total,
    Money AmountDue,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] DepositView? Deposit,
    bool CanCancel,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Instant? CancelDeadlineUtc,
    bool CanReschedule,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Instant? RescheduleDeadlineUtc,
    bool CanReview,
    bool CanRebook,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] MeAppointmentView.ReviewRef? Review,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? PreAppointmentNote,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? CustomerNote,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] MeAppointmentView.CancellationRef? Cancellation,
    string Version)
{
    /// <summary>Sözleşme MeBrandRef.</summary>
    internal sealed record BrandRef(Guid Id, string Name, string Slug, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? LogoUrl);

    /// <summary>Sözleşme MeBranchRef.</summary>
    internal sealed record BranchRef(Guid Id, string Name, string District);

    /// <summary>MeAppointment.branch ("Ara", "Yol tarifi").</summary>
    internal sealed record BranchDetail(Guid Id, string Name, string AddressLine, string District, string? Phone, decimal? Latitude, decimal? Longitude);

    /// <summary>Sözleşme MeAppointmentItem.</summary>
    internal sealed record Item(
        Guid ServiceId,
        string ServiceName,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Guid? StaffId,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? StaffName,
        Instant StartUtc,
        Instant EndUtc,
        Money Price,
        bool PaidFromPackage);

    /// <summary>Sözleşme MeAppointmentReview.</summary>
    internal sealed record ReviewRef(int Rating, Instant CreatedUtc);

    /// <summary>MeAppointment.cancellation.</summary>
    internal sealed record CancellationRef(Instant CancelledUtc, string CancelledBy, string Reason);
}
