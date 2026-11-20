using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Omvila.SharedKernel;

namespace Omvila.Infrastructure.Http;

/// <summary>
/// Bütün hata yanıtları RFC 9457 problem+json ve sözleşmedeki zorunlu alanlarla döner:
/// type, title, status, code, traceId (docs/sozlesme/api-kurallari.md § Hatalar).
/// </summary>
public static class ProblemDetailsSetup
{
    private const string TypeBase = "https://docs.omvila.com/errors/";

    public static IServiceCollection AddOmvilaProblemDetails(this IServiceCollection services) =>
        services.AddProblemDetails(options => options.CustomizeProblemDetails = context =>
        {
            var problem = context.ProblemDetails;
            var status = problem.Status ?? context.HttpContext.Response.StatusCode;
            if (!problem.Extensions.TryGetValue("code", out var existing) || existing is not string code)
            {
                code = CodeForStatus(status);
                problem.Extensions["code"] = code;
            }

            problem.Status = status;
            problem.Type = TypeBase + code;
            problem.Detail = null; // İç ayrıntı (yığın, mesaj) istemciye gitmez.
            problem.Extensions.Remove("exception");
            problem.Extensions["traceId"] = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
            problem.Extensions.Remove("requestId");
        });

    /// <summary>
    /// Bir iş kuralı kodu taşımayan yanıtlar (çerçevenin ürettiği 404, 405, 500…) için kod.
    /// 5xx için try_again: istemci bir kez tekrar dener (docs/acik-sorular.md T-18).
    /// </summary>
    public static string CodeForStatus(int status) => status switch
    {
        StatusCodes.Status401Unauthorized => ErrorCodes.Unauthorized,
        StatusCodes.Status403Forbidden => ErrorCodes.Forbidden,
        StatusCodes.Status404NotFound => ErrorCodes.NotFound,
        StatusCodes.Status412PreconditionFailed => ErrorCodes.ConcurrencyConflict,
        StatusCodes.Status428PreconditionRequired => ErrorCodes.PreconditionRequired,
        StatusCodes.Status429TooManyRequests => ErrorCodes.RateLimited,
        >= 500 => ErrorCodes.TryAgain,
        _ => ErrorCodes.ValidationFailed,
    };
}
