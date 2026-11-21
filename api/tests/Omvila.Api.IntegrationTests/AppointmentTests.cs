using System.Text.Json;
using NodaTime;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-02 kabul kriterleri 3 (aşamalı hizmet, bekleme dilimi), 11 (panel yeni randevu, saat dışı uyarısı),
// 12 (sürükle-bırak, dolu yer slot_taken), 13 (tekrarlayan: atlanan tarih, yalnız bu / bu ve sonrakiler), 14 (Geldiyse geri
// al, sayaç), 16 (kapalı gün ve "Yalnızca bu gün için aç"), 17 (salt okunur: yeni randevu yok), C6 (idempotency);
// roller-yetkiler.md (personel yalnız kendi kolonu ve kendi randevuları); sözleşme manage-calendar.
// Saat: 2026-11-25 09:00Z (Çarşamba 12:00 İstanbul). Şube Pzt–Cmt 09:00–19:00, Pazar kapalı.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class AppointmentTests(PostgresFixture db) : IAsyncLifetime
{
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private string _merkez = null!;
    private string _elif = null!;
    private string _zeynep = null!;
    private string _boya = null!;
    private string _fon = null!;
    private string _kesim = null!;
    private string _selin = null!;
    private int _requests;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db);
        _api = new ManageApi(_factory, db);
        _token = await _api.SignupAsync("+905324187705", "Kuaför Elif");
        var session = (await _api.GetAsync("/api/v1/manage/session", _token)).Body;
        _merkez = session.GetProperty("branches")[0].GetProperty("id").GetString()!;
        _elif = session.GetProperty("membership").GetProperty("staffId").GetString()!;

        var hours = await _api.GetAsync($"/api/v1/manage/branches/{_merkez}/working-hours", _token);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/branches/{_merkez}/working-hours", Week(), _token, hours.ETag)).Status.ShouldBe(200);
        var elif = await _api.GetAsync($"/api/v1/manage/staff/{_elif}", _token);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{_elif}/branches/{_merkez}/working-hours", Week(), _token, elif.ETag)).Status.ShouldBe(200);
        _zeynep = (await PostAsync("/api/v1/manage/staff",
            new { fullName = "Zeynep Aksoy", phone = "+905382741905", role = "Staff", calendarColor = "#2F5D8A", branchIds = new[] { _merkez } })).Body.GetProperty("id").GetString()!;

        var category = (await PostAsync("/api/v1/manage/service-categories", new { name = "Saç" })).Body.GetProperty("id").GetString()!;
        _boya = await ServiceAsync(category, "Boya", 120, new[]
        {
            new { name = "Uygulama", durationMinutes = 45, staffBusy = true },
            new { name = "Bekleme", durationMinutes = 40, staffBusy = false },
            new { name = "Yıkama", durationMinutes = 35, staffBusy = true },
        });
        _fon = await ServiceAsync(category, "Fön", 30, null);
        _kesim = await ServiceAsync(category, "Saç kesimi", 45, null);
        _selin = (await PostAsync("/api/v1/manage/customers", new { fullName = "Selin Öztürk", phone = "+905550004418" })).Body.GetProperty("id").GetString()!;
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task Criterion3_StagedService_WaitingSlotBookable_BusyStageIsSlotTaken_CalendarShowsWaitingSegment()
    {
        var boya = await CreateAsync(_elif, _boya, "2026-11-26T07:30:00Z");
        boya.Status.ShouldBe(201, boya.Body.ToString());

        (await CreateAsync(_elif, _fon, "2026-11-26T08:15:00Z")).Status.ShouldBe(201, "11:15–11:45 bekleme dilimi");
        var busy = await CreateAsync(_elif, _fon, "2026-11-26T07:45:00Z");
        busy.Status.ShouldBe(409);
        busy.Body.GetProperty("code").GetString().ShouldBe("slot_taken");
        busy.Body.GetProperty("meta").GetProperty("alternatives").GetArrayLength().ShouldBeGreaterThan(0);

        var calendar = await _api.GetAsync($"/api/v1/manage/calendar?branchId={_merkez}&from=2026-11-26&to=2026-11-26", _token);
        calendar.Status.ShouldBe(200);
        var block = calendar.Body.GetProperty("appointments").EnumerateArray().Single(a => string.Equals(a.GetProperty("serviceName").GetString(), "Boya", StringComparison.Ordinal));
        var segments = block.GetProperty("segments").EnumerateArray().ToList();
        segments.Count.ShouldBe(3);
        segments[1].GetProperty("staffBusy").GetBoolean().ShouldBeFalse();
        segments[1].GetProperty("startUtc").GetString().ShouldBe("2026-11-26T08:15:00Z");
        block.GetProperty("customerName").GetString().ShouldBe("Selin Öztürk");
        block.GetProperty("isNewCustomer").GetBoolean().ShouldBeTrue();
        calendar.Body.GetProperty("staff").GetArrayLength().ShouldBe(2);
        calendar.Body.GetProperty("days")[0].GetProperty("isClosed").GetBoolean().ShouldBeFalse();
    }

    [Fact]
    public async Task Criterion11_PhoneAppointment_NewCustomer_Confirmed_OutboxEvent_OutsideHoursNeedsSecondConfirmation()
    {
        var created = await PostAsync("/api/v1/manage/appointments", new
        {
            branchId = _merkez,
            newCustomer = new { fullName = "Derya Şahin", phone = "0533 111 22 18" },
            channel = "Phone",
            startUtc = "2026-12-02T11:30:00Z",
            items = new[] { new { serviceId = _kesim, staffId = _elif } },
            notifyCustomer = true,
        });

        created.Status.ShouldBe(201, created.Body.ToString());
        created.ETag.ShouldNotBeNull();
        var appointment = created.Body.GetProperty("appointment");
        appointment.GetProperty("status").GetString().ShouldBe("Confirmed");
        appointment.GetProperty("channel").GetString().ShouldBe("Phone");
        appointment.GetProperty("customer").GetProperty("fullName").GetString().ShouldBe("Derya Şahin");
        appointment.GetProperty("customer").GetProperty("phone").GetString().ShouldBe("+905331112218", "sahip tam numarayı görür");
        appointment.GetProperty("noShowAtUtc").GetString().ShouldBe("2026-12-02T11:45:00Z");
        appointment.GetProperty("history")[0].GetProperty("kind").GetString().ShouldBe("Created");
        created.Body.GetProperty("skippedOccurrences").GetArrayLength().ShouldBe(0);
        (await _api.ScalarAsync<string>(
            "SELECT payload::jsonb->>'notifyCustomer' FROM infra.outbox_events WHERE type LIKE '%AppointmentConfirmed'")).ShouldBe("true");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM scheduling.appointment_access_codes")).ShouldBe(1);

        var outside = new
        {
            branchId = _merkez,
            brandCustomerId = appointment.GetProperty("customer").GetProperty("brandCustomerId").GetString(),
            channel = "Phone",
            startUtc = "2026-12-02T17:00:00Z",
            items = new[] { new { serviceId = _kesim, staffId = _elif } },
        };
        var warned = await PostAsync("/api/v1/manage/appointments", outside);
        warned.Status.ShouldBe(409);
        warned.Body.GetProperty("code").GetString().ShouldBe("outside_working_hours");
        (await PostAsync("/api/v1/manage/appointments", new
        {
            outside.branchId,
            outside.brandCustomerId,
            outside.channel,
            outside.startUtc,
            outside.items,
            allowOutsideWorkingHours = true,
        })).Status.ShouldBe(201);
    }

    [Fact]
    public async Task Criterion12_DragToOtherStaff_Saved_OccupiedIsSlotTaken_StaleIfMatchIs412()
    {
        var created = await CreateAsync(_elif, _kesim, "2026-12-02T11:30:00Z");
        var id = Id(created);
        var itemId = created.Body.GetProperty("appointment").GetProperty("items")[0].GetProperty("id").GetString();

        var moved = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/reschedule",
            new { startUtc = "2026-12-02T11:00:00Z", items = new[] { new { itemId, staffId = _zeynep } } }, _token, created.ETag);

        moved.Status.ShouldBe(200, moved.Body.ToString());
        moved.Body.GetProperty("items")[0].GetProperty("staffId").GetString().ShouldBe(_zeynep);
        moved.Body.GetProperty("items")[0].GetProperty("staffName").GetString().ShouldBe("Zeynep Aksoy");
        moved.Body.GetProperty("startUtc").GetString().ShouldBe("2026-12-02T11:00:00Z");
        moved.Body.GetProperty("history").EnumerateArray().Select(h => h.GetProperty("kind").GetString()!).ShouldContain("Rescheduled", StringComparer.Ordinal);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM infra.outbox_events WHERE type LIKE '%AppointmentRescheduled'")).ShouldBe(1);

        (await CreateAsync(_zeynep, _kesim, "2026-12-02T12:00:00Z")).Status.ShouldBe(201);
        var occupied = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/reschedule",
            new { startUtc = "2026-12-02T12:15:00Z" }, _token, moved.ETag);
        occupied.Status.ShouldBe(409);
        occupied.Body.GetProperty("code").GetString().ShouldBe("slot_taken");

        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/reschedule",
            new { startUtc = "2026-12-02T08:00:00Z" }, _token, created.ETag)).Status.ShouldBe(412);
        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/check-in", new { }, _token, created.ETag)).Status.ShouldBe(412);
    }

    [Fact]
    public async Task Criterion13_EveryFourWeeks_ConflictingDateSkipped_CancelThisOnlyKeepsOthers()
    {
        (await CreateAsync(_elif, _kesim, "2026-12-24T13:00:00Z")).Status.ShouldBe(201);

        var created = await CreateAsync(_elif, _kesim, "2026-11-26T13:00:00Z", recurrence: new { frequency = "Weekly", intervalWeeks = 4 });

        created.Status.ShouldBe(201, created.Body.ToString());
        created.Body.GetProperty("skippedOccurrences").EnumerateArray().Select(d => d.GetString()).ShouldBe(["2026-12-24"]);
        created.Body.GetProperty("appointment").GetProperty("isRecurring").GetBoolean().ShouldBeTrue();
        var series = await SeriesAsync(Id(created));
        series.Select(s => s.Start).ShouldBe(["2026-11-26", "2027-01-21", "2027-02-18"]);

        var january = await _api.GetAsync($"/api/v1/manage/appointments/{series[1].Id}", _token);
        var cancelled = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{series[1].Id}/cancel",
            new { reason = "CustomerRequested", scope = "ThisOnly", notifyCustomer = false }, _token, january.ETag);

        cancelled.Status.ShouldBe(200, cancelled.Body.ToString());
        cancelled.Body.GetProperty("cancellation").GetProperty("reason").GetString().ShouldBe("CustomerRequested");
        (await SeriesAsync(Id(created))).Select(s => s.Status).ShouldBe(["Confirmed", "Cancelled", "Confirmed"]);
    }

    [Fact]
    public async Task Criterion13_CancelThisAndFollowing_CancelsLaterOccurrences_EarlierStay()
    {
        var created = await CreateAsync(_elif, _kesim, "2026-11-26T07:00:00Z",
            recurrence: new { frequency = "Weekly", intervalWeeks = 2, endsOn = "2027-01-07" });
        var series = await SeriesAsync(Id(created));
        series.Select(s => s.Start).ShouldBe(["2026-11-26", "2026-12-10", "2026-12-24", "2027-01-07"]);

        var third = await _api.GetAsync($"/api/v1/manage/appointments/{series[2].Id}", _token);
        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{series[2].Id}/cancel",
            new { reason = "Other", note = "Taşınıyor", scope = "ThisAndFollowing" }, _token, third.ETag)).Status.ShouldBe(200);

        (await SeriesAsync(Id(created))).Select(s => s.Status).ShouldBe(["Confirmed", "Confirmed", "Cancelled", "Cancelled"]);
        (await _api.ScalarAsync<string>("SELECT ends_on::text FROM scheduling.appointment_series")).ShouldBe("2026-12-23");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM infra.outbox_events WHERE type LIKE '%AppointmentCancelledBySalon'")).ShouldBe(2);
    }

    [Fact]
    public async Task Criterion13_RescheduleThisAndFollowing_MovesLaterOccurrences_AndSplitsSeries()
    {
        var created = await CreateAsync(_elif, _kesim, "2026-11-26T07:00:00Z",
            recurrence: new { frequency = "Weekly", intervalWeeks = 1, endsOn = "2026-12-10" });
        var series = await SeriesAsync(Id(created));
        var second = await _api.GetAsync($"/api/v1/manage/appointments/{series[1].Id}", _token);

        var moved = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{series[1].Id}/reschedule",
            new { startUtc = "2026-12-03T08:00:00Z", scope = "ThisAndFollowing" }, _token, second.ETag);

        moved.Status.ShouldBe(200, moved.Body.ToString());
        (await _api.ScalarAsync<string>(
            "SELECT string_agg(to_char(start_utc AT TIME ZONE 'UTC', 'MM-DD HH24:MI'), ',' ORDER BY start_utc) FROM scheduling.appointments"))
            .ShouldBe("11-26 07:00,12-03 08:00,12-10 08:00");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM scheduling.appointment_series")).ShouldBe(2);
    }

    [Fact]
    public async Task Criterion14_UndoNoShow_SameDay_CheckedIn_CounterDrops_TakenSlotIsSlotTaken()
    {
        var created = await CreateAsync(_elif, _kesim, "2026-11-25T09:30:00Z");
        var id = Id(created);
        await AdvanceAsync(Duration.FromMinutes(46));
        await MarkNoShowAsync(id);

        var current = await _api.GetAsync($"/api/v1/manage/appointments/{id}", _token);
        current.Body.GetProperty("status").GetString().ShouldBe("NoShow");
        var undone = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/undo-no-show", new { }, _token, current.ETag);

        undone.Status.ShouldBe(200, undone.Body.ToString());
        undone.Body.GetProperty("status").GetString().ShouldBe("CheckedIn");
        undone.Body.GetProperty("history").EnumerateArray().Select(h => h.GetProperty("kind").GetString()).ShouldBe(["Created", "NoShow", "NoShowUndone"]);
        (await _api.ScalarAsync<int>($"SELECT no_show_count FROM customers.brand_customers WHERE id = '{_selin}'")).ShouldBe(0);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM scheduling.appointment_item_segments WHERE is_active")).ShouldBe(1);
    }

    [Fact]
    public async Task Criterion14_UndoNoShow_WhenSlotWasGivenAway_IsSlotTaken()
    {
        var created = await CreateAsync(_elif, _kesim, "2026-11-25T09:30:00Z");
        await AdvanceAsync(Duration.FromMinutes(46));
        await MarkNoShowAsync(Id(created));
        (await CreateAsync(_elif, _fon, "2026-11-25T09:50:00Z", allowOutside: true)).Status.ShouldBe(201);

        var current = await _api.GetAsync($"/api/v1/manage/appointments/{Id(created)}", _token);
        var undone = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{Id(created)}/undo-no-show", new { }, _token, current.ETag);

        undone.Status.ShouldBe(409);
        undone.Body.GetProperty("code").GetString().ShouldBe("slot_taken");
        (await _api.ScalarAsync<int>($"SELECT no_show_count FROM customers.brand_customers WHERE id = '{_selin}'")).ShouldBe(1);
    }

    [Fact]
    public async Task CheckIn_AllowedFromOneHourBefore_TooEarlyIsInvalidTransition()
    {
        var early = await CreateAsync(_elif, _kesim, "2026-11-25T12:00:00Z");
        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{Id(early)}/check-in", new { }, _token, early.ETag))
            .Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");

        var soon = await CreateAsync(_elif, _kesim, "2026-11-25T09:30:00Z");
        var checkedIn = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{Id(soon)}/check-in", new { }, _token, soon.ETag);
        checkedIn.Status.ShouldBe(200);
        checkedIn.Body.GetProperty("status").GetString().ShouldBe("CheckedIn");
    }

    [Fact]
    public async Task Criterion16_ClosedSunday_ExtraOpeningOverrideOpensIt()
    {
        var closed = await _api.GetAsync($"/api/v1/manage/calendar?branchId={_merkez}&from=2026-11-29&to=2026-11-29", _token);
        closed.Status.ShouldBe(200);
        closed.Body.GetProperty("days")[0].GetProperty("isClosed").GetBoolean().ShouldBeTrue();

        (await PostAsync($"/api/v1/manage/branches/{_merkez}/overrides",
            new { fromDate = "2026-11-29", kind = "ExtraOpening", isClosed = false, intervals = new[] { new { start = "10:00", end = "16:00" } } })).Status.ShouldBe(201);

        var open = await _api.GetAsync($"/api/v1/manage/calendar?branchId={_merkez}&from=2026-11-29&to=2026-11-29", _token);
        var day = open.Body.GetProperty("days")[0];
        day.GetProperty("isClosed").GetBoolean().ShouldBeFalse();
        var elif = day.GetProperty("staffHours").EnumerateArray().Single(s => string.Equals(s.GetProperty("staffId").GetString(), _elif, StringComparison.Ordinal));
        elif.GetProperty("intervals")[0].GetProperty("start").GetString().ShouldBe("10:00");
    }

    [Fact]
    public async Task Calendar_Validation_RangeOver31Days_And_OtherBranchIs404()
    {
        (await _api.GetAsync($"/api/v1/manage/calendar?branchId={_merkez}&from=2026-11-01&to=2026-12-02", _token)).Status.ShouldBe(400);
        (await _api.GetAsync($"/api/v1/manage/calendar?branchId={_merkez}&from=2026-11-01&to=2026-12-01", _token)).Status.ShouldBe(200);
        (await _api.GetAsync($"/api/v1/manage/calendar?branchId=0193a8f0-0000-7000-8000-0000000000ff&from=2026-11-01&to=2026-11-01", _token)).Status.ShouldBe(404);
        (await _api.GetAsync("/api/v1/manage/appointments/0193a8f0-0000-7000-8000-0000000000ff", _token)).Status.ShouldBe(404);
    }

    [Fact]
    public async Task StaffRole_SeesOnlyOwnColumnAndAppointments_CannotCancelOrBookForOthers()
    {
        var elifs = await CreateAsync(_elif, _kesim, "2026-11-26T08:00:00Z");
        (await CreateAsync(_zeynep, _kesim, "2026-11-26T08:00:00Z")).Status.ShouldBe(201);
        var token = (await _api.SignInAsync("+905382741905")).Body.GetProperty("accessToken").GetString()!;

        var calendar = await _api.GetAsync($"/api/v1/manage/calendar?branchId={_merkez}&from=2026-11-26&to=2026-11-26", token);
        calendar.Status.ShouldBe(200);
        calendar.Body.GetProperty("staff").EnumerateArray().Select(s => s.GetProperty("staffId").GetString()).ShouldBe([_zeynep]);
        calendar.Body.GetProperty("appointments").EnumerateArray().Select(a => a.GetProperty("staffId").GetString()).ShouldBe([_zeynep]);

        (await _api.GetAsync($"/api/v1/manage/appointments/{Id(elifs)}", token)).Status.ShouldBe(404, "başkasının randevusu (K)");
        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{Id(elifs)}/cancel", new { reason = "Other" }, token, elifs.ETag))
            .Status.ShouldBe(403, "appointment.cancel yok");
        (await CreateAsync(_elif, _fon, "2026-11-27T08:00:00Z", token: token)).Status.ShouldBe(403, "yalnız kendine");
        var own = await CreateAsync(_zeynep, _fon, "2026-11-27T08:00:00Z", token: token);
        own.Status.ShouldBe(201);
        own.Body.GetProperty("appointment").GetProperty("customer").TryGetProperty("phone", out _).ShouldBeFalse("personel maskeli görür");
        (await _api.GetAsync($"/api/v1/manage/customers/{_selin}/appointments", token)).Body.GetProperty("items").GetArrayLength().ShouldBe(2);
    }

    [Fact]
    public async Task SearchSlots_PanelMode_ReturnsTodayWithoutMinNotice_AndStaffNames()
    {
        var result = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/branches/{_merkez}/availability/slots",
            new { items = new[] { new { serviceId = _kesim, staffId = _zeynep } }, date = "2026-11-25" }, _token);

        result.Status.ShouldBe(200, result.Body.ToString());
        result.Body.GetProperty("timeZone").GetString().ShouldBe("Europe/Istanbul");
        var first = result.Body.GetProperty("slots")[0];
        first.GetProperty("items")[0].GetProperty("staffName").GetString().ShouldBe("Zeynep Aksoy");
        first.GetProperty("total").GetProperty("amountMinor").GetInt64().ShouldBe(100000);
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/branches/{_merkez}/availability/slots", new { date = "2026-11-25" }, _token)).Status.ShouldBe(400);
    }

    [Fact]
    public async Task C6_SameIdempotencyKeyTwice_OneAppointment_Replayed()
    {
        var key = new Dictionary<string, string>(StringComparer.Ordinal) { ["Idempotency-Key"] = "0193a8f0-0000-7000-8000-00000000c006" };
        var body = Body(_elif, _kesim, "2026-11-26T08:00:00Z");

        var first = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/appointments", body, _token, key);
        var second = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/appointments", body, _token, key);

        first.Status.ShouldBe(201);
        second.Status.ShouldBe(201);
        second.Header("Idempotent-Replayed").ShouldBe("true");
        Id(second).ShouldBe(Id(first));
        (await _api.ScalarAsync<long>("SELECT count(*) FROM scheduling.appointments")).ShouldBe(1);
    }

    [Fact]
    public async Task CustomerCard_ListsAppointments_NewestFirst_AndNextAppointment()
    {
        await CreateAsync(_elif, _kesim, "2026-11-26T08:00:00Z");
        await CreateAsync(_elif, _boya, "2026-12-03T08:00:00Z");

        var list = await _api.GetAsync($"/api/v1/manage/customers/{_selin}/appointments?limit=1", _token);
        list.Status.ShouldBe(200);
        list.Body.GetProperty("items")[0].GetProperty("serviceNames")[0].GetString().ShouldBe("Boya");
        var next = await _api.GetAsync($"/api/v1/manage/customers/{_selin}/appointments?limit=1&cursor={list.Body.GetProperty("nextCursor").GetString()}", _token);
        next.Body.GetProperty("items")[0].GetProperty("serviceNames")[0].GetString().ShouldBe("Saç kesimi");
        next.Body.GetProperty("nextCursor").ValueKind.ShouldBe(JsonValueKind.Null);

        (await _api.GetAsync($"/api/v1/manage/customers/{_selin}", _token)).Body.GetProperty("nextAppointmentUtc").GetString().ShouldBe("2026-11-26T08:00:00Z");
        var customers = await _api.GetAsync("/api/v1/manage/customers", _token);
        customers.Body.GetProperty("items")[0].GetProperty("nextAppointmentUtc").GetString().ShouldBe("2026-11-26T08:00:00Z");
    }

    [Fact]
    public async Task Criterion17_ReadOnlySubscription_BlocksNewAppointment()
    {
        await _api.ScalarAsync<int>("UPDATE platform.subscriptions SET status = 'ReadOnly' RETURNING 1");

        var result = await CreateAsync(_elif, _kesim, "2026-11-26T08:00:00Z");

        result.Status.ShouldBe(403);
        result.Body.GetProperty("code").GetString().ShouldBe("subscription_read_only");
    }

    /// <summary>Saati ilerletir ve erişim jetonu süresi dolmasın diye yeniden giriş yapar (giriş saati 2 dk daha ilerletir).</summary>
    private async Task AdvanceAsync(Duration duration)
    {
        _factory.Clock.Advance(duration);
        _token = (await _api.SignInAsync("+905324187705")).Body.GetProperty("accessToken").GetString()!;
    }

    private async Task MarkNoShowAsync(string appointmentId) => await _api.ScalarAsync<int>($"""
        UPDATE scheduling.appointments SET status = 'NoShow', no_show_utc = start_utc + interval '15 minutes' WHERE id = '{appointmentId}';
        UPDATE scheduling.appointment_item_segments SET is_active = false
          WHERE appointment_item_id IN (SELECT id FROM scheduling.appointment_items WHERE appointment_id = '{appointmentId}');
        UPDATE customers.brand_customers SET no_show_count = no_show_count + 1 WHERE id = '{_selin}';
        SELECT 1;
        """);

    private async Task<List<(string Id, string Start, string Status)>> SeriesAsync(string appointmentId)
    {
        var text = await _api.ScalarAsync<string>($"""
            SELECT string_agg(id::text || '/' || to_char(start_utc AT TIME ZONE 'Europe/Istanbul', 'YYYY-MM-DD') || '/' || status, ',' ORDER BY start_utc)
            FROM scheduling.appointments WHERE series_id = (SELECT series_id FROM scheduling.appointments WHERE id = '{appointmentId}')
            """);
        return [.. text.Split(',').Select(p => p.Split('/')).Select(p => (p[0], p[1], p[2]))];
    }

    private static string Id(ManageApi.Response created) => created.Body.GetProperty("appointment").GetProperty("id").GetString()!;

    private object Body(string staffId, string serviceId, string startUtc, object? recurrence = null, bool allowOutside = false) => new
    {
        branchId = _merkez,
        brandCustomerId = _selin,
        channel = "Panel",
        startUtc,
        items = new[] { new { serviceId, staffId } },
        recurrence,
        notifyCustomer = true,
        allowOutsideWorkingHours = allowOutside,
    };

    private Task<ManageApi.Response> CreateAsync(
        string staffId, string serviceId, string startUtc, object? recurrence = null, bool allowOutside = false, string? token = null) =>
        _api.SendAsync(HttpMethod.Post, "/api/v1/manage/appointments", Body(staffId, serviceId, startUtc, recurrence, allowOutside), token ?? _token, Key());

    private Task<ManageApi.Response> PostAsync(string path, object body) => _api.SendAsync(HttpMethod.Post, path, body, _token, Key());

    private async Task<string> ServiceAsync(string category, string name, int minutes, object? stages) =>
        (await PostAsync("/api/v1/manage/services", new
        {
            categoryId = category,
            name,
            durationMinutes = minutes,
            price = new { amountMinor = 100000, currency = "TRY" },
            priceDisplay = "Fixed",
            deposit = new { mode = "Inherit" },
            isOnlineBookable = true,
            stages,
            staffIds = new[] { _elif, _zeynep },
        })).Body.GetProperty("id").GetString()!;

    private static object Week() => new
    {
        days = Enumerable.Range(1, 6).Select(d => new { dayOfWeek = d, intervals = new[] { new { start = "09:00", end = "19:00" } } }).ToArray(),
    };

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{++_requests:D12}" };
}
