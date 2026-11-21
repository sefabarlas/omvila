using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Platform.Domain;

namespace Omvila.Modules.Platform.Infrastructure.Configurations;

internal sealed class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
    {
        builder.ToTable("audit_logs", table =>
        {
            table.HasCheckConstraint("ck_audit_logs_actor_type", "actor_type IN ('User', 'PlatformUser', 'Customer', 'System')");
            table.HasCheckConstraint(
                "ck_audit_logs_action", "action IN ('Create', 'Update', 'Delete', 'View', 'Approve', 'Match', 'Export', 'Anonymize')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityAlwaysColumn();
        builder.Property(x => x.ActorType).HasConversion<string>().HasMaxLength(15);
        builder.Property(x => x.Action).HasConversion<string>().HasMaxLength(15);
        builder.Property(x => x.EntityType).HasMaxLength(AuditLogEntry.EntityTypeMaxLength);
        builder.Property(x => x.Changes).HasColumnType("jsonb");
        builder.Property(x => x.IpAddress).HasMaxLength(AuditLogEntry.IpAddressMaxLength);
        builder.Property(x => x.UserAgent).HasMaxLength(AuditLogEntry.UserAgentMaxLength);
        builder.HasIndex(x => new { x.BrandId, x.OccurredUtc });
    }
}
