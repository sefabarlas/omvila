using NodaTime;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// A1 girdisi. <see cref="ApprovalTimeout"/> doluysa online randevu onay ister ve <c>PendingApproval</c> başlar
/// (şimdi + ApprovalTimeoutMinutes); değeri <see cref="ApprovalPolicy.HoldFor"/> çözer. Panel, telefon ve kapıda sıra için
/// yok sayılır (T-47 (3)). <see cref="Deposit"/> doluysa (yalnız online) randevu kapora bekler: <c>PendingApproval</c>, tutma
/// şimdi + havale süresi (onay da gerekiyorsa tek süre budur; durum-makinesi.md § Oluşturma kuralı).
/// <see cref="PackageItemsBySeqNo"/>: paketten düşülen kalemler (plan sıra numarası → müşterinin paket kalemi; online "Fön
/// paketinizden düşülsün"); seans Sales'te ayrılır.
/// </summary>
internal sealed record NewAppointment(
    Guid BrandId,
    Guid BranchId,
    Guid CustomerId,
    Guid BrandCustomerId,
    AppointmentChannel Channel,
    ClientApp? ClientApp,
    string? CustomerNote,
    string? StaffNote,
    Guid? SeriesId,
    Guid? CreatedByUserId,
    Duration? ApprovalTimeout = null,
    DepositHold? Deposit = null,
    IReadOnlyDictionary<int, Guid>? PackageItemsBySeqNo = null);
