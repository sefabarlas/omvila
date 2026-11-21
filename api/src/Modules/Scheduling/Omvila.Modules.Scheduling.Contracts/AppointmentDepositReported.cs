using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Müşteri "Havaleyi yaptım, salona bildir" dedi (dilim-04 kabul kriteri 4; yalnız ilk bildirimde). Notifications kaporayı
/// işaretleyebilen kullanıcılara (appointment.deposit.mark, şube kapsamı) <c>staff.deposit_reported</c> push'u ve panel
/// kaydı yazar.
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="BranchId">Şube (alıcı kapsamı).</param>
/// <param name="BrandCustomerId">Müşteri kartı.</param>
/// <param name="AmountMinor">İstenen kapora (kuruş).</param>
public sealed record AppointmentDepositReported(
    Guid BrandId,
    Guid AppointmentId,
    Guid BranchId,
    Guid BrandCustomerId,
    long AmountMinor) : IIntegrationEvent;
