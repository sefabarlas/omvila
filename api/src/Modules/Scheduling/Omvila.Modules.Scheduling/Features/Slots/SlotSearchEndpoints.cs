using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Infrastructure.Availability;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Slots;

/// <summary>
/// Panel için boş saatler ("Elif için boş saatler"; sözleşme manageSearchSlots). Motor panel kipinde çalışır: en az ön
/// bildirim ve en ileri tarih uygulanmaz, online kapalı personel ve hizmet dahildir (T-48). Yeni randevu akışının parçası
/// olduğu için appointment.create ister; personel (K) yalnız kendi saatlerini arar (fark etmez kalem kendisine bağlanır).
/// </summary>
internal static class SlotSearchEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/manage/branches", Audiences.Manage);
        group.MapPost("/{branchId:guid}/availability/slots", SearchAsync).WithName("manageSearchSlots")
            .RequirePermission(Permissions.Appointment.Create);
    }

    // Kural: musaitlik.md § Algoritma ve § Panelden elle ekleme
    private static async Task<IResult> SearchAsync(
        Guid branchId,
        SlotSearchRequest body,
        HttpContext http,
        ITenantContext tenant,
        IPermissionResolver permissions,
        ITenantQueries tenants,
        AvailabilitySnapshotLoader loader,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var access = ManageAccess.For(http.User, permissions, Permissions.Appointment.Create);
        if (!access.Branches.Includes(branchId) || !await tenants.BranchExistsAsync(brandId, branchId, ct))
        {
            return Error.NotFound().ToProblem();
        }

        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (body.Date is null)
        {
            errors["date"] = ["required"];
        }

        var items = new List<RequestedItem>();
        for (var i = 0; i < (body.Items?.Count ?? 0); i++)
        {
            if (body.Items![i]?.ServiceId is { } serviceId)
            {
                var staffId = body.Items[i].StaffId ?? (access.OwnOnly ? access.OwnStaffId : null);
                items.Add(new RequestedItem(serviceId, staffId));
            }
            else
            {
                errors[$"items[{i}].serviceId"] = ["required"];
            }
        }

        var request = AvailabilityRequest.Create(items, body.SameStaffForAll ?? true, AvailabilityMode.Panel);
        if (!request.IsSuccess)
        {
            foreach (var (field, codes) in request.Error.FieldErrors)
            {
                errors.TryAdd(field, codes);
            }
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        if (items.Exists(i => !access.IsOwnStaff(i.StaffId)))
        {
            return Error.Forbidden().ToProblem();
        }

        var date = body.Date!.Value;
        var fixedStaff = items.TrueForAll(i => i.StaffId is not null) ? items.Select(i => i.StaffId!.Value).Distinct().ToList() : null;
        var snapshot = await loader.LoadAsync(brandId, branchId, date, date, [.. items.Select(i => i.ServiceId).Distinct()], fixedStaff, ct);
        if (snapshot is null)
        {
            return Error.NotFound().ToProblem();
        }

        var day = AvailabilityEngine.GetSlots(snapshot, request.Value, date);
        var staffIds = day.Slots.SelectMany(s => s.Items.Select(i => i.StaffId)).Distinct().ToList();
        var staff = await tenants.GetStaffCardsAsync(brandId, staffIds, ct);
        return TypedResults.Ok(SlotMapping.ToResult(day, snapshot.Zone, staff));
    }
}
