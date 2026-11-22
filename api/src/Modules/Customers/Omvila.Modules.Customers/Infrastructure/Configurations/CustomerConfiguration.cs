using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Customers.Domain;

namespace Omvila.Modules.Customers.Infrastructure.Configurations;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public const string PhoneIndex = "ix_customers_phone_e164";

    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers", table =>
            table.HasCheckConstraint("ck_customers_birth_year", "birth_year IS NULL OR birth_year >= 1900"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.PhoneE164).HasMaxLength(20);
        builder.Property(x => x.FullName).HasMaxLength(120);
        builder.Property(x => x.LanguageCode).HasMaxLength(5);
        builder.Property(x => x.Email).HasMaxLength(160);
        builder.Property(x => x.Version).IsRowVersion();
        // Telefon global olarak tektir (alan-modeli.md § Customer.PhoneE164).
        builder.HasIndex(x => x.PhoneE164).IsUnique().HasDatabaseName(PhoneIndex);
    }
}
