using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Platform.Domain;

namespace Omvila.Modules.Platform.Infrastructure.Configurations;

internal sealed class InvoiceNumberCounterConfiguration : IEntityTypeConfiguration<InvoiceNumberCounter>
{
    public void Configure(EntityTypeBuilder<InvoiceNumberCounter> builder)
    {
        builder.ToTable("invoice_number_counters", table => table.HasCheckConstraint("ck_invoice_number_counters_last_value", "last_value > 0"));
        builder.HasKey(x => x.Year);
        builder.Property(x => x.Year).ValueGeneratedNever();
    }
}
