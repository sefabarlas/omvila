using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Omvila.Infrastructure.Idempotency;

public static class IdempotencyExtensions
{
    /// <summary>Uç Idempotency-Key ister (kaynak oluşturan ve para kaydeden POST'lar).</summary>
    public static RouteHandlerBuilder RequireIdempotencyKey(this RouteHandlerBuilder builder) =>
        builder.AddEndpointFilter<IdempotencyEndpointFilter>()
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

    /// <summary>
    /// Idempotency-Key taşıyan isteklerin gövdesini tamponlar: istek özeti model bağlamadan sonra da okunabilsin.
    /// Yönlendirmeden önce, uçlardan önce çağrılır.
    /// </summary>
    public static IApplicationBuilder UseIdempotencyRequestBuffering(this IApplicationBuilder app) =>
        app.Use(async (context, next) =>
        {
            if (context.Request.Headers.ContainsKey(IdempotencyEndpointFilter.HeaderName))
            {
                context.Request.EnableBuffering();
            }

            await next(context);
        });
}
