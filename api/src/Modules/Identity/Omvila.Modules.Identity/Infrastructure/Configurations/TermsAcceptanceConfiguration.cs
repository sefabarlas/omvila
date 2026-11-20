using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Identity.Domain;

namespace Omvila.Modules.Identity.Infrastructure.Configurations;

internal sealed class TermsAcceptanceConfiguration : IEntityTypeConfiguration<TermsAcceptance>
{
    public void Configure(EntityTypeBuilder<TermsAcceptance> builder)
    {
        builder.ToTable("terms_acceptances", table =>
            table.HasCheckConstraint("ck_terms_acceptances_document_type", "document_type IN ('TermsOfUse', 'PrivacyNotice')"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.DocumentType).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.DocumentVersion).HasMaxLength(20);
        // IP adresi kişisel veridir; günlüğe yazılmaz (guvenlik-kvkk.md), yalnız onay kanıtı olarak saklanır.
        builder.Property(x => x.IpAddress).HasMaxLength(45);
        builder.HasIndex(x => new { x.UserId, x.DocumentType, x.DocumentVersion }).IsUnique();
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
