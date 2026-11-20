using Microsoft.AspNetCore.Http;
using Omvila.SharedKernel;

namespace Omvila.Infrastructure.Http;

public static class IfMatchHttpContextExtensions
{
    /// <summary>RequireIfMatch() ile işaretli uçta istemcinin gönderdiği sürüm (xmin).</summary>
    public static uint GetIfMatchVersion(this HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        return httpContext.Items.TryGetValue(IfMatchEndpointFilter.ItemKey, out var value) && value is uint version
            ? version
            : throw new DomainException("Uç RequireIfMatch() ile işaretlenmemiş.");
    }
}
