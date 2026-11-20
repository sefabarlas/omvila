using Microsoft.AspNetCore.Http;
using Omvila.SharedKernel;

namespace Omvila.Infrastructure.Http;

/// <summary>
/// Düzenleme ve durum eylemleri If-Match ister: yoksa 428 precondition_required, biçimi bozuksa 400.
/// Geçerli sürüm handler'a <see cref="IfMatchHttpContextExtensions.GetIfMatchVersion"/> ile ulaşır.
/// </summary>
internal sealed class IfMatchEndpointFilter : IEndpointFilter
{
    internal const string ItemKey = "omvila.if-match";

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var header = context.HttpContext.Request.Headers.IfMatch.ToString();
        if (string.IsNullOrWhiteSpace(header))
        {
            return TypedResults.Problem(
                statusCode: StatusCodes.Status428PreconditionRequired,
                extensions: new Dictionary<string, object?>(StringComparer.Ordinal) { ["code"] = ErrorCodes.PreconditionRequired });
        }

        if (!ETagVersion.TryParse(header, out var version))
        {
            return Error.Validation("If-Match", "invalid_format").ToProblem();
        }

        context.HttpContext.Items[ItemKey] = version;
        return await next(context);
    }
}
