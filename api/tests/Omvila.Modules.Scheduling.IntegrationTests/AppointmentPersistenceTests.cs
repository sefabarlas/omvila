using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Npgsql;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.SharedKernel;
using Omvila.TestKit;

namespace Omvila.Modules.Scheduling.IntegrationTests;

// Kural: alan-modeli.md § Scheduling (kalem, dilim tstzrange [Start, End), seri şablonu, erişim kodu UNIQUE), ADR-0006
// (randevu tabloları markaya aittir; başka marka göremez), durum-makinesi.md § Değişmezler (iptalde dilimler pasif),
// A14 (eski dilimler pasif, yenileri aktif, tek transaction).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class AppointmentPersistenceTests(PostgresFixture db) : IAsyncLifetime
{
    private static readonly Guid BrandA = Guid.Parse("0193a8f0-0000-7000-8000-00000000ab01");
    private static readonly Guid BrandB = Guid.Parse("0193a8f0-0000-7000-8000-00000000ab02");
    private static readonly Guid Branch = Guid.Parse("0193a8f0-0000-7000-8000-00000000ab11");
    private static readonly Guid Elif = Guid.Parse("0193a8f0-0000-7000-8000-00000000ab21");
    private static readonly Guid Service = Guid.Parse("0193a8f0-0000-7000-8000-00000000ab31");

    /// <summary>Sal 25 Kas 2026 10:30 İstanbul.</summary>
    private static readonly Instant Start = Instant.FromUtc(2026, 11, 25, 7, 30);

    private static readonly string[] Tables = ["appointments", "appointment_items", "appointment_item_segments", "appointment_series", "appointment_access_codes"];

    private ServiceProvider _services = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await using (var connection = await db.OpenMigratorConnectionAsync(Ct))
        {
            await using var command = new NpgsqlCommand(
                "TRUNCATE scheduling.waitlist_offers, scheduling.waitlist_entries, scheduling.appointment_access_codes, scheduling.appointment_item_segments, scheduling.appointment_items, scheduling.appointments, scheduling.appointment_series;",
                connection);
            await command.ExecuteNonQueryAsync(Ct);
        }

        _services = db.CreateServices(configure: s => s.AddSchedulingModule(db.Configuration()));
    }

    public async ValueTask DisposeAsync() => await _services.DisposeAsync();

    [Fact]
    public async Task Appointment_WithItemsAndSegments_RoundTrips_IncludingTstzrange()
    {
        var created = await InBrandAsync(BrandA, async (context, ids) =>
        {
            var appointment = NewAppointment(BrandA, ids, seriesId: null);
            context.Appointments.Add(appointment);
            await context.SaveChangesAsync(Ct);
            return appointment;
        });

        var loaded = await InBrandAsync(BrandA, (context, _) => Load(context, created.Id));

        loaded.Status.ShouldBe(AppointmentStatus.Confirmed);
        loaded.Channel.ShouldBe(AppointmentChannel.Online);
        loaded.StartUtc.ShouldBe(Start);
        loaded.EndUtc.ShouldBe(Start + Duration.FromMinutes(120));
        loaded.TotalAmountMinor.ShouldBe(120_000);
        loaded.CurrencyCode.ShouldBe("TRY");
        loaded.Version.ShouldNotBe(0u);
        var item = loaded.Items.ShouldHaveSingleItem();
        item.ServiceNameSnapshot.ShouldBe("Boya");
        item.Segments.OrderBy(s => s.SeqNo).Select(s => (s.During, s.StaffBusy, s.Kind, s.IsActive, s.StaffId)).ShouldBe(
        [
            (new Interval(Start, Start + Duration.FromMinutes(45)), true, SegmentKind.Stage, true, (Guid?)Elif),
            (new Interval(Start + Duration.FromMinutes(45), Start + Duration.FromMinutes(85)), false, SegmentKind.Stage, true, Elif),
            (new Interval(Start + Duration.FromMinutes(85), Start + Duration.FromMinutes(120)), true, SegmentKind.Stage, true, Elif),
        ]);

        (await ScalarAsync<string>(
                $"SELECT string_agg(lower_inc(during)::text || upper_inc(during)::text || (upper(during) - lower(during))::text, ',' ORDER BY seq_no) FROM scheduling.appointment_item_segments"))
            .ShouldBe("truefalse00:45:00,truefalse00:40:00,truefalse00:35:00", "[Start, End) yarı açık tstzrange");
        (await ScalarAsync<bool>(
                $"SELECT EXISTS (SELECT 1 FROM scheduling.appointment_item_segments WHERE staff_id = '{Elif}' AND staff_busy AND is_active AND during && tstzrange('{Start.Plus(Duration.FromMinutes(50))}', '{Start.Plus(Duration.FromMinutes(80))}'))"))
            .ShouldBeFalse("bekleme dilimi personel-meşgul değildir (M3)");
    }

    [Fact]
    public async Task Cancel_And_Reschedule_PersistSegmentActivity()
    {
        var created = await InBrandAsync(BrandA, async (context, ids) =>
        {
            var appointment = NewAppointment(BrandA, ids, seriesId: null);
            context.Appointments.Add(appointment);
            await context.SaveChangesAsync(Ct);
            return appointment;
        });

        await InBrandAsync(BrandA, async (context, ids) =>
        {
            var appointment = await Load(context, created.Id);
            appointment.Reschedule(Plan(Start + Duration.FromHours(2)), ChangeActor.Salon, Start - Duration.FromDays(1), Duration.FromHours(12), ids, null)
                .IsSuccess.ShouldBeTrue();
            await context.SaveChangesAsync(Ct);
            return true;
        });

        (await ScalarAsync<long>("SELECT count(*) FROM scheduling.appointment_item_segments WHERE is_active")).ShouldBe(3);
        (await ScalarAsync<long>("SELECT count(*) FROM scheduling.appointment_item_segments WHERE NOT is_active AND superseded_utc IS NOT NULL")).ShouldBe(3);

        await InBrandAsync(BrandA, async (context, _) =>
        {
            var appointment = await Load(context, created.Id);
            appointment.CancelBySalon(CancellationReason.CustomerRequested, null, null, Start).IsSuccess.ShouldBeTrue();
            await context.SaveChangesAsync(Ct);
            return true;
        });

        (await ScalarAsync<long>("SELECT count(*) FROM scheduling.appointment_item_segments WHERE is_active")).ShouldBe(0);
        (await ScalarAsync<string>("SELECT status || '/' || cancelled_by || '/' || cancellation_reason FROM scheduling.appointments"))
            .ShouldBe("Cancelled/Salon/CustomerRequested");
    }

    [Fact]
    public async Task StaleVersion_IsAConcurrencyConflict()
    {
        var created = await InBrandAsync(BrandA, async (context, ids) =>
        {
            var appointment = NewAppointment(BrandA, ids, seriesId: null);
            context.Appointments.Add(appointment);
            await context.SaveChangesAsync(Ct);
            return appointment;
        });

        await Should.ThrowAsync<DbUpdateConcurrencyException>(() => InBrandAsync(BrandA, async (context, _) =>
        {
            var appointment = await Load(context, created.Id);
            context.Entry(appointment).Property(a => a.Version).OriginalValue = appointment.Version - 1;
            appointment.CheckIn(Start, null).IsSuccess.ShouldBeTrue();
            await context.SaveChangesAsync(Ct);
            return true;
        }));
    }

    [Fact]
    public async Task Series_TemplateItems_AndAccessCode_RoundTrip_CodeIsUnique()
    {
        var (seriesId, code) = await InBrandAsync(BrandA, async (context, ids) =>
        {
            var series = AppointmentSeries.Create(
                ids.New(), BrandA, Branch, Guid.Parse("0193a8f0-0000-7000-8000-00000000ab41"), Guid.Parse("0193a8f0-0000-7000-8000-00000000ab42"),
                [new SeriesItemTemplate(Service, Elif, 0)], WeeklyRecurrence.Create(4).Value,
                new LocalDate(2026, 11, 25), new LocalTime(10, 30), null, new LocalDate(2027, 2, 24), Start);
            context.AppointmentSeries.Add(series);
            var appointment = NewAppointment(BrandA, ids, series.Id);
            context.Appointments.Add(appointment);
            var accessCode = AppointmentAccessCode.IssueForManage(ids.New(), BrandA, appointment.Id, appointment.EndUtc, Start);
            context.AppointmentAccessCodes.Add(accessCode);
            await context.SaveChangesAsync(Ct);
            return (series.Id, accessCode.Code);
        });

        var loaded = await InBrandAsync(BrandA, (context, _) => context.AppointmentSeries.SingleAsync(s => s.Id == seriesId, Ct));
        loaded.TemplateItems.ShouldBe([new SeriesItemTemplate(Service, Elif, 0)]);
        loaded.Rule.IntervalWeeks.ShouldBe(4);
        loaded.LocalStartTime.ShouldBe(new LocalTime(10, 30));
        (await InBrandAsync(BrandA, (context, _) => context.AppointmentAccessCodes.SingleAsync(c => c.Code == code, Ct))).ExpiresUtc
            .ShouldBe(Start + Duration.FromMinutes(120) + Duration.FromDays(7));

        var duplicate = await Should.ThrowAsync<DbUpdateException>(() => InBrandAsync(BrandA, async (context, ids) =>
        {
            var appointment = NewAppointment(BrandA, ids, seriesId: null);
            context.Appointments.Add(appointment);
            context.AppointmentAccessCodes.Add(AppointmentAccessCode.IssueForManage(ids.New(), BrandA, appointment.Id, appointment.EndUtc, Start, code));
            await context.SaveChangesAsync(Ct);
            return true;
        }));
        duplicate.InnerException.ShouldBeOfType<PostgresException>().ConstraintName.ShouldBe("ix_appointment_access_codes_code");
    }

    [Fact]
    public async Task OtherBrand_CannotReadAnySchedulingRow()
    {
        await InBrandAsync(BrandA, async (context, ids) =>
        {
            var series = AppointmentSeries.Create(
                ids.New(), BrandA, Branch, Guid.Parse("0193a8f0-0000-7000-8000-00000000ab41"), Guid.Parse("0193a8f0-0000-7000-8000-00000000ab42"),
                [new SeriesItemTemplate(Service, Elif, 0)], WeeklyRecurrence.Create(1).Value,
                new LocalDate(2026, 11, 25), new LocalTime(10, 30), null, new LocalDate(2027, 2, 24), Start);
            context.AppointmentSeries.Add(series);
            var appointment = NewAppointment(BrandA, ids, series.Id);
            context.Appointments.Add(appointment);
            context.AppointmentAccessCodes.Add(AppointmentAccessCode.IssueForManage(ids.New(), BrandA, appointment.Id, appointment.EndUtc, Start));
            await context.SaveChangesAsync(Ct);
            return true;
        });

        foreach (var table in Tables)
        {
            (await CountInBrandAsync(BrandA, table)).ShouldBeGreaterThan(0, table);
            (await CountInBrandAsync(BrandB, table)).ShouldBe(0, table);
        }
    }

    [Fact]
    public async Task OtherBrand_CannotInsertRowsForTheFirstBrand() =>
        (await Should.ThrowAsync<DbUpdateException>(() => InBrandAsync(BrandB, async (context, ids) =>
        {
            context.Appointments.Add(NewAppointment(BrandA, ids, seriesId: null));
            await context.SaveChangesAsync(Ct);
            return true;
        }))).InnerException.ShouldBeOfType<PostgresException>().SqlState.ShouldBe(PostgresErrorCodes.InsufficientPrivilege);

    private static AppointmentPlan Plan(Instant start) => AppointmentPlanner.Plan(start,
    [
        new ItemOffer(Service, Elif, "Boya", Money.Try(120_000), Money.Zero(Currency.TRY), 0, 0,
            [new PlannedStage("Uygulama", 45, true), new PlannedStage("Bekleme", 40, false), new PlannedStage("Yıkama ve fön", 35, true)]),
    ]).Value;

    private static Appointment NewAppointment(Guid brandId, IIdGenerator ids, Guid? seriesId) => Appointment.Create(
        new NewAppointment(
            brandId,
            Branch,
            Guid.Parse("0193a8f0-0000-7000-8000-00000000ab41"),
            Guid.Parse("0193a8f0-0000-7000-8000-00000000ab42"),
            AppointmentChannel.Online,
            ClientApp.BookingPage,
            CustomerNote: null,
            StaffNote: null,
            seriesId,
            CreatedByUserId: null),
        Plan(Start),
        ids,
        Start - Duration.FromDays(2));

    private static Task<Appointment> Load(SchedulingDbContext context, Guid id) =>
        context.Appointments.Include(a => a.Items).ThenInclude(i => i.Segments).SingleAsync(a => a.Id == id, Ct);

    private async Task<T> InBrandAsync<T>(Guid brandId, Func<SchedulingDbContext, IIdGenerator, Task<T>> action)
    {
        await using var scope = _services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.ForBrand(brandId), Ct);
        await session.BeginAsync(Ct);
        var result = await action(scope.ServiceProvider.GetRequiredService<SchedulingDbContext>(), scope.ServiceProvider.GetRequiredService<IIdGenerator>());
        await session.CommitAsync(Ct);
        return result;
    }

    private async Task<long> CountInBrandAsync(Guid brandId, string table)
    {
        await using var scope = _services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.ForBrand(brandId), Ct);
        await session.BeginAsync(Ct);
        await using var command = new NpgsqlCommand($"SELECT count(*) FROM scheduling.{table}", session.Connection, session.Transaction);
        var count = (long)(await command.ExecuteScalarAsync(Ct))!;
        await session.RollbackAsync(Ct);
        return count;
    }

    private async Task<T> ScalarAsync<T>(string sql)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand(sql, connection);
        return (T)(await command.ExecuteScalarAsync(Ct))!;
    }
}
