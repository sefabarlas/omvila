using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Reviews.Domain;

namespace Omvila.Modules.Reviews.Infrastructure;

internal sealed class ReviewsDbContext(DbContextOptions<ReviewsDbContext> options) : DbContext(options)
{
    public const string Schema = "reviews";

    public DbSet<Review> Reviews => Set<Review>();

    public DbSet<ReviewInvitation> Invitations => Set<ReviewInvitation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReviewsDbContext).Assembly);
    }
}
