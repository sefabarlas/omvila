using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Domain;

/// <summary>Havale bildirimi durumu (durum-makinesi.md § 10: Pending → Matched / NotFound / Rejected).</summary>
internal enum PaymentNoticeStatus
{
    Pending,
    Matched,
    NotFound,
    Rejected,
}
