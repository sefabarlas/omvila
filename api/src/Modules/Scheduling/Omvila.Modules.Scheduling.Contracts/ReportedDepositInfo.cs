using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>Müşterinin bildirdiği, salonun henüz onaylamadığı kapora havalesi (tahsilat kaydı değildir).</summary>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="BrandId">Marka.</param>
/// <param name="BranchId">Şube.</param>
/// <param name="Amount">İstenen kapora.</param>
/// <param name="ReportedUtc">Müşterinin "Havaleyi yaptım" dediği an.</param>
/// <param name="StartUtc">Randevu başlangıcı.</param>
/// <param name="ServiceNames">Kalemlerin hizmet adları, sırayla.</param>
public sealed record ReportedDepositInfo(
    Guid AppointmentId, Guid BrandId, Guid BranchId, Money Amount, Instant ReportedUtc, Instant StartUtc, IReadOnlyList<string> ServiceNames);
