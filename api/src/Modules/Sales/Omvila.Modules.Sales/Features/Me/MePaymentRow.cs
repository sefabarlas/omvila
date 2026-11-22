using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Me;

/// <summary>Sözleşme MePaymentRow (Ödemelerim satırı; salt okunur). Boş isteğe bağlı alanlar yazılmaz.</summary>
internal sealed record MePaymentRow(
    Guid Id,
    string Kind,
    string Status,
    MeBrandRef Brand,
    Money Amount,
    string Method,
    Instant OccurredUtc,
    string TimeZone,
    Money? RefundedAmount,
    Guid? AppointmentId,
    Instant? AppointmentStartUtc,
    IReadOnlyList<string>? ServiceNames,
    string? PackageName,
    int? PackageSessionCount);
