using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Features.Appointments;
using Omvila.SharedKernel;
using Omvila.TestKit;
using static Omvila.Modules.Scheduling.IntegrationTests.AvailabilityData;

namespace Omvila.Modules.Scheduling.IntegrationTests;

// Kural: durum-makinesi.md § Oluşturma kuralı (Online + kapora → PendingApproval, şimdi + TransferWindowMinutes; kapora ve
// onay birlikte tek süre; panel kapora istemez), alan-modeli.md § Appointment.DepositTransferCode (marka içinde UNIQUE;
// çakışmada yazma yolu yeni kod üretir). Kalem kaporası Catalog'dan gelir; burada sahte çözümle (her kalem 300 ₺) yalnız
// yazma yolu sınanır. Çözümün kendisi Catalog birim testlerinde ve uç testlerinde.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class DepositHoldTests(PostgresFixture db) : IAsyncLifetime
{
    private static readonly Duration Window = Duration.FromMinutes(240);

    private readonly SequenceTransferCodes _codes = new();
    private ServiceProvider _services = null!;
    private AvailabilityData _data = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private Instant Now => _services.GetRequiredService<IClock>().GetCurrentInstant();

    public async ValueTask InitializeAsync()
    {
        _services = AvailabilityData.CreateServices(db, configure: s =>
        {
            s.Replace(ServiceDescriptor.Scoped<IServiceDeposits, FixedDeposits>());
            s.Replace(ServiceDescriptor.Singleton<TransferCodeGenerator>(_codes));
        });
        _data = await AvailabilityData.SeedAsync(db, _services, Ct);
    }

    public async ValueTask DisposeAsync() => await _services.DisposeAsync();

    [Fact]
    public async Task Online_WithDeposit_IsPending_HoldIsTransferWindow_SumOfItems()
    {
        var created = await CreateAsync(At(Thursday, 15, 0), AppointmentChannel.Online, approval: null);

        var appointment = created.Value.Appointment;
        appointment.Status.ShouldBe(AppointmentStatus.PendingApproval);
        appointment.DisplayStatus.ShouldBe(DisplayStatus.DepositPending);
        appointment.HoldReason.ShouldBe(HoldReason.Deposit);
        appointment.HoldExpiresUtc.ShouldBe(Now + Window);
        appointment.DepositRequiredMinor.ShouldBe(30_000);
        appointment.DepositStatus.ShouldBe(DepositStatus.Awaiting);
        appointment.DepositTransferCode.ShouldBe("OMV-2222");
        appointment.AmountDueMinor.ShouldBe(appointment.TotalAmountMinor, "kapora gelmeden salonda ödenecek toplamdır");
    }

    [Fact]
    public async Task Online_WithDepositAndApproval_SingleHold_IsTransferWindow()
    {
        var created = await CreateAsync(At(Thursday, 15, 0), AppointmentChannel.Online, approval: Duration.FromMinutes(120));

        created.Value.Appointment.HoldReason.ShouldBe(HoldReason.ApprovalAndDeposit);
        created.Value.Appointment.HoldExpiresUtc.ShouldBe(Now + Window, "onay süresi değil havale süresi");
    }

    [Fact]
    public async Task PanelChannel_NoDeposit_EvenWithTerms()
    {
        var created = await CreateAsync(At(Thursday, 15, 0), AppointmentChannel.Panel, approval: null);

        created.Value.Appointment.Status.ShouldBe(AppointmentStatus.Confirmed);
        created.Value.Appointment.DepositRequiredMinor.ShouldBeNull();
        created.Value.Appointment.DepositTransferCode.ShouldBeNull();
    }

    [Fact]
    public async Task TransferCode_TakenInBrand_IsRegenerated_OnSave()
    {
        _codes.Enqueue("OMV-2222", "OMV-2222", "OMV-3333");

        var first = await CreateAsync(At(Thursday, 15, 0), AppointmentChannel.Online, approval: null);
        var second = await CreateAsync(At(Thursday, 17, 0), AppointmentChannel.Online, approval: null);

        first.Value.Appointment.DepositTransferCode.ShouldBe("OMV-2222");
        second.IsSuccess.ShouldBeTrue(second.IsSuccess ? string.Empty : second.Error.Code);
        second.Value.Appointment.DepositTransferCode.ShouldBe("OMV-3333", "çakışan kod yeniden üretildi");
        (await _data.InBrandAsync(
            async sp => await sp.GetRequiredService<Infrastructure.SchedulingDbContext>().Appointments
                .CountAsync(a => a.DepositTransferCode != null, Ct), Ct)).ShouldBe(2);
    }

    [Fact]
    public void Generator_Format_PrefixDashFourUnambiguousCharacters()
    {
        var codes = Enumerable.Range(0, 500).Select(_ => new TransferCodeGenerator().New("KE")).ToList();

        codes.ShouldAllBe(c => c.Length == 7 && c.StartsWith("KE-", StringComparison.Ordinal));
        codes.SelectMany(c => c[3..]).ShouldAllBe(ch => TransferCodeGenerator.Alphabet.Contains(ch));
        TransferCodeGenerator.Alphabet.ShouldNotContain('0');
        TransferCodeGenerator.Alphabet.ShouldNotContain('O');
        TransferCodeGenerator.Alphabet.ShouldNotContain('1');
        TransferCodeGenerator.Alphabet.ShouldNotContain('I');
        new TransferCodeGenerator().New("ABCDE").Length.ShouldBeLessThanOrEqualTo(TransferCodeGenerator.MaxLength);
    }

    private async Task<Result<AppointmentWriteResult>> CreateAsync(Instant start, AppointmentChannel channel, Duration? approval)
    {
        await using var scope = _services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.ForBrand(_data.BrandId), Ct);
        await session.BeginAsync(Ct);
        var mode = channel == AppointmentChannel.Online ? AvailabilityMode.Public : AvailabilityMode.Panel;
        var slot = new SlotRequest(_data.Merkez, start, [new RequestedItem(_data.Haircut, _data.Elif)], SameStaffForAll: false, mode);
        var result = await scope.ServiceProvider.GetRequiredService<AppointmentWriter>().CreateAsync(
            new CreateAppointmentCommand(
                _data.BrandId,
                slot,
                CustomerId: Guid.Parse("0193a8f0-0000-7000-8000-00000000dd01"),
                BrandCustomerId: Guid.Parse("0193a8f0-0000-7000-8000-00000000dd02"),
                channel,
                channel == AppointmentChannel.Online ? ClientApp.BookingPage : ClientApp.Panel,
                CustomerNote: null,
                StaffNote: null,
                SeriesId: null,
                CreatedByUserId: null,
                approval is { } timeout ? new ApprovalPolicy(BrandRequiresApproval: true, timeout) : null,
                new DepositTerms(CustomerNoShowCount: 2, "OMV", Window)),
            Ct);
        if (result.IsSuccess)
        {
            await session.CommitAsync(Ct);
        }
        else
        {
            await session.RollbackAsync(Ct);
        }

        return result;
    }

    /// <summary>Her kaleme 300 ₺ kapora (kanal Online ise).</summary>
    private sealed class FixedDeposits : IServiceDeposits
    {
        public Task<IReadOnlyList<Money?>> ResolveAsync(Guid brandId, ServiceDepositRequest request, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<Money?>>([.. request.Items.Select(_ => request.IsOnlineChannel ? Money.Try(30_000) : (Money?)null)]);
    }

    /// <summary>Sıradaki kodları verir; sıra boşsa "OMV-2222".</summary>
    private sealed class SequenceTransferCodes : TransferCodeGenerator
    {
        private readonly ConcurrentQueue<string> _queue = new();

        public void Enqueue(params string[] codes)
        {
            foreach (var code in codes)
            {
                _queue.Enqueue(code);
            }
        }

        public override string New(string prefix) => _queue.TryDequeue(out var code) ? code : $"{prefix}-2222";
    }
}
