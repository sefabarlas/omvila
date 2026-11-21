using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Scheduling.Domain;

namespace Omvila.Modules.Scheduling.Infrastructure.Configurations;

internal sealed class WalkInTicketConfiguration : IEntityTypeConfiguration<WalkInTicket>
{
    public void Configure(EntityTypeBuilder<WalkInTicket> builder)
    {
        builder.ToTable("walk_in_tickets", table =>
        {
            table.HasCheckConstraint("ck_walk_in_tickets_status", EnumCheck.In<WalkInTicketStatus>("status"));
            table.HasCheckConstraint("ck_walk_in_tickets_queue_number", "queue_number >= 1");
            table.HasCheckConstraint("ck_walk_in_tickets_served", "status <> 'Served' OR appointment_id IS NOT NULL");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(10);
        builder.Property(x => x.CustomerName).HasMaxLength(120);
        builder.Property(x => x.PhoneE164).HasMaxLength(20);
        builder.Property(x => x.ServiceName).HasMaxLength(120);
        builder.Property(x => x.Version).IsRowVersion();
        builder.Ignore(x => x.IsInQueue);

        // Numara şube + gün içinde tektir (numaratörün yanında ikinci güvence); sıra okuması da bu indeksi kullanır.
        builder.HasIndex(x => new { x.BrandId, x.BranchId, x.QueueDate, x.QueueNumber }).IsUnique();
    }
}
