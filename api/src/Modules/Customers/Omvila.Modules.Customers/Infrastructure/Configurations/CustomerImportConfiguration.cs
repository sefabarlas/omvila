using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Customers.Domain;

namespace Omvila.Modules.Customers.Infrastructure.Configurations;

internal sealed class CustomerImportConfiguration : IEntityTypeConfiguration<CustomerImport>
{
    public void Configure(EntityTypeBuilder<CustomerImport> builder)
    {
        builder.ToTable("customer_imports", table =>
            table.HasCheckConstraint("ck_customer_imports_status", "status IN ('Previewed', 'Committed', 'Failed')"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.FileName).HasMaxLength(CustomerImport.FileNameMaxLength);
        builder.Property(x => x.MappedColumns).HasMaxLength(100);
        builder.Property(x => x.ErrorsJson).HasColumnName("errors").HasColumnType("jsonb");
        builder.Property(x => x.RowsJson).HasColumnName("rows").HasColumnType("jsonb");
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(15);
        builder.HasIndex(x => new { x.BrandId, x.CreatedUtc });
    }
}
