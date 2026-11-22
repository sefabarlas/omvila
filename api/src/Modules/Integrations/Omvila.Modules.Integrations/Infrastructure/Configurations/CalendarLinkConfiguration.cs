using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Integrations.Domain;

namespace Omvila.Modules.Integrations.Infrastructure.Configurations;

internal sealed class CalendarLinkConfiguration : IEntityTypeConfiguration<CalendarLink>
{
    public void Configure(EntityTypeBuilder<CalendarLink> builder)
    {
        builder.ToTable("calendar_links", table =>
        {
            table.HasCheckConstraint("ck_calendar_links_status", "status IN ('Active', 'TokenExpired', 'Revoked')");
            table.HasCheckConstraint("ck_calendar_links_provider", "provider IN ('Google')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Provider).HasMaxLength(20);
        builder.Property(x => x.ExternalCalendarId).HasMaxLength(256);
        builder.Property(x => x.AccountEmailMasked).HasMaxLength(200);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        // Personel başına tek bağlantı (yeniden bağlanınca aynı satır).
        builder.HasIndex(x => new { x.BrandId, x.StaffId }).IsUnique();
        // Periyodik işler etkin bağlantıları tarar.
        builder.HasIndex(x => x.Status).HasFilter("status = 'Active'");
    }
}
