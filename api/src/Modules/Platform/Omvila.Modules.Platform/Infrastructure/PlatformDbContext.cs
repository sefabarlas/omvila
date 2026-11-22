using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Platform.Domain;

namespace Omvila.Modules.Platform.Infrastructure;

internal sealed class PlatformDbContext(DbContextOptions<PlatformDbContext> options) : DbContext(options)
{
    public const string Schema = "platform";

    public DbSet<Plan> Plans => Set<Plan>();

    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    public DbSet<SubscriptionInvoice> Invoices => Set<SubscriptionInvoice>();

    public DbSet<PaymentNotice> PaymentNotices => Set<PaymentNotice>();

    public DbSet<CreditPackage> CreditPackages => Set<CreditPackage>();

    public DbSet<InvoiceNumberCounter> InvoiceNumberCounters => Set<InvoiceNumberCounter>();

    public DbSet<AuditLogEntry> AuditLogs => Set<AuditLogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PlatformDbContext).Assembly);
    }
}
