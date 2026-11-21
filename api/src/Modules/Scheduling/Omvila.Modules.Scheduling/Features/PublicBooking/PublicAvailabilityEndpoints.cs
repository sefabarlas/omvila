using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NodaTime;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Features.Slots;
using Omvila.Modules.Scheduling.Infrastructure.Availability;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.PublicBooking;

/// <summary>
/// Rezervasyon sayfasının gün şeridi ve saat ızgarası (sözleşme publicSearchDays, publicSearchSlots; ekranlar Saat,
/// DoluGun; dilim-02 kabul kriterleri 5–8, 16). Motor public kipinde çalışır: en az ön bildirim, en ileri tarih, ızgara,
/// yalnız online açık personel ve hizmet (musaitlik.md § 7). "Fark etmez" kalemde personel motorun deterministik kuralıyla
/// atanır ve saatin kaleminde döner (T-48 (6)). Yan etkisizdir; yazma yolu aynı saati kilit altında yeniden doğrular.
/// </summary>
internal static class PublicAvailabilityEndpoints
{
    /// <summary>Sözleşme DaySearchRequest.to: "En fazla 31 gün".</summary>
    private const int MaxDayRange = 31;

    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/public/branches");

        // Rezervasyon sayfası üyeliksizdir (ürün kuralı 7): boş günler ve saatler herkese açık bağlantıyla görüntülenir.
        group.MapPost("/{branchId:guid}/availability/days", DaysAsync).WithName("publicSearchDays").AllowAnonymous()
            .AddEndpointFilter<PublicBranchFilter>();

        // Üyeliksiz: saat ızgarası herkese açıktır.
        group.MapPost("/{branchId:guid}/availability/slots", SlotsAsync).WithName("publicSearchSlots").AllowAnonymous()
            .AddEndpointFilter<PublicBranchFilter>();
    }

    // Kural: musaitlik.md § Gün listesi
    private static async Task<IResult> DaysAsync(
        DaySearchRequest body, HttpContext http, IPublicBookingQueries booking, AvailabilitySnapshotLoader loader, CancellationToken ct)
    {
        var scope = PublicBranchFilter.ScopeOf(http);
        var sameStaff = body.SameStaffForAll ?? true;
        var items = PublicStaffSelection.Read(body.Items, sameStaff, await booking.GetBookingOptionsAsync(scope.BrandId, ct));
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (body.From is null)
        {
            errors["from"] = ["required"];
        }

        if (body.To is null)
        {
            errors["to"] = ["required"];
        }
        else if (body.From is { } f && (body.To < f || Period.Between(f, body.To.Value, PeriodUnits.Days).Days >= MaxDayRange))
        {
            errors["to"] = ["out_of_range"];
        }

        if (!items.IsSuccess || errors.Count > 0)
        {
            return Error.Validation(Merge(errors, items)).ToProblem();
        }

        var (from, to) = (body.From!.Value, body.To!.Value);
        var request = AvailabilityRequest.Create(items.Value, sameStaff, AvailabilityMode.Public).Value;
        var serviceIds = items.Value.Select(i => i.ServiceId).Distinct().ToList();
        var fixedStaff = PublicStaffSelection.FixedStaff(items.Value, sameStaff);
        var snapshot = await loader.LoadAsync(scope.BrandId, scope.BranchId, from, to, serviceIds, fixedStaff, ct);
        if (snapshot is null)
        {
            return Error.NotFound().ToProblem();
        }

        var maxDate = AvailabilityEngine.LastBookableDate(snapshot, request);
        var days = AvailabilityEngine.GetDays(snapshot, request, from, to);
        Instant? next = null;

        // "Perşembe dolu · en yakın uygun saat Cuma 10:30": aralıkta boş gün yoksa en ileri tarihe kadar ilk uygun saat.
        if (!days.Any(d => d.HasAvailability) && to < maxDate)
        {
            var later = await loader.LoadAsync(scope.BrandId, scope.BranchId, to.PlusDays(1), maxDate, serviceIds, fixedStaff, ct);
            next = later is null
                ? null
                : AvailabilityEngine.GetDays(later, request, to.PlusDays(1), maxDate).FirstOrDefault(d => d.HasAvailability)?.FirstSlot?.Start;
        }

        return TypedResults.Ok(new DaySearchResult(
            snapshot.Zone.Id,
            maxDate,
            [.. days.Select(d => new DaySearchResult.Day(d.Date, d.HasAvailability, d.IsClosed, d.FirstSlot?.Start))],
            next));
    }

    // Kural: musaitlik.md § Algoritma, § "Fark etmez"
    private static async Task<IResult> SlotsAsync(
        SlotSearchRequest body,
        HttpContext http,
        IPublicBookingQueries booking,
        ITenantQueries tenants,
        AvailabilitySnapshotLoader loader,
        CancellationToken ct)
    {
        var scope = PublicBranchFilter.ScopeOf(http);
        var sameStaff = body.SameStaffForAll ?? true;
        var items = PublicStaffSelection.Read(body.Items, sameStaff, await booking.GetBookingOptionsAsync(scope.BrandId, ct));
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (body.Date is null)
        {
            errors["date"] = ["required"];
        }

        if (!items.IsSuccess || errors.Count > 0)
        {
            return Error.Validation(Merge(errors, items)).ToProblem();
        }

        var date = body.Date!.Value;
        var request = AvailabilityRequest.Create(items.Value, sameStaff, AvailabilityMode.Public).Value;
        var snapshot = await loader.LoadAsync(
            scope.BrandId, scope.BranchId, date, date, [.. items.Value.Select(i => i.ServiceId).Distinct()], PublicStaffSelection.FixedStaff(items.Value, sameStaff), ct);
        if (snapshot is null)
        {
            return Error.NotFound().ToProblem();
        }

        var day = AvailabilityEngine.GetSlots(snapshot, request, date);
        var staff = await PublicNamesAsync(tenants, scope.BrandId, day.Slots.SelectMany(s => s.Items.Select(i => i.StaffId)).Distinct().ToList(), ct);
        return TypedResults.Ok(SlotMapping.ToResult(day, snapshot.Zone, staff));
    }

    /// <summary>Rezervasyon sayfasında personelin görünen adı (yoksa ad soyad).</summary>
    internal static async Task<IReadOnlyDictionary<Guid, StaffCard>> PublicNamesAsync(
        ITenantQueries tenants, Guid brandId, IReadOnlyCollection<Guid> staffIds, CancellationToken ct) =>
        (await tenants.GetStaffCardsAsync(brandId, staffIds, ct))
            .ToDictionary(p => p.Key, p => p.Value with { FullName = p.Value.DisplayName ?? p.Value.FullName });

    private static Dictionary<string, string[]> Merge<T>(Dictionary<string, string[]> errors, Result<T> items)
    {
        if (!items.IsSuccess)
        {
            foreach (var (field, codes) in items.Error.FieldErrors)
            {
                errors.TryAdd(field, codes);
            }
        }

        return errors;
    }
}
