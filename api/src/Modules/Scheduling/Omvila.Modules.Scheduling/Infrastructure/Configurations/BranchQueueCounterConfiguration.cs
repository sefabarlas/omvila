using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Scheduling.Domain;

namespace Omvila.Modules.Scheduling.Infrastructure.Configurations;

internal sealed class BranchQueueCounterConfiguration : IEntityTypeConfiguration<BranchQueueCounter>
{
    public void Configure(EntityTypeBuilder<BranchQueueCounter> builder)
    {
        builder.ToTable("branch_queue_counters", table => table.HasCheckConstraint("ck_branch_queue_counters_last_number", "last_number >= 1"));
        builder.HasKey(x => new { x.BrandId, x.BranchId, x.QueueDate });
    }
}
