using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Domain;

/// <summary>
/// Prim kaydı (alan-modeli.md § CommissionEntry). Hizmet primi randevu <c>Completed</c> olunca kalem başına <c>Accrued</c>
/// doğar; oran yazıldığı andaki değerin kopyasıdır. İptal veya iadede kayıt silinmez, eksi tutarlı <c>Reversed</c> ters kayıt
/// açılır (paket iadesinde satış priminin iade oranındaki ters kaydı). Satış primi paket satılınca satan personele doğar.
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
            PeriodYearMonth = Period(localDate),
            Status = CommissionStatus.Accrued,
            CreatedUtc = now,
        };
    }

    /// <summary>Satış primi (manageSellPackage): taban paket fiyatı, satan personele, dönem satışın şube yerel günü.</summary>
    // Kural: alan-modeli.md § CommissionEntry ("Satış primi paket satılınca doğar"); dilim-05 kabul kriteri 6
    public static CommissionEntry AccrueSale(
        IIdGenerator ids, Guid brandId, Guid branchId, Guid staffId, Guid customerPackageId, Money basis, decimal percent, LocalDate localDate, Instant now)
    {
        ArgumentNullException.ThrowIfNull(ids);
        return new CommissionEntry
        {
            Id = ids.New(),
            BrandId = brandId,
            BranchId = branchId,
            StaffId = staffId,
            Type = CommissionType.Sale,
            CustomerPackageId = customerPackageId,
            BasisMinor = basis.AmountMinor,
            Percent = percent,
            AmountMinor = basis.Percent(percent, MidpointRounding.AwayFromZero).AmountMinor,
            CurrencyCode = basis.Currency.ToString(),
            PeriodYearMonth = Period(localDate),
            Status = CommissionStatus.Accrued,
            CreatedUtc = now,
        };
    }

    /// <summary>
    /// Ters kayıt (iade): asıl kayıt değişmez; <paramref name="reversedBasisMinor"/> kadar tabanın primi eksi tutarla
    /// <c>Reversed</c> açılır. Taban asılın tabanını aşamaz; tam iadede tutar asılın tam tersidir. Dönem iadenin yerel günüdür.
    /// </summary>
    // Kural: alan-modeli.md § CommissionEntry ("İptal veya iadede kayıt silinmez; eksi tutarlı Reversed ters kayıt açılır"); durum-makinesi.md § 7
    public CommissionEntry Reverse(IIdGenerator ids, long reversedBasisMinor, LocalDate localDate, Instant now)
    {
        ArgumentNullException.ThrowIfNull(ids);
        if (Status == CommissionStatus.Reversed)
        {
            throw new DomainException("Ters kayıt yeniden ters çevrilmez.");
        }

        if (reversedBasisMinor <= 0 || reversedBasisMinor > BasisMinor)
        {
            throw new DomainException("Ters kaydın tabanı sıfırdan büyük ve asıl tabandan küçük veya eşit olmalı.");
        }

        var amount = reversedBasisMinor == BasisMinor
            ? AmountMinor
            : new Money(reversedBasisMinor, Enum.Parse<Currency>(CurrencyCode)).Percent(Percent, MidpointRounding.AwayFromZero).AmountMinor;
        return new CommissionEntry
        {
            Id = ids.New(),
            BrandId = BrandId,
            BranchId = BranchId,
            StaffId = StaffId,
            Type = Type,
            AppointmentItemId = AppointmentItemId,
            CustomerPackageId = CustomerPackageId,
            BasisMinor = -reversedBasisMinor,
            Percent = Percent,
            AmountMinor = -amount,
            CurrencyCode = CurrencyCode,
            PeriodYearMonth = Period(localDate),
            Status = CommissionStatus.Reversed,
            ReversesEntryId = Id,
            CreatedUtc = now,
        };
    }

    /// <summary>
    /// Satış priminin oranı: pakete özel "Satış primi" (Package.SaleCommissionPercentOverride) varsa o, yoksa satan personelin
    /// "Paket satış primi" (Staff.SaleCommissionPercent); ikisi de yoksa 0.
    /// </summary>
    // Kural: alan-modeli.md § Package, § Staff (SaleCommissionPercent)
    public static decimal SalePercent(decimal? packageOverride, decimal? staffPercent) => packageOverride ?? staffPercent ?? 0m;

    /// <summary>
    /// Hizmet priminin oranı: personele özel hizmet oranı (ServiceStaff.CommissionPercentOverride) varsa o, yoksa personelin
    /// "Hizmet primi" (Staff.ServiceCommissionPercent); ikisi de yoksa 0.
    /// </summary>
    // Kural: alan-modeli.md § ServiceStaff (CommissionPercentOverride), § Staff (ServiceCommissionPercent)
    public static decimal ServicePercent(decimal? serviceStaffOverride, decimal? staffPercent) => serviceStaffOverride ?? staffPercent ?? 0m;

    private static int Period(LocalDate localDate) => (localDate.Year * 100) + localDate.Month;
}
