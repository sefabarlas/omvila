using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Catalog.Domain;

namespace Omvila.Modules.Catalog.Infrastructure.Configurations;

internal sealed class ServiceStageRowConfiguration : IEntityTypeConfiguration<ServiceStageRow>
{
    public void Configure(EntityTypeBuilder<ServiceStageRow> builder)
    {
        builder.ToTable("service_stages", table => table.HasCheckConstraint("ck_service_stages_duration", "duration_minutes BETWEEN 5 AND 600"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Name).HasMaxLength(60);
        builder.HasIndex(x => new { x.BrandId, x.ServiceId, x.SeqNo }).IsUnique();
        builder.HasOne<Service>().WithMany().HasForeignKey(x => x.ServiceId).OnDelete(DeleteBehavior.Cascade);
    }
}
