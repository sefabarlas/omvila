using NodaTime;
using Omvila.Modules.Scheduling.Features.Appointments;

namespace Omvila.Modules.Scheduling.Features.PublicBooking;

/// <summary>Sözleşme PublicAppointmentCreate.</summary>
internal sealed record PublicAppointmentCreate(
    IReadOnlyList<AppointmentItemRequest>? Items,
    bool? SameStaffForAll,
    Instant? StartUtc,
    string? FullName,
    string? CustomerNote,
    IReadOnlyList<PublicAppointmentCreate.PackageUse>? UsePackageItems,
    bool? PrivacyNoticeAcknowledged,
    string? PrivacyNoticeVersion,
    bool? MarketingConsent)
{
    /// <summary>PublicAppointmentCreate.usePackageItems[] (paket kullanımı Dilim 5).</summary>
    internal sealed record PackageUse(Guid? ServiceId, Guid? CustomerPackageItemId);
}
