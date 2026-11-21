using NodaTime;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-03 kabul kriterleri 1–6 (onay gerekli salonda online talep, Onayla, Reddet, 120 dk yanıtsız otomatik iptal,
// personel 403, müşteri saat değiştirince yeniden onay); durum-makinesi.md A1, A2, A4, A5, A13 ve § Sistem işleri
// (ExpireAppointmentHolds); sözleşme manageApproveAppointment, manageDeclineAppointment, manageListPendingApprovals
// (If-Match, hold_expired); T-62, T-63.
// Saat: 2026-11-25 09:00Z (Çarşamba 12:00 İstanbul). Şube ve Elif Pzt–Cmt 09:00–19:00. Talep Cuma 14:30 (11:30Z), 45 dk.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class ApprovalTests(PostgresFixture db) : IAsyncLifetime
{
    private const string AyşePhone = "+905321234567";
    private const string FridayAt1430 = "2026-11-27T11:30:00Z";
    private const string ExpireJob = "Omvila.Modules.Scheduling.Jobs.ExpireAppointmentHolds, Omvila.Modules.Scheduling";
    private const string DispatchSmsJob = "Omvila.Modules.Notifications.Jobs.DispatchNotifications, Omvila.Modules.Notifications";
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
        // Onay süresi (120 dk) jeton ömürlerinden uzun: testte saat ilerletildiğinde oturum düşmesin.
        _factory = new OmvilaApiFactory(db, new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["Auth:AccessTokenMinutes"] = "600",
            ["Auth:PublicBookingTokenMinutes"] = "600",
        });
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
        await _api.ScalarAsync<int>("UPDATE tenancy.brand_settings SET requires_approval = true RETURNING 1");
        _bookingToken = await VerifyAsync();
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    /// <summary>Talep şimdi oluşursa tutmanın biteceği an: şimdi + ApprovalTimeoutMinutes (120).</summary>
    private string HoldFromNow => Iso(_factory.Clock.GetCurrentInstant() + Duration.FromMinutes(120));

    private static string Iso(Instant at) => NodaTime.Text.InstantPattern.General.Format(at);

    [Fact]
    public async Task Criterion1_OnlineOnApprovalSalon_IsPending_Hold120_SlotClosed_RequestedEvent_NoConfirmation()
    {
        var now = Iso(_factory.Clock.GetCurrentInstant());
        var holdExpires = HoldFromNow;
        var created = await CreateAsync(FridayAt1430);

        created.Status.ShouldBe(201, created.Body.ToString());
        created.Body.GetProperty("status").GetString().ShouldBe("PendingApproval");
        created.Body.GetProperty("displayStatus").GetString().ShouldBe("PendingApproval");
        created.Body.GetProperty("holdExpiresUtc").GetString().ShouldBe(holdExpires);
        created.Body.GetProperty("canCancel").GetBoolean().ShouldBeTrue();

        var second = await CreateAsync(FridayAt1430);
        second.Status.ShouldBe(409, second.Body.ToString());
        second.Body.GetProperty("code").GetString().ShouldBe("slot_taken");

        (await OutboxCountAsync("AppointmentRequested")).ShouldBe(1);
        (await OutboxCountAsync("AppointmentConfirmed")).ShouldBe(0);
        (await _api.ScalarAsync<string>("SELECT payload::jsonb->>'holdExpiresUtc' FROM infra.outbox_events WHERE type LIKE '%.AppointmentRequested'"))
            .ShouldBe(holdExpires);

        // D2 işleyicileri onay bekleyen talebe "onaylandı" SMS'i ve hatırlatma yazmaz.
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        (await _api.ScalarAsync<long>(
            "SELECT count(*) FROM notifications.notification_outbox WHERE event_code IN ('appointment.confirmed', 'appointment.reminder')")).ShouldBe(0);

        var queue = await _api.GetAsync("/api/v1/manage/approval-queue", _token);
        queue.Status.ShouldBe(200, queue.Body.ToString());
        queue.Body.GetProperty("generatedUtc").GetString().ShouldBe(now);
        queue.Body.GetProperty("totalCount").GetInt32().ShouldBe(1);
        queue.Body.GetProperty("earliestHoldExpiresUtc").GetString().ShouldBe(holdExpires);
        var item = queue.Body.GetProperty("items")[0];
        item.GetProperty("appointmentId").GetString().ShouldBe(created.Body.GetProperty("id").GetString());
        item.GetProperty("branchId").GetString().ShouldBe(_merkez);
        item.GetProperty("timeZone").GetString().ShouldBe("Europe/Istanbul");
        item.GetProperty("customerName").GetString().ShouldBe("Ayşe Yılmaz");
        item.GetProperty("isNewCustomer").GetBoolean().ShouldBeTrue();
        item.GetProperty("serviceSummary").GetString().ShouldBe("Saç kesimi");
        item.GetProperty("staffName").GetString().ShouldNotBeNullOrEmpty();
        item.GetProperty("holdExpiresUtc").GetString().ShouldBe(holdExpires);
        item.GetProperty("channel").GetString().ShouldBe("Online");

        var filtered = await _api.GetAsync($"/api/v1/manage/approval-queue?branchId={_merkez}", _token);
        filtered.Body.GetProperty("totalCount").GetInt32().ShouldBe(1);
    }

    [Fact]
    public async Task Criterion2_Approve_IsConfirmed_ApprovedEvent_History_ThenInvalidTransition()
    {
        var hold = HoldFromNow;
        var id = (await CreateAsync(FridayAt1430)).Body.GetProperty("id").GetString()!;
        var detail = await DetailAsync(id);
        detail.Body.GetProperty("status").GetString().ShouldBe("PendingApproval");
        detail.Body.GetProperty("holdExpiresUtc").GetString().ShouldBe(hold);

        _factory.Clock.Advance(Duration.FromMinutes(25));
        var stale = await ApproveAsync(id, "W/\"1\"");
        stale.Status.ShouldBe(412, stale.Body.ToString());
        var approved = await ApproveAsync(id, detail.ETag);

        approved.Status.ShouldBe(200, approved.Body.ToString());
        approved.ETag.ShouldNotBeNull();
        string.Equals(approved.ETag, detail.ETag, StringComparison.Ordinal).ShouldBeFalse();
        approved.Body.GetProperty("status").GetString().ShouldBe("Confirmed");
        (!approved.Body.TryGetProperty("holdExpiresUtc", out var cleared) || cleared.ValueKind == System.Text.Json.JsonValueKind.Null).ShouldBeTrue();
        Kinds(approved).ShouldBe(["Created", "Approved"]);
        (await OutboxCountAsync("AppointmentApproved")).ShouldBe(1);
        (await _api.GetAsync("/api/v1/manage/approval-queue", _token)).Body.GetProperty("totalCount").GetInt32().ShouldBe(0);

        var again = await ApproveAsync(id, approved.ETag);
        again.Status.ShouldBe(409);
        again.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");
        (await DeclineAsync(id, approved.ETag)).Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");
    }

    [Fact]
    public async Task Criterion3_Decline_IsCancelledDeclinedBySalon_CustomerSeesCancelled_NeverDeclinedWording_SlotFreed()
    {
        var created = await CreateAsync(FridayAt1430);
        var id = created.Body.GetProperty("id").GetString()!;
        var code = created.Body.GetProperty("accessCode").GetString()!;
        var detail = await DetailAsync(id);

        var declined = await DeclineAsync(id, detail.ETag);

        declined.Status.ShouldBe(200, declined.Body.ToString());
        declined.Body.GetProperty("status").GetString().ShouldBe("Cancelled");
        declined.Body.GetProperty("cancellation").GetProperty("reason").GetString().ShouldBe("DeclinedBySalon");
        declined.Body.GetProperty("cancellation").GetProperty("cancelledBy").GetString().ShouldBe("Salon");
        Kinds(declined).ShouldBe(["Created", "Declined"]);
        (await OutboxCountAsync("AppointmentDeclined")).ShouldBe(1);

        var shown = await _api.SendAsync(HttpMethod.Get, $"/api/v1/public/appointments/{code}", null);
        shown.Status.ShouldBe(200);
        shown.Body.GetProperty("status").GetString().ShouldBe("Cancelled");
        shown.Body.GetProperty("displayStatus").GetString().ShouldBe("Cancelled");
        shown.Body.GetProperty("canCancel").GetBoolean().ShouldBeFalse();
        shown.Body.GetRawText().ShouldNotContain("reddedildi", Case.Insensitive);

        (await CreateAsync(FridayAt1430)).Status.ShouldBe(201, "saat yeniden alınabilir");
    }

    [Fact]
    public async Task Criterion4_Unanswered121Minutes_JobCancelsApprovalExpired_SlotFreed_Event()
    {
        var id = (await CreateAsync(FridayAt1430)).Body.GetProperty("id").GetString()!;

        _factory.Clock.Advance(Duration.FromMinutes(119));
        await _api.RunJobAsync(ExpireJob);
        (await DetailAsync(id)).Body.GetProperty("status").GetString().ShouldBe("PendingApproval");

        _factory.Clock.Advance(Duration.FromMinutes(2));
        await _api.RunJobAsync(ExpireJob);

        var detail = await DetailAsync(id);
        detail.Body.GetProperty("status").GetString().ShouldBe("Cancelled");
        detail.Body.GetProperty("cancellation").GetProperty("reason").GetString().ShouldBe("ApprovalExpired");
        detail.Body.GetProperty("cancellation").GetProperty("cancelledBy").GetString().ShouldBe("System");
        Kinds(detail).ShouldBe(["Created", "HoldExpired"]);
        (await OutboxCountAsync("AppointmentApprovalExpired")).ShouldBe(1);
        (await _api.ScalarAsync<string>("SELECT payload::jsonb->>'reason' FROM infra.outbox_events WHERE type LIKE '%.AppointmentApprovalExpired'"))
            .ShouldBe("ApprovalExpired");

        var queue = await _api.GetAsync("/api/v1/manage/approval-queue", _token);
        queue.Body.GetProperty("totalCount").GetInt32().ShouldBe(0);
        queue.Body.GetProperty("earliestHoldExpiresUtc").ValueKind.ShouldBe(System.Text.Json.JsonValueKind.Null);
        queue.Body.GetProperty("items").GetArrayLength().ShouldBe(0);

        await _api.RunJobAsync(ExpireJob);
        (await OutboxCountAsync("AppointmentApprovalExpired")).ShouldBe(1, "iş ikinci kez iptal etmez");
        (await CreateAsync(FridayAt1430)).Status.ShouldBe(201, "saat boşaldı");
    }

    [Fact]
    public async Task Criterion5_StaffRole_ApproveDeclineQueue_AreForbidden()
    {
        var id = (await CreateAsync(FridayAt1430)).Body.GetProperty("id").GetString()!;
        var etag = (await DetailAsync(id)).ETag;
        var staff = await _api.AddMemberAndSignInAsync(_token, "+905382741905", "Staff", _merkez);

        var approve = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/approve", new { }, staff, IfMatch(etag));
        approve.Status.ShouldBe(403, approve.Body.ToString());
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/decline", null, staff, IfMatch(etag))).Status.ShouldBe(403);
        (await _api.GetAsync("/api/v1/manage/approval-queue", staff)).Status.ShouldBe(403);
        (await DetailAsync(id)).Body.GetProperty("status").GetString().ShouldBe("PendingApproval");
    }

    [Fact]
    public async Task Criterion6_CustomerReschedule_AfterApproval_BackToPending_WithNewHold()
    {
        var created = await CreateAsync(FridayAt1430);
        var id = created.Body.GetProperty("id").GetString()!;
        var code = created.Body.GetProperty("accessCode").GetString()!;
        (await ApproveAsync(id, (await DetailAsync(id)).ETag)).Status.ShouldBe(200);

        _factory.Clock.Advance(Duration.FromMinutes(30));
        var newHold = HoldFromNow;
        var shown = await _api.SendAsync(HttpMethod.Get, $"/api/v1/public/appointments/{code}", null);
        shown.Body.GetProperty("status").GetString().ShouldBe("Confirmed");
        var moved = await _api.SendAsync(
            HttpMethod.Post, $"/api/v1/public/appointments/{code}/reschedule", new { startUtc = "2026-11-27T12:30:00Z" }, headers: IfMatch(shown.ETag));

        moved.Status.ShouldBe(200, moved.Body.ToString());
        moved.Body.GetProperty("status").GetString().ShouldBe("PendingApproval");
        moved.Body.GetProperty("holdExpiresUtc").GetString().ShouldBe(newHold);
        moved.Body.GetProperty("startUtc").GetString().ShouldBe("2026-11-27T12:30:00Z");
        (await OutboxCountAsync("AppointmentRequested")).ShouldBe(2);
        (await _api.ScalarAsync<long>(
            "SELECT count(*) FROM infra.outbox_events WHERE type LIKE '%.AppointmentRequested' AND payload::jsonb->>'isReschedule' = 'true'")).ShouldBe(1);
        (await OutboxCountAsync("AppointmentRescheduled")).ShouldBe(1);

        var queue = await _api.GetAsync("/api/v1/manage/approval-queue", _token);
        queue.Body.GetProperty("items")[0].GetProperty("startUtc").GetString().ShouldBe("2026-11-27T12:30:00Z");
    }

    [Fact]
    public async Task HoldPassed_ButJobNotRun_ApproveAndDecline_AreHoldExpired()
    {
        var id = (await CreateAsync(FridayAt1430)).Body.GetProperty("id").GetString()!;
        var etag = (await DetailAsync(id)).ETag;

        _factory.Clock.Advance(Duration.FromMinutes(120));

        var approve = await ApproveAsync(id, etag);
        approve.Status.ShouldBe(409, approve.Body.ToString());
        approve.Body.GetProperty("code").GetString().ShouldBe("hold_expired");
        (await DeclineAsync(id, etag)).Body.GetProperty("code").GetString().ShouldBe("hold_expired");
        (await DetailAsync(id)).Body.GetProperty("status").GetString().ShouldBe("PendingApproval");
        (await _api.GetAsync("/api/v1/manage/approval-queue", _token)).Body.GetProperty("totalCount").GetInt32().ShouldBe(0, "yanıtlanamayan talep kuyrukta değil");
    }

    [Fact]
    public async Task ParallelApproveAndExpiryJob_ExactlyOneOutcome()
    {
        var id = (await CreateAsync(FridayAt1430)).Body.GetProperty("id").GetString()!;
        var etag = (await DetailAsync(id)).ETag;
        _factory.Clock.Advance(Duration.FromMinutes(120));

        var approve = Task.Run(() => ApproveAsync(id, etag), TestContext.Current.CancellationToken);
        var job = Task.Run(() => _api.RunJobAsync(ExpireJob), TestContext.Current.CancellationToken);
        await Task.WhenAll(approve, job);

        var status = (await DetailAsync(id)).Body.GetProperty("status").GetString();
        var approved = (await approve).Status == 200;
        status.ShouldBe(approved ? "Confirmed" : "Cancelled");
        (await OutboxCountAsync("AppointmentApproved") + await OutboxCountAsync("AppointmentApprovalExpired")).ShouldBe(1);
    }

    [Fact]
    public async Task PanelAppointment_OnApprovalSalon_StaysConfirmed()
    {
        var created = await PostAsync("/api/v1/manage/appointments", new
        {
            branchId = _merkez,
            newCustomer = new { fullName = "Selin Öztürk", phone = "+905550004418" },
            channel = "Phone",
            startUtc = FridayAt1430,
            items = new[] { new { serviceId = _kesim, staffId = _elif } },
            notifyCustomer = false,
        });
        created.Status.ShouldBe(201, created.Body.ToString());
        created.Body.GetProperty("appointment").GetProperty("status").GetString().ShouldBe("Confirmed");
        (await OutboxCountAsync("AppointmentRequested")).ShouldBe(0);
    }

    // bildirimler.md appointment.requested; T-65: onay bekleyen talebe hatırlatma planlanmaz.
    [Fact]
    public async Task Requested_CustomerGetsRequestReceivedSms_NoReminders()
    {
        var created = await CreateAsync(FridayAt1430);
        var code = created.Body.GetProperty("accessCode").GetString()!;

        await DispatchAsync();

        (await PayloadsAsync("appointment.requested", "salon", "tarih", "saat", "bağlantı"))
            .ShouldBe($"{AyşePhone}|Kuaför Elif|Cum 27 Kas|14:30|omvila.com/r/{code}");
        (await SentBodiesAsync()).ShouldContain(
            $"Kuaför Elif: Talebiniz alındı. Cum 27 Kas 14:30 için salon onayladığında size haber vereceğiz. omvila.com/r/{code}");
        (await RemindersAsync("Pending")).ShouldBeNull();
    }

    // bildirimler.md appointment.approved; hatırlatmalar onay anında planlanır (24 sa ve 2 sa önce).
    [Fact]
    public async Task Approved_CustomerGetsApprovalSms_RemindersScheduledAtApproval()
    {
        var created = await CreateAsync(FridayAt1430);
        var id = created.Body.GetProperty("id").GetString()!;
        var code = created.Body.GetProperty("accessCode").GetString()!;
        await DispatchAsync();

        (await ApproveAsync(id, (await DetailAsync(id)).ETag)).Status.ShouldBe(200);
        await DispatchAsync();

        (await PayloadsAsync("appointment.approved", "salon", "tarih", "saat", "hizmet", "bağlantı"))
            .ShouldBe($"{AyşePhone}|Kuaför Elif|Cum 27 Kas|14:30|Saç kesimi|omvila.com/r/{code}");
        (await SentBodiesAsync()).ShouldContain($"Kuaför Elif talebinizi onayladı: Cum 27 Kas 14:30, Saç kesimi. Görüşmek üzere. omvila.com/r/{code}");
        (await RemindersAsync("Pending")).ShouldBe("11-26 11:30|-,11-27 09:30|same_day");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'appointment.confirmed'")).ShouldBe(0);
    }

    // Kriter 3: müşteri SMS'inde "iptal" yazar, "reddedildi" geçmez; rezervasyon sayfası bağlantısı.
    [Fact]
    public async Task Declined_CustomerSmsSaysCancelled_NeverRejected_WithBookingLink()
    {
        var id = (await CreateAsync(FridayAt1430)).Body.GetProperty("id").GetString()!;
        await DispatchAsync();

        (await DeclineAsync(id, (await DetailAsync(id)).ETag)).Status.ShouldBe(200);
        await DispatchAsync();

        (await PayloadsAsync("appointment.declined", "salon", "tarih", "saat", "rezervasyon bağlantısı"))
            .ShouldBe($"{AyşePhone}|Kuaför Elif|Cum 27 Kas|14:30|omvila.com/kuafor-elif");
        var bodies = await SentBodiesAsync();
        bodies.ShouldContain("Kuaför Elif, Cum 27 Kas 14:30 talebinizi onaylayamadı, talep iptal edildi. Başka bir saat seçin: omvila.com/kuafor-elif");
        bodies.ShouldNotContain("reddedil", Case.Insensitive);
        (await RemindersAsync("Pending")).ShouldBeNull();
    }

    // Kriter 4: 120 dk yanıtsız → müşteriye "zamanında yanıtlayamadı, talep iptal edildi".
    [Fact]
    public async Task ApprovalExpired_CustomerGetsExpiredSms()
    {
        await CreateAsync(FridayAt1430);
        await DispatchAsync();

        _factory.Clock.Advance(Duration.FromMinutes(120));
        await _api.RunJobAsync(ExpireJob);
        await DispatchAsync();

        (await PayloadsAsync("appointment.approval_expired", "salon", "tarih", "saat", "rezervasyon bağlantısı"))
            .ShouldBe($"{AyşePhone}|Kuaför Elif|Cum 27 Kas|14:30|omvila.com/kuafor-elif");
        (await SentBodiesAsync()).ShouldContain(
            "Kuaför Elif, Cum 27 Kas 14:30 talebinizi zamanında yanıtlayamadı, talep iptal edildi. Başka bir saat seçin: omvila.com/kuafor-elif");
    }

    // Kriter 6 + A13: onaylı randevunun saati müşteri tarafından değişip yeniden onaya düşünce hatırlatmalar düşer, "Talebiniz
    // alındı" yeniden gider; ikinci onayda yeni saate göre planlanır.
    [Fact]
    public async Task CustomerReschedule_BackToPending_CancelsReminders_ReapprovalReschedulesThem()
    {
        var created = await CreateAsync(FridayAt1430);
        var id = created.Body.GetProperty("id").GetString()!;
        var code = created.Body.GetProperty("accessCode").GetString()!;
        await DispatchAsync();
        (await ApproveAsync(id, (await DetailAsync(id)).ETag)).Status.ShouldBe(200);
        await DispatchAsync();
        (await RemindersAsync("Pending")).ShouldBe("11-26 11:30|-,11-27 09:30|same_day");

        var shown = await _api.SendAsync(HttpMethod.Get, $"/api/v1/public/appointments/{code}", null);
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/appointments/{code}/reschedule", new { startUtc = "2026-11-27T12:30:00Z" },
            headers: IfMatch(shown.ETag))).Status.ShouldBe(200);
        await DispatchAsync();

        (await RemindersAsync("Pending")).ShouldBeNull();
        (await _api.ScalarAsync<string>(
            "SELECT string_agg(payload->>'saat', ',' ORDER BY created_utc) FROM notifications.notification_outbox WHERE event_code = 'appointment.requested'"))
            .ShouldBe("14:30,15:30");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'appointment.rescheduled'")).ShouldBe(0);

        (await ApproveAsync(id, (await DetailAsync(id)).ETag)).Status.ShouldBe(200);
        await DispatchAsync();

        (await RemindersAsync("Pending")).ShouldBe("11-26 12:30|-,11-27 10:30|same_day");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'appointment.approved'")).ShouldBe(2);
    }

    // İşleyici çalışana kadar talep onaylandıysa eski "Talebiniz alındı" yazılmaz; yalnız onay mesajı gider.
    [Fact]
    public async Task RequestedProcessedAfterApproval_NoStaleRequestReceivedSms()
    {
        var id = (await CreateAsync(FridayAt1430)).Body.GetProperty("id").GetString()!;
        (await ApproveAsync(id, (await DetailAsync(id)).ETag)).Status.ShouldBe(200);
        await DispatchAsync();

        (await _api.ScalarAsync<string>(
            "SELECT string_agg(event_code, ',' ORDER BY event_code) FROM notifications.notification_outbox WHERE event_code LIKE 'appointment.%' AND event_code <> 'appointment.reminder'"))
            .ShouldBe("appointment.approved");
    }

    // T-46 (3): "Randevu bilgilendirme SMS'leri" kapalı müşteriye onay akışı SMS'i yazılmaz.
    [Fact]
    public async Task CustomerWithAppointmentSmsDisabled_GetsNoApprovalFlowSms()
    {
        var id = (await CreateAsync(FridayAt1430)).Body.GetProperty("id").GetString()!;
        await _api.ScalarAsync<int>("UPDATE customers.brand_customers SET appointment_sms_enabled = false RETURNING 1");
        (await ApproveAsync(id, (await DetailAsync(id)).ETag)).Status.ShouldBe(200);
        await DispatchAsync();

        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code LIKE 'appointment.%'")).ShouldBe(0);
    }

    /// <summary>Olayları işler ve bekleyen SMS'leri gönderir (sahte sağlayıcı; deneme kontörüyle).</summary>
    private async Task DispatchAsync()
    {
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        await _api.RunJobAsync(DispatchSmsJob);
    }

    private Task<string> PayloadsAsync(string eventCode, params string[] keys) =>
        _api.ScalarAsync<string>($"""
            SELECT string_agg(recipient_phone || '|' || {string.Join(" || '|' || ", keys.Select(k => $"(payload->>'{k}')"))}, ',')
            FROM notifications.notification_outbox WHERE event_code = '{eventCode}'
            """);

    /// <summary>Müşteriye giden SMS metinleri (giriş kodu hariç).</summary>
    private async Task<string> SentBodiesAsync()
    {
        var joined = await _api.ScalarAsync<object>(
            $"SELECT string_agg(body, E'\\n') FROM infra.dev_sms_log WHERE phone_e164 = '{AyşePhone}' AND body NOT LIKE 'OMVILA giriş kodunuz%'");
        return joined as string ?? string.Empty;
    }

    private async Task<string?> RemindersAsync(string status) =>
        await _api.ScalarAsync<object>($"""
            SELECT string_agg(to_char(scheduled_utc AT TIME ZONE 'UTC', 'MM-DD HH24:MI') || '|' || coalesce(template_variant, '-'), ',' ORDER BY scheduled_utc)
            FROM notifications.notification_outbox WHERE event_code = 'appointment.reminder' AND status = '{status}'
            """) as string;

    private static string[] Kinds(ManageApi.Response response) =>
        [.. response.Body.GetProperty("history").EnumerateArray().Select(e => e.GetProperty("kind").GetString()!)];

    private Task<ManageApi.Response> DetailAsync(string id) => _api.GetAsync($"/api/v1/manage/appointments/{id}", _token);

    private Task<ManageApi.Response> ApproveAsync(string id, string? etag) =>
        _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/approve", null, _token, IfMatch(etag));

    private Task<ManageApi.Response> DeclineAsync(string id, string? etag) =>
        _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/decline", null, _token, IfMatch(etag));

    private Task<long> OutboxCountAsync(string type) => _api.ScalarAsync<long>($"SELECT count(*) FROM infra.outbox_events WHERE type LIKE '%.{type}'");

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
