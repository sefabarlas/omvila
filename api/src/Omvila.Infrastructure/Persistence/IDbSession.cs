using Npgsql;

namespace Omvila.Infrastructure.Persistence;

/// <summary>
/// İstek (veya iş) başına tek bağlantı ve tek transaction. Bütün modül DbContext'leri bunu paylaşır
/// (docs/mimari/mimari.md § Modüller arası iletişim).
/// </summary>
public interface IDbSession : ITenantContext
{
    bool IsActive { get; }

    NpgsqlConnection Connection { get; }

    NpgsqlTransaction Transaction { get; }

    /// <summary>Bağlantıyı açar, transaction'ı başlatır ve geçerli kiracı bağlamını uygular.</summary>
    Task BeginAsync(CancellationToken ct);

    /// <summary>Kiracı bağlamını değiştirir; transaction açıksa hemen uygulanır (ör. public uçta slug çözüldükten sonra).</summary>
    Task SetTenantAsync(TenantScope scope, CancellationToken ct);

    Task CommitAsync(CancellationToken ct);

    Task RollbackAsync(CancellationToken ct);
}
