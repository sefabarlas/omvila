using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure.Persistence;
using Omvila.SharedKernel;

namespace Omvila.Infrastructure.Http;

/// <summary>Düzenlenebilir kaynak uçlarının ortak parçaları (api-kurallari.md § Eşzamanlı düzenleme; sürüm = xmin).</summary>
public static class VersionedResults
{
    /// <summary>
    /// If-Match sürümü güncel mi (değilse uç 412 döner). Kaydetmede de xmin denetlensin diye "Version" özelliğinin
    /// özgün değeri istemcinin sürümüne çekilir.
    /// </summary>
    public static bool MatchesVersion<TEntity>(this DbContext db, TEntity entity, uint current, uint expected)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(db);
        db.Entry(entity).Property<uint>("Version").OriginalValue = expected;
        return current == expected;
    }

    /// <summary>200 + ETag.</summary>
    public static Ok<T> Versioned<T>(HttpContext http, uint version, T body)
    {
        ArgumentNullException.ThrowIfNull(http);
        http.Response.Headers.ETag = ETagVersion.Format(version);
        return TypedResults.Ok(body);
    }

    /// <summary>manage yüzeyinde kiracı bağlamı jetondan kurulur; markasız manage isteği olmaz.</summary>
    public static Guid RequireBrandId(this ITenantContext tenant)
    {
        ArgumentNullException.ThrowIfNull(tenant);
        return tenant.Scope.BrandId ?? throw new DomainException("Manage isteğinde marka bağlamı yok.");
    }
}
