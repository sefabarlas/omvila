using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NodaTime;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Idempotency;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.Customers;

/// <summary>
/// Müşteri uçları (sözleşme manage-customers; Musteriler, MusteriEkle, MusteriKarti; dilim-02 kabul kriteri 15).
/// Okuma customer.view, ekleme customer.edit ister. Telefon customer.phone.view yoksa ve marka maskeliyorsa maskeli döner.
/// Yaklaşan randevu (nextAppointmentUtc) ve randevular sekmesi Scheduling'in IAppointmentQueries sözleşmesinden okunur.
/// </summary>
internal static class CustomerEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/manage/customers", Audiences.Manage);
        group.MapGet(string.Empty, ListAsync).WithName("manageListCustomers").RequirePermission(Permissions.Customer.View);
        group.MapPost(string.Empty, CreateAsync).WithName("manageCreateCustomer")
            .RequirePermission(Permissions.Customer.Edit).RequireIdempotencyKey();
        group.MapGet("/{brandCustomerId:guid}", GetAsync).WithName("manageGetCustomer").RequirePermission(Permissions.Customer.View);
        group.MapGet("/{brandCustomerId:guid}/appointments", ListAppointmentsAsync).WithName("manageListCustomerAppointments")
            .RequirePermission(Permissions.Customer.View);
    }

    private static async Task<IResult> ListAsync(
        string? q,
        string? filter,
        int? limit,
        string? cursor,
        ClaimsPrincipal user,
        ITenantContext tenant,
        CustomerReader reader,
        ITenantQueries tenants,
        IPermissionResolver permissions,
        IAppointmentQueries appointments,
        IClock clock,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var view = await CustomerViewContext.LoadAsync(brandId, user, tenants, permissions, clock, ct);
        var result = await reader.ListAsync(brandId, q, filter, limit, cursor, view, ct);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblem();
        }

        var items = await WithNextAppointmentAsync(brandId, result.Value.Items, appointments, clock, ct);
        return TypedResults.Ok(result.Value with { Items = items });
    }

    private static async Task<IResult> CreateAsync(
        CustomerCreateRequest body,
        ClaimsPrincipal user,
        ITenantContext tenant,
        CustomerWriter writer,
        CustomerReader reader,
        ITenantQueries tenants,
        IPermissionResolver permissions,
        IClock clock,
        CancellationToken ct)
    {
        // Yeni kartın yaklaşan randevusu olamaz; nextAppointmentUtc yazılmaz.
        var brandId = tenant.RequireBrandId();
        var view = await CustomerViewContext.LoadAsync(brandId, user, tenants, permissions, clock, ct);
        var created = await writer.CreateAsync(brandId, body, view, ct);
        if (!created.IsSuccess)
        {
            return created.Error.ToProblem();
        }

        var row = await reader.FindAsync(brandId, created.Value, ct) ?? throw new DomainException("Eklenen müşteri okunamadı.");
        return TypedResults.Created($"/api/v1/manage/customers/{row.Id}", row.ToResponse(view));
    }

    private static async Task<IResult> GetAsync(
        Guid brandCustomerId,
        ClaimsPrincipal user,
        ITenantContext tenant,
        CustomerReader reader,
        ITenantQueries tenants,
        IPermissionResolver permissions,
        IAppointmentQueries appointments,
        IClock clock,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var row = await reader.FindAsync(brandId, brandCustomerId, ct);
        if (row is null)
        {
            return Error.NotFound().ToProblem();
        }

        var view = await CustomerViewContext.LoadAsync(brandId, user, tenants, permissions, clock, ct);
        var items = await WithNextAppointmentAsync(brandId, [row.ToResponse(view)], appointments, clock, ct);
        return TypedResults.Ok(items[0]);
    }

    // Personel rolü (takvimde yalnız kendi kolonu, calendar.view = K) yalnız kendi kalemi olan randevuları görür.
    private static async Task<IResult> ListAppointmentsAsync(
        Guid brandCustomerId,
        int? limit,
        string? cursor,
        ClaimsPrincipal user,
        ITenantContext tenant,
        CustomerReader reader,
        IPermissionResolver permissions,
        IAppointmentQueries appointments,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        if (!await reader.ExistsAsync(brandId, brandCustomerId, ct))
        {
            return Error.NotFound().ToProblem();
        }

        var role = user.FindFirst(OmvilaClaims.Role)?.Value;
        Guid? onlyStaffId = null;
        if (role is not null && permissions.Resolve(role, Permissions.Calendar.View) == PermissionScope.Own)
        {
            onlyStaffId = Guid.TryParse(user.FindFirst(OmvilaClaims.StaffId)?.Value, out var staffId) ? staffId : Guid.Empty;
        }

        var page = await appointments.ListForCustomerAsync(brandId, brandCustomerId, onlyStaffId, limit ?? CustomerReader.DefaultLimit, cursor, ct);
        return page.ToHttp(p => TypedResults.Ok(new CustomerAppointmentList(
            [.. p.Items.Select(a => new CustomerAppointmentRow(
                a.AppointmentId, a.BranchId, a.StartUtc, a.Status, a.DisplayStatus, a.ServiceNames, a.StaffNames,
                new Money(a.TotalAmountMinor, Enum.TryParse<Currency>(a.CurrencyCode, out var c) ? c : PlatformDefaults.DefaultCurrency)))],
            p.NextCursor)));
    }

    internal static async Task<IReadOnlyList<CustomerSummaryResponse>> WithNextAppointmentAsync(
        Guid brandId, IReadOnlyList<CustomerSummaryResponse> items, IAppointmentQueries appointments, IClock clock, CancellationToken ct)
    {
        var next = await appointments.GetNextAppointmentsAsync(brandId, [.. items.Select(i => i.Id)], clock.GetCurrentInstant(), ct);
        return [.. items.Select(i => next.TryGetValue(i.Id, out var at) ? i with { NextAppointmentUtc = at } : i)];
    }
}
