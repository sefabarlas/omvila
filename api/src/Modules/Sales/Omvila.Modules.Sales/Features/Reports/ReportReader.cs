using NodaTime;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Sales.Domain;
using Omvila.Modules.Sales.Infrastructure;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Reports;

/// <summary>
/// "Raporlar ve prim" üst kartları ve günlük tahsilat (sözleşme manageGetReportOverview; dilim-06 kabul kriteri 6; T-120). Okuma
/// modeli Sales'tedir: tahsilat ve paket seans payı Sales'in verisi; randevular Scheduling.Contracts
/// <see cref="IAppointmentQueries.ListForReportAsync"/>, kapalı günler Tenancy.Contracts <see cref="IBranchScheduleQueries"/> ile
/// toplu okunur. Randevu aralığa başlangıcının yerel gününe göre girer; tahsilat kaydedildiği, iade işaretlendiği anın yerel gününe.
/// </summary>
internal sealed class ReportReader(
    SalesDbContext db, ITenantQueries tenants, IBranchScheduleQueries schedules, IAppointmentQueries appointments)
{
    private const string Completed = "Completed";
    private const string NoShow = "NoShow";
    private const string Online = "Online";

    // Kural: sözleşme ReportOverview; dilim-06 kabul kriteri 6 (peşin paket tahsilatta tam, hak edilende seans kullanıldıkça)
    public async Task<Result<ReportOverview>> GetAsync(
        Guid brandId, Guid? branchId, BranchScope scope, string? from, string? to, CancellationToken ct)
    {
        var resolved = await ReportWindow.ResolveAsync(tenants, brandId, branchId, scope, from, to, ct);
        if (!resolved.IsSuccess)
        {
            return resolved.Error;
        }

        var w = resolved.Value;
        var current = await appointments.ListForReportAsync(brandId, w.BranchIds, w.StartUtc, w.EndUtc, ct);
        var previous = await appointments.ListForReportAsync(brandId, w.BranchIds, w.PreviousStartUtc, w.StartUtc, ct);
        var completed = current.Where(a => Is(a, Completed)).ToList();
        var previousCompleted = previous.Where(a => Is(a, Completed)).ToList();
        var shares = await PackageSessionShares.LoadAsync(
            db,
            [.. completed.Concat(previousCompleted).SelectMany(a => a.Items).Where(i => i.CustomerPackageItemId is not null).Select(i => i.CustomerPackageItemId!.Value)],
            ct);
        var earned = Earned(completed, shares);
        var noShows = current.Where(a => Is(a, NoShow)).ToList();
        var online = current.Count(a => string.Equals(a.Channel, Online, StringComparison.Ordinal));

        var movements = await CollectionLedger.LoadAsync(db, brandId, w.BranchIds, w.StartUtc, w.EndUtc, ct);
        var byDay = movements.GroupBy(m => m.At.InZone(w.Zone).Date).ToDictionary(g => g.Key, g => g.Sum(m => m.AmountMinor));
        var closed = await ClosedDaysAsync(brandId, w, ct);

        return new ReportOverview(
            w.BranchId,
            w.Zone.Id,
            w.From,
            w.To,
            new Money(earned, w.Currency),
            completed.Count,
            ReportFigures.ChangePercent(earned, Earned(previousCompleted, shares)),
            new Money(movements.Sum(m => m.AmountMinor), w.Currency),
            new ReportNoShow(
                noShows.Count,
                ReportFigures.Percent(noShows.Count, noShows.Count + completed.Count),
                new Money(noShows.Sum(a => ReportFigures.NoShowLossMinor(a.PayableAmountMinor, a.RetainedDepositMinor)), w.Currency)),
            new ReportOnline(online, ReportFigures.Percent(online, current.Count)),
            [.. w.Dates.Select(d => new ReportDailyCollection(d, byDay.GetValueOrDefault(d), closed.Contains(d)))],
            w.Currency.ToString());
    }

    /// <summary>Tamamlanan randevuların hak edilen geliri (kalem tutarı veya paket seans payı).</summary>
    public static long Earned(IEnumerable<ReportAppointment> completed, IReadOnlyDictionary<Guid, long> shares) =>
        completed.SelectMany(a => a.Items).Sum(i => ReportFigures.EarnedMinor(i.NetAmountMinor, i.CustomerPackageItemId, shares));

    private static bool Is(ReportAppointment appointment, string status) => string.Equals(appointment.Status, status, StringComparison.Ordinal);

    /// <summary>
    /// Kapalı günler ("Pazartesi kapalı"): şube seçiliyse o şubenin; marka genelinde kapsamdaki bütün etkin şubelerin kapalı olduğu
    /// günler. Şube sayısı plan sınırıyla küçük olduğundan şube başına bir takvim okuması yapılır.
    /// </summary>
    private async Task<HashSet<LocalDate>> ClosedDaysAsync(Guid brandId, ReportWindow w, CancellationToken ct)
    {
        IReadOnlyList<Guid> branchIds = w.BranchId is { } id
            ? [id]
            : [.. ((await tenants.GetSummaryAsync(brandId, w.BranchIds, ct))?.Branches ?? []).Select(b => b.Id)];
        var closed = w.Dates.ToHashSet();
        if (branchIds.Count == 0)
        {
            return [];
        }

        foreach (var branch in branchIds)
        {
            var schedule = await schedules.GetAsync(brandId, branch, w.From, w.To, [], ct);
            if (schedule is not null)
            {
                closed.RemoveWhere(d => !IsClosed(schedule, d));
            }
        }

        return closed;
    }

    // Kural: musaitlik.md § 1 (özel gün haftalık saatin yerine geçer; kapalı özel gün veya o gün saati olmayan şube kapalıdır)
    private static bool IsClosed(BranchSchedule schedule, LocalDate date) =>
        schedule.Overrides.FirstOrDefault(o => o.Date == date) is { } day
            ? day.IsClosed || day.Intervals.Count == 0
            : !schedule.Hours.Any(h => h.Day == date.DayOfWeek);
}
