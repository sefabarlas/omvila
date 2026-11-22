using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Scheduling.Domain;

namespace Omvila.Modules.Scheduling.Infrastructure.Configurations;

internal sealed class BranchDailySummaryConfiguration : IEntityTypeConfiguration<BranchDailySummary>
{
    public void Configure(EntityTypeBuilder<BranchDailySummary> builder)
    {
        builder.ToTable("branch_daily_summaries", table => table.HasCheckConstraint("ck_branch_daily_summaries_completed_count", "completed_count >= 0"));
        builder.HasKey(x => new { x.BrandId, x.BranchId, x.BusinessDate });
    }
}
