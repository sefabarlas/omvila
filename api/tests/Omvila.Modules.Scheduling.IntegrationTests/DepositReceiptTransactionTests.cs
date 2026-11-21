using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NodaTime;
using NodaTime.Testing;
using Npgsql;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Sales;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Features;
using Omvila.Modules.Scheduling.Features.Appointments;
using Omvila.Modules.Scheduling.Features.Deposits;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Scheduling.Jobs;
using Omvila.SharedKernel;
using Omvila.TestKit;
using static Omvila.Modules.Scheduling.IntegrationTests.AvailabilityData;

namespace Omvila.Modules.Scheduling.IntegrationTests;

// Kural: mimari.md § Modüller arası iletişim ("Tek bağlantı, tek transaction": aynı transaction'da olması zorunlu iş senkron
// Contracts çağrısıyla); durum-makinesi.md A3. "Kapora geldi" tahsilatı Sales'e (IDepositPayments) yazar, sonra randevuyu
// günceller. Randevunun yazımı başarısız olursa tahsilat da geri alınmalı: gerçek iki transaction'la, otomatik iptal işi
// satırı önce kilitler; işaretleme tahsilatı yazdıktan sonra UPDATE'te bekler ve iş commit edince eşzamanlılık hatası (412) alır.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class DepositReceiptTransactionTests(PostgresFixture db) : IAsyncLifetime
{
    private static readonly Duration Window = Duration.FromMinutes(120);

    private ServiceProvider _services = null!;
    private AvailabilityData _data = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private FakeClock Clock => (FakeClock)_services.GetRequiredService<IClock>();

    public async ValueTask InitializeAsync()
    {
        _services = AvailabilityData.CreateServices(db, configure: s =>
        {
            s.AddSalesModule();
            s.Replace(ServiceDescriptor.Scoped<IServiceDeposits, FixedDeposits>());
        });
        _data = await AvailabilityData.SeedAsync(db, _services, Ct);
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand("TRUNCATE infra.outbox_events CASCADE; TRUNCATE sales.payment_allocations, sales.payments;", connection);
        await command.ExecuteNonQueryAsync(Ct);
    }

    public async ValueTask DisposeAsync() => await _services.DisposeAsync();

    [Fact]
    public async Task MarkReceived_PaymentAndAppointment_ShareOneTransaction_CommitTogether()
    {
        var held = await CreateDepositPendingAsync();
        await using var scope = _services.CreateAsyncScope();
        var session = await BeginAsync(scope, TenantScope.ForBrand(_data.BrandId));
        var schedulingTx = scope.ServiceProvider.GetRequiredService<SchedulingDbContext>().Database.CurrentTransaction!.GetDbTransaction();

        var marked = await scope.ServiceProvider.GetRequiredService<DepositReceipts>()
            .MarkReceivedAsync(held.Id, OwnerAccess(), userId: null, held.Version, Ct);

        marked.IsSuccess.ShouldBeTrue();
        schedulingTx.ShouldBeSameAs(session.Transaction);
        // Tahsilat Sales DbContext'iyle yazıldı; oturumun transaction'ı onu commit'ten önce görür, başka bağlantı görmez.
        await using (var inSession = new NpgsqlCommand("SELECT count(*) FROM sales.payments", session.Connection, session.Transaction))
        {
            ((long)(await inSession.ExecuteScalarAsync(Ct))!).ShouldBe(1);
        }

        (await PaymentCountAsync()).ShouldBe(0, "commit edilmeden başka bağlantı görmez");
        await session.CommitAsync(Ct);

        (await PaymentCountAsync()).ShouldBe(1);
        (await LoadAsync(held.Id)).DepositPaymentId.ShouldNotBeNull();
    }

    [Fact]
    public async Task JobExpiresFirst_MarkReceivedFailsAfterRecordingPayment_PaymentRolledBack()
    {
        var held = await CreateDepositPendingAsync();
        Clock.Advance(Window);
        await using var jobScope = _services.CreateAsyncScope();
        var jobSession = await BeginAsync(jobScope, TenantScope.Platform);
        await jobScope.ServiceProvider.GetRequiredService<ExpireAppointmentHolds>().RunAsync(Ct);

        // İşaretleme tutmadan önceki bir anda gelmiş gibi: bellekte geçerli, tahsilatı yazar, randevu UPDATE'i iş commit edene kadar bekler.
        Clock.Reset(Clock.GetCurrentInstant() - Duration.FromMinutes(1));
        var mark = Task.Run(
            async () =>
            {
                await using var scope = _services.CreateAsyncScope();
                var session = await BeginAsync(scope, TenantScope.ForBrand(_data.BrandId));
                try
                {
                    return await scope.ServiceProvider.GetRequiredService<DepositReceipts>()
                        .MarkReceivedAsync(held.Id, OwnerAccess(), userId: null, held.Version, Ct);
                }
                finally
                {
                    // İstek hattında TransactionEndpointFilter istisnada geri alır.
                    await session.RollbackAsync(Ct);
                }
            },
            Ct);
        await WaitForLockWaitAsync();
        await jobSession.CommitAsync(Ct);

        await Should.ThrowAsync<DbUpdateConcurrencyException>(() => mark.WaitAsync(TimeSpan.FromSeconds(30), Ct));
        (await PaymentCountAsync()).ShouldBe(0, "randevu yazılamadı; aynı transaction'daki tahsilat da geri alındı");
        var final = await LoadAsync(held.Id);
        final.Status.ShouldBe(AppointmentStatus.Cancelled);
        final.CancellationReason.ShouldBe(CancellationReason.DepositNotReceived);
        final.DepositPaymentId.ShouldBeNull();
    }

    private async Task<Appointment> CreateDepositPendingAsync()
    {
        await using var scope = _services.CreateAsyncScope();
        var session = await BeginAsync(scope, TenantScope.ForBrand(_data.BrandId));
        var slot = new SlotRequest(_data.Merkez, At(Thursday, 15, 0), [new RequestedItem(_data.Haircut, _data.Elif)], SameStaffForAll: false, AvailabilityMode.Public);
        var written = await scope.ServiceProvider.GetRequiredService<AppointmentWriter>().CreateAsync(
            new CreateAppointmentCommand(
                _data.BrandId, slot, Guid.Parse("0193a8f0-0000-7000-8000-00000000ee01"), Guid.Parse("0193a8f0-0000-7000-8000-00000000ee02"),
                AppointmentChannel.Online, ClientApp.BookingPage, null, null, null, null, null, new DepositTerms(CustomerNoShowCount: 2, "OMV", Window)),
            Ct);
        await session.CommitAsync(Ct);
        written.Value.Appointment.DisplayStatus.ShouldBe(DisplayStatus.DepositPending);
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

    /// <summary>İşaretlemenin UPDATE'i iş transaction'ının satır kilidini bekleyene kadar (pg_locks: verilmemiş kilit).</summary>
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

        throw new TimeoutException("İşaretleme satır kilidini beklemeye başlamadı.");
    }

    private async Task<long> PaymentCountAsync()
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand("SELECT count(*) FROM sales.payments", connection);
        return (long)(await command.ExecuteScalarAsync(Ct))!;
    }

    private async Task<Appointment> LoadAsync(Guid id) =>
        await _data.InBrandAsync(sp => sp.GetRequiredService<SchedulingDbContext>().Appointments.AsNoTracking().SingleAsync(a => a.Id == id, Ct), Ct);

    /// <summary>Her kaleme 300 ₺ kapora (kanal Online ise).</summary>
    private sealed class FixedDeposits : IServiceDeposits
    {
        public Task<IReadOnlyList<Money?>> ResolveAsync(Guid brandId, ServiceDepositRequest request, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<Money?>>([.. request.Items.Select(_ => request.IsOnlineChannel ? Money.Try(30_000) : (Money?)null)]);
    }
}
