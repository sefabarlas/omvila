using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Scheduling.Domain;

namespace Omvila.Modules.Scheduling.Infrastructure.Configurations;

internal sealed class AppointmentAccessCodeConfiguration : IEntityTypeConfiguration<AppointmentAccessCode>
{
    /// <summary>Kod platform genelinde tektir (bağlantı markasızdır); bu yüzden brand_id ile başlamaz.</summary>
    public const string CodeIndex = "ix_appointment_access_codes_code";

    public void Configure(EntityTypeBuilder<AppointmentAccessCode> builder)
    {
        builder.ToTable("appointment_access_codes", table =>
        {
            table.HasCheckConstraint("ck_appointment_access_codes_purpose", EnumCheck.In<AccessCodePurpose>("purpose"));
            table.HasCheckConstraint("ck_appointment_access_codes_code", "code ~ '^[0-9A-Za-z]{10}$'");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Code).HasMaxLength(AccessCodeGenerator.Length);
        builder.Property(x => x.Purpose).HasConversion<string>().HasMaxLength(15);

        builder.HasOne<Appointment>().WithMany().HasForeignKey(x => x.AppointmentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName(CodeIndex);
        builder.HasIndex(x => new { x.BrandId, x.AppointmentId });
    }
}
