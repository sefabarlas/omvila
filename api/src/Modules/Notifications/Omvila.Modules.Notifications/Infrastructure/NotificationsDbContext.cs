using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Notifications.Domain;

namespace Omvila.Modules.Notifications.Infrastructure;

internal sealed class NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : DbContext(options)
{
    public const string Schema = "notifications";

    public DbSet<NotificationMessage> Outbox => Set<NotificationMessage>();

    public DbSet<CreditTransaction> CreditTransactions => Set<CreditTransaction>();

    public DbSet<BrandSmsSender> SmsSenders => Set<BrandSmsSender>();

    public DbSet<Device> Devices => Set<Device>();

    public DbSet<InAppNotification> InAppNotifications => Set<InAppNotification>();

    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();

    public DbSet<CustomerNotification> CustomerNotifications => Set<CustomerNotification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationsDbContext).Assembly);
    }
}
