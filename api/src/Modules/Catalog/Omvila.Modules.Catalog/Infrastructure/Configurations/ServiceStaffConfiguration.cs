using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Catalog.Domain;

namespace Omvila.Modules.Catalog.Infrastructure.Configurations;

internal sealed class ServiceStaffConfiguration : IEntityTypeConfiguration<ServiceStaff>
{
    public void Configure(EntityTypeBuilder<ServiceStaff> builder)
    {
        builder.ToTable("service_staff", table => table.HasCheckConstraint("ck_service_staff_surcharge", "surcharge_minor >= 0"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.CommissionPercentOverride).HasPrecision(5, 2);
        // Personel Tenancy'dedir: modüller arası yabancı anahtar yok, kimlik uçta denetlenir.
        builder.HasIndex(x => new { x.BrandId, x.ServiceId, x.StaffId }).IsUnique();
        builder.HasIndex(x => new { x.BrandId, x.StaffId });
        builder.HasOne<Service>().WithMany().HasForeignKey(x => x.ServiceId).OnDelete(DeleteBehavior.Cascade);
    }
}
