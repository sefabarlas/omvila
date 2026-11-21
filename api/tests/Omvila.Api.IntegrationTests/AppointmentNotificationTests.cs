using NodaTime;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-02 kabul kriterleri 11 (panel: "Müşteriye SMS gönder" açık → onay mesajı), 13 (seri), 14 (MarkNoShows, Geldiyse
// geri al, sayaç), 18 (Europe/Berlin yaz saati günü); bildirimler.md (onay, saat değişikliği, salon iptali, hatırlatmalar,
// "Müşteriye bildir" anahtarı, iptalde bekleyen hatırlatmalar düşer); durum-makinesi.md § Sistem işleri (MarkNoShows,
// ScheduleReminders, ExtendRecurringSeries); T-46 (3) müşterinin randevu SMS tercihi.
// Saat: 2026-11-25 09:00Z (Çarşamba 12:00 İstanbul). Şube her gün 09:00–19:00.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class AppointmentNotificationTests(PostgresFixture db) : IAsyncLifetime
{
    private const string SelinPhone = "+905550004418";
    private const string OwnerPhone = "+905324187705";
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private string _merkez = null!;
    private string _elif = null!;
    private string _kesim = null!;
    private string _selin = null!;
    private int _requests;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db);
        _api = new ManageApi(_factory, db);
        _token = await _api.SignupAsync(OwnerPhone, "Kuaför Elif");
        var session = (await _api.GetAsync("/api/v1/manage/session", _token)).Body;
        _merkez = session.GetProperty("branches")[0].GetProperty("id").GetString()!;
        _elif = session.GetProperty("membership").GetProperty("staffId").GetString()!;

        var hours = await _api.GetAsync($"/api/v1/manage/branches/{_merkez}/working-hours", _token);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/branches/{_merkez}/working-hours", Week(), _token, hours.ETag)).Status.ShouldBe(200);
        var elif = await _api.GetAsync($"/api/v1/manage/staff/{_elif}", _token);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{_elif}/branches/{_merkez}/working-hours", Week(), _token, elif.ETag)).Status.ShouldBe(200);
        var category = (await PostAsync("/api/v1/manage/service-categories", new { name = "Saç" })).Body.GetProperty("id").GetString()!;
        _kesim = (await PostAsync("/api/v1/manage/services", new
        {
            categoryId = category,
            name = "Saç kesimi",
            durationMinutes = 45,
            price = new { amountMinor = 45000, currency = "TRY" },
            priceDisplay = "Fixed",
            deposit = new { mode = "Inherit" },
            isOnlineBookable = true,
            staffIds = new[] { _elif },
        })).Body.GetProperty("id").GetString()!;
        _selin = (await PostAsync("/api/v1/manage/customers", new { fullName = "Selin Öztürk", phone = SelinPhone })).Body.GetProperty("id").GetString()!;
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task Criterion11_PanelCreate_NotifyOn_ConfirmationAndReminders_NotifyOff_OnlyReminders()
    {
        var notified = await CreateAsync("2026-12-02T11:30:00Z", notify: true);
        var silent = await CreateAsync("2026-12-03T11:30:00Z", notify: false);
        notified.Status.ShouldBe(201, notified.Body.ToString());
        silent.Status.ShouldBe(201, silent.Body.ToString());

        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);

        var code = await _api.ScalarAsync<string>($"SELECT code FROM scheduling.appointment_access_codes WHERE appointment_id = '{Id(notified)}'");
        (await _api.ScalarAsync<string>("""
            SELECT string_agg(recipient_phone || '|' || coalesce(reference_id::text, '') || '|' || (payload->>'salon') || '|' || (payload->>'tarih') || '|' || (payload->>'saat')
              || '|' || (payload->>'hizmet') || '|' || (payload->>'personel') || '|' || (payload->>'bağlantı'), ',')
            FROM notifications.notification_outbox WHERE event_code = 'appointment.confirmed'
            """)).ShouldBe($"{SelinPhone}|{Id(notified)}|Kuaför Elif|Çar 2 Ara|14:30|Saç kesimi|Kuaför Elif|omvila.com/r/{code}");
        (await RemindersAsync("Pending")).ShouldBe("12-01 11:30|-,12-02 09:30|same_day,12-02 11:30|-,12-03 09:30|same_day", "anahtar kapalı olsa da hatırlatmalar planlanır");
    }

    [Fact]
    public async Task Reminders_FollowTheBrandsNotificationSettings()
    {
        // Kural: sureler-ve-ayarlar.md ReminderOffsetsMinutes (Brand); kanonik düzenleyici notification-settings (T-62 (4)).
        var settings = await _api.GetAsync("/api/v1/manage/brand/notification-settings", _token);
        (await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/brand/notification-settings",
            new { reminderOffsetsMinutes = Of(180), reviewInviteEnabled = true }, _token, settings.ETag)).Status.ShouldBe(200);
        await CreateAsync("2026-12-02T11:30:00Z", notify: false);
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        var off = await _api.GetAsync("/api/v1/manage/brand/notification-settings", _token);
        (await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/brand/notification-settings",
            new { reminderOffsetsMinutes = Array.Empty<int>(), reviewInviteEnabled = true }, _token, off.ETag)).Status.ShouldBe(200);
        await CreateAsync("2026-12-03T11:30:00Z", notify: false);

        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);

        (await RemindersAsync("Pending")).ShouldBe("12-02 08:30|same_day", "yalnız 3 saat öncesi; boş liste hatırlatma yazmaz");
    }

    [Fact]
    public async Task CustomerWithAppointmentSmsDisabled_GetsNoConfirmationOrReminders()
    {
        await _api.ScalarAsync<int>($"UPDATE customers.brand_customers SET appointment_sms_enabled = false WHERE id = '{_selin}' RETURNING 1");

        (await CreateAsync("2026-12-02T11:30:00Z", notify: true)).Status.ShouldBe(201);
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);

        (await AppointmentMessagesAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task Reschedule_SendsUpdate_AndMovesReminders()
    {
        var created = await CreateAsync("2026-12-02T11:30:00Z", notify: true);
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);

        var moved = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{Id(created)}/reschedule",
            new { startUtc = "2026-12-03T08:00:00Z", notifyCustomer = true }, _token, created.ETag);
        moved.Status.ShouldBe(200, moved.Body.ToString());
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);

        (await _api.ScalarAsync<string>("""
            SELECT (payload->>'yeni tarih') || ' ' || (payload->>'saat') FROM notifications.notification_outbox WHERE event_code = 'appointment.rescheduled'
            """)).ShouldBe("Per 3 Ara 11:00");
        (await RemindersAsync("Cancelled")).ShouldBe("12-01 11:30|-,12-02 09:30|same_day");
        (await RemindersAsync("Pending")).ShouldBe("12-02 08:00|-,12-03 06:00|same_day");
    }

    [Fact]
    public async Task Reschedule_WithNotifyOff_NoUpdateMessage_RemindersStillMove()
    {
        var created = await CreateAsync("2026-12-02T11:30:00Z", notify: true);
        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{Id(created)}/reschedule",
            new { startUtc = "2026-12-02T12:30:00Z", notifyCustomer = false }, _token, created.ETag)).Status.ShouldBe(200);

        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);

        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'appointment.rescheduled'")).ShouldBe(0);
        (await RemindersAsync("Pending")).ShouldBe("12-01 12:30|-,12-02 10:30|same_day", "olaylar sırayla işlenir; tek takım hatırlatma kalır");
    }

    [Fact]
    public async Task CancelBySalon_NotifiesCustomerWithBookingLink_AndCancelsReminders_NotifyOffSendsNothing()
    {
        var first = await CreateAsync("2026-12-02T11:30:00Z", notify: true);
        var second = await CreateAsync("2026-12-03T11:30:00Z", notify: false);
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);

        (await CancelAsync(first, notify: true)).Status.ShouldBe(200);
        (await CancelAsync(second, notify: false)).Status.ShouldBe(200);
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);

        (await _api.ScalarAsync<string>("""
            SELECT string_agg(reference_id::text || '|' || (payload->>'tarih') || ' ' || (payload->>'saat') || '|' || (payload->>'rezervasyon bağlantısı'), ',')
            FROM notifications.notification_outbox WHERE event_code = 'appointment.cancelled_by_salon'
            """)).ShouldBe($"{Id(first)}|Çar 2 Ara 14:30|omvila.com/kuafor-elif");
        (await RemindersAsync("Pending")).ShouldBeNull();
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'appointment.reminder' AND status = 'Cancelled'")).ShouldBe(4);
    }

    [Fact]
    public async Task CancelThisAndFollowing_OneCancellationMessage_AllRemindersCancelled()
    {
        var created = await CreateAsync("2026-11-26T08:00:00Z", notify: true, recurrence: new { frequency = "Weekly", intervalWeeks = 1, endsOn = "2026-12-10" });
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'appointment.confirmed'")).ShouldBe(1, "yalnız serinin ilki");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'appointment.reminder'")).ShouldBe(5, "ilk randevunun 24 sa öncesi geçmişte");

        var secondId = await _api.ScalarAsync<Guid>($"""
            SELECT id FROM scheduling.appointments WHERE series_id = (SELECT series_id FROM scheduling.appointments WHERE id = '{Id(created)}') ORDER BY start_utc OFFSET 1 LIMIT 1
            """);
        var second = await _api.GetAsync($"/api/v1/manage/appointments/{secondId}", _token);
        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{secondId}/cancel",
            new { reason = "CustomerRequested", scope = "ThisAndFollowing", notifyCustomer = true }, _token, second.ETag)).Status.ShouldBe(200);
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);

        (await _api.ScalarAsync<string>("SELECT reference_id::text FROM notifications.notification_outbox WHERE event_code = 'appointment.cancelled_by_salon'"))
            .ShouldBe(secondId.ToString());
        (await RemindersAsync("Pending")).ShouldBe("11-26 06:00|same_day", "ilk randevununki kalır");
    }

    [Fact]
    public async Task CustomerCancelFromLink_CancelsReminders_NoMessageToCustomer()
    {
        var created = await CreateAsync("2026-12-02T11:30:00Z", notify: true);
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        var code = await _api.ScalarAsync<string>("SELECT code FROM scheduling.appointment_access_codes");
        var shown = await _api.SendAsync(HttpMethod.Get, $"/api/v1/public/appointments/{code}", null);

        var cancelled = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/appointments/{code}/cancel", new { reason = "PlanChanged" },
            headers: new Dictionary<string, string>(StringComparer.Ordinal) { ["If-Match"] = shown.ETag! });
        cancelled.Status.ShouldBe(200, cancelled.Body.ToString());
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);

        (await RemindersAsync("Pending")).ShouldBeNull();
        (await RemindersAsync("Cancelled")).ShouldBe("12-01 11:30|-,12-02 09:30|same_day");
        (await AppointmentMessagesAsync()).ShouldBe(3, "onay + iki hatırlatma; iptalde müşteriye mesaj yok");
        Id(created).ShouldNotBeNull();
    }

    [Fact]
    public async Task Criterion14_MarkNoShows_At1245_CounterUp_UndoCheckedIn_CounterDown()
    {
        var created = await CreateAsync("2026-11-25T09:30:00Z", notify: false); // 12:30 İstanbul

        SetClock(Instant.FromUtc(2026, 11, 25, 9, 44)); // 12:44
        await _api.RunJobAsync(ManageApi.MarkNoShowsJob);
        (await StatusAsync(Id(created))).ShouldBe("Confirmed");

        SetClock(Instant.FromUtc(2026, 11, 25, 9, 45)); // 12:45 = başlangıç + NoShowAfterMinutes (15)
        await _api.RunJobAsync(ManageApi.MarkNoShowsJob);

        (await StatusAsync(Id(created))).ShouldBe("NoShow");
        (await NoShowCountAsync()).ShouldBe(1);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM scheduling.appointment_item_segments WHERE is_active")).ShouldBe(0);

        await _api.RunJobAsync(ManageApi.MarkNoShowsJob);
        (await NoShowCountAsync()).ShouldBe(1, "ikinci çalışma sayacı yeniden artırmaz");

        _token = (await _api.SignInAsync(OwnerPhone)).Body.GetProperty("accessToken").GetString()!;
        var current = await _api.GetAsync($"/api/v1/manage/appointments/{Id(created)}", _token);
        var undone = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{Id(created)}/undo-no-show", new { }, _token, current.ETag);

        undone.Status.ShouldBe(200, undone.Body.ToString());
        undone.Body.GetProperty("status").GetString().ShouldBe("CheckedIn");
        (await NoShowCountAsync()).ShouldBe(0);
        await _api.RunJobAsync(ManageApi.MarkNoShowsJob);
        (await StatusAsync(Id(created))).ShouldBe("CheckedIn");
    }

    [Fact]
    public async Task MarkNoShows_UsesBrandSetting_AndSkipsCheckedIn()
    {
        await _api.ScalarAsync<int>("UPDATE tenancy.brand_settings SET no_show_after_minutes = 30 RETURNING 1");
        var late = await CreateAsync("2026-11-25T09:15:00Z", notify: false);
        var arrived = await CreateAsync("2026-11-25T10:00:00Z", notify: false);
        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{Id(arrived)}/check-in", new { }, _token, arrived.ETag)).Status.ShouldBe(200);

        SetClock(Instant.FromUtc(2026, 11, 25, 9, 44)); // 09:15 + 30 dk geçmedi
        await _api.RunJobAsync(ManageApi.MarkNoShowsJob);
        (await StatusAsync(Id(late))).ShouldBe("Confirmed");

        SetClock(Instant.FromUtc(2026, 11, 25, 9, 45));
        await _api.RunJobAsync(ManageApi.MarkNoShowsJob);
        (await StatusAsync(Id(late))).ShouldBe("NoShow");
        (await StatusAsync(Id(arrived))).ShouldBe("CheckedIn");
    }

    [Fact]
    public async Task ExtendRecurringSeries_KeepsThreeMonthHorizon_SkipsConflictingDate_Idempotent()
    {
        var created = await CreateAsync("2026-11-26T13:00:00Z", notify: true, recurrence: new { frequency = "Weekly", intervalWeeks = 1 });
        created.Status.ShouldBe(201, created.Body.ToString());
        (await _api.ScalarAsync<string>("SELECT generated_until::text FROM scheduling.appointment_series")).ShouldBe("2027-02-25");
        (await CreateAsync("2027-03-04T13:00:00Z", notify: false)).Status.ShouldBe(201, "serinin ileride çakışacağı tarih");

        SetClock(Instant.FromUtc(2026, 12, 16, 9, 0)); // 12:00 İstanbul → ufuk 2027-03-16 (hariç)
        await _api.RunJobAsync(ManageApi.ExtendRecurringSeriesJob);

        (await SeriesDatesAsync()).Split(',').TakeLast(3).ShouldBe(["2027-02-18", "2027-02-25", "2027-03-11"], "2027-03-04 atlandı");
        (await _api.ScalarAsync<string>("SELECT generated_until::text FROM scheduling.appointment_series")).ShouldBe("2027-03-15");

        await _api.RunJobAsync(ManageApi.ExtendRecurringSeriesJob);
        (await SeriesDatesAsync()).Split(',').Length.ShouldBe(15, "aynı gün ikinci çalışma üretmez");

        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'appointment.confirmed'")).ShouldBe(1, "uzatılan tekrara onay SMS'i yok");
        (await _api.ScalarAsync<long>($"""
            SELECT count(*) FROM notifications.notification_outbox o JOIN scheduling.appointments a ON a.id = o.reference_id
            WHERE o.event_code = 'appointment.reminder' AND a.start_utc = '2027-03-11T13:00:00Z'
            """)).ShouldBe(2);
    }

    [Fact]
    public async Task ExtendRecurringSeries_BeforeThreeAmLocal_WaitsForThatDay()
    {
        (await CreateAsync("2026-11-26T13:00:00Z", notify: false, recurrence: new { frequency = "Weekly", intervalWeeks = 1 })).Status.ShouldBe(201);

        SetClock(Instant.FromUtc(2026, 12, 2, 23, 0)); // 3 Ara 02:00 İstanbul
        await _api.RunJobAsync(ManageApi.ExtendRecurringSeriesJob);
        (await _api.ScalarAsync<string>("SELECT generated_until::text FROM scheduling.appointment_series")).ShouldBe("2027-03-01");

        SetClock(Instant.FromUtc(2026, 12, 3, 0, 0)); // 03:00 İstanbul
        await _api.RunJobAsync(ManageApi.ExtendRecurringSeriesJob);
        (await _api.ScalarAsync<string>("SELECT generated_until::text FROM scheduling.appointment_series")).ShouldBe("2027-03-02");
        (await SeriesDatesAsync()).Split(',')[^1].ShouldBe("2027-02-25", "yeni tarih 2027-03-04 henüz ufukta değil");
    }

    [Fact]
    public async Task Criterion18_BerlinBranch_SpringForwardDay_PanelAndBookingShowLocalTimes()
    {
        await _api.ScalarAsync<int>($"UPDATE tenancy.branches SET time_zone_id = 'Europe/Berlin' WHERE id = '{_merkez}' RETURNING 1");
        SetClock(Instant.FromUtc(2027, 3, 26, 9, 0)); // Cuma
        _token = (await _api.SignInAsync(OwnerPhone)).Body.GetProperty("accessToken").GetString()!;
        var items = new[] { new { serviceId = _kesim, staffId = _elif } };

        // Rezervasyon sayfası: Cumartesi (CET, +1) ve Pazar (CEST, +2; 02:00 → 03:00) 09:00 yerel.
        var saturday = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/availability/slots", new { items, date = "2027-03-27" });
        saturday.Status.ShouldBe(200, saturday.Body.ToString());
        saturday.Body.GetProperty("timeZone").GetString().ShouldBe("Europe/Berlin");
        saturday.Body.GetProperty("slots")[0].GetProperty("localTime").GetString().ShouldBe("09:00");
        saturday.Body.GetProperty("slots")[0].GetProperty("startUtc").GetString().ShouldBe("2027-03-27T08:00:00Z");
        var sunday = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/availability/slots", new { items, date = "2027-03-28" });
        sunday.Body.GetProperty("slots")[0].GetProperty("localTime").GetString().ShouldBe("09:00");
        sunday.Body.GetProperty("slots")[0].GetProperty("startUtc").GetString().ShouldBe("2027-03-28T07:00:00Z");
        var last = sunday.Body.GetProperty("slots").EnumerateArray().Last();
        last.GetProperty("localTime").GetString().ShouldBe("18:15");
        last.GetProperty("startUtc").GetString().ShouldBe("2027-03-28T16:15:00Z");

        // Panel: 10:00 yerel randevu, takvimde çalışma saati 09:00–19:00 ve boş saat aramasında aynı yerel saat.
        var created = await CreateAsync("2027-03-28T08:00:00Z", notify: true);
        created.Status.ShouldBe(201, created.Body.ToString());
        created.Body.GetProperty("appointment").GetProperty("timeZone").GetString().ShouldBe("Europe/Berlin");
        var calendar = await _api.GetAsync($"/api/v1/manage/calendar?branchId={_merkez}&from=2027-03-28&to=2027-03-28", _token);
        calendar.Status.ShouldBe(200, calendar.Body.ToString());
        calendar.Body.GetProperty("timeZone").GetString().ShouldBe("Europe/Berlin");
        var hours = calendar.Body.GetProperty("days")[0].GetProperty("staffHours")[0].GetProperty("intervals")[0];
        (hours.GetProperty("start").GetString(), hours.GetProperty("end").GetString()).ShouldBe(("09:00", "19:00"));
        calendar.Body.GetProperty("appointments")[0].GetProperty("startUtc").GetString().ShouldBe("2027-03-28T08:00:00Z");
        var panelSlots = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/branches/{_merkez}/availability/slots", new { items, date = "2027-03-28" }, _token);
        panelSlots.Body.GetProperty("slots").EnumerateArray()
            .Single(s => string.Equals(s.GetProperty("localTime").GetString(), "10:45", StringComparison.Ordinal))
            .GetProperty("startUtc").GetString().ShouldBe("2027-03-28T08:45:00Z");

        // SMS de şube yerel saatini yazar.
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        (await _api.ScalarAsync<string>("SELECT (payload->>'tarih') || ' ' || (payload->>'saat') FROM notifications.notification_outbox WHERE event_code = 'appointment.confirmed'"))
            .ShouldBe("Paz 28 Mar 10:00");
        (await RemindersAsync("Pending")).ShouldBe("03-27 08:00|-,03-28 06:00|same_day", "24 saat önce = Cumartesi 09:00 CET; 2 saat önce = 08:00 CEST");
    }

    /// <summary>Saati ileri bir ana getirir (kurulumdaki girişler saati birkaç dakika ilerletmiş olabilir).</summary>
    private void SetClock(Instant at)
    {
        var now = _factory.Clock.GetCurrentInstant();
        now.ShouldBeLessThanOrEqualTo(at);
        _factory.Clock.Advance(at - now);
    }

    private async Task<string?> RemindersAsync(string status)
    {
        var value = await _api.ScalarAsync<object>($"""
            SELECT string_agg(to_char(scheduled_utc AT TIME ZONE 'UTC', 'MM-DD HH24:MI') || '|' || coalesce(template_variant, '-'), ',' ORDER BY scheduled_utc)
            FROM notifications.notification_outbox WHERE event_code = 'appointment.reminder' AND status = '{status}'
            """);
        return value as string;
    }

    private Task<long> AppointmentMessagesAsync() =>
        _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code LIKE 'appointment.%'");

    private Task<string> StatusAsync(string id) => _api.ScalarAsync<string>($"SELECT status FROM scheduling.appointments WHERE id = '{id}'");

    private Task<int> NoShowCountAsync() => _api.ScalarAsync<int>($"SELECT no_show_count FROM customers.brand_customers WHERE id = '{_selin}'");

    private Task<string> SeriesDatesAsync() => _api.ScalarAsync<string>("""
        SELECT string_agg(to_char(start_utc AT TIME ZONE 'Europe/Istanbul', 'YYYY-MM-DD'), ',' ORDER BY start_utc)
        FROM scheduling.appointments WHERE series_id IS NOT NULL
        """);

    private async Task<ManageApi.Response> CancelAsync(ManageApi.Response created, bool notify)
    {
        var current = await _api.GetAsync($"/api/v1/manage/appointments/{Id(created)}", _token);
        return await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{Id(created)}/cancel",
            new { reason = "StaffUnavailable", notifyCustomer = notify }, _token, current.ETag);
    }

    private static string Id(ManageApi.Response created) => created.Body.GetProperty("appointment").GetProperty("id").GetString()!;

    private Task<ManageApi.Response> CreateAsync(string startUtc, bool notify, object? recurrence = null) =>
        PostAsync("/api/v1/manage/appointments", new
        {
            branchId = _merkez,
            brandCustomerId = _selin,
            channel = "Phone",
            startUtc,
            items = new[] { new { serviceId = _kesim, staffId = _elif } },
            recurrence,
            notifyCustomer = notify,
        });

    private Task<ManageApi.Response> PostAsync(string path, object body) => _api.SendAsync(HttpMethod.Post, path, body, _token, Key());

    private static object Week() => new
    {
        days = Enumerable.Range(1, 7).Select(d => new { dayOfWeek = d, intervals = new[] { new { start = "09:00", end = "19:00" } } }).ToArray(),
    };

    private static int[] Of(params int[] values) => values;

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{++_requests:D12}" };
}
