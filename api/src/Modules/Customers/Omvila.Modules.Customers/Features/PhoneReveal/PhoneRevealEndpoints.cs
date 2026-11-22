using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Customers.Features.Customers;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.PhoneReveal;

/// <summary>
/// Maskeli numarayı arama anında açma (sözleşme manageRevealCustomerPhone; işletme uygulaması "Ara" / "SMS"; roller-yetkiler.md
/// customer.phone.view dipnotu). İzin customer.view. Personel rolü (calendar.view = K) yalnız kendi kalemi olan bir randevusu
/// bulunan müşterinin numarasını açar; başka müşteri, başka markanın kaydı, telefonsuz ve anonimleştirilmiş kayıt 404. Her başarılı çağrı denetime
/// yazılır (kullanıcı, kayıt, amaç; numara yazılmaz); yanıt önbelleğe alınmaz.
/// </summary>
internal static class PhoneRevealEndpoints
{
    private static readonly string[] Purposes = ["Call", "Sms"];

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapApiGroup("/api/v1/manage/customers", Audiences.Manage)
            .MapPost("/{brandCustomerId:guid}/reveal-phone", RevealAsync)
            .WithName("manageRevealCustomerPhone")
            .RequirePermission(Permissions.Customer.View);

    // Kural: roller-yetkiler.md customer.phone.view (** "maskesiz numara istemciye yalnız arama anında … denetime yazılır"), § Kapsam kuralları
    private static async Task<IResult> RevealAsync(
        Guid brandCustomerId,
        PhoneRevealRequest? body,
        HttpContext http,
        ClaimsPrincipal user,
        ITenantContext tenant,
        IPermissionResolver permissions,
        CustomerReader reader,
        IAppointmentQueries appointments,
        IAuditLog audit,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var row = await reader.FindAsync(brandId, brandCustomerId, ct);
        if (row is null || row.IsAnonymized || row.PhoneE164 is not { Length: > 0 } phone)
        {
            return Error.NotFound().ToProblem();
        }

        var role = user.FindFirst(OmvilaClaims.Role)?.Value;
        if (role is not null && permissions.Resolve(role, Permissions.Calendar.View) == PermissionScope.Own)
        {
            var staffId = Guid.TryParse(user.FindFirst(OmvilaClaims.StaffId)?.Value, out var id) ? id : Guid.Empty;
            var own = await appointments.ListForCustomerAsync(brandId, brandCustomerId, staffId, 1, null, ct);
            if (!own.IsSuccess || own.Value.Items.Count == 0)
            {
                return Error.NotFound().ToProblem();
            }
        }

        var purpose = body?.Purpose;
        if (string.IsNullOrEmpty(purpose) || !Purposes.Contains(purpose, StringComparer.Ordinal))
        {
            return Error.Validation("purpose", string.IsNullOrEmpty(purpose) ? "required" : "invalid_format").ToProblem();
        }

        await audit.WriteAsync(
            AuditActor.From(http).Entry(brandId, AuditAction.View, nameof(Domain.BrandCustomer), brandCustomerId, new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["operation"] = "reveal_phone",
                ["purpose"] = purpose,
            }),
            ct);
        http.Response.Headers.CacheControl = "no-store";
        return TypedResults.Ok(new RevealedPhone(phone));
    }
}
