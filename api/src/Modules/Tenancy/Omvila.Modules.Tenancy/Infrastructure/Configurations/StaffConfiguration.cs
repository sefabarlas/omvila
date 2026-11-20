using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Infrastructure.Configurations;

internal sealed class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.ToTable("staff", table =>
            table.HasCheckConstraint("ck_staff_calendar_color", "calendar_color ~ '^#[0-9A-Fa-f]{6}$'"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.FullName).HasMaxLength(120);
        builder.Property(x => x.DisplayName).HasMaxLength(120);
        builder.Property(x => x.Title).HasMaxLength(60);
        builder.Property(x => x.CalendarColor).HasMaxLength(7);
        builder.Property(x => x.ServiceCommissionPercent).HasPrecision(5, 2);
        builder.Property(x => x.SaleCommissionPercent).HasPrecision(5, 2);
        builder.Property(x => x.Version).IsRowVersion();
        builder.HasIndex(x => x.BrandId);
        // Kullanıcı (Identity) bir markada en çok bir personel kaydına bağlıdır. Modüller arası yabancı anahtar yok.
        builder.HasIndex(x => new { x.BrandId, x.UserId }).IsUnique().HasFilter("user_id IS NOT NULL");
        builder.HasOne<Brand>().WithMany().HasForeignKey(x => x.BrandId).OnDelete(DeleteBehavior.Restrict);
    }
}
