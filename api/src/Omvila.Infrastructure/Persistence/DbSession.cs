using System.Data;
using Npgsql;
using Omvila.SharedKernel;

namespace Omvila.Infrastructure.Persistence;

internal sealed class DbSession(NpgsqlDataSource dataSource) : IDbSession, IAsyncDisposable
{
    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _transaction;

    public TenantScope Scope { get; private set; } = TenantScope.None;

    public bool IsActive => _transaction is not null;

    public NpgsqlConnection Connection =>
        _connection ?? throw new DomainException("Veritabanı oturumu başlatılmadı.");

    public NpgsqlTransaction Transaction =>
        _transaction ?? throw new DomainException("Veritabanı oturumu başlatılmadı.");

    public async Task BeginAsync(CancellationToken ct)
    {
        if (IsActive)
        {
            throw new DomainException("Veritabanı oturumu zaten başlatıldı.");
        }

        _connection = await dataSource.OpenConnectionAsync(ct);
        _transaction = await _connection.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
        await ApplyTenantAsync(ct);
    }

    public async Task SetTenantAsync(TenantScope scope, CancellationToken ct)
    {
        Scope = scope;
        if (IsActive)
        {
            await ApplyTenantAsync(ct);
        }
    }

    public async Task CommitAsync(CancellationToken ct)
    {
        await Transaction.CommitAsync(ct);
        await EndAsync();
    }

    public async Task RollbackAsync(CancellationToken ct)
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync(ct);
        }

        await EndAsync();
    }

    public async ValueTask DisposeAsync() => await EndAsync();

    // set_config(..., true): değer transaction'a yereldir, havuza dönen bağlantıda kalmaz (ADR-0006).
    private async Task ApplyTenantAsync(CancellationToken ct)
    {
        await using var command = new NpgsqlCommand(
            "SELECT set_config('app.brand_id', $1, true), set_config('app.customer_id', $2, true), set_config('app.is_platform', $3, true)",
            Connection,
            Transaction);
        command.Parameters.Add(new NpgsqlParameter { Value = Scope.BrandId?.ToString() ?? string.Empty });
        command.Parameters.Add(new NpgsqlParameter { Value = Scope.CustomerId?.ToString() ?? string.Empty });
        command.Parameters.Add(new NpgsqlParameter { Value = Scope.IsPlatform ? "true" : "false" });
        await command.ExecuteNonQueryAsync(ct);
    }

    private async Task EndAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }
    }
}
