using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Customers.Domain;

namespace Omvila.Modules.Customers.Infrastructure.Configurations;

internal sealed class DataDeletionRequestConfiguration : IEntityTypeConfiguration<DataDeletionRequest>
{
    public void Configure(EntityTypeBuilder<DataDeletionRequest> builder)
    {
        builder.ToTable("data_deletion_requests", table =>
        {
            table.HasCheckConstraint("ck_data_deletion_requests_requested_via", "requested_via IN ('CustomerApp', 'Panel')");
            table.HasCheckConstraint("ck_data_deletion_requests_status", "status IN ('Pending', 'Completed', 'Rejected')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.CustomerNameMasked).HasMaxLength(200);
        builder.Property(x => x.RequestedVia).HasConversion<string>().HasMaxLength(15);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(15);
        builder.HasIndex(x => new { x.BrandId, x.RequestedUtc });
        builder.HasIndex(x => x.CustomerId);
        builder.HasOne<Customer>().WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
    }
}
