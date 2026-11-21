using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Identity.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.CustomerPayments;

/// <summary>
/// Müşteri kartı Ödemeler sekmesi (sözleşme manageListCustomerPayments). payment.collect (Owner, BranchManager kendi şubelerinin
/// tahsilatları; personel 403). Kart markada yoksa 404.
/// </summary>
internal static class CustomerPaymentEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var customers = app.MapApiGroup("/api/v1/manage/customers", Audiences.Manage);
        customers.MapGet("/{brandCustomerId:guid}/payments", ListAsync).WithName("manageListCustomerPayments")
            .RequirePermission(Permissions.Payment.Collect);
    }

    private static async Task<IResult> ListAsync(
        Guid brandCustomerId,
        int? limit,
        string? cursor,
        ClaimsPrincipal user,
        ITenantContext tenant,
        ICustomerDirectory customers,
        CustomerPaymentReader reader,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        if (!(await customers.GetDisplayInfoAsync(brandId, [brandCustomerId], ct)).ContainsKey(brandCustomerId))
        {
            return Error.NotFound().ToProblem();
        }

        var result = await reader.ListAsync(brandId, brandCustomerId, limit, cursor, BranchScope.From(user), ct);
        return result.ToHttp(page => TypedResults.Ok(page));
    }
}
