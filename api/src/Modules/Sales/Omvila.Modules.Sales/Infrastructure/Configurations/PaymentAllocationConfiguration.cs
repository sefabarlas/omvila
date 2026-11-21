using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Sales.Domain;

namespace Omvila.Modules.Sales.Infrastructure.Configurations;

internal sealed class PaymentAllocationConfiguration : IEntityTypeConfiguration<PaymentAllocation>
{
    public void Configure(EntityTypeBuilder<PaymentAllocation> builder)
    {
        builder.ToTable("payment_allocations", table =>
        {
            table.HasCheckConstraint("ck_payment_allocations_target_type", EnumCheck.In<AllocationTargetType>("target_type"));
            table.HasCheckConstraint("ck_payment_allocations_amount", "amount_minor > 0");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.TargetType).HasConversion<string>().HasMaxLength(20);
        // Randevunun veya paketin tahsilatları.
        builder.HasIndex(x => new { x.BrandId, x.TargetType, x.TargetId });
        builder.HasIndex(x => x.PaymentId);
    }
}
