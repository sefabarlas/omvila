using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Contracts;

/// <summary>Randevuya tahsis edilmiş bir tahsilat (sözleşme PaymentView'un kaynağı).</summary>
/// <param name="Id">Tahsilat.</param>
/// <param name="BranchId">Şube.</param>
/// <param name="Method">Cash, Card, Transfer veya PackageRedemption.</param>
/// <param name="Amount">Randevuya tahsis edilen tutar.</param>
/// <param name="IsDeposit">Kapora havalesi.</param>
/// <param name="Status">Recorded, PartiallyRefunded, Refunded veya Voided.</param>
/// <param name="RefundedAmountMinor">"Salon iade etti" tutarı (yoksa 0).</param>
/// <param name="PaidUtc">Tahsilat anı.</param>
/// <param name="CollectedByUserId">Kaydeden kullanıcı.</param>
/// <param name="Reference">Havale açıklama kodu vb.</param>
public sealed record AppointmentPaymentRecord(
    Guid Id,
    Guid BranchId,
    string Method,
    Money Amount,
    bool IsDeposit,
    string Status,
    long RefundedAmountMinor,
    Instant PaidUtc,
    Guid? CollectedByUserId,
    string? Reference);
