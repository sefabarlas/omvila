using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Integrations.Domain;

namespace Omvila.Modules.Integrations.Infrastructure;

internal sealed class IntegrationsDbContext(DbContextOptions<IntegrationsDbContext> options) : DbContext(options)
{
    public const string Schema = "integrations";

    public DbSet<CalendarLink> CalendarLinks => Set<CalendarLink>();

    public DbSet<AppointmentCalendarSync> CalendarSyncs => Set<AppointmentCalendarSync>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IntegrationsDbContext).Assembly);
    }
}
