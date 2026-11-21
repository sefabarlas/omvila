using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Waitlist;

namespace Omvila.Modules.Scheduling.Infrastructure.Configurations;

internal sealed class WaitlistOfferConfiguration : IEntityTypeConfiguration<WaitlistOffer>
{
    public const string CodeIndex = "ix_waitlist_offers_access_code";

    public void Configure(EntityTypeBuilder<WaitlistOffer> builder)
    {
        builder.ToTable("waitlist_offers", table =>
        {
            table.HasCheckConstraint("ck_waitlist_offers_status", EnumCheck.In<WaitlistOfferStatus>("status"));
            table.HasCheckConstraint("ck_waitlist_offers_access_code", "access_code ~ '^[0-9A-Za-z]{10}$'");
            table.HasCheckConstraint("ck_waitlist_offers_time", "offered_end_utc > offered_start_utc AND hold_expires_utc > offered_utc");
            table.HasCheckConstraint("ck_waitlist_offers_appointment", "(status = 'Accepted') = (appointment_id IS NOT NULL)");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(10);
        builder.Property(x => x.AccessCode).HasMaxLength(AccessCodeGenerator.Length);
        builder.Property(x => x.Version).IsRowVersion();
        builder.HasOne<Appointment>().WithMany().HasForeignKey(x => x.AppointmentId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.AccessCode).IsUnique().HasDatabaseName(CodeIndex);
        // Kayıtta en çok bir bekleyen teklif (alan-modeli.md § WaitlistOffer).
        builder.HasIndex(x => x.WaitlistEntryId).IsUnique().HasFilter("status = 'Pending'").HasDatabaseName("ix_waitlist_offers_one_pending");
        // Müsaitlik yükleyicisi: personelin tutulan saatleri (yalnız bekleyen teklifler).
        builder.HasIndex(x => new { x.BrandId, x.OfferedStaffId, x.OfferedStartUtc }).HasFilter("status = 'Pending'")
            .HasDatabaseName("ix_waitlist_offers_pending_staff");
        // ExpireWaitlistOffers platform kapsamında her dakika tarar.
        builder.HasIndex(x => x.HoldExpiresUtc).HasFilter("status = 'Pending'").HasDatabaseName("ix_waitlist_offers_hold_candidates");
    }
}
