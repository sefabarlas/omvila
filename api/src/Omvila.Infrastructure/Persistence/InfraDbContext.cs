using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure.Idempotency;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Sms;

namespace Omvila.Infrastructure.Persistence;

/// <summary>
/// Ortak altyapı tabloları (infra şeması): outbox_events, processed_events, job_runs, idempotency_keys.
/// Hiçbiri markaya ait değildir (brand_id yok); marka kimliği gerekiyorsa gövdededir.
/// </summary>
internal sealed class InfraDbContext(DbContextOptions<InfraDbContext> options) : DbContext(options)
{
    public const string Schema = "infra";

    public DbSet<OutboxMessage> OutboxEvents => Set<OutboxMessage>();

    public DbSet<ProcessedEvent> ProcessedEvents => Set<ProcessedEvent>();

    public DbSet<JobRun> JobRuns => Set<JobRun>();

    public DbSet<IdempotencyRecord> IdempotencyKeys => Set<IdempotencyRecord>();

    public DbSet<DevSmsMessage> DevSmsLog => Set<DevSmsMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InfraDbContext).Assembly, type => type.Namespace?.EndsWith(".Configurations", StringComparison.Ordinal) == true);
    }
}
