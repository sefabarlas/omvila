using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Domain;

/// <summary>
/// Prim kaydı (alan-modeli.md § CommissionEntry). Hizmet primi randevu <c>Completed</c> olunca kalem başına <c>Accrued</c>
/// doğar; oran yazıldığı andaki değerin kopyasıdır. İptal veya iadede kayıt silinmez, eksi tutarlı <c>Reversed</c> ters kayıt
/// açılır (ters kayıt yazan akış paket iadesiyle gelir).
/// </summary>
internal sealed class CommissionEntry
{
    private CommissionEntry()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid BranchId { get; private set; }

    public Guid StaffId { get; private set; }

    public CommissionType Type { get; private set; }

    public Guid? AppointmentItemId { get; private set; }

    public Guid? CustomerPackageId { get; private set; }

    public long BasisMinor { get; private set; }

    /// <summary>Oranın kopyası (0–100).</summary>
    public decimal Percent { get; private set; }

    public long AmountMinor { get; private set; }

    public string CurrencyCode { get; private set; } = string.Empty;

    /// <summary>Şubenin yerel takvimine göre yıl ve ay (202611).</summary>
    public int PeriodYearMonth { get; private set; }

    public CommissionStatus Status { get; private set; }

    public Guid? ReversesEntryId { get; private set; }

    public Instant CreatedUtc { get; private set; }

    public uint Version { get; private set; }

    /// <summary>Hizmet primi: taban × oran, kuruşa yarım yukarı yuvarlanır. Dönem, tamamlamanın şube yerel günüdür.</summary>
    // Kural: durum-makinesi.md A11 ("prim doğar"), § 7 (Accrued)
    public static CommissionEntry AccrueService(
        IIdGenerator ids, Guid brandId, Guid branchId, Guid staffId, Guid appointmentItemId, Money basis, decimal percent, LocalDate localDate, Instant now)
    {
        ArgumentNullException.ThrowIfNull(ids);
        return new CommissionEntry
        {
            Id = ids.New(),
            BrandId = brandId,
            BranchId = branchId,
            StaffId = staffId,
            Type = CommissionType.Service,
            AppointmentItemId = appointmentItemId,
            BasisMinor = basis.AmountMinor,
            Percent = percent,
            AmountMinor = basis.Percent(percent, MidpointRounding.AwayFromZero).AmountMinor,
            CurrencyCode = basis.Currency.ToString(),
            PeriodYearMonth = (localDate.Year * 100) + localDate.Month,
            Status = CommissionStatus.Accrued,
            CreatedUtc = now,
        };
    }

    /// <summary>
    /// Hizmet priminin oranı: personele özel hizmet oranı (ServiceStaff.CommissionPercentOverride) varsa o, yoksa personelin
    /// "Hizmet primi" (Staff.ServiceCommissionPercent); ikisi de yoksa 0.
    /// </summary>
    // Kural: alan-modeli.md § ServiceStaff (CommissionPercentOverride), § Staff (ServiceCommissionPercent)
    public static decimal ServicePercent(decimal? serviceStaffOverride, decimal? staffPercent) => serviceStaffOverride ?? staffPercent ?? 0m;
}
