using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Scheduling.Domain;

namespace Omvila.Modules.Scheduling.Infrastructure.Configurations;

internal sealed class AppointmentSeriesConfiguration : IEntityTypeConfiguration<AppointmentSeries>
{
    public void Configure(EntityTypeBuilder<AppointmentSeries> builder)
    {
        builder.ToTable("appointment_series", table =>
        {
            table.HasCheckConstraint("ck_appointment_series_status", EnumCheck.In<SeriesStatus>("status"));
            table.HasCheckConstraint("ck_appointment_series_rule", "recurrence_rule IN ('FREQ=WEEKLY;INTERVAL=1', 'FREQ=WEEKLY;INTERVAL=2', 'FREQ=WEEKLY;INTERVAL=4')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(10);
        builder.Property(x => x.RecurrenceRule).HasMaxLength(100);
        builder.Property(x => x.Version).IsRowVersion();
        builder.Ignore(x => x.Rule);
        builder.Ignore(x => x.TemplateItems);
        builder.ComplexCollection<List<SeriesItemTemplate>, SeriesItemTemplate>("_templateItems", items => items.ToJson("template_items"));

        builder.HasIndex(x => new { x.BrandId, x.BranchId, x.Status });
    }
}
