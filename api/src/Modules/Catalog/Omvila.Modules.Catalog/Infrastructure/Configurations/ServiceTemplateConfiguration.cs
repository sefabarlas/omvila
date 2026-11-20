using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Catalog.Domain;

namespace Omvila.Modules.Catalog.Infrastructure.Configurations;

internal sealed class ServiceTemplateConfiguration : IEntityTypeConfiguration<ServiceTemplate>
{
    public void Configure(EntityTypeBuilder<ServiceTemplate> builder)
    {
        builder.ToTable("service_templates", table =>
            table.HasCheckConstraint("ck_service_templates_duration", "default_duration_minutes BETWEEN 5 AND 720"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.IndustryCode).HasMaxLength(20);
        builder.Property(x => x.CategoryName).HasMaxLength(60);
        builder.Property(x => x.Name).HasMaxLength(120);
        builder.HasIndex(x => new { x.IndustryCode, x.Name }).IsUnique();
    }
}
