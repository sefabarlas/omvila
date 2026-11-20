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
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.Customers;

/// <summary>
/// Müşteri uçları (sözleşme manage-customers; Musteriler, MusteriEkle, MusteriKarti; dilim-02 kabul kriteri 15).
/// Okuma customer.view, ekleme customer.edit ister. Telefon customer.phone.view yoksa ve marka maskeliyorsa maskeli döner.
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
        IClock clock,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var view = await CustomerViewContext.LoadAsync(brandId, user, tenants, permissions, clock, ct);
        var result = await reader.ListAsync(brandId, q, filter, limit, cursor, view, ct);
        return result.ToHttp(r => TypedResults.Ok(r));
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
        return TypedResults.Ok(row.ToResponse(view));
    }

    // TODO(Scheduling): randevular Scheduling modülüyle gelecek (Dilim 2 sonraki PR). O zamana kadar liste boştur; kart
    // yine markada aranır (başka marka / bilinmeyen kart 404). Personel rolünde yalnız kendi randevuları dönecek.
    private static async Task<IResult> ListAppointmentsAsync(
        Guid brandCustomerId, int? limit, ITenantContext tenant, CustomerReader reader, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        if (!await reader.ExistsAsync(brandId, brandCustomerId, ct))
        {
            return Error.NotFound().ToProblem();
        }

        if (limit is < 1 or > CustomerReader.MaxLimit)
        {
            return Error.Validation("limit", "out_of_range").ToProblem();
        }

        return TypedResults.Ok(new CustomerAppointmentList([], null));
    }
}
