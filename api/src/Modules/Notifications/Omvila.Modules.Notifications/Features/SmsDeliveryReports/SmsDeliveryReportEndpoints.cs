using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Sms;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Features.SmsDeliveryReports;

/// <summary>
/// SMS teslim raporu webhook'u (sözleşme webhookSmsDeliveryReport; T-62 (8); dilim-03 kriter 10). Sağlayıcı kodu
/// keyed <see cref="ISmsDeliveryReportVerifier"/>'a eşlenir: kayıtsız kod 404, imzasız veya imzası tutmayan istek 401
/// (hiçbir kayda dokunulmaz), anlaşılmayan gövde 400, aksi 204.
/// </summary>
internal static class SmsDeliveryReportEndpoints
{
    // Teslim raporu küçüktür; sınır kötü niyetli büyük gövdeyi imza hesaplanmadan keser.
    private const int MaxBodyBytes = 64 * 1024;

    public static void Map(IEndpointRouteBuilder app)
    {
        // Sunucudan sunucuya çağrı: CORS yüzeyi ve OMVILA jetonu yok.
        var group = app.MapApiGroup("/api/v1/webhooks");

        // Sağlayıcı OMVILA jetonu taşımaz (security: []); kimlik sağlayıcının imzasıyla adaptörde doğrulanır.
        group.MapPost("/sms/{provider}", HandleAsync)
            .WithName("webhookSmsDeliveryReport")
            .AllowAnonymous()
            .Accepts<Dictionary<string, object>>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        string provider, HttpRequest request, IServiceProvider services, SmsDeliveryReportHandler handler, CancellationToken ct)
    {
        if (services.GetKeyedService<ISmsDeliveryReportVerifier>(provider) is not { } verifier)
        {
            return Error.NotFound().ToProblem();
        }

        // İmza ham baytlar üzerindendir: gövde model bağlamadan önce olduğu gibi okunur.
        var body = await ReadBodyAsync(request, ct);
        if (body is null)
        {
            return Error.Validation("body", "too_large").ToProblem();
        }

        var verification = verifier.Verify(request.Headers, body);
        switch (verification.Status)
        {
            case SmsDeliveryReportVerificationStatus.Unauthorized:
                return Error.Unauthorized().ToProblem();
            case SmsDeliveryReportVerificationStatus.Malformed:
                return Error.Validation("body", "invalid_format").ToProblem();
            default:
                await handler.ApplyAsync(verification.Reports, ct);
                return TypedResults.NoContent();
        }
    }

    private static async Task<byte[]?> ReadBodyAsync(HttpRequest request, CancellationToken ct)
    {
        if (request.ContentLength > MaxBodyBytes)
        {
            return null;
        }

        request.EnableBuffering();
        using var buffer = new MemoryStream();
        var chunk = new byte[8192];
        int read;
        while ((read = await request.Body.ReadAsync(chunk, ct)) > 0)
        {
            if (buffer.Length + read > MaxBodyBytes)
            {
                return null;
            }

            await buffer.WriteAsync(chunk.AsMemory(0, read), ct);
        }

        request.Body.Position = 0;
        return buffer.ToArray();
    }
}
