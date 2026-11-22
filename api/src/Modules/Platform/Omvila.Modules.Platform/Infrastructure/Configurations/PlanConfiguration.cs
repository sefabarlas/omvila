using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Platform.Domain;

namespace Omvila.Modules.Platform.Infrastructure.Configurations;

internal sealed class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("plans");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Code).HasMaxLength(40);
        builder.Property(x => x.Name).HasMaxLength(80);
        builder.Property(x => x.Description).HasMaxLength(Plan.DescriptionMaxLength);
        builder.Property(x => x.Features)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, Json),
                v => JsonSerializer.Deserialize<PlanFeatures>(v, Json) ?? PlanFeatures.None)
            .HasDefaultValueSql("'{\"walkInQueue\":false,\"waitlist\":false,\"packages\":false,\"crossBranchReports\":false,\"prioritySupport\":false}'::jsonb");
        builder.Property(x => x.Version).IsRowVersion();
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
