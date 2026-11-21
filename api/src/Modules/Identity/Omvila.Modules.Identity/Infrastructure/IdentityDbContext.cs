using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Identity.Domain;

namespace Omvila.Modules.Identity.Infrastructure;

internal sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public const string Schema = "identity";

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<OtpChallenge> OtpChallenges => Set<OtpChallenge>();

    public DbSet<User> Users => Set<User>();

    public DbSet<UserMembership> Memberships => Set<UserMembership>();

    public DbSet<TermsAcceptance> TermsAcceptances => Set<TermsAcceptance>();

    public DbSet<TrustedDevice> TrustedDevices => Set<TrustedDevice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
    }
}
