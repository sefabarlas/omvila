using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Infrastructure.Configurations;

internal sealed class BookingPageSettingsConfiguration : IEntityTypeConfiguration<BookingPageSettings>
{
    public void Configure(EntityTypeBuilder<BookingPageSettings> builder)
    {
        builder.ToTable("booking_page_settings", table =>
            table.HasCheckConstraint("ck_booking_page_settings_cover_color", "cover_color_token IN ('petrol', 'mercan', 'kum')"));
        builder.HasKey(x => x.BrandId);
        builder.Property(x => x.BrandId).ValueGeneratedNever();
        builder.Property(x => x.CoverColorToken).HasMaxLength(20);
        builder.Property(x => x.PreAppointmentNote).HasMaxLength(200);
        builder.Property(x => x.Version).IsRowVersion();
        builder.HasOne<Brand>().WithOne().HasForeignKey<BookingPageSettings>(x => x.BrandId).OnDelete(DeleteBehavior.Restrict);
    }
}
