using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Sales.Domain;
using Omvila.Modules.Sales.Infrastructure;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Reports;

/// <summary>
/// "Personel ve prim" tablosu (sözleşme manageGetCommissionReport; T-120). Prim tutarları CommissionEntry'den okunur (Accrued,
/// Approved, Paid ve eksi Reversed ters kayıtlar; kayıt aralığa oluştuğu anın yerel gününe göre girer, şube = kaydın şubesi).
/// Randevu sayısı ve hizmet cirosu aralıkta başlayıp tamamlanan randevuların personelin kalemlerinden (hak edilen gelirle aynı
/// kural: kalem tutarı veya paket seans payı). Gelmeyen ve iptal edilen randevu prime ve ciroya girmez. Personel (K) yalnız kendi
/// satırını görür; personel kaydı yoksa tablo boştur.
/// </summary>
internal sealed class CommissionReportReader(SalesDbContext db, ITenantQueries tenants, IAppointmentQueries appointments)
{
    private const string Completed = "Completed";
    private static readonly StringComparer TurkishOrder = StringComparer.Create(CultureInfo.GetCultureInfo("tr-TR"), ignoreCase: true);

    /// <param name="brandId">Marka.</param>
    /// <param name="branchId">İstenen şube.</param>
    /// <param name="scope">Jetondaki şube kapsamı.</param>
    /// <param name="ownStaffId">commission.view K ise jetondaki personel (yoksa <see cref="Guid.Empty"/>); değilse null.</param>
    /// <param name="from">İlk gün.</param>
    /// <param name="to">Son gün.</param>
    /// <param name="ct">İptal belirteci.</param>
    // Kural: sözleşme CommissionReport; roller-yetkiler.md commission.view (Owner ✔, BranchManager Ş, Staff K "Primim")
    public async Task<Result<CommissionReport>> GetAsync(
        Guid brandId, Guid? branchId, BranchScope scope, Guid? ownStaffId, string? from, string? to, CancellationToken ct)
    {
        var resolved = await ReportWindow.ResolveAsync(tenants, brandId, branchId, scope, from, to, ct);
        if (!resolved.IsSuccess)
        {
            return resolved.Error;
        }

        var w = resolved.Value;
        if (ownStaffId == Guid.Empty)
        {
            return new CommissionReport(w.BranchId, w.From, w.To, w.Currency.ToString(), [], 0);
        }

        var entries = db.CommissionEntries.AsNoTracking()
            .Where(e => e.BrandId == brandId && e.CreatedUtc >= w.StartUtc && e.CreatedUtc < w.EndUtc);
        if (w.BranchIds is not null)
        {
            var ids = w.BranchIds.ToList();
            entries = entries.Where(e => ids.Contains(e.BranchId));
        }

        if (ownStaffId is { } own)
        {
            entries = entries.Where(e => e.StaffId == own);
        }

        var commissions = await entries
            .GroupBy(e => new { e.StaffId, e.Type })
            .Select(g => new { g.Key.StaffId, g.Key.Type, AmountMinor = g.Sum(e => e.AmountMinor) })
            .ToListAsync(ct);

        var completed = (await appointments.ListForReportAsync(brandId, w.BranchIds, w.StartUtc, w.EndUtc, ct))
            .Where(a => string.Equals(a.Status, Completed, StringComparison.Ordinal))
            .SelectMany(a => a.Items.Where(i => i.StaffId is not null && (ownStaffId is null || i.StaffId == ownStaffId)).Select(i => (a.AppointmentId, Item: i)))
            .ToList();
        var shares = await PackageSessionShares.LoadAsync(
            db, [.. completed.Where(x => x.Item.CustomerPackageItemId is not null).Select(x => x.Item.CustomerPackageItemId!.Value)], ct);
        var work = completed.GroupBy(x => x.Item.StaffId!.Value).ToDictionary(
            g => g.Key,
            g => (Count: g.Select(x => x.AppointmentId).Distinct().Count(),
                Revenue: g.Sum(x => ReportFigures.EarnedMinor(x.Item.NetAmountMinor, x.Item.CustomerPackageItemId, shares))));

        var staffIds = commissions.Select(c => c.StaffId).Concat(work.Keys).ToHashSet();
        var cards = await tenants.GetStaffCardsAsync(brandId, staffIds, ct);
        var rows = staffIds.Select(id =>
            {
                var service = commissions.Where(c => c.StaffId == id && c.Type == CommissionType.Service).Sum(c => c.AmountMinor);
                var sale = commissions.Where(c => c.StaffId == id && c.Type == CommissionType.Sale).Sum(c => c.AmountMinor);
                var (count, revenue) = work.GetValueOrDefault(id);
                return new CommissionReportRow(
                    id, cards.GetValueOrDefault(id)?.FullName ?? string.Empty, count, revenue, ReportFigures.Percent(service, revenue), service, sale, service + sale);
            })
            .OrderByDescending(r => r.TotalCommissionMinor).ThenBy(r => r.StaffName, TurkishOrder).ThenBy(r => r.StaffId)
            .ToList();
        return new CommissionReport(w.BranchId, w.From, w.To, w.Currency.ToString(), rows, rows.Sum(r => r.TotalCommissionMinor));
    }
}
