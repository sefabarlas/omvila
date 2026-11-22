using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Customers.Domain;

namespace Omvila.Modules.Customers.Infrastructure.Configurations;

internal sealed class DataExportConfiguration : IEntityTypeConfiguration<DataExport>
{
    public void Configure(EntityTypeBuilder<DataExport> builder)
    {
        builder.ToTable("data_exports", table =>
            table.HasCheckConstraint("ck_data_exports_status", "status IN ('Queued', 'Processing', 'Ready', 'Sent', 'Failed')"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Email).HasMaxLength(DataExport.EmailMaxLength);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(15);
        builder.Property(x => x.FileName).HasMaxLength(100);
        builder.Property(x => x.FailureReason).HasMaxLength(200);
        builder.Ignore(x => x.IsOpen);
        builder.Ignore(x => x.ContractStatus);
        builder.HasIndex(x => new { x.BrandId, x.RequestedUtc });
        // İş kuyruğu: bekleyen istekler (platform bağlamında taranır).
        builder.HasIndex(x => x.Status).HasFilter("status IN ('Queued', 'Processing')");
    }
}
