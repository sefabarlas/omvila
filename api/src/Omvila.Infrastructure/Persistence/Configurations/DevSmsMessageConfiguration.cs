using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Infrastructure.Sms;

namespace Omvila.Infrastructure.Persistence.Configurations;

internal sealed class DevSmsMessageConfiguration : IEntityTypeConfiguration<DevSmsMessage>
{
    public void Configure(EntityTypeBuilder<DevSmsMessage> builder)
    {
        builder.ToTable("dev_sms_log");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.PhoneE164).HasMaxLength(16);
        builder.Property(x => x.Sender).HasMaxLength(11);
        builder.HasIndex(x => new { x.PhoneE164, x.SentUtc });
    }
}
