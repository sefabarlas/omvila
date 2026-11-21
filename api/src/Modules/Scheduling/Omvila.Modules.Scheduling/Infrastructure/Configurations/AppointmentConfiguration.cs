using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Scheduling.Domain;

namespace Omvila.Modules.Scheduling.Infrastructure.Configurations;

internal sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public const string TransferCodeIndex = "ix_appointments_brand_id_deposit_transfer_code";

    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointments", table =>
        {
            table.HasCheckConstraint("ck_appointments_status", EnumCheck.In<AppointmentStatus>("status"));
            table.HasCheckConstraint("ck_appointments_channel", EnumCheck.In<AppointmentChannel>("channel"));
            table.HasCheckConstraint("ck_appointments_client_app", EnumCheck.NullOrIn<ClientApp>("client_app"));
            table.HasCheckConstraint("ck_appointments_hold_reason", EnumCheck.NullOrIn<HoldReason>("hold_reason"));
            table.HasCheckConstraint("ck_appointments_cancelled_by", EnumCheck.NullOrIn<CancelledBy>("cancelled_by"));
            table.HasCheckConstraint("ck_appointments_cancellation_reason", EnumCheck.NullOrIn<CancellationReason>("cancellation_reason"));
            table.HasCheckConstraint("ck_appointments_time", "end_utc > start_utc");
            table.HasCheckConstraint("ck_appointments_amounts", "total_amount_minor >= 0 AND (deposit_required_minor IS NULL OR deposit_required_minor >= 0)");
            table.HasCheckConstraint("ck_appointments_deposit_transfer_code", "deposit_transfer_code IS NULL OR deposit_required_minor IS NOT NULL");
            // Değişmez: HoldExpiresUtc yalnız PendingApproval'da doludur (durum-makinesi.md § Değişmezler).
            table.HasCheckConstraint("ck_appointments_hold", "hold_expires_utc IS NULL OR status = 'PendingApproval'");
            table.HasCheckConstraint("ck_appointments_cancellation", "status <> 'Cancelled' OR (cancelled_utc IS NOT NULL AND cancelled_by IS NOT NULL AND cancellation_reason IS NOT NULL)");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Channel).HasConversion<string>().HasMaxLength(10);
        builder.Property(x => x.ClientApp).HasConversion<string>().HasMaxLength(15);
        builder.Property(x => x.HoldReason).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.CancelledBy).HasConversion<string>().HasMaxLength(10);
        builder.Property(x => x.CancellationReason).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsFixedLength();
        builder.Property(x => x.CustomerNote).HasMaxLength(500);
        builder.Property(x => x.StaffNote).HasMaxLength(1000);
        builder.Property(x => x.CancellationNote).HasMaxLength(300);
        builder.Property(x => x.Version).IsRowVersion();
        builder.Ignore(x => x.DisplayStatus);
        builder.Ignore(x => x.DepositCustomerReported);
        builder.Ignore(x => x.DepositStatus);
        builder.Ignore(x => x.AmountDueMinor);
        builder.Property(x => x.DepositTransferCode).HasMaxLength(TransferCodeGenerator.MaxLength);

        builder.HasMany(x => x.Items).WithOne().HasForeignKey(i => i.AppointmentId).OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(x => x.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasOne<AppointmentSeries>().WithMany().HasForeignKey(x => x.SeriesId).OnDelete(DeleteBehavior.Restrict);

        // Takvim (şube + gün), müşteri kartı randevular sekmesi, seri.
        builder.HasIndex(x => new { x.BrandId, x.BranchId, x.StartUtc });
        builder.HasIndex(x => new { x.BrandId, x.BrandCustomerId, x.StartUtc });
        builder.HasIndex(x => new { x.BrandId, x.SeriesId });
        // Kapora açıklama kodu marka içinde benzersiz (alan-modeli.md § Appointment.DepositTransferCode); çakışmada yazma yolu
        // yeni kod üretir (AppointmentWriter).
        builder.HasIndex(x => new { x.BrandId, x.DepositTransferCode }).IsUnique()
            .HasFilter("deposit_transfer_code IS NOT NULL").HasDatabaseName(TransferCodeIndex);
        // MarkNoShows platform kapsamında her dakika tarar: yalnız Geldi denmemiş onaylı randevular.
        builder.HasIndex(x => x.StartUtc).HasFilter("status = 'Confirmed' AND checked_in_utc IS NULL").HasDatabaseName("ix_appointments_no_show_candidates");
        // ExpireAppointmentHolds platform kapsamında her dakika tarar: yalnız onay bekleyen talepler.
        builder.HasIndex(x => x.HoldExpiresUtc).HasFilter("status = 'PendingApproval'").HasDatabaseName("ix_appointments_hold_candidates");
    }
}
