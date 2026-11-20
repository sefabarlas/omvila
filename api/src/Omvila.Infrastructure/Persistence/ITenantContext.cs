namespace Omvila.Infrastructure.Persistence;

/// <summary>Geçerli kiracı bağlamı. Modül DbContext'lerinin global sorgu filtresi bunu okur (ADR-0006, katman 1).</summary>
public interface ITenantContext
{
    TenantScope Scope { get; }
}
