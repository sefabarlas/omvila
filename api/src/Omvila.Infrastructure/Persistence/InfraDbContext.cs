using Microsoft.EntityFrameworkCore;

namespace Omvila.Infrastructure.Persistence;

/// <summary>
/// Ortak altyapı tabloları (infra şeması): outbox_events, processed_events, job_runs, idempotency_keys.
/// Tablolar PR-5b ve PR-5c'de eklenir; ilk migration şemayı kurar ve uygulama rolüne açar.
/// </summary>
internal sealed class InfraDbContext(DbContextOptions<InfraDbContext> options) : DbContext(options)
{
    public const string Schema = "infra";

    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.HasDefaultSchema(Schema);
}
