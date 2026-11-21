using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Testing;
using Npgsql;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Features;
using Omvila.Modules.Scheduling.Features.Appointments;
using Omvila.Modules.Scheduling.Features.Approvals;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Scheduling.Jobs;
using Omvila.TestKit;
using static Omvila.Modules.Scheduling.IntegrationTests.AvailabilityData;

namespace Omvila.Modules.Scheduling.IntegrationTests;

// Kural: durum-makinesi.md A2 / A5 (onay ile otomatik iptal yarışı), T-63: tam olarak biri kazanır. Onay satırı xmin ile
// yazar; iş süresi dolan tutmaları FOR UPDATE SKIP LOCKED ile seçer. İki sıra da gerçek iki transaction'la sınanır: onay
// önce yazarsa iş satırı atlar; iş önce kilitlerse onayın UPDATE'i bekler ve commit sonrası eşzamanlılık hatası (412) alır.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class ApprovalRaceTests(PostgresFixture db) : IAsyncLifetime
{
    private static readonly Duration Timeout = Duration.FromMinutes(120);

    private ServiceProvider _services = null!;
    private AvailabilityData _data = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private FakeClock Clock => (FakeClock)_services.GetRequiredService<IClock>();

    public async ValueTask InitializeAsync()
    {
        _services = AvailabilityData.CreateServices(db);
        _data = await AvailabilityData.SeedAsync(db, _services, Ct);
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand("TRUNCATE infra.outbox_events CASCADE", connection);
        await command.ExecuteNonQueryAsync(Ct);
    }

    public async ValueTask DisposeAsync() => await _services.DisposeAsync();

    [Fact]
    public async Task ApproveWritesFirst_JobSkipsTheLockedRow_ApprovalWins()
    {
        var held = await CreatePendingAsync();
        await using var approveScope = _services.CreateAsyncScope();
        var approveSession = await BeginAsync(approveScope, TenantScope.ForBrand(_data.BrandId));
        Clock.Advance(Timeout - Duration.FromMinutes(1));
        var approved = await approveScope.ServiceProvider.GetRequiredService<AppointmentApprovals>()
            .ApproveAsync(held.Id, OwnerAccess(), userId: null, held.Version, waiveDeposit: false, Ct);
        approved.IsSuccess.ShouldBeTrue();

        // Onay henüz commit edilmedi; saat dolmuş görünse de iş kilitli satırı atlar.
        Clock.Advance(Duration.FromMinutes(2));
        (await _services.GetRequiredService<JobRunner>().RunOnceAsync(typeof(ExpireAppointmentHolds), Ct)).ShouldBe(JobOutcome.Succeeded);
        await approveSession.CommitAsync(Ct);

        var final = await LoadAsync(held.Id);
        final.Status.ShouldBe(AppointmentStatus.Confirmed);
        (await OutboxCountAsync("AppointmentApprovalExpired")).ShouldBe(0);
        (await OutboxCountAsync("AppointmentApproved")).ShouldBe(1);
    }

    [Fact]
    public async Task JobLocksFirst_ParallelApproveBlocks_ThenFailsWithConcurrency_ExpiryWins()
    {
        var held = await CreatePendingAsync();
        Clock.Advance(Timeout);
        await using var jobScope = _services.CreateAsyncScope();
        var jobSession = await BeginAsync(jobScope, TenantScope.Platform);
        await jobScope.ServiceProvider.GetRequiredService<ExpireAppointmentHolds>().RunAsync(Ct);

        // Onay isteği tutmadan önceki bir anda gelmiş gibi (saat geri): bellekte geçerli, UPDATE iş commit edene kadar bekler.
        Clock.Reset(Clock.GetCurrentInstant() - Duration.FromMinutes(1));
        var approve = Task.Run(
            async () =>
            {
                await using var scope = _services.CreateAsyncScope();
                var session = await BeginAsync(scope, TenantScope.ForBrand(_data.BrandId));
                try
                {
                    return await scope.ServiceProvider.GetRequiredService<AppointmentApprovals>()
                        .ApproveAsync(held.Id, OwnerAccess(), userId: null, held.Version, waiveDeposit: false, Ct);
                }
                finally
                {
                    await session.RollbackAsync(Ct);
                }
            },
            Ct);
        await WaitForLockWaitAsync();
        await jobSession.CommitAsync(Ct);

        await Should.ThrowAsync<DbUpdateConcurrencyException>(() => approve.WaitAsync(TimeSpan.FromSeconds(30), Ct));
        var final = await LoadAsync(held.Id);
        final.Status.ShouldBe(AppointmentStatus.Cancelled);
        final.CancellationReason.ShouldBe(CancellationReason.ApprovalExpired);
        (await OutboxCountAsync("AppointmentApprovalExpired")).ShouldBe(1);
        (await OutboxCountAsync("AppointmentApproved")).ShouldBe(0);
    }

    private async Task<Appointment> CreatePendingAsync()
    {
        await using var scope = _services.CreateAsyncScope();
        var session = await BeginAsync(scope, TenantScope.ForBrand(_data.BrandId));
        var slot = new SlotRequest(_data.Merkez, At(Thursday, 15, 0), [new RequestedItem(_data.Haircut, _data.Elif)], SameStaffForAll: false, AvailabilityMode.Public);
        var written = await scope.ServiceProvider.GetRequiredService<AppointmentWriter>().CreateAsync(
            new CreateAppointmentCommand(
                _data.BrandId, slot, Guid.Parse("0193a8f0-0000-7000-8000-00000000cc01"), Guid.Parse("0193a8f0-0000-7000-8000-00000000cc02"),
                AppointmentChannel.Online, ClientApp.BookingPage, null, null, null, null, new ApprovalPolicy(BrandRequiresApproval: true, Timeout)),
            Ct);
        await session.CommitAsync(Ct);
        written.Value.Appointment.Status.ShouldBe(AppointmentStatus.PendingApproval);
        return written.Value.Appointment;
    }

    private static async Task<IDbSession> BeginAsync(AsyncServiceScope scope, TenantScope tenant)
    {
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(tenant, Ct);
        await session.BeginAsync(Ct);
        return session;
    }

    private static ManageAccess OwnerAccess() =>
        new(BranchScope.From(new ClaimsPrincipal(new ClaimsIdentity([new Claim(OmvilaClaims.BranchIds, OmvilaClaims.AllBranches)]))), false, null, true, true);

    /// <summary>Onay UPDATE'i iş transaction'ının satır kilidini bekleyene kadar (pg_locks: verilmemiş kilit).</summary>
    private async Task WaitForLockWaitAsync()
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        for (var i = 0; i < 200; i++)
        {
            await using var command = new NpgsqlCommand("SELECT count(*) FROM pg_locks WHERE NOT granted", connection);
            if ((long)(await command.ExecuteScalarAsync(Ct))! > 0)
            {
                return;
            }

            await Task.Delay(50, Ct);
        }

        throw new TimeoutException("Onay isteği satır kilidini beklemeye başlamadı.");
    }

    private async Task<Appointment> LoadAsync(Guid id) =>
        await _data.InBrandAsync(sp => sp.GetRequiredService<SchedulingDbContext>().Appointments.AsNoTracking().SingleAsync(a => a.Id == id, Ct), Ct);

    private async Task<long> OutboxCountAsync(string type)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand($"SELECT count(*) FROM infra.outbox_events WHERE type LIKE '%.{type}'", connection);
        return (long)(await command.ExecuteScalarAsync(Ct))!;
    }
}
