using Microsoft.AspNetCore.Http;
using Omvila.Infrastructure.Http;
using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Security;

/// <summary>
/// Çerezle yenileme yapan web isteği X-Requested-With başlığı taşımalı (CSRF, ADR-0007). Başlıksız çerez
/// isteği 401 unauthorized alır. Gövdede jeton gönderen mobil istek bu denetime takılmaz.
/// </summary>
internal sealed class XRequestedWithFilter : IEndpointFilter
{
    public const string HeaderName = "X-Requested-With";

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.HttpContext.Request;
        if (RefreshTokenCookie.Read(request) is not null && string.IsNullOrWhiteSpace(request.Headers[HeaderName]))
        {
            return Error.Unauthorized().ToProblem();
        }

        return await next(context);
    }
}
