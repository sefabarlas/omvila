using NodaTime;

namespace Omvila.Modules.Scheduling.DemoData;

/// <summary>
/// Tek kalemli demo randevusu. <paramref name="CustomerNoShowCount"/>: kapora çözümü için müşterinin o anki gelmeme sayısı
/// (yalnız kaporalı sonuçlarda kullanılır; RiskyCustomersOnly politikasında eşiğe ulaşmalı). <paramref name="CustomerPackageItemId"/>:
/// <see cref="DemoAppointmentOutcome.Completed"/> sonucunda kalem bu müşteri paketi kaleminden düşülür.
/// </summary>
public sealed record DemoAppointment(
    Guid BrandId,
    Guid BranchId,
    Guid CustomerId,
    Guid BrandCustomerId,
    Instant StartUtc,
    Guid ServiceId,
    Guid StaffId,
    DemoAppointmentOutcome Outcome,
    int CustomerNoShowCount = 0,
    Guid? CustomerPackageItemId = null);
