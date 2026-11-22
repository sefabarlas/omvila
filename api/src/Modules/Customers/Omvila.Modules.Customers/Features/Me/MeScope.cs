using Omvila.Infrastructure.Persistence;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.Me;

/// <summary>me yüzeyinde kiracı bağlamı jetondan (sub = global Customer) kurulur; müşterisiz me isteği olmaz (ADR-0006).</summary>
internal static class MeScope
{
    public static Guid CustomerId(ITenantContext tenant)
    {
        ArgumentNullException.ThrowIfNull(tenant);
        return tenant.Scope.CustomerId ?? throw new DomainException("me isteğinde müşteri bağlamı yok.");
    }
}
