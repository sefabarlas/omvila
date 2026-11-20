using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Identity.Domain;

namespace Omvila.Modules.Identity.Infrastructure.Configurations;

internal sealed class UserMembershipConfiguration : IEntityTypeConfiguration<UserMembership>
{
    public void Configure(EntityTypeBuilder<UserMembership> builder)
    {
        builder.ToTable("user_memberships", table =>
        {
            table.HasCheckConstraint("ck_user_memberships_role", "role IN ('Owner', 'BranchManager', 'Staff')");
            table.HasCheckConstraint("ck_user_memberships_status", "status IN ('Invited', 'Active', 'Disabled')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Role).HasMaxLength(20);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        // RLS performansı: brand_id ilk sütun. v1'de kullanıcı tek markaya üye (S-16).
        builder.HasIndex(x => new { x.BrandId, x.UserId }).IsUnique();
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
