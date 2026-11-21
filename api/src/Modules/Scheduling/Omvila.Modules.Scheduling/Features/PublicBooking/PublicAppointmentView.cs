using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.PublicBooking;

/// <summary>
/// Sözleşme PublicAppointment (rezervasyon başarı ekranı ve SMS bağlantısı "Randevum"). <see cref="Deposit"/> Dilim 4'e
/// kadar yazılmaz (kapora yok).
/// </summary>
internal sealed record PublicAppointmentView(
    Guid Id,
    string AccessCode,
    Uri ManageUrl,
    string Status,
    string DisplayStatus,
    PublicAppointmentView.BrandView Brand,
    PublicAppointmentView.BranchView Branch,
    Instant StartUtc,
    Instant EndUtc,
    string TimeZone,
    Instant? HoldExpiresUtc,
    IReadOnlyList<PublicAppointmentView.ItemView> Items,
    Money Total,
    Money AmountDue,
    object? Deposit,
    bool CanCancel,
    Instant? CancelDeadlineUtc,
    bool CanReschedule,
    Instant? RescheduleDeadlineUtc,
    string? PreAppointmentNote,
    PublicAppointmentView.CancellationView? Cancellation,
    string Version)
{
    /// <summary>PublicAppointment.brand.</summary>
    internal sealed record BrandView(string Name, string Slug, string? LogoUrl);

    /// <summary>PublicAppointment.branch.</summary>
    internal sealed record BranchView(Guid Id, string Name, string AddressLine, string District, string? Phone, decimal? Latitude, decimal? Longitude);

    /// <summary>PublicAppointment.items[].</summary>
    internal sealed record ItemView(string ServiceName, string? StaffName, Instant StartUtc, Instant EndUtc, Money Price, bool PaidFromPackage);

    /// <summary>PublicAppointment.cancellation.</summary>
    internal sealed record CancellationView(Instant CancelledUtc, string CancelledBy, string Reason);
}
