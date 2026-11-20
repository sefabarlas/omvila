using Microsoft.AspNetCore.Http;
using Omvila.SharedKernel;

namespace Omvila.Infrastructure.Http;

/// <summary>
/// Handler sonucunu HTTP'ye çevirir (docs/standartlar/kodlama-dotnet.md § Hata eşleme).
/// Uç nokta iş kuralı içermez: <c>return result.ToHttp(r => TypedResults.Ok(r));</c>
/// </summary>
public static class ResultHttpExtensions
{
    public static IResult ToHttp<T>(this Result<T> result, Func<T, IResult> onSuccess)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        return result.IsSuccess ? onSuccess(result.Value) : result.Error.ToProblem();
    }

    public static IResult ToHttp(this Result result, Func<IResult> onSuccess)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        return result.IsSuccess ? onSuccess() : result.Error.ToProblem();
    }

    /// <summary>problem+json: code, alan hataları (errors) ve koda özgü ek bilgi (meta).</summary>
    public static IResult ToProblem(this Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        var extensions = new Dictionary<string, object?>(StringComparer.Ordinal) { ["code"] = error.Code };
        if (error.FieldErrors.Count > 0)
        {
            extensions["errors"] = error.FieldErrors;
        }

        if (error.Meta.Count > 0)
        {
            extensions["meta"] = error.Meta;
        }

        return TypedResults.Problem(statusCode: StatusFor(error.Kind), extensions: extensions);
    }

    public static int StatusFor(ErrorKind kind) => kind switch
    {
        ErrorKind.Validation => StatusCodes.Status400BadRequest,
        ErrorKind.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorKind.Forbidden => StatusCodes.Status403Forbidden,
        ErrorKind.NotFound => StatusCodes.Status404NotFound,
        ErrorKind.Conflict => StatusCodes.Status409Conflict,
        ErrorKind.PreconditionFailed => StatusCodes.Status412PreconditionFailed,
        ErrorKind.Unprocessable => StatusCodes.Status422UnprocessableEntity,
        ErrorKind.TooManyRequests => StatusCodes.Status429TooManyRequests,
        _ => throw new DomainException($"Eşlenmemiş hata türü: {kind}"),
    };
}
