using Omvila.Modules.Scheduling.Domain;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>
/// A1 girdisi. Müşteri (CustomerId, BrandCustomerId) çağıran uçta <c>ICustomerDirectory.EnsureBrandCustomerAsync</c> ile
/// çözülür; marka kimliği jetondan (manage) veya slug'dan (public) gelir, istekten okunmaz. <see cref="Approval"/> yalnız
/// online oluşturmada verilir: plan onay istiyorsa randevu <c>PendingApproval</c> başlar (panel ve telefon her zaman onaylı).
/// <see cref="Deposit"/> yalnız online oluşturmada ve markanın aktif kapora hesabı varken verilir; kalem kaporaları
/// yazma yolunda plandan çözülür (Catalog <c>IServiceDeposits</c>). <see cref="PackageUses"/> (yalnız online) planın aynı
/// hizmetli kalemlerini paketten düşer: o kalemlere kapora istenmez; seansı çağıran Sales'te ayırır.
/// </summary>
internal sealed record CreateAppointmentCommand(
    Guid BrandId,
    SlotRequest Slot,
    Guid CustomerId,
    Guid BrandCustomerId,
    AppointmentChannel Channel,
    ClientApp? ClientApp,
    string? CustomerNote,
    string? StaffNote,
    Guid? SeriesId,
    Guid? CreatedByUserId,
    ApprovalPolicy? Approval = null,
    DepositTerms? Deposit = null,
    IReadOnlyList<PackageUse>? PackageUses = null);
