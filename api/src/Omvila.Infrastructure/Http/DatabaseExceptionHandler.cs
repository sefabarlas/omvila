using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Omvila.SharedKernel;

namespace Omvila.Infrastructure.Http;

/// <summary>
/// Veritabanı garantilerinden gelen hataları sözleşme kodlarına çevirir (kodlama-dotnet.md § Hata eşleme):
/// eşzamanlılık → 412 concurrency_conflict, 23P01 (çakışma engeli) → 409 slot_taken,
/// 23505 (kayıtlı benzersiz indeks) → 409 indekse eşlenen kod. Diğerleri varsayılan 500'e düşer.
/// </summary>
internal sealed class DatabaseExceptionHandler(
    IProblemDetailsService problemDetails, IEnumerable<UniqueConstraintError> uniqueConstraints) : IExceptionHandler
{
    private readonly Dictionary<string, string> _uniqueCodes =
        uniqueConstraints.ToDictionary(u => u.ConstraintName, u => u.Code, StringComparer.Ordinal);

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var mapped = Map(exception);
        if (mapped is not { } error)
        {
            return false;
        }

        httpContext.Response.StatusCode = error.Status;
        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails { Status = error.Status, Extensions = { ["code"] = error.Code } },
        });
    }

    private (int Status, string Code)? Map(Exception exception)
    {
        if (exception is DbUpdateConcurrencyException)
        {
            return (StatusCodes.Status412PreconditionFailed, ErrorCodes.ConcurrencyConflict);
        }

        var postgres = exception as PostgresException ?? exception.InnerException as PostgresException;
        return postgres switch
        {
            { SqlState: PostgresErrorCodes.ExclusionViolation } => (StatusCodes.Status409Conflict, ErrorCodes.SlotTaken),
            { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: { } name } when _uniqueCodes.TryGetValue(name, out var code)
                => (StatusCodes.Status409Conflict, code),
            _ => null,
        };
    }
}
