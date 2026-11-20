using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Platform.Domain;

namespace Omvila.Modules.Platform.Infrastructure.Configurations;

internal sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("subscriptions", table =>
        {
            table.HasCheckConstraint("ck_subscriptions_status", "status IN ('Trialing', 'Active', 'PastDue', 'ReadOnly', 'Cancelled')");
            table.HasCheckConstraint("ck_subscriptions_billing_cycle", "billing_cycle IN ('Monthly', 'Yearly')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.BillingCycle).HasConversion<string>().HasMaxLength(10);
        builder.HasIndex(x => x.BrandId).IsUnique();
        builder.HasOne<Plan>().WithMany().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Restrict);
    }
}
