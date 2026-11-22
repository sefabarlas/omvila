using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Sales.Domain;
using Omvila.Modules.Sales.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.DemoData;

/// <summary>
/// Demo verisi (<c>--role=seed</c>; acik-sorular.md T-74, T-114) için Sales yazma yolu: paket tanımı, paket satışı (tahsilat ve
/// satış primi), tamamlamanın seans kullanımı, tahsilatları ve hizmet primi, gün kapatma. Kayıtlar domain metodlarıyla ve
/// üretimdeki sınıflarla (<see cref="AppointmentPayments"/>, <see cref="PackageSessions"/>) yazılır; yalnız "şimdi" verilen ana
/// sabitlenir: geçmiş randevunun tahsilatı kendi gününe, kasaya ve prim dönemine düşer. Çağıranın transaction'ında ve markanın
/// kiracı bağlamında çalışır.
/// </summary>
public sealed class SalesDemoData
{
    private readonly SalesDbContext _db;
    private readonly ITenantQueries _tenants;
    private readonly IStaffServiceAssignments _assignments;
    private readonly IIdGenerator _ids;

    internal SalesDemoData(SalesDbContext db, ITenantQueries tenants, IStaffServiceAssignments assignments, IIdGenerator ids)
    {
        _db = db;
        _tenants = tenants;
        _assignments = assignments;
        _ids = ids;
    }

    /// <summary>Paket tanımı ekler; kimliğini döner.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="input">Tanım.</param>
    /// <param name="now">Oluşturma anı.</param>
    /// <param name="ct">İptal belirteci.</param>
    public async Task<Guid> AddPackageAsync(Guid brandId, DemoPackage input, Instant now, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(input);
        var package = Package.Create(
            _ids,
            brandId,
            new PackageDraft(
                input.Name, input.Price, [new PackageDraftItem(input.ServiceId, input.SessionCount)], input.ValidityDays, input.SaleCommissionPercent,
                IsAvailableAllBranches: true, IsActive: true),
            now);
        _db.Packages.Add(package);
        await _db.SaveChangesAsync(ct);
        return package.Id;
    }

    /// <summary>
    /// Paket satar (PackageSales gibi): müşteri paketi, pakete tahsisli tahsilat ve satış primi (pakete özel oran ?? satan
    /// personelin şubedeki oranı). Dönüş: müşteri paketinin tek kalemi (demo paketleri tek hizmetlidir).
    /// </summary>
    /// <param name="input">Satış.</param>
    /// <param name="ct">İptal belirteci.</param>
    public async Task<Guid> SellPackageAsync(DemoPackageSale input, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(input);
        var package = await _db.Packages.Include(p => p.Items).SingleAsync(p => p.Id == input.PackageId && p.BrandId == input.BrandId, ct);
        var staffPercent = await _tenants.GetBranchSellerCommissionPercentAsync(input.BrandId, input.SoldByStaffId, input.BranchId, ct)
            ?? throw new DomainException("Demo paketini satan personel şubede değil.");
        var sold = CustomerPackage.Sell(_ids, package, input.BranchId, input.BrandCustomerId, input.SoldByStaffId, input.SaleDate, userId: null, input.SoldUtc);
        if (!sold.IsSuccess)
        {
            throw new DomainException($"Demo paketi satılamadı: {sold.Error.Code}");
        }

        var customerPackage = sold.Value;
        _db.CustomerPackages.Add(customerPackage);
        _db.Payments.Add(Payment.RecordPackageSale(
            _ids, input.BrandId, input.BranchId, input.BrandCustomerId, customerPackage.Id, AppointmentPayments.ToDomain(input.Method), package.Price,
            userId: null, input.SoldUtc));
        var percent = CommissionEntry.SalePercent(package.SaleCommissionPercentOverride, staffPercent);
        if (percent > 0)
        {
            _db.CommissionEntries.Add(CommissionEntry.AccrueSale(
                _ids, input.BrandId, input.BranchId, input.SoldByStaffId, customerPackage.Id, package.Price, percent, input.SaleDate, input.SoldUtc));
        }

        await _db.SaveChangesAsync(ct);
        return customerPackage.Items.Single().Id;
    }

    /// <summary>
    /// Tamamlanan randevunun seans kullanımı (varsa) ve tahsilatları + hizmet primi (<see cref="IAppointmentPayments"/> ile aynı
    /// kurallar), <paramref name="completedUtc"/> anında.
    /// </summary>
    /// <param name="request">Tahsilat ve prim girdisi.</param>
    /// <param name="reservation">Paketten düşülen kalemin seans talebi; yoksa null.</param>
    /// <param name="completedUtc">Tamamlama anı.</param>
    /// <param name="ct">İptal belirteci.</param>
    public async Task RecordCompletionAsync(
        AppointmentCompletionRequest request, PackageSessionReservation? reservation, Instant completedUtc, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        var clock = new AtClock(completedUtc);
        if (reservation is not null && !(await new PackageSessions(_db, _ids, clock).ReserveAsync(reservation, ct)).IsSuccess)
        {
            throw new DomainException("Demo randevusunda paket seansı ayrılamadı.");
        }

        await new AppointmentPayments(_db, _tenants, _assignments, _ids, clock).RecordCompletionAsync(request, ct);
    }

    /// <summary>
    /// Günü kapatır (CashDays gibi): beklenen nakit o günün şubedeki nakit tahsilatları − nakit iadeleri (W3); sayılan
    /// beklenen + <paramref name="differenceMinor"/>. Dönüş: beklenen nakit (kuruş).
    /// </summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="branchId">Şube.</param>
    /// <param name="day">Şube yerel günü.</param>
    /// <param name="zone">Şubenin saat dilimi.</param>
    /// <param name="differenceMinor">Sayılan − beklenen (kuruş).</param>
    /// <param name="note">Fark açıklaması.</param>
    /// <param name="closedUtc">Kapanış anı.</param>
    /// <param name="ct">İptal belirteci.</param>
    public async Task<long> CloseCashDayAsync(
        Guid brandId, Guid branchId, LocalDate day, DateTimeZone zone, long differenceMinor, string? note, Instant closedUtc, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(zone);
        var start = zone.AtStartOfDay(day).ToInstant();
        var end = zone.AtStartOfDay(day.PlusDays(1)).ToInstant();
        var entries = await _db.Payments.AsNoTracking()
            .Where(p => p.BrandId == brandId && p.BranchId == branchId && p.PaidUtc >= start && p.PaidUtc < end)
            .Select(p => new CashEntry(p.Method, p.AmountMinor, p.IsDeposit, p.Status))
            .ToListAsync(ct);
        var refunded = await _db.Payments.AsNoTracking()
            .Where(p => p.BrandId == brandId && p.BranchId == branchId && p.Method == PaymentMethod.Cash
                && p.RefundedUtc != null && p.RefundedUtc >= start && p.RefundedUtc < end)
            .SumAsync(p => p.RefundedAmountMinor, ct);
        var expected = CashDayTotals.Calculate(entries, refunded).ExpectedCashMinor;
        var currency = PlatformDefaults.DefaultCurrency;
        var closed = CashDayClose.Close(
            _ids, brandId, branchId, day, closedUtc.InZone(zone).Date, new Money(expected, currency), new Money(expected + differenceMinor, currency), note,
            userId: null, closedUtc);
        if (!closed.IsSuccess)
        {
            throw new DomainException($"Demo kasa günü kapatılamadı: {closed.Error.Code}");
        }

        _db.CashDayCloses.Add(closed.Value);
        await _db.SaveChangesAsync(ct);
        return expected;
    }

    /// <summary>Verilen anda duran saat (geçmiş tahsilat, seans ve prim kayıtları için).</summary>
    private sealed class AtClock(Instant at) : IClock
    {
        public Instant GetCurrentInstant() => at;
    }
}
