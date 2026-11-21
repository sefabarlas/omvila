using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Approvals;

/// <summary>Onay akışı olaylarının randevudan kurulması (Scheduling.Contracts; outbox'a aynı transaction'da yazılır).</summary>
internal static class ApprovalEvents
{
    /// <summary>A1 (online, onay gerekli) veya A13 (müşteri saat değiştirdi, yeniden onay).</summary>
    public static AppointmentRequested Requested(Appointment a, bool isReschedule)
    {
        ArgumentNullException.ThrowIfNull(a);
        var hold = a.HoldExpiresUtc ?? throw new DomainException("Onay bekleyen talebin tutma süresi yok.");
        return new AppointmentRequested(a.BrandId, a.Id, a.BranchId, a.BrandCustomerId, a.StartUtc, hold, isReschedule);
    }

    public static AppointmentApproved Approved(Appointment a, Guid? userId)
    {
        ArgumentNullException.ThrowIfNull(a);
        return new AppointmentApproved(a.BrandId, a.Id, a.BranchId, a.BrandCustomerId, a.StartUtc, userId);
    }

    public static AppointmentDeclined Declined(Appointment a, Guid? userId)
    {
        ArgumentNullException.ThrowIfNull(a);
        return new AppointmentDeclined(a.BrandId, a.Id, a.BranchId, a.BrandCustomerId, a.StartUtc, userId);
    }

    /// <summary>A5; neden randevunun iptal nedenidir (ApprovalExpired, kaporada DepositNotReceived).</summary>
    public static AppointmentApprovalExpired Expired(Appointment a)
    {
        ArgumentNullException.ThrowIfNull(a);
        var reason = a.CancellationReason ?? throw new DomainException("Süresi dolan talebin iptal nedeni yok.");
        return new AppointmentApprovalExpired(a.BrandId, a.Id, a.BranchId, a.BrandCustomerId, a.StartUtc, reason.ToString());
    }
}
