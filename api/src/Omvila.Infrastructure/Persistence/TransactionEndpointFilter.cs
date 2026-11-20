using Microsoft.AspNetCore.Http;

namespace Omvila.Infrastructure.Persistence;

/// <summary>
/// API ucunun transaction'ını kapatır (oturumu DbSessionMiddleware açar). Commit, yanıt yazılmadan önce yapılır:
/// istemci başarı görüp commit'in düşmesi mümkün olmaz. 4xx/5xx sonuç ve istisna rollback edilir.
/// </summary>
internal sealed class TransactionEndpointFilter(IDbSession session) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var ct = context.HttpContext.RequestAborted;
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

    private static bool IsSuccess(object? result) => result switch
    {
        IStatusCodeHttpResult { StatusCode: { } status } => status < 400,
        _ => true,
    };
}
