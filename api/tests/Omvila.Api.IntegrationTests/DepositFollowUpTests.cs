using System.Text.Json;
using NodaTime;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-04 kabul kriterleri 4 ("Havaleyi yaptım" → depositCustomerReported, salon push'u, durum değişmez) ve 6 (süre
// dolunca Cancelled/DepositNotReceived + "Havale yaptıysanız salonu arayın" SMS'i); V6 ("Hatırlat" randevu başına bir kez),
// V7 ("Salon iade etti": Owner + BranchManager, tutar ≤ alınan, kısmi, tek kez), V5 (personel 403); durum-makinesi.md A5, A13,
// A14; bildirimler.md deposit.awaiting, deposit.not_received, staff.deposit_reported, appointment.cancelled_by_salon (kapora
// eki); sözleşme publicReportDepositTransfer, manageRemindDeposit, manageMarkDepositRefunded. Saat: 2026-11-25 09:00Z.
// Randevu Cuma 14:30 (11:30Z), Balyaj 2.800 ₺, kapora %30 = 840 ₺, havale süresi 120 dk.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class DepositFollowUpTests(PostgresFixture db) : IAsyncLifetime
{
    private const string RiskyPhone = "+905321234567";
    private const string OtherPhone = "+905339876543";
    private const string FridayAt1430 = "2026-11-27T11:30:00Z";
    private const string DispatchSmsJob = "Omvila.Modules.Notifications.Jobs.DispatchNotifications, Omvila.Modules.Notifications";
    private const string ExpireJob = "Omvila.Modules.Scheduling.Jobs.ExpireAppointmentHolds, Omvila.Modules.Scheduling";
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private string _brandId = null!;
    private string _merkez = null!;
    private string _elif = null!;
    private string _balyaj = null!;
    private int _requests;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db, new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["Auth:AccessTokenMinutes"] = "600",
            ["Auth:PublicBookingTokenMinutes"] = "600",
        });
        _api = new ManageApi(_factory, db);

        // Sahte SMS günlüğü ResetAsync ile boşalmaz: bu sınıfın müşteri telefonlarının eski kayıtları silinir.
        await _api.ScalarAsync<long>(
            $"WITH d AS (DELETE FROM infra.dev_sms_log WHERE phone_e164 IN ('{RiskyPhone}', '{OtherPhone}') RETURNING 1) SELECT count(*) FROM d");
        _token = await _api.SignupAsync("+905324187705", "Kuaför Elif");
        var session = (await _api.GetAsync("/api/v1/manage/session", _token)).Body;
        _brandId = session.GetProperty("brand").GetProperty("id").GetString()!;
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

        (await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/brand/bank-account", new
        {
            accountHolderName = "Elif Kaya",
            bankName = "Ziraat Bankası",
            iban = "TR29 0006 4000 0011 2345 6789 01",
            transferCodePrefix = "OMV",
            transferWindowMinutes = 120,
        }, _token, "W/\"0\"")).Status.ShouldBe(200);
        var category = (await PostAsync("/api/v1/manage/service-categories", new { name = "Saç" })).Body.GetProperty("id").GetString()!;
        _balyaj = (await PostAsync("/api/v1/manage/services", new
        {
            categoryId = category,
            name = "Balyaj",
            durationMinutes = 60,
            price = new { amountMinor = 280000, currency = "TRY" },
            priceDisplay = "Fixed",
            deposit = new { mode = "Inherit", percent = 30 },
            isOnlineBookable = true,
            staffIds = new[] { _elif },
        })).Body.GetProperty("id").GetString()!;

        // Marka varsayılanı RiskyCustomersOnly, eşik 2: bu müşteri kapora öder, diğeri ödemez.
        var risky = (await PostAsync("/api/v1/manage/customers", new { fullName = "Ayşe Yılmaz", phone = RiskyPhone })).Body.GetProperty("id").GetString()!;
        await _api.ScalarAsync<int>($"UPDATE customers.brand_customers SET no_show_count = 2 WHERE id = '{risky}' RETURNING 1");
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    // ───────── "Havaleyi yaptım" (kabul kriteri 4) ─────────

    [Fact]
    public async Task Criterion4_Report_CustomerReported_StatusUnchanged_ApproversNotified_StaffNot()
    {
        var staff = await _api.AddMemberAndSignInAsync(_token, "+905382741905", "Staff", _merkez);
        var manager = await _api.AddMemberAndSignInAsync(_token, "+905382741906", "BranchManager", _merkez);
        var (id, _, code) = await CreateDepositPendingAsync();
        var at = Iso(_factory.Clock.GetCurrentInstant());

        var reported = await ReportAsync(code);

        reported.Status.ShouldBe(200, reported.Body.ToString());
        reported.ETag.ShouldNotBeNull();
        reported.Body.GetProperty("status").GetString().ShouldBe("PendingApproval");
        reported.Body.GetProperty("displayStatus").GetString().ShouldBe("DepositPending");
        var deposit = reported.Body.GetProperty("deposit");
        deposit.GetProperty("status").GetString().ShouldBe("CustomerReported");
        deposit.GetProperty("customerReportedUtc").GetString().ShouldBe(at);
        deposit.GetProperty("instructions").GetProperty("iban").GetString().ShouldNotBeNullOrEmpty("havale bilgisi görünmeye devam eder");
        var detail = (await DetailAsync(id)).Body;
        detail.GetProperty("deposit").GetProperty("status").GetString().ShouldBe("CustomerReported");
        detail.GetProperty("history").EnumerateArray().Select(e => e.GetProperty("kind").GetString()).ShouldBe(["Created", "DepositReported"]);

        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);

        var page = (await _api.GetAsync("/api/v1/manage/notifications", _token)).Body;
        var item = page.GetProperty("items").EnumerateArray().Single(i => string.Equals(i.GetProperty("event").GetString(), "staff.deposit_reported", StringComparison.Ordinal));
        item.GetProperty("title").GetString().ShouldBe("Havale bildirildi");
        item.GetProperty("body").GetString().ShouldBe("Ayşe Yılmaz · 840 ₺ kaporayı gönderdiğini söyledi. Kontrol edin.");
        item.GetProperty("branchId").GetString().ShouldBe(_merkez);
        item.GetProperty("target").GetProperty("type").GetString().ShouldBe("Deposit");
        item.GetProperty("target").GetProperty("id").GetString().ShouldBe(id);
        (await UnreadAsync(manager, "staff.deposit_reported")).ShouldBe(1);
        (await UnreadAsync(staff, "staff.deposit_reported")).ShouldBe(0, "V5: personel kapora işaretleyemez");
        (await _api.ScalarAsync<string>(
            "SELECT string_agg(DISTINCT (payload->>'müşteri') || '|' || (payload->>'tutar') || '|' || reference_id, ',') FROM notifications.notification_outbox WHERE event_code = 'staff.deposit_reported'"))
            .ShouldBe($"Ayşe Y.|840 ₺|{id}");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'staff.deposit_reported'"))
            .ShouldBe(2, "Owner ve şubenin yöneticisi");
    }

    [Fact]
    public async Task Report_Twice_SameResult_NotifiesOnce()
    {
        var (_, _, code) = await CreateDepositPendingAsync();
        var first = await ReportAsync(code);
        _factory.Clock.Advance(Duration.FromMinutes(3));

        var again = await ReportAsync(code);

        again.Status.ShouldBe(200, again.Body.ToString());
        again.Body.GetProperty("deposit").GetProperty("customerReportedUtc").GetString()
            .ShouldBe(first.Body.GetProperty("deposit").GetProperty("customerReportedUtc").GetString());
        string.Equals(again.ETag, first.ETag, StringComparison.Ordinal).ShouldBeTrue("ikinci çağrı yazmaz");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM infra.outbox_events WHERE type LIKE '%.AppointmentDepositReported'")).ShouldBe(1);
    }

    [Fact]
    public async Task Report_NoDeposit_Expired_Received_UnknownCode_AreRejected()
    {
        var plain = await CreateAsync(OtherPhone, "Derya Şahin", "2026-11-27T13:30:00Z");
        var noDeposit = await ReportAsync(plain.Body.GetProperty("accessCode").GetString()!);
        noDeposit.Status.ShouldBe(409, noDeposit.Body.ToString());
        noDeposit.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");

        var (id, _, code) = await CreateDepositPendingAsync();
        (await MarkReceivedAsync(id)).Status.ShouldBe(200);
        (await ReportAsync(code)).Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");

        var (_, _, expiring) = await CreateDepositPendingAsync("2026-11-27T14:30:00Z");
        _factory.Clock.Advance(Duration.FromMinutes(120));
        var expired = await ReportAsync(expiring);
        expired.Status.ShouldBe(409, expired.Body.ToString());
        expired.Body.GetProperty("code").GetString().ShouldBe("hold_expired");

        (await ReportAsync("ZZZZZZZZZZ")).Status.ShouldBe(404);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM infra.outbox_events WHERE type LIKE '%.AppointmentDepositReported'")).ShouldBe(0);
    }

    // ───────── "Hatırlat" (V6) ─────────

    [Fact]
    public async Task Remind_ResendsDepositAwaitingSms_RecordsTime_OncePerAppointment()
    {
        var (id, transferCode, _) = await CreateDepositPendingAsync();
        await DispatchAsync();
        _factory.Clock.Advance(Duration.FromMinutes(30));
        var at = Iso(_factory.Clock.GetCurrentInstant());

        var reminded = await RemindAsync(id, _token);

        reminded.Status.ShouldBe(204, reminded.Body.ToString());
        var detail = (await DetailAsync(id)).Body;
        detail.GetProperty("deposit").GetProperty("remindedUtc").GetString().ShouldBe(at);
        detail.GetProperty("status").GetString().ShouldBe("PendingApproval");
        detail.GetProperty("history").EnumerateArray().Select(e => e.GetProperty("kind").GetString()).ShouldBe(["Created", "DepositReminded"]);

        await DispatchAsync();
        var bodies = await _api.ScalarAsync<string>(
            $"SELECT string_agg(body, '§' ORDER BY sent_utc) FROM infra.dev_sms_log WHERE phone_e164 = '{RiskyPhone}' AND body LIKE '%kaporayı%'");
        var sms = bodies.Split('§');
        sms.Length.ShouldBe(2, "oluşturmadaki ve hatırlatmadaki kapora SMS'i");
        sms[1].ShouldContain("Cum 27 Kas 14:30 için 840 TL kaporayı 1 sa 30 dk içinde gönderin. IBAN TR290006400000112345678901");
        sms[1].ShouldEndWith($"açıklama {transferCode}");
        (await _api.ScalarAsync<string>(
            "SELECT string_agg(array_to_string(channel_plan, ',') || '|' || status, ',') FROM notifications.notification_outbox WHERE event_code = 'deposit.awaiting'"))
            .ShouldBe("Sms|Sent,Sms|Sent");

        var again = await RemindAsync(id, _token);
        again.Status.ShouldBe(409, again.Body.ToString());
        again.Body.GetProperty("code").GetString().ShouldBe("deposit_reminder_already_sent");
    }

    [Fact]
    public async Task Remind_WithoutCredits_IsInsufficientCredits_RightNotSpent()
    {
        var (id, _, _) = await CreateDepositPendingAsync();
        await SetCreditsAsync(0);

        var refused = await RemindAsync(id, _token);

        refused.Status.ShouldBe(409, refused.Body.ToString());
        refused.Body.GetProperty("code").GetString().ShouldBe("insufficient_credits");
        IsNullOrAbsent((await DetailAsync(id)).Body.GetProperty("deposit"), "remindedUtc").ShouldBeTrue();
        (await _api.ScalarAsync<long>("SELECT count(*) FROM infra.outbox_events WHERE type LIKE '%.AppointmentDepositReminded'")).ShouldBe(0);

        await SetCreditsAsync(10);
        (await RemindAsync(id, _token)).Status.ShouldBe(204, "hak harcanmamıştı");
    }

    [Fact]
    public async Task Remind_ReadOnlySubscription_IsForbidden_NothingWritten()
    {
        // Kural: durum-makinesi.md § 8 ReadOnly ("SMS kapalı"); K5 (salonun yeni başlattığı SMS kapalı); T-118.
        var (id, _, _) = await CreateDepositPendingAsync();
        await _api.ScalarAsync<int>("UPDATE platform.subscriptions SET status = 'ReadOnly' RETURNING 1");

        var refused = await RemindAsync(id, _token);

        refused.Status.ShouldBe(403);
        refused.Body.GetProperty("code").GetString().ShouldBe("subscription_read_only");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM infra.outbox_events WHERE type LIKE '%.AppointmentDepositReminded'")).ShouldBe(0);
    }

    [Fact]
    public async Task Remind_NotAwaited_Expired_Staff_OtherBrand_AreRejected()
    {
        var (id, _, _) = await CreateDepositPendingAsync();
        var staff = await _api.AddMemberAndSignInAsync(_token, "+905382741905", "Staff", _merkez);
        (await RemindAsync(id, staff)).Status.ShouldBe(403);
        var other = await _api.SignupAsync("+905427771122", "Berber Can");
        (await RemindAsync(id, other)).Status.ShouldBe(404);

        var (expiring, _, _) = await CreateDepositPendingAsync("2026-11-27T13:30:00Z");
        (await MarkReceivedAsync(id)).Status.ShouldBe(200);
        var received = await RemindAsync(id, _token);
        received.Status.ShouldBe(409, received.Body.ToString());
        received.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");

        _factory.Clock.Advance(Duration.FromMinutes(120));
        var expired = await RemindAsync(expiring, _token);
        expired.Status.ShouldBe(409, expired.Body.ToString());
        expired.Body.GetProperty("code").GetString().ShouldBe("hold_expired");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM infra.outbox_events WHERE type LIKE '%.AppointmentDepositReminded'")).ShouldBe(0);
    }

    // ───────── "Salon iade etti" (V7) ─────────

    [Fact]
    public async Task Refund_Partial_ByBranchManager_RefundedView_PaymentPartiallyRefunded_NoteOnlyInManageHistory_NoCustomerMessage()
    {
        var manager = await _api.AddMemberAndSignInAsync(_token, "+905382741906", "BranchManager", _merkez);
        var (id, _, code) = await CreateDepositPendingAsync();
        var etag = (await MarkReceivedAsync(id)).ETag;
        await DispatchAsync();
        var messages = await _api.ScalarAsync<long>($"SELECT count(*) FROM notifications.notification_outbox WHERE recipient_phone = '{RiskyPhone}'");
        _factory.Clock.Advance(Duration.FromHours(1));
        var at = Iso(_factory.Clock.GetCurrentInstant());

        var refunded = await RefundAsync(id, etag, new { amount = new { amountMinor = 30000, currency = "TRY" }, note = "Müşteri erteledi" }, manager);

        refunded.Status.ShouldBe(200, refunded.Body.ToString());
        string.Equals(refunded.ETag, etag, StringComparison.Ordinal).ShouldBeFalse("sürüm değişir");
        var body = refunded.Body;
        body.GetProperty("status").GetString().ShouldBe("Confirmed", "randevu durumu değişmez");
        var deposit = body.GetProperty("deposit");
        deposit.GetProperty("status").GetString().ShouldBe("Refunded");
        deposit.GetProperty("refundedUtc").GetString().ShouldBe(at);
        deposit.GetProperty("refundedAmount").GetProperty("amountMinor").GetInt64().ShouldBe(30000);
        deposit.GetProperty("receivedUtc").GetString().ShouldNotBeNullOrEmpty();
        body.GetProperty("amountDue").GetProperty("amountMinor").GetInt64().ShouldBe(280000 - (84000 - 30000), "T-83: iade edilen kısım yeniden salonda ödenir");
        var last = body.GetProperty("history").EnumerateArray().Last();
        last.GetProperty("kind").GetString().ShouldBe("DepositRefunded");
        last.GetProperty("detail").GetString().ShouldBe("Müşteri erteledi");
        (await _api.ScalarAsync<string>("SELECT status || '/' || refunded_amount_minor || '/' || amount_minor FROM sales.payments"))
            .ShouldBe("PartiallyRefunded/30000/84000");

        var shown = (await _api.SendAsync(HttpMethod.Get, $"/api/v1/public/appointments/{code}", null)).Body;
        shown.GetProperty("deposit").GetProperty("status").GetString().ShouldBe("Refunded");
        shown.GetRawText().ShouldNotContain("Müşteri erteledi", Case.Sensitive, "not müşteriye gösterilmez");

        await DispatchAsync();
        (await _api.ScalarAsync<long>($"SELECT count(*) FROM notifications.notification_outbox WHERE recipient_phone = '{RiskyPhone}'"))
            .ShouldBe(messages, "iade işaretinde müşteriye bildirim gitmez");
    }

    [Fact]
    public async Task Refund_FullAfterSalonCancel_PaymentRefunded_CancelSmsCarriesDepositSentence()
    {
        var (id, _, _) = await CreateDepositPendingAsync();
        var etag = (await MarkReceivedAsync(id)).ETag;

        var cancelled = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/cancel", new { reason = "StaffUnavailable" }, _token, etag);
        cancelled.Status.ShouldBe(200, cancelled.Body.ToString());
        await DispatchAsync();
        (await _api.ScalarAsync<string>(
            $"SELECT body FROM infra.dev_sms_log WHERE phone_e164 = '{RiskyPhone}' AND body LIKE '%iptal etti%'"))
            .ShouldBe("Kuaför Elif, Cum 27 Kas 14:30 randevunuzu iptal etti. Kapora iadesi için salon sizinle iletişime geçecek. omvila.com/kuafor-elif");

        var refunded = await RefundAsync(id, cancelled.ETag, new { amount = new { amountMinor = 84000, currency = "TRY" } }, _token);

        refunded.Status.ShouldBe(200, refunded.Body.ToString());
        refunded.Body.GetProperty("status").GetString().ShouldBe("Cancelled");
        refunded.Body.GetProperty("deposit").GetProperty("status").GetString().ShouldBe("Refunded");
        IsNullOrAbsent(refunded.Body.GetProperty("history").EnumerateArray().Last(), "detail").ShouldBeTrue();
        (await _api.ScalarAsync<string>("SELECT status || '/' || refunded_amount_minor FROM sales.payments")).ShouldBe("Refunded/84000");
    }

    [Fact]
    public async Task Refund_SameIdempotencyKey_Replays_SecondKeyIsInvalidStateTransition()
    {
        var (id, _, _) = await CreateDepositPendingAsync();
        var etag = (await MarkReceivedAsync(id)).ETag;
        var key = Key();
        var body = new { amount = new { amountMinor = 84000, currency = "TRY" } };

        var first = await RefundAsync(id, etag, body, _token, key);
        var replay = await RefundAsync(id, etag, body, _token, key);
        var second = await RefundAsync(id, first.ETag, body, _token);

        first.Status.ShouldBe(200, first.Body.ToString());
        replay.Status.ShouldBe(200, replay.Body.ToString());
        replay.Headers!["Idempotent-Replayed"].ShouldBe("true");
        second.Status.ShouldBe(409, second.Body.ToString());
        second.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");
        (await _api.ScalarAsync<string>("SELECT status || '/' || refunded_amount_minor FROM sales.payments")).ShouldBe("Refunded/84000");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(84001)]
    public async Task Refund_OutOfRange_IsValidationFailed_NothingWritten(long amountMinor)
    {
        var (id, _, _) = await CreateDepositPendingAsync();
        var etag = (await MarkReceivedAsync(id)).ETag;

        var refused = await RefundAsync(id, etag, new { amount = new { amountMinor, currency = "TRY" } }, _token);

        refused.Status.ShouldBe(400, refused.Body.ToString());
        refused.Body.GetProperty("code").GetString().ShouldBe("validation_failed");
        refused.Body.GetProperty("errors").GetProperty("amount")[0].GetString().ShouldBe("out_of_range");
        (await _api.ScalarAsync<string>("SELECT status || '/' || refunded_amount_minor FROM sales.payments")).ShouldBe("Recorded/0");
    }

    [Fact]
    public async Task Refund_NotReceived_Staff_OtherBrand_StaleIfMatch_AreRejected()
    {
        var (id, _, _) = await CreateDepositPendingAsync();
        var body = new { amount = new { amountMinor = 1000, currency = "TRY" } };
        var notReceived = await RefundAsync(id, (await DetailAsync(id)).ETag, body, _token);
        notReceived.Status.ShouldBe(409, notReceived.Body.ToString());
        notReceived.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");

        var etag = (await MarkReceivedAsync(id)).ETag;
        var staff = await _api.AddMemberAndSignInAsync(_token, "+905382741905", "Staff", _merkez);
        (await RefundAsync(id, etag, body, staff)).Status.ShouldBe(403);
        var other = await _api.SignupAsync("+905427771122", "Berber Can");
        (await RefundAsync(id, etag, body, other)).Status.ShouldBe(404);
        (await RefundAsync(id, "W/\"1\"", body, _token)).Status.ShouldBe(412);
        (await _api.ScalarAsync<string>("SELECT status FROM sales.payments")).ShouldBe("Recorded");
    }

    // ───────── A5 kapora gelmedi (kabul kriteri 6) ─────────

    [Fact]
    public async Task Criterion6_HoldExpires_DepositNotReceivedSms_NotApprovalExpired()
    {
        var (id, _, _) = await CreateDepositPendingAsync();
        _factory.Clock.Advance(Duration.FromMinutes(120));

        await _api.RunJobAsync(ExpireJob);
        await DispatchAsync();

        var detail = (await DetailAsync(id)).Body;
        detail.GetProperty("status").GetString().ShouldBe("Cancelled");
        detail.GetProperty("cancellation").GetProperty("reason").GetString().ShouldBe("DepositNotReceived");
        (await _api.ScalarAsync<string>($"SELECT body FROM infra.dev_sms_log WHERE phone_e164 = '{RiskyPhone}' AND body LIKE '%gelmediği için%'"))
            .ShouldBe("Kuaför Elif: 840 TL kapora 2 saat içinde gelmediği için Cum 27 Kas 14:30 randevunuz iptal edildi. Havale yaptıysanız salonu arayın.");
        (await _api.ScalarAsync<string>(
            "SELECT array_to_string(channel_plan, ',') || '|' || credits_consumed || '|' || status FROM notifications.notification_outbox WHERE event_code = 'deposit.not_received'"))
            .ShouldBe("Sms|1|Sent", "yalnız SMS, tek parça");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'appointment.approval_expired'")).ShouldBe(0);
    }

    // ───────── A13 / A14 kapora aktarımı ─────────

    [Fact]
    public async Task A14_CustomerReschedule_ReceivedDeposit_IsCarriedOver_PaymentUnchanged()
    {
        var (id, transferCode, code) = await CreateDepositPendingAsync();
        (await MarkReceivedAsync(id)).Status.ShouldBe(200);
        var shown = await _api.SendAsync(HttpMethod.Get, $"/api/v1/public/appointments/{code}", null);

        var moved = await _api.SendAsync(
            HttpMethod.Post, $"/api/v1/public/appointments/{code}/reschedule", new { startUtc = "2026-11-27T13:30:00Z" }, headers: Headers(shown.ETag));

        moved.Status.ShouldBe(200, moved.Body.ToString());
        moved.Body.GetProperty("status").GetString().ShouldBe("Confirmed");
        moved.Body.GetProperty("startUtc").GetString().ShouldBe("2026-11-27T13:30:00Z");
        var deposit = moved.Body.GetProperty("deposit");
        deposit.GetProperty("status").GetString().ShouldBe("Received");
        deposit.GetProperty("transferCode").GetString().ShouldBe(transferCode);
        moved.Body.GetProperty("amountDue").GetProperty("amountMinor").GetInt64().ShouldBe(196000);
        (await _api.ScalarAsync<long>(
            $"SELECT count(*) FROM scheduling.appointments a JOIN sales.payment_allocations p ON p.payment_id = a.deposit_payment_id WHERE a.id = '{id}' AND p.target_id = a.id"))
            .ShouldBe(1, "tahsilat aynı randevuda kalır");
    }

    [Fact]
    public async Task A13_CustomerReschedule_AwaitedDepositWithApproval_HoldIsTransferWindow_DepositSmsResent()
    {
        await _api.ScalarAsync<int>("UPDATE tenancy.brand_settings SET requires_approval = true RETURNING 1");
        var (_, transferCode, code) = await CreateDepositPendingAsync();
        await DispatchAsync();
        _factory.Clock.Advance(Duration.FromMinutes(30));
        var shown = await _api.SendAsync(HttpMethod.Get, $"/api/v1/public/appointments/{code}", null);

        var moved = await _api.SendAsync(
            HttpMethod.Post, $"/api/v1/public/appointments/{code}/reschedule", new { startUtc = "2026-11-27T13:30:00Z" }, headers: Headers(shown.ETag));

        moved.Status.ShouldBe(200, moved.Body.ToString());
        moved.Body.GetProperty("displayStatus").GetString().ShouldBe("DepositPending");
        moved.Body.GetProperty("holdExpiresUtc").GetString().ShouldBe(Iso(_factory.Clock.GetCurrentInstant() + Duration.FromMinutes(120)));
        moved.Body.GetProperty("deposit").GetProperty("transferCode").GetString().ShouldBe(transferCode);
        moved.Body.GetProperty("deposit").GetProperty("instructions").GetProperty("iban").GetString().ShouldNotBeNullOrEmpty();

        await DispatchAsync();
        (await _api.ScalarAsync<string>(
            $"SELECT body FROM infra.dev_sms_log WHERE phone_e164 = '{RiskyPhone}' AND body LIKE '%kaporayı%' ORDER BY sent_utc DESC LIMIT 1"))
            .ShouldContain("Cum 27 Kas 16:30 için 840 TL kaporayı 2 saat içinde gönderin.");
    }

    private async Task<(string Id, string TransferCode, string AccessCode)> CreateDepositPendingAsync(string startUtc = FridayAt1430)
    {
        var created = await CreateAsync(RiskyPhone, "Ayşe Yılmaz", startUtc);
        created.Status.ShouldBe(201, created.Body.ToString());
        created.Body.GetProperty("displayStatus").GetString().ShouldBe("DepositPending");
        return (
            created.Body.GetProperty("id").GetString()!,
            created.Body.GetProperty("deposit").GetProperty("transferCode").GetString()!,
            created.Body.GetProperty("accessCode").GetString()!);
    }

    private Task<ManageApi.Response> DetailAsync(string id) => _api.GetAsync($"/api/v1/manage/appointments/{id}", _token);

    private Task<ManageApi.Response> ReportAsync(string accessCode) =>
        _api.SendAsync(HttpMethod.Post, $"/api/v1/public/appointments/{accessCode}/deposit/report", null);

    private Task<ManageApi.Response> RemindAsync(string id, string token) =>
        _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/deposit/remind", null, token);

    private async Task<ManageApi.Response> MarkReceivedAsync(string id)
    {
        var marked = await _api.SendAsync(
            HttpMethod.Post, $"/api/v1/manage/appointments/{id}/deposit/mark-received", null, _token, Headers((await DetailAsync(id)).ETag, Key()));
        marked.Status.ShouldBe(200, marked.Body.ToString());
        return marked;
    }

    private Task<ManageApi.Response> RefundAsync(string id, string? etag, object body, string token, Dictionary<string, string>? key = null) =>
        _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/deposit/refund", body, token, Headers(etag, key ?? Key()));

    private async Task<int> UnreadAsync(string token, string eventCode) =>
        (await _api.GetAsync("/api/v1/manage/notifications", token)).Body.GetProperty("items").EnumerateArray()
            .Count(i => string.Equals(i.GetProperty("event").GetString(), eventCode, StringComparison.Ordinal));

    private async Task SetCreditsAsync(int balance)
    {
        var current = await _api.ScalarAsync<long>($"SELECT coalesce(sum(amount), 0)::bigint FROM notifications.credit_transactions WHERE brand_id = '{_brandId}'");
        await _api.ScalarAsync<int>($"""
            INSERT INTO notifications.credit_transactions (id, brand_id, type, amount, balance_after, reference_type, created_utc)
              VALUES (gen_random_uuid(), '{_brandId}', 'Grant', {balance - current}, {balance}, 'Test', now()) RETURNING amount
            """);
    }

    private async Task DispatchAsync()
    {
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        await _api.RunJobAsync(DispatchSmsJob);
    }

    private async Task<ManageApi.Response> CreateAsync(string phone, string fullName, string startUtc) =>
        await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/appointments", new
        {
            items = new[] { new { serviceId = _balyaj, staffId = _elif } },
            sameStaffForAll = true,
            startUtc,
            fullName,
            privacyNoticeAcknowledged = true,
            privacyNoticeVersion = "2026-11-01",
        }, await VerifyAsync(phone), Key());

    /// <summary>Kod iste → SMS kaydından kodu oku → doğrula; rezervasyon jetonunu döner.</summary>
    private async Task<string> VerifyAsync(string phone)
    {
        _factory.Clock.Advance(Duration.FromMinutes(2)); // aynı telefona OTP bekleme süresi
        var requested = await _api.SendAsync(HttpMethod.Post, "/api/v1/public/otp/request", new { phone, branchId = _merkez });
        requested.Status.ShouldBe(200, requested.Body.ToString());
        var code = await _api.ScalarAsync<string>(
            $"SELECT payload->>'kod' FROM notifications.notification_outbox WHERE event_code = 'otp.code' AND recipient_phone = '{phone}' ORDER BY created_utc DESC LIMIT 1");
        var verified = await _api.SendAsync(HttpMethod.Post, "/api/v1/public/otp/verify",
            new { otpId = requested.Body.GetProperty("challenge").GetProperty("otpId").GetGuid(), code });
        verified.Status.ShouldBe(200, verified.Body.ToString());
        return verified.Body.GetProperty("bookingToken").GetString()!;
    }

    private Task<ManageApi.Response> PostAsync(string path, object body) => _api.SendAsync(HttpMethod.Post, path, body, _token, Key());

    private static bool IsNullOrAbsent(JsonElement element, string name) =>
        !element.TryGetProperty(name, out var value) || value.ValueKind == JsonValueKind.Null;

    private static string Iso(Instant at) => NodaTime.Text.InstantPattern.General.Format(at);

    private static Dictionary<string, string> Headers(string? etag, Dictionary<string, string>? extra = null)
    {
        var headers = new Dictionary<string, string>(extra ?? [], StringComparer.Ordinal);
        if (etag is not null)
        {
            headers["If-Match"] = etag;
        }

        return headers;
    }

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _requests):D12}" };
}
