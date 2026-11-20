using Microsoft.AspNetCore.Http;

namespace Omvila.Infrastructure.Persistence;

/// <summary>
/// İşaretli API uçlarında oturumu (bağlantı + transaction + kiracı bağlamı) parametre bağlamadan önce açar:
/// uç parametreleri (DbContext ve onu kullanan servisler) endpoint filtrelerinden önce çözülür.
/// Commit TransactionEndpointFilter'da yanıt yazılmadan yapılır; buraya açık dönen oturum (bağlama hatası,
/// istisna, filtreye ulaşmayan yanıt) geri alınır. Kimlik doğrulamadan sonra çalışır.
/// </summary>
internal sealed class DbSessionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IDbSession session)
    {
        if (context.GetEndpoint()?.Metadata.GetMetadata<TransactionalEndpointMetadata>() is null)
        {
            await next(context);
            return;
        }

        var ct = context.RequestAborted;
        if (TenantResolution.FromUser(context.User) is { } tenant)
        {
            await session.SetTenantAsync(tenant, ct);
        }

        await session.BeginAsync(ct);
        try
        {
            await next(context);
        }
        finally
        {
            if (session.IsActive)
            {
                await session.RollbackAsync(CancellationToken.None);
            }
        }
    }
}
