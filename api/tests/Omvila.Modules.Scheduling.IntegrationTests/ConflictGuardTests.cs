using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Testing;
using Npgsql;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Features.Appointments;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Scheduling.Infrastructure.ConflictGuard;
using Omvila.SharedKernel;
using Omvila.TestKit;
using static Omvila.Modules.Scheduling.IntegrationTests.AvailabilityData;

namespace Omvila.Modules.Scheduling.IntegrationTests;

// Kural: musaitlik.md § Çakışma engeli: üç katman (katman 1 exclusion constraint, katman 2 sıralı advisory lock + kilit
// altında yeniden hesap, lock_timeout → 503 try_again), § Saat değişikliği, § Panelden elle ekleme ve zorunlu bütünleşik
// testler C1, C2, C4, C5, C7 (C3 kaynak tabloları gelince, C6 idempotency uç parçasında). Her paralel istek kendi DI
// kapsamında, kendi bağlantısı ve transaction'ıyla çalışır; istekler aynı anda başlasın diye kapıda bekler.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class ConflictGuardTests(PostgresFixture db) : IAsyncLifetime
{
    private static readonly string[] ExpectedAlternatives = ["2026-11-26T11:15:00Z", "2026-11-26T12:45:00Z"];

    private ServiceProvider _services = null!;
    private AvailabilityData _data = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private FakeClock Clock => (FakeClock)_services.GetRequiredService<IClock>();

    public async ValueTask InitializeAsync()
    {
        _services = AvailabilityData.CreateServices(db);
        _data = await AvailabilityData.SeedAsync(db, _services, Ct);

        // Mert Nişantaşı'nda da 10:00–20:00 çalışır (C2'nin iki şubesi de çalışma saati içinde olsun).
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand(
            """
            INSERT INTO tenancy.staff_working_hours (id, brand_id, staff_branch_id, day_of_week, start_time, end_time)
              SELECT gen_random_uuid(), sb.brand_id, sb.id, d, '10:00', '20:00'
              FROM tenancy.staff_branches sb, generate_series(1, 6) d
              WHERE sb.staff_id = $1 AND sb.branch_id = $2;
            """,
            connection);
        command.Parameters.Add(new NpgsqlParameter { Value = _data.Mert });
        command.Parameters.Add(new NpgsqlParameter { Value = _data.Nisantasi });
        await command.ExecuteNonQueryAsync(Ct);
    }

    public async ValueTask DisposeAsync() => await _services.DisposeAsync();

    [Fact]
    public async Task C1_TwentyParallelRequests_SameSlot_ExactlyOneSucceeds()
    {
        var start = At(Thursday, 15, 0);

        var results = await ParallelAsync(Enumerable.Range(0, 20).Select(_ => Create(Public(start, (_data.Haircut, _data.Elif)))));

        results.Count(r => r.IsSuccess).ShouldBe(1);
        results.Where(r => !r.IsSuccess).ShouldAllBe(r => r.Error!.Code == ErrorCodes.SlotTaken);
        results.Where(r => !r.IsSuccess).Count().ShouldBe(19);
        (await ActiveSegmentCountAsync(_data.Elif)).ShouldBe(1);
    }

    [Fact]
    public async Task C1_AnyStaff_TwentyParallelRequests_AreSpreadAcrossFreeStaff_NoDoubleBooking()
    {
        var start = At(Thursday, 15, 0);

        var results = await ParallelAsync(Enumerable.Range(0, 20).Select(_ => Create(Public(start, (_data.Haircut, null)))));

        var succeeded = results.Where(r => r.IsSuccess).Select(r => r.Value.Appointment.Items[0].StaffId!.Value).ToList();
        succeeded.Count.ShouldBe(StaffCount, "12 personel, her biri bir kez");
        succeeded.ShouldBeUnique();
        results.Where(r => !r.IsSuccess).ShouldAllBe(r => r.Error!.Code == ErrorCodes.SlotTaken);
    }

    [Fact]
    public async Task C2_SameStaff_TwoBranches_SameTime_Parallel_ExactlyOneSucceeds()
    {
        var start = At(Thursday, 15, 0);

        var results = await ParallelAsync(
        [
            Create(Public(start, (_data.Haircut, _data.Mert)) with { BranchId = _data.Merkez }),
            Create(Public(start, (_data.Haircut, _data.Mert)) with { BranchId = _data.Nisantasi }),
        ]);

        results.Count(r => r.IsSuccess).ShouldBe(1);
        results.Single(r => !r.IsSuccess).Error!.Code.ShouldBe(ErrorCodes.SlotTaken);
    }

    [Fact]
    public async Task C2_TransferBufferBetweenBranches_IsProtectedByTheLock_NotOnlyByTheConstraint()
    {
        // 15:00–15:45 Merkez ve 15:45 Nişantaşı dilimleri örtüşmez (kısıt geçirir) ama 30 dk geçiş süresi gerekir.
        var results = await ParallelAsync(
        [
            Create(Public(At(Thursday, 15, 0), (_data.Haircut, _data.Mert)) with { BranchId = _data.Merkez }),
            Create(Public(At(Thursday, 15, 45), (_data.Haircut, _data.Mert)) with { BranchId = _data.Nisantasi }),
        ]);

        results.Count(r => r.IsSuccess).ShouldBe(1);
        results.Single(r => !r.IsSuccess).Error!.Code.ShouldBe(ErrorCodes.SlotTaken);
    }

    [Fact]
    public async Task C4_CancelledAppointmentsSlot_IsImmediatelyBookable()
    {
        var slot = Public(At(Thursday, 15, 0), (_data.Haircut, _data.Elif));
        var first = (await InRequestAsync(Create(slot))).Value.Appointment;

        (await InRequestAsync(Create(slot))).Error!.Code.ShouldBe(ErrorCodes.SlotTaken);
        await InRequestAsync(async sp =>
        {
            var context = sp.GetRequiredService<SchedulingDbContext>();
            var appointment = await context.Appointments.Include(a => a.Items).ThenInclude(i => i.Segments).SingleAsync(a => a.Id == first.Id, Ct);
            var cancelled = appointment.CancelBySalon(CancellationReason.CustomerRequested, null, null, Clock.GetCurrentInstant());
            await context.SaveChangesAsync(Ct);
            return cancelled.IsSuccess ? Result<bool>.Success(true) : cancelled.Error;
        });

        (await InRequestAsync(Create(slot))).IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task C5_Reschedule_OverlappingItsOwnOldTime_Succeeds_OldSegmentsSuperseded()
    {
        var created = (await InRequestAsync(Create(Public(At(Thursday, 15, 0), (_data.Haircut, _data.Elif))))).Value.Appointment;

        var moved = await InRequestAsync(sp => sp.GetRequiredService<AppointmentWriter>().RescheduleAsync(
            new RescheduleAppointmentCommand(created.Id, At(Thursday, 15, 30), ChangeActor.Salon, Duration.FromHours(12), UserId: null),
            Ct));

        moved.IsSuccess.ShouldBeTrue();
        var segments = await InRequestAsync(async sp => Result<List<AppointmentItemSegment>>.Success(
            await sp.GetRequiredService<SchedulingDbContext>().AppointmentItemSegments.AsNoTracking()
                .Where(s => s.AppointmentItemId == created.Items[0].Id).ToListAsync(Ct)));
        segments.Value.Single(s => s.IsActive).During.ShouldBe(new Interval(At(Thursday, 15, 30), At(Thursday, 16, 15)));
        segments.Value.Single(s => !s.IsActive).SupersededUtc.ShouldNotBeNull();
    }

    [Fact]
    public async Task C5_Reschedule_OntoAnotherAppointment_IsSlotTaken_AndCanChangeStaff()
    {
        await InRequestAsync(Create(Public(At(Thursday, 16, 0), (_data.Haircut, _data.Elif))));
        var created = (await InRequestAsync(Create(Public(At(Thursday, 15, 0), (_data.Haircut, _data.Elif))))).Value.Appointment;

        Task<Result<AppointmentWriteResult>> Move(IReadOnlyList<ItemStaffChange>? changes) => InRequestAsync(sp => sp.GetRequiredService<AppointmentWriter>().RescheduleAsync(
            new RescheduleAppointmentCommand(created.Id, At(Thursday, 16, 0), ChangeActor.Salon, Duration.FromHours(12), null, changes), Ct));

        (await Move(null)).Error!.Code.ShouldBe(ErrorCodes.SlotTaken);
        var moved = await Move([new ItemStaffChange(created.Items[0].Id, _data.Mert)]);
        moved.Value.Appointment.Items[0].StaffId.ShouldBe(_data.Mert);
    }

    [Fact]
    public async Task C7_LockOrdering_CrossedMultiItemAppointments_InParallel_NoDeadlock_AllComplete()
    {
        var a = _data.Staff[2];
        var b = _data.Staff[3];

        // Her turda biri A→B, diğeri B→A; saatleri örtüşmez, ikisi de başarılı olmalı (kilit sırası aynı: A, B).
        var requests = Enumerable.Range(0, 5).SelectMany(round =>
        {
            var start = At(Thursday, 10 + (round * 2), 0);
            return new[]
            {
                Create(Panel(start, (_data.Haircut, a), (_data.Haircut, b))),
                Create(Panel(start, (_data.Haircut, b), (_data.Haircut, a))),
            };
        });

        var results = await ParallelAsync(requests);

        results.ShouldAllBe(r => r.IsSuccess);
    }

    [Fact]
    public async Task LockTimeout_IsTryAgain_AndTheTransactionStaysUsable()
    {
        await using var holder = new NpgsqlConnection(db.AppConnectionString);
        await holder.OpenAsync(Ct);
        await using var hold = await holder.BeginTransactionAsync(Ct);
        await using (var command = new NpgsqlCommand("SELECT pg_advisory_xact_lock(hashtext($1))", holder, hold))
        {
            command.Parameters.Add(new NpgsqlParameter { Value = AppointmentLocks.StaffKey(_data.Elif, Thursday) });
            await command.ExecuteNonQueryAsync(Ct);
        }

        var (timedOut, next) = await InRequestPairAsync(async writer =>
        {
            var first = await writer.CreateAsync(Command(Public(At(Thursday, 15, 0), (_data.Haircut, _data.Elif))), Ct);
            var second = await writer.CreateAsync(Command(Public(At(Thursday, 15, 0), (_data.Haircut, _data.Mert))), Ct);
            return (first, second);
        });

        timedOut.Error!.Code.ShouldBe(ErrorCodes.TryAgain);
        timedOut.Error.Kind.ShouldBe(ErrorKind.Unavailable);
        next.IsSuccess.ShouldBeTrue("kilit denemesi savepoint'te geri alındı");
        (await ActiveSegmentCountAsync(_data.Mert)).ShouldBe(1);
    }

    [Fact]
    public async Task Panel_OutsideWorkingHours_NeedsConfirmation_ButNeverOverridesAConflict()
    {
        var evening = At(Thursday, 20, 30);

        (await InRequestAsync(Create(Panel(evening, (_data.Haircut, _data.Elif))))).Error!.Code.ShouldBe(ErrorCodes.OutsideWorkingHours);
        var confirmed = await InRequestAsync(Create(Panel(evening, (_data.Haircut, _data.Elif)) with { AllowOutsideWorkingHours = true }));
        confirmed.Value.AcceptedWarnings.ShouldBe(SlotViolation.OutsideWorkingHours);
        confirmed.Value.Appointment.Status.ShouldBe(AppointmentStatus.Confirmed);

        (await InRequestAsync(Create(Panel(evening, (_data.Haircut, _data.Elif)) with { AllowOutsideWorkingHours = true })))
            .Error!.Code.ShouldBe(ErrorCodes.SlotTaken, "çakışma ikinci onayla geçilemez");
        (await InRequestAsync(Create(Public(At(Thursday, 20, 30), (_data.Haircut, _data.Mert)) with { AllowOutsideWorkingHours = true })))
            .Error!.Code.ShouldBe(ErrorCodes.SlotTaken, "rezervasyon sayfası uyarı geçemez");
    }

    [Fact]
    public async Task Panel_StaffOnTimeOff_NeedsConfirmation()
    {
        var friday = At(Thursday.PlusDays(1), 11, 0);

        (await InRequestAsync(Create(Panel(friday, (_data.Haircut, _data.Elif))))).Error!.Code.ShouldBe(ErrorCodes.StaffOnTimeOff);
        (await InRequestAsync(Create(Panel(friday, (_data.Haircut, _data.Elif)) with { AllowOutsideWorkingHours = true })))
            .Value.AcceptedWarnings.ShouldBe(SlotViolation.StaffOnTimeOff);
    }

    [Fact]
    public async Task Create_WritesItemsSegmentsAndAccessCode_PublicWindowAndGridAreValidated()
    {
        var result = (await InRequestAsync(Create(Public(At(Thursday, 15, 0), (_data.Haircut, null))))).Value;

        result.AccessCode!.AppointmentId.ShouldBe(result.Appointment.Id);
        result.AccessCode.ExpiresUtc.ShouldBe(AppointmentAccessCode.ExpiryFor(At(Thursday, 15, 45)));
        result.Appointment.Items[0].Segments.ShouldHaveSingleItem().StaffId.ShouldBe(result.Appointment.Items[0].StaffId);
        result.Appointment.TotalAmountMinor.ShouldBe(45_000);

        (await InRequestAsync(Create(Public(At(Thursday, 15, 5), (_data.Haircut, _data.Elif))))).Error!.FieldErrors["startUtc"].ShouldBe(["invalid_format"]);
        (await InRequestAsync(Create(Public(TestClock.Start + Duration.FromMinutes(30), (_data.Haircut, _data.Elif))))).Error!.Code
            .ShouldBe(ErrorCodes.SlotTaken, "en az ön bildirim geçti: saat artık sunulmuyor");
        (await InRequestAsync(Create(Public(At(Thursday, 15, 0), (_data.Color, _data.Mert))))).Error!.FieldErrors["items"].ShouldBe(["out_of_range"]);
        (await InRequestAsync(Create(Public(At(Thursday, 15, 0), (_data.Haircut, _data.Elif)) with { BranchId = Guid.Parse("0193a8f0-0000-7000-8000-0000000000ff") })))
            .Error!.Code.ShouldBe(ErrorCodes.NotFound);
    }

    [Fact]
    public async Task M17_ExpiredHold_IsCancelledOnTheWritePath_AndTheSlotIsTaken()
    {
        var slot = Public(At(Thursday, 15, 0), (_data.Haircut, _data.Elif));
        var held = (await InRequestAsync(Create(slot, approvalTimeout: Duration.FromMinutes(120)))).Value.Appointment;
        held.Status.ShouldBe(AppointmentStatus.PendingApproval);
        (await InRequestAsync(Create(slot))).Error!.Code.ShouldBe(ErrorCodes.SlotTaken, "tutma henüz geçerli");

        Clock.Advance(Duration.FromMinutes(120));

        (await InRequestAsync(Create(slot))).IsSuccess.ShouldBeTrue();
        var expired = await InRequestAsync(async sp => Result<Appointment>.Success(
            await sp.GetRequiredService<SchedulingDbContext>().Appointments.AsNoTracking().SingleAsync(a => a.Id == held.Id, Ct)));
        expired.Value.Status.ShouldBe(AppointmentStatus.Cancelled);
        expired.Value.CancelledBy.ShouldBe(CancelledBy.System);
        expired.Value.CancellationReason.ShouldBe(CancellationReason.ApprovalExpired);
    }

    [Fact]
    public async Task Alternatives_AreTheNearestFreeSlotsOfTheSameDay()
    {
        var start = At(Thursday, 15, 0);
        await InRequestAsync(Create(Public(start, (_data.Haircut, _data.Elif))));

        var alternatives = await InRequestAsync(async sp => Result<IReadOnlyList<Instant>>.Success(
            await sp.GetRequiredService<AppointmentWriter>().SuggestAlternativesAsync(_data.BrandId, Public(start, (_data.Haircut, _data.Elif)), 2, null, null, Ct)));

        alternatives.Value.ShouldBe([At(Thursday, 14, 15), At(Thursday, 15, 45)]);
        AppointmentWriter.AlternativesMeta(alternatives.Value)["alternatives"].ShouldBe(ExpectedAlternatives);
    }

    [Fact]
    public async Task Constraint_RejectsADirectOverlappingInsert_ButNotWaitingOrInactiveSegments()
    {
        var start = At(Thursday, 15, 0);
        await InsertDirectAsync(_data.Elif, start);

        var overlap = await Should.ThrowAsync<DbUpdateException>(() => InsertDirectAsync(_data.Elif, start + Duration.FromMinutes(30)));
        overlap.InnerException.ShouldBeOfType<PostgresException>().SqlState.ShouldBe(PostgresErrorCodes.ExclusionViolation);
        ((PostgresException)overlap.InnerException!).ConstraintName.ShouldBe("ex_segment_staff_overlap");

        await InsertDirectAsync(_data.Elif, start + Duration.FromMinutes(45)); // 15:45'te biten ile başlayan çakışmaz (M1).
        await InsertDirectAsync(_data.Mert, start); // başka personel
        await InsertDirectAsync(_data.Elif, start, cancelled: true); // pasif dilim kapsam dışı
    }

    [Fact]
    public async Task Constraint_Violation_RollsBackToTheSaveChangesSavepoint_TransactionStaysUsable()
    {
        var start = At(Thursday, 15, 0);
        await InsertDirectAsync(_data.Elif, start);

        await InRequestAsync(async sp =>
        {
            var context = sp.GetRequiredService<SchedulingDbContext>();
            var ids = sp.GetRequiredService<IIdGenerator>();
            var conflicting = NewHaircut(ids, _data.Elif, start);
            context.Appointments.Add(conflicting);
            await Should.ThrowAsync<DbUpdateException>(() => context.SaveChangesAsync(Ct));
            foreach (var entry in context.ChangeTracker.Entries().ToList())
            {
                entry.State = EntityState.Detached;
            }

            context.Appointments.Add(NewHaircut(ids, _data.Mert, start));
            await context.SaveChangesAsync(Ct);
            return Result<bool>.Success(true);
        });

        (await ActiveSegmentCountAsync(_data.Mert)).ShouldBe(1, "aynı transaction hatadan sonra commit edildi");
    }

    private SlotRequest Public(Instant start, params (Guid Service, Guid? Staff)[] items) =>
        new(_data.Merkez, start, [.. items.Select(i => new RequestedItem(i.Service, i.Staff))], SameStaffForAll: false, AvailabilityMode.Public);

    private SlotRequest Panel(Instant start, params (Guid Service, Guid? Staff)[] items) =>
        Public(start, items) with { Mode = AvailabilityMode.Panel };

    private CreateAppointmentCommand Command(SlotRequest slot, Duration? approvalTimeout = null) => new(
        _data.BrandId,
        slot,
        CustomerId: Guid.Parse("0193a8f0-0000-7000-8000-00000000cc01"),
        BrandCustomerId: Guid.Parse("0193a8f0-0000-7000-8000-00000000cc02"),
        slot.Mode == AvailabilityMode.Public ? AppointmentChannel.Online : AppointmentChannel.Panel,
        slot.Mode == AvailabilityMode.Public ? ClientApp.BookingPage : ClientApp.Panel,
        CustomerNote: null,
        StaffNote: null,
        SeriesId: null,
        CreatedByUserId: null,
        approvalTimeout is { } timeout ? new ApprovalPolicy(BrandRequiresApproval: true, timeout) : null);

    private Func<IServiceProvider, Task<Result<AppointmentWriteResult>>> Create(SlotRequest slot, Duration? approvalTimeout = null) =>
        sp => sp.GetRequiredService<AppointmentWriter>().CreateAsync(Command(slot, approvalTimeout), Ct);

    /// <summary>Bir uç isteği gibi: kendi kapsamı, bağlantısı ve transaction'ı; başarıda commit, hatada rollback.</summary>
    private async Task<Result<T>> InRequestAsync<T>(Func<IServiceProvider, Task<Result<T>>> action, Task? gate = null, Action? ready = null)
    {
        await using var scope = _services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.ForBrand(_data.BrandId), Ct);
        await session.BeginAsync(Ct);
        try
        {
            ready?.Invoke();
            if (gate is not null)
            {
                await gate;
            }

            var result = await action(scope.ServiceProvider);
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
        catch
        {
            await session.RollbackAsync(Ct);
            throw;
        }
    }

    private async Task<(Result<AppointmentWriteResult> First, Result<AppointmentWriteResult> Second)> InRequestPairAsync(
        Func<AppointmentWriter, Task<(Result<AppointmentWriteResult>, Result<AppointmentWriteResult>)>> action)
    {
        (Result<AppointmentWriteResult>, Result<AppointmentWriteResult>) pair = default;
        await InRequestAsync(async sp =>
        {
            pair = await action(sp.GetRequiredService<AppointmentWriter>());
            return Result<bool>.Success(true);
        });
        return pair;
    }

    /// <summary>Her istek bağlantısını ve transaction'ını açtıktan sonra kapıda bekler; hepsi hazır olunca birlikte koşar.</summary>
    private async Task<Result<AppointmentWriteResult>[]> ParallelAsync(IEnumerable<Func<IServiceProvider, Task<Result<AppointmentWriteResult>>>> requests)
    {
        var list = requests.ToList();
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var waiting = list.Count;
        var allReady = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        void Ready()
        {
            if (Interlocked.Decrement(ref waiting) == 0)
            {
                allReady.SetResult();
            }
        }

        var tasks = list.Select(request => Task.Run(() => InRequestAsync(request, gate.Task, Ready), Ct)).ToList();
        await allReady.Task.WaitAsync(TimeSpan.FromSeconds(30), Ct);
        gate.SetResult();
        return await Task.WhenAll(tasks).WaitAsync(TimeSpan.FromSeconds(60), Ct);
    }

    private async Task<int> ActiveSegmentCountAsync(Guid staffId) =>
        (await InRequestAsync(async sp => Result<int>.Success(
            await sp.GetRequiredService<SchedulingDbContext>().AppointmentItemSegments.CountAsync(s => s.StaffId == staffId && s.IsActive && s.StaffBusy, Ct)))).Value;

    /// <summary>Yazma yolunu atlayarak doğrudan kayıt (yalnız kısıtın kendisini sınamak için).</summary>
    private Task<Result<bool>> InsertDirectAsync(Guid staffId, Instant start, bool cancelled = false) =>
        InRequestAsync(async sp =>
        {
            var context = sp.GetRequiredService<SchedulingDbContext>();
            var appointment = NewHaircut(sp.GetRequiredService<IIdGenerator>(), staffId, start);
            if (cancelled)
            {
                appointment.CancelBySalon(CancellationReason.CustomerRequested, null, null, start - Duration.FromDays(1));
            }

            context.Appointments.Add(appointment);
            await context.SaveChangesAsync(Ct);
            return Result<bool>.Success(true);
        });

    private Appointment NewHaircut(IIdGenerator ids, Guid staffId, Instant start) => Appointment.Create(
        new NewAppointment(_data.BrandId, _data.Merkez, ids.New(), ids.New(), AppointmentChannel.Phone, null, null, null, null, null),
        AppointmentPlanner.Plan(start, [new ItemOffer(_data.Haircut, staffId, "Saç kesimi", Money.Try(45_000), Money.Zero(Currency.TRY), 0, 0, [new PlannedStage("Saç kesimi", 45, true)])]).Value,
        ids,
        start - Duration.FromDays(3));
}
