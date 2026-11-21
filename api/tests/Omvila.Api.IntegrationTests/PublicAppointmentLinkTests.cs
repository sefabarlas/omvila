using NodaTime;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-02 kabul kriteri 10 (SMS bağlantısı: görüntüle, 24 saatten fazla varken iptal → Cancelled ve saat yeniden
// alınabilir, 12 saatten az kala saat değişikliği cutoff_passed, geçersiz kod "Bu randevu bulunamadı"); durum-makinesi.md
// A6, A14; alan-modeli.md § AppointmentAccessCode; sözleşme public-appointment-link (If-Match, ETag); T-52.
// Saat: 2026-11-25 09:00Z (Çarşamba 12:00 İstanbul). Şube ve Elif Pzt–Cmt 09:00–19:00. Randevu Cuma 14:30 (11:30Z), 45 dk.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class PublicAppointmentLinkTests(PostgresFixture db) : IAsyncLifetime
{
    private const string AyşePhone = "+905321234567";
    private const string FridayAt1430 = "2026-11-27T11:30:00Z";
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private string _merkez = null!;
    private string _elif = null!;
    private string _kesim = null!;
    private string _bookingToken = null!;
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

        var week = new
        {
            days = Enumerable.Range(1, 6).Select(d => new { dayOfWeek = d, intervals = new[] { new { start = "09:00", end = "19:00" } } }).ToArray(),
        };
        var hours = await _api.GetAsync($"/api/v1/manage/branches/{_merkez}/working-hours", _token);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/branches/{_merkez}/working-hours", week, _token, hours.ETag)).Status.ShouldBe(200);
        var elif = await _api.GetAsync($"/api/v1/manage/staff/{_elif}", _token);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{_elif}/branches/{_merkez}/working-hours", week, _token, elif.ETag)).Status.ShouldBe(200);

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
        _bookingToken = await VerifyAsync();
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task Criterion10_ShowsAppointment_CancelPlanChanged_CalendarFreed_SlotBookableAgain()
    {
        var code = await BookAsync(FridayAt1430);

        var shown = await GetAsync(code);
        shown.Status.ShouldBe(200, shown.Body.ToString());
        shown.ETag.ShouldNotBeNull();
        shown.Body.GetProperty("accessCode").GetString().ShouldBe(code);
        shown.Body.GetProperty("status").GetString().ShouldBe("Confirmed");
        shown.Body.GetProperty("startUtc").GetString().ShouldBe(FridayAt1430);
        shown.Body.GetProperty("canCancel").GetBoolean().ShouldBeTrue();
        shown.Body.GetProperty("canReschedule").GetBoolean().ShouldBeTrue();
        shown.Body.GetProperty("branch").GetProperty("id").GetString().ShouldBe(_merkez);

        var cancelled = await CancelAsync(code, new { reason = "PlanChanged" }, shown.ETag);
        cancelled.Status.ShouldBe(200, cancelled.Body.ToString());
        string.Equals(cancelled.ETag, shown.ETag, StringComparison.Ordinal).ShouldBeFalse();
        cancelled.Body.GetProperty("status").GetString().ShouldBe("Cancelled");
        cancelled.Body.GetProperty("canCancel").GetBoolean().ShouldBeFalse();
        cancelled.Body.GetProperty("canReschedule").GetBoolean().ShouldBeFalse();
        cancelled.Body.GetProperty("cancellation").GetProperty("cancelledBy").GetString().ShouldBe("Customer");
        cancelled.Body.GetProperty("cancellation").GetProperty("reason").GetString().ShouldBe("PlanChanged");

        (await _api.ScalarAsync<int>("SELECT cancel_count FROM customers.brand_customers")).ShouldBe(1);
        (await _api.ScalarAsync<string>("SELECT payload::jsonb->>'reason' FROM infra.outbox_events WHERE type LIKE '%AppointmentCancelledByCustomer'"))
            .ShouldBe("PlanChanged");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM scheduling.appointment_item_segments WHERE is_active")).ShouldBe(0);
        (await GetAsync(code)).Body.GetProperty("status").GetString().ShouldBe("Cancelled", "iptal edilen randevu bağlantıdan görünmeye devam eder");

        var calendar = await _api.GetAsync($"/api/v1/manage/calendar?branchId={_merkez}&from=2026-11-27&to=2026-11-27", _token);
        calendar.Status.ShouldBe(200, calendar.Body.ToString());
        calendar.Body.GetProperty("appointments").GetArrayLength().ShouldBe(0);

        var slots = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/availability/slots", new { items = Items(), date = "2026-11-27" });
        slots.Body.GetProperty("slots").EnumerateArray().ShouldContain(s => s.GetProperty("startUtc").GetString() == FridayAt1430);
        var again = await CreateAsync(FridayAt1430);
        again.Status.ShouldBe(201, again.Body.ToString());
    }

    [Fact]
    public async Task Cancel_ReasonRequired_OtherNeedsNote_UnknownReasonRejected()
    {
        var code = await BookAsync(FridayAt1430);
        var etag = (await GetAsync(code)).ETag;

        var missing = await CancelAsync(code, new { note = "x" }, etag);
        missing.Status.ShouldBe(400, missing.Body.ToString());
        missing.Body.GetProperty("errors").GetProperty("reason")[0].GetString().ShouldBe("required");

        var other = await CancelAsync(code, new { reason = "Other" }, etag);
        other.Status.ShouldBe(400, other.Body.ToString());
        other.Body.GetProperty("errors").GetProperty("note")[0].GetString().ShouldBe("required");

        var salonReason = await CancelAsync(code, new { reason = "StaffUnavailable" }, etag);
        salonReason.Status.ShouldBe(400, salonReason.Body.ToString());
        salonReason.Body.GetProperty("errors").GetProperty("reason")[0].GetString().ShouldBe("invalid_format");

        (await _api.ScalarAsync<string>("SELECT status FROM scheduling.appointments")).ShouldBe("Confirmed");
        var withNote = await CancelAsync(code, new { reason = "Other", note = "Şehir dışındayım" }, etag);
        withNote.Status.ShouldBe(200, withNote.Body.ToString());
    }

    [Fact]
    public async Task Criterion10_LessThan12HoursLeft_RescheduleAndSlotsCutoffPassed_CancelToo()
    {
        var code = await BookAsync(FridayAt1430);
        _factory.Clock.Reset(Instant.FromUtc(2026, 11, 27, 0, 0)); // başlangıca 11,5 saat

        var shown = await GetAsync(code);
        shown.Body.GetProperty("canReschedule").GetBoolean().ShouldBeFalse();
        shown.Body.GetProperty("canCancel").GetBoolean().ShouldBeFalse();

        var moved = await RescheduleAsync(code, "2026-11-28T08:00:00Z", shown.ETag);
        moved.Status.ShouldBe(409, moved.Body.ToString());
        moved.Body.GetProperty("code").GetString().ShouldBe("cutoff_passed");

        var slots = await SlotsAsync(code, "2026-11-28");
        slots.Status.ShouldBe(409, slots.Body.ToString());
        slots.Body.GetProperty("code").GetString().ShouldBe("cutoff_passed");

        var cancelled = await CancelAsync(code, new { reason = "Sick" }, shown.ETag);
        cancelled.Status.ShouldBe(409, cancelled.Body.ToString());
        cancelled.Body.GetProperty("code").GetString().ShouldBe("cutoff_passed");
        (await _api.ScalarAsync<string>("SELECT start_utc::text FROM scheduling.appointments")).ShouldStartWith("2026-11-27 11:30");
    }

    [Fact]
    public async Task RescheduleSlots_IgnoreOwnAppointment_ExcludeOthers_RescheduleSucceeds_OutboxEvent()
    {
        var code = await BookAsync(FridayAt1430);
        (await CreateAsync("2026-11-27T13:00:00Z")).Status.ShouldBe(201); // 16:00 başka randevu

        var slots = await SlotsAsync(code, "2026-11-27");
        slots.Status.ShouldBe(200, slots.Body.ToString());
        var times = slots.Body.GetProperty("slots").EnumerateArray().Select(s => s.GetProperty("localTime").GetString()!).ToHashSet(StringComparer.Ordinal);
        times.Contains("14:00").ShouldBeTrue("kendi randevusuyla örtüşen saat boş sayılır");
        times.Contains("14:30").ShouldBeTrue();
        times.Contains("16:00").ShouldBeFalse("başka randevu");

        var etag = (await GetAsync(code)).ETag;
        var moved = await RescheduleAsync(code, "2026-11-27T11:00:00Z", etag);
        moved.Status.ShouldBe(200, moved.Body.ToString());
        string.Equals(moved.ETag, etag, StringComparison.Ordinal).ShouldBeFalse();
        moved.Body.GetProperty("startUtc").GetString().ShouldBe("2026-11-27T11:00:00Z");
        moved.Body.GetProperty("status").GetString().ShouldBe("Confirmed");
        (await _api.ScalarAsync<string>("SELECT payload::jsonb->>'changedBy' FROM infra.outbox_events WHERE type LIKE '%AppointmentRescheduled'")).ShouldBe("Customer");
        (await _api.ScalarAsync<string>("SELECT payload::jsonb->>'previousStartUtc' FROM infra.outbox_events WHERE type LIKE '%AppointmentRescheduled'"))
            .ShouldBe(FridayAt1430);

        var taken = await RescheduleAsync(code, "2026-11-27T13:00:00Z", moved.ETag);
        taken.Status.ShouldBe(409, taken.Body.ToString());
        taken.Body.GetProperty("code").GetString().ShouldBe("slot_taken");
        taken.Body.GetProperty("meta").GetProperty("alternatives").GetArrayLength().ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task StaleIfMatch_Is412_MissingIfMatch_Is428()
    {
        var code = await BookAsync(FridayAt1430);
        var stale = (await GetAsync(code)).ETag;
        (await RescheduleAsync(code, "2026-11-27T12:00:00Z", stale)).Status.ShouldBe(200);

        var reschedule = await RescheduleAsync(code, "2026-11-27T12:30:00Z", stale);
        reschedule.Status.ShouldBe(412, reschedule.Body.ToString());
        var cancel = await CancelAsync(code, new { reason = "PlanChanged" }, stale);
        cancel.Status.ShouldBe(412, cancel.Body.ToString());
        (await CancelAsync(code, new { reason = "PlanChanged" }, ifMatch: null)).Status.ShouldBe(428);
    }

    [Fact]
    public async Task CancelledAppointment_CannotBeRescheduled_InvalidStateTransition()
    {
        var code = await BookAsync(FridayAt1430);
        var cancelled = await CancelAsync(code, new { reason = "WantsDifferentTime" }, (await GetAsync(code)).ETag);
        cancelled.Status.ShouldBe(200, cancelled.Body.ToString());

        var moved = await RescheduleAsync(code, "2026-11-27T12:00:00Z", cancelled.ETag);
        moved.Status.ShouldBe(409, moved.Body.ToString());
        moved.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");
        var slots = await SlotsAsync(code, "2026-11-27");
        slots.Status.ShouldBe(409, slots.Body.ToString());
        slots.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");
        var twice = await CancelAsync(code, new { reason = "PlanChanged" }, cancelled.ETag);
        twice.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");
    }

    [Fact]
    public async Task UnknownMalformedRevokedOrExpiredCode_Is404()
    {
        var code = await BookAsync(FridayAt1430);
        var unknown = await GetAsync("Zz9Zz9Zz9Z");
        unknown.Status.ShouldBe(404);
        unknown.Body.GetProperty("code").GetString().ShouldBe("not_found");
        (await GetAsync("abc")).Status.ShouldBe(404);
        (await SlotsAsync("Zz9Zz9Zz9Z", "2026-11-27")).Status.ShouldBe(404);
        (await RescheduleAsync("Zz9Zz9Zz9Z", FridayAt1430, "\"1\"")).Status.ShouldBe(404);

        // Randevu bitişinden 7 gün sonra (12:15Z + 7 gün) bağlantı geçersiz.
        _factory.Clock.Reset(Instant.FromUtc(2026, 12, 4, 12, 14));
        (await GetAsync(code)).Status.ShouldBe(200);
        _factory.Clock.Reset(Instant.FromUtc(2026, 12, 4, 12, 15));
        (await GetAsync(code)).Status.ShouldBe(404);

        _factory.Clock.Reset(Instant.FromUtc(2026, 11, 25, 10, 0));
        await _api.ScalarAsync<int>("UPDATE scheduling.appointment_access_codes SET revoked_utc = now() RETURNING 1");
        (await GetAsync(code)).Status.ShouldBe(404);
    }

    [Fact]
    public async Task ReadOnlySubscription_LinkStillShowsAndCancels()
    {
        var code = await BookAsync(FridayAt1430);
        await _api.ScalarAsync<int>("UPDATE platform.subscriptions SET status = 'ReadOnly' RETURNING 1");

        var shown = await GetAsync(code);
        shown.Status.ShouldBe(200, shown.Body.ToString());
        (await CancelAsync(code, new { reason = "PlanChanged" }, shown.ETag)).Status.ShouldBe(200);
    }

    private object[] Items() => [new { serviceId = _kesim, staffId = _elif }];

    private async Task<string> BookAsync(string startUtc)
    {
        var created = await CreateAsync(startUtc);
        created.Status.ShouldBe(201, created.Body.ToString());
        return created.Body.GetProperty("accessCode").GetString()!;
    }

    private Task<ManageApi.Response> CreateAsync(string startUtc) =>
        _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/appointments", new
        {
            items = new[] { new { serviceId = _kesim, staffId = _elif } },
            sameStaffForAll = true,
            startUtc,
            fullName = "Ayşe Yılmaz",
            privacyNoticeAcknowledged = true,
            privacyNoticeVersion = "2026-11-01",
        }, _bookingToken, Key());

    private Task<ManageApi.Response> GetAsync(string code) => _api.SendAsync(HttpMethod.Get, $"/api/v1/public/appointments/{code}", null);

    private Task<ManageApi.Response> CancelAsync(string code, object body, string? ifMatch) =>
        _api.SendAsync(HttpMethod.Post, $"/api/v1/public/appointments/{code}/cancel", body, headers: IfMatch(ifMatch));

    private Task<ManageApi.Response> RescheduleAsync(string code, string startUtc, string? ifMatch) =>
        _api.SendAsync(HttpMethod.Post, $"/api/v1/public/appointments/{code}/reschedule", new { startUtc }, headers: IfMatch(ifMatch));

    private Task<ManageApi.Response> SlotsAsync(string code, string date) =>
        _api.SendAsync(HttpMethod.Post, $"/api/v1/public/appointments/{code}/availability/slots", new { date });

    private static Dictionary<string, string>? IfMatch(string? etag) =>
        etag is null ? null : new Dictionary<string, string>(StringComparer.Ordinal) { ["If-Match"] = etag };

    /// <summary>Kod iste → SMS kaydından kodu oku → doğrula; rezervasyon jetonunu döner.</summary>
    private async Task<string> VerifyAsync()
    {
        var requested = await _api.SendAsync(HttpMethod.Post, "/api/v1/public/otp/request", new { phone = AyşePhone, branchId = _merkez });
        requested.Status.ShouldBe(200, requested.Body.ToString());
        var code = await _api.ScalarAsync<string>(
            $"SELECT payload->>'kod' FROM notifications.notification_outbox WHERE event_code = 'otp.code' AND recipient_phone = '{AyşePhone}' ORDER BY created_utc DESC LIMIT 1");
        var verified = await _api.SendAsync(HttpMethod.Post, "/api/v1/public/otp/verify",
            new { otpId = requested.Body.GetProperty("challenge").GetProperty("otpId").GetGuid(), code });
        verified.Status.ShouldBe(200, verified.Body.ToString());
        return verified.Body.GetProperty("bookingToken").GetString()!;
    }

    private Task<ManageApi.Response> PostAsync(string path, object body) => _api.SendAsync(HttpMethod.Post, path, body, _token, Key());

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _requests):D12}" };
}
