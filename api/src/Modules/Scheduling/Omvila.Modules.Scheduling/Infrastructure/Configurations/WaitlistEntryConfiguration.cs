using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Waitlist;

namespace Omvila.Modules.Scheduling.Infrastructure.Configurations;

internal sealed class WaitlistEntryConfiguration : IEntityTypeConfiguration<WaitlistEntry>
{
    /// <summary>Kod platform genelinde tektir (bağlantı markasızdır); bu yüzden brand_id ile başlamaz.</summary>
    public const string CodeIndex = "ix_waitlist_entries_access_code";

    public void Configure(EntityTypeBuilder<WaitlistEntry> builder)
    {
        builder.ToTable("waitlist_entries", table =>
        {
            table.HasCheckConstraint("ck_waitlist_entries_status", EnumCheck.In<WaitlistEntryStatus>("status"));
            table.HasCheckConstraint("ck_waitlist_entries_source", EnumCheck.In<WaitlistSource>("source"));
            table.HasCheckConstraint("ck_waitlist_entries_access_code", "access_code ~ '^[0-9A-Za-z]{10}$'");
            table.HasCheckConstraint("ck_waitlist_entries_services", "cardinality(service_ids) BETWEEN 1 AND 5");
            table.HasCheckConstraint("ck_waitlist_entries_closed", "(status = 'Waiting') = (closed_utc IS NULL)");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(10);
        builder.Property(x => x.Source).HasConversion<string>().HasMaxLength(10);
        builder.Property(x => x.AccessCode).HasMaxLength(AccessCodeGenerator.Length);
        builder.Property(x => x.Note).HasMaxLength(WaitlistEntry.NoteMaxLength);
        builder.PrimitiveCollection(x => x.TimeWindows).ElementType(e => e.HasConversion<string>());
        builder.Property(x => x.Version).IsRowVersion();
        builder.Ignore(x => x.PendingOffer);
        builder.Ignore(x => x.Preferences);

        builder.HasMany(x => x.Offers).WithOne().HasForeignKey(o => o.WaitlistEntryId).OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(x => x.Offers).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => x.AccessCode).IsUnique().HasDatabaseName(CodeIndex);
        // Panel listesi ve teklif motoru: şubenin bekleyenleri, öncelik ve kayıt sırasıyla.
        builder.HasIndex(x => new { x.BrandId, x.BranchId, x.Status, x.JoinedUtc });
        builder.HasIndex(x => new { x.BrandId, x.BrandCustomerId });
    }
}
