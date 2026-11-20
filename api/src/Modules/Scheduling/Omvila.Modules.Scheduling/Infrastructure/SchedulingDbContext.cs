using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Scheduling.Domain;

namespace Omvila.Modules.Scheduling.Infrastructure;

internal sealed class SchedulingDbContext(DbContextOptions<SchedulingDbContext> options) : DbContext(options)
{
    public const string Schema = "scheduling";

    public DbSet<Appointment> Appointments => Set<Appointment>();

    public DbSet<AppointmentItem> AppointmentItems => Set<AppointmentItem>();

    public DbSet<AppointmentItemSegment> AppointmentItemSegments => Set<AppointmentItemSegment>();

    public DbSet<AppointmentSeries> AppointmentSeries => Set<AppointmentSeries>();

    public DbSet<AppointmentAccessCode> AppointmentAccessCodes => Set<AppointmentAccessCode>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchedulingDbContext).Assembly);
    }
}
