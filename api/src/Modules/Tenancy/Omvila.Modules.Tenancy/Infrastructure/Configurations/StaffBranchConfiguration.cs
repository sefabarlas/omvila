using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Infrastructure.Configurations;

internal sealed class StaffBranchConfiguration : IEntityTypeConfiguration<StaffBranch>
{
    public void Configure(EntityTypeBuilder<StaffBranch> builder)
    {
        builder.ToTable("staff_branches");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.HasIndex(x => new { x.BrandId, x.StaffId, x.BranchId }).IsUnique();
        builder.HasIndex(x => new { x.BrandId, x.BranchId });
        builder.HasOne<Staff>().WithMany().HasForeignKey(x => x.StaffId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
    }
}
