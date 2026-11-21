using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Cash;

/// <summary>Sözleşme PaymentView (Kasa satırı).</summary>
internal sealed record CashPaymentView(
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
    CashCustomerRef? Customer,
    Guid? AppointmentId,
    Instant? AppointmentStartUtc,
    IReadOnlyList<string> ServiceNames,
    bool IsBalanceAfterDeposit,
    string? StaffName,
    Guid? CustomerPackageId,
    string? PackageName,
    int? PackageSessionCount,
    bool RecordedAfterDayClose);
