using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>
/// Sözleşme PaymentView (randevu ayrıntısında). Kaydeden kullanıcının adı modüller arası okunamadığı için
/// <c>collectedByName</c> yazılmaz (geçmişteki actorName gibi; T-88). Kasa ve müşteri kartı alanları (customer,
/// recordedAfterDayClose, paket) o uçlarla gelir.
/// </summary>
internal sealed record PaymentView(
    Guid Id,
    Guid BranchId,
    string Method,
    Money Amount,
    bool IsDeposit,
    string Status,
    Money? RefundedAmount,
    Instant PaidUtc,
    string? CollectedByName,
    string? Reference,
    string Purpose,
    Guid? AppointmentId,
    Instant? AppointmentStartUtc,
    IReadOnlyList<string> ServiceNames,
    bool IsBalanceAfterDeposit,
    string? StaffName);
