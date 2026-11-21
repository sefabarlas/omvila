using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Sales.Domain;

namespace Omvila.Modules.Sales.Infrastructure.Configurations;

internal sealed class CommissionEntryConfiguration : IEntityTypeConfiguration<CommissionEntry>
{
    public void Configure(EntityTypeBuilder<CommissionEntry> builder)
    {
        builder.ToTable("commission_entries", table =>
        {
            table.HasCheckConstraint("ck_commission_entries_type", EnumCheck.In<CommissionType>("type"));
            table.HasCheckConstraint("ck_commission_entries_status", EnumCheck.In<CommissionStatus>("status"));
            table.HasCheckConstraint("ck_commission_entries_percent", "percent >= 0 AND percent <= 100");
            // Hizmet primi kaleme, satış primi pakete bağlıdır; eksi tutar yalnız ters kayıtta.
            table.HasCheckConstraint(
                "ck_commission_entries_target",
                "(type = 'Service' AND appointment_item_id IS NOT NULL) OR (type = 'Sale' AND customer_package_id IS NOT NULL)");
            table.HasCheckConstraint(
                "ck_commission_entries_reversal",
                "(status = 'Reversed') = (reverses_entry_id IS NOT NULL) AND (amount_minor >= 0 OR status = 'Reversed')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Percent).HasPrecision(5, 2);
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsFixedLength();
        builder.Property(x => x.Version).IsRowVersion();

        // Personelin dönem primi ("Primim", prim raporu).
        builder.HasIndex(x => new { x.BrandId, x.StaffId, x.PeriodYearMonth });
        // Bir kalemin tek asıl hizmet primi olur (tekrarlanan tamamlamaya karşı); ters kayıtlar hariç.
        builder.HasIndex(x => new { x.BrandId, x.AppointmentItemId })
            .IsUnique()
            .HasFilter("type = 'Service' AND reverses_entry_id IS NULL");
        // Bir müşteri paketinin tek asıl satış primi olur; iade ters kaydı için asıl kayıt bulunur.
        builder.HasIndex(x => new { x.BrandId, x.CustomerPackageId })
            .IsUnique()
            .HasFilter("type = 'Sale' AND reverses_entry_id IS NULL");
    }
}
