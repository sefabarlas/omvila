using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Approvals;

/// <summary>
/// Onay kuyruğu (sözleşme manageListPendingApprovals; GenelBakis "Onay bekliyor" bentosu, "Dikkat isteyenler"). Yalnız
/// tutma süresi henüz geçmemiş PendingApproval randevular (süresi geçmiş olan yanıtlanamaz, iş bir dakika içinde iptal
/// eder), <c>holdExpiresUtc</c> artan sıralı, sayfasız (T-62 (2)). Şube verilmezse kullanıcının kapsamındaki bütün şubeler;
/// kapsam dışı şube 404. Müşteri adları ve personel kartları toplu okunur.
/// </summary>
internal sealed class ApprovalQueueReader(SchedulingDbContext db, ITenantQueries tenants, ICustomerDirectory customers, IClock clock)
{
    private const string ServiceSeparator = " + ";

    // Kural: dilim-03 kabul kriterleri 1, 5; roller-yetkiler.md appointment.approve (Ş)
    public async Task<Result<PendingApprovalList>> GetAsync(Guid brandId, Guid? branchId, ManageAccess access, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(access);
        if (branchId is { } requested && (!access.Branches.Includes(requested) || !await tenants.BranchExistsAsync(brandId, requested, ct)))
        {
            return Error.NotFound();
        }

        var now = clock.GetCurrentInstant();
        var scopeIds = access.Branches.BranchIds.ToList();
        var query = db.Appointments.AsNoTracking().Include(a => a.Items)
            .Where(a => a.Status == AppointmentStatus.PendingApproval && a.HoldExpiresUtc != null && a.HoldExpiresUtc > now);
        if (branchId is { } only)
        {
            query = query.Where(a => a.BranchId == only);
        }
        else if (!access.Branches.All)
        {
            query = query.Where(a => scopeIds.Contains(a.BranchId));
        }

        var pending = await query.OrderBy(a => a.HoldExpiresUtc).ThenBy(a => a.Id).ToListAsync(ct);
        if (pending.Count == 0)
        {
            return new PendingApprovalList(now, 0, null, []);
        }

        var summary = await tenants.GetSummaryAsync(brandId, access.Branches.All ? null : scopeIds, ct);
        var branchNames = summary?.Branches.ToDictionary(b => b.Id, b => b.Name) ?? [];
        var zones = new Dictionary<Guid, string>();
        foreach (var id in pending.Select(a => a.BranchId).Distinct())
        {
            zones[id] = await tenants.GetBranchTimeZoneAsync(brandId, id, ct) ?? PlatformDefaults.DefaultTimeZoneId;
        }

        var people = await customers.GetDisplayInfoAsync(brandId, [.. pending.Select(a => a.BrandCustomerId).Distinct()], ct);
        var staffIds = pending.Select(FirstStaff).Where(s => s is not null).Select(s => s!.Value).Distinct().ToList();
        var staff = await tenants.GetStaffCardsAsync(brandId, staffIds, ct);
        var items = pending.Select(a =>
        {
            var info = people.GetValueOrDefault(a.BrandCustomerId);
            var first = FirstStaff(a);
            return new PendingApprovalItem(
                a.Id,
                a.BranchId,
                branchNames.GetValueOrDefault(a.BranchId) ?? string.Empty,
                zones[a.BranchId],
                info?.FullName ?? string.Empty,
                // Sözleşme: "İlk randevusu mu" (tamamlanmış ziyareti yok).
                info is null || info.VisitCount == 0,
                string.Join(ServiceSeparator, Active(a).Select(i => i.ServiceNameSnapshot)),
                first is { } staffId && staff.TryGetValue(staffId, out var card) ? card.FullName : null,
                a.StartUtc,
                a.EndUtc,
                a.HoldExpiresUtc!.Value,
                a.CreatedUtc,
                a.Channel.ToString(),
                a.DepositStatus?.ToString(),
                DepositAmount(a));
        }).ToList();
        return new PendingApprovalList(now, items.Count, items[0].HoldExpiresUtc, items);
    }

    private static Money? DepositAmount(Appointment a) =>
        a.DepositRequiredMinor is { } deposit
            ? new Money(deposit, Enum.TryParse<Currency>(a.CurrencyCode, out var c) ? c : PlatformDefaults.DefaultCurrency)
            : null;

    private static IEnumerable<AppointmentItem> Active(Appointment a) =>
        a.Items.Where(i => i.Status != AppointmentItemStatus.Cancelled).OrderBy(i => i.SeqNo);

    private static Guid? FirstStaff(Appointment a) => Active(a).FirstOrDefault()?.StaffId;
}
