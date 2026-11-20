using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Omvila.Infrastructure.Security;

namespace Omvila.Infrastructure.Persistence;

/// <summary>
/// API uçlarını tek transaction'a sarar. Commit, yanıt yazılmadan önce yapılır: istemci başarı görüp
/// commit'in düşmesi mümkün olmaz. 4xx/5xx sonuç ve istisna rollback edilir.
/// </summary>
internal sealed class TransactionEndpointFilter(IDbSession session) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var ct = context.HttpContext.RequestAborted;
        if (TenantFromUser(context.HttpContext.User) is { } tenant)
        {
            await session.SetTenantAsync(tenant, ct);
        }

        await session.BeginAsync(ct);
        try
        {
            var result = await next(context);
            if (IsSuccess(result))
            {
                await session.CommitAsync(ct);
            }
            else
            {
                await session.RollbackAsync(ct);
            }

            return result;
        }
        catch
        {
            await session.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    // Kiracı bağlamı yalnız doğrulanmış jetondan (ADR-0006); istek gövdesinden veya başlıktan okunmaz.
    private static TenantScope? TenantFromUser(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var audience = user.FindFirst("aud")?.Value;
        return audience switch
        {
            Audiences.Manage when Guid.TryParse(user.FindFirst(OmvilaClaims.BrandId)?.Value, out var brand) => TenantScope.ForBrand(brand),
            Audiences.Admin => TenantScope.Platform,
            Audiences.Me when Guid.TryParse(user.FindFirst(OmvilaClaims.Subject)?.Value, out var customer) => TenantScope.ForCustomer(customer),
            _ => null,
        };
    }

    private static bool IsSuccess(object? result) => result switch
    {
        IStatusCodeHttpResult { StatusCode: { } status } => status < 400,
        _ => true,
    };
}
