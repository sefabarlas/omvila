using Microsoft.AspNetCore.Http;
using Omvila.Infrastructure.Http;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.PublicBooking;

/// <summary>
/// Public şube uçlarının kiracı bağlamı: yoldaki <c>branchId</c> platform bağlamında çözülür ve oturum yalnız o şubenin
/// markasına kurulur (ADR-0006; istekten marka kimliği okunmaz). Şube yoksa 404; online randevu kapalıysa (salt okunur
/// abonelik) 403 <c>booking_disabled</c> (durum-makinesi.md § Abonelik). Idempotency filtresinden önce eklenir: anahtar
/// marka kapsamında saklanır.
/// </summary>
internal sealed class PublicBranchFilter(IPublicBookingQueries booking) : IEndpointFilter
{
    private const string ItemKey = "omvila.public-branch";

    public static PublicBranchScope ScopeOf(HttpContext http) =>
        http.Items[ItemKey] as PublicBranchScope ?? throw new InvalidOperationException("PublicBranchFilter eklenmemiş.");

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var http = context.HttpContext;
        if (!Guid.TryParse(http.Request.RouteValues["branchId"]?.ToString(), out var branchId)
            || await booking.EnterBranchAsync(branchId, http.RequestAborted) is not { } scope)
        {
            return Error.NotFound().ToProblem();
        }

        if (!scope.BookingEnabled)
        {
            return Error.Forbidden(ErrorCodes.BookingDisabled).ToProblem();
        }

        http.Items[ItemKey] = scope;
        return await next(context);
    }
}
