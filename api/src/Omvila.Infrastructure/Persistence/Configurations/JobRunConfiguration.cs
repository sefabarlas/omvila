using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Infrastructure.Jobs;

namespace Omvila.Infrastructure.Persistence.Configurations;

internal sealed class JobRunConfiguration : IEntityTypeConfiguration<JobRun>
{
    public void Configure(EntityTypeBuilder<JobRun> builder)
    {
        builder.ToTable("job_runs", table => table.HasCheckConstraint("ck_job_runs_status", "status IN ('Succeeded', 'Failed')"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.JobKey).HasMaxLength(100);
        builder.Property(x => x.Status).HasMaxLength(20);
        builder.Property(x => x.Error).HasMaxLength(300);
        builder.HasIndex(x => new { x.JobKey, x.StartedUtc });
    }
}
