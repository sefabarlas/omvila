using System.Text.Json;
using NodaTime;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-04 kabul kriterleri 5 ("Kapora geldi" → Confirmed, Payment, "kaporanız alındı" SMS'i, "Salonda ödenecek" düşer)
// ve 7 ("Kaporasız onayla" → Confirmed, kapora istenmiyor); durum-makinesi.md A2, A3; alan-modeli.md § Payment,
// PaymentAllocation; bildirimler.md deposit.received (yalnız SMS, V3); V5 (personel 403); sözleşme manageMarkDepositReceived,
// manageApproveAppointment. Saat: 2026-11-25 09:00Z. Randevu Cuma 14:30 (11:30Z), Balyaj 2.800 ₺, kapora %30 = 840 ₺.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class DepositReceivedTests(PostgresFixture db) : IAsyncLifetime
{
    private const string RiskyPhone = "+905321234567";
    private const string OtherPhone = "+905339876543";
    private const string FridayAt1430 = "2026-11-27T11:30:00Z";
    private const string DispatchSmsJob = "Omvila.Modules.Notifications.Jobs.DispatchNotifications, Omvila.Modules.Notifications";
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
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

    // ───────── A3 "Kapora geldi" ─────────

    [Fact]
    public async Task Criterion5_MarkReceived_Confirmed_PaymentAndAllocation_AmountDueDrops_History()
    {
        var (id, code, _) = await CreateDepositPendingAsync();
        var etag = (await DetailAsync(id)).ETag;
        var at = Iso(_factory.Clock.GetCurrentInstant());

        var marked = await MarkAsync(id, etag, Key());

        marked.Status.ShouldBe(200, marked.Body.ToString());
        marked.ETag.ShouldNotBeNull();
        string.Equals(marked.ETag, etag, StringComparison.Ordinal).ShouldBeFalse("sürüm değişir");
        var body = marked.Body;
        body.GetProperty("status").GetString().ShouldBe("Confirmed");
        body.GetProperty("displayStatus").GetString().ShouldBe("Confirmed");
        IsNullOrAbsent(body, "holdExpiresUtc").ShouldBeTrue();
        var deposit = body.GetProperty("deposit");
        deposit.GetProperty("status").GetString().ShouldBe("Received");
        deposit.GetProperty("receivedUtc").GetString().ShouldBe(at);
        deposit.GetProperty("required").GetProperty("amountMinor").GetInt64().ShouldBe(84000);
        deposit.GetProperty("transferCode").GetString().ShouldBe(code);
        body.GetProperty("total").GetProperty("amountMinor").GetInt64().ShouldBe(280000);
        body.GetProperty("amountDue").GetProperty("amountMinor").GetInt64().ShouldBe(196000, "salonda ödenecek kapora kadar düşer");
        body.GetProperty("history").EnumerateArray().Select(e => e.GetProperty("kind").GetString()).ShouldBe(["Created", "DepositReceived"]);

        // Payment(Method = Transfer, IsDeposit, Recorded, Reference = açıklama kodu) + tutarın tamamı randevuya tahsis.
        (await _api.ScalarAsync<string>(
            "SELECT method || '/' || is_deposit || '/' || status || '/' || amount_minor || '/' || currency_code || '/' || refunded_amount_minor || '/' || reference FROM sales.payments"))
            .ShouldBe($"Transfer/true/Recorded/84000/TRY/0/{code}");
        (await _api.ScalarAsync<string>(
            $"SELECT a.target_type || '/' || a.amount_minor FROM sales.payment_allocations a JOIN sales.payments p ON p.id = a.payment_id WHERE a.target_id = '{id}' AND a.brand_id = p.brand_id"))
            .ShouldBe("Appointment/84000");
        (await _api.ScalarAsync<long>(
            $"""
            SELECT count(*) FROM scheduling.appointments a JOIN sales.payments p ON p.id = a.deposit_payment_id
            WHERE a.id = '{id}' AND a.deposit_received_by_user_id = p.collected_by_user_id AND p.branch_id = a.branch_id
              AND p.brand_customer_id = a.brand_customer_id AND a.deposit_received_utc = p.paid_utc
            """)).ShouldBe(1);
    }

    [Fact]
    public async Task Criterion5_MarkReceived_DepositReceivedSms_OneCredit_RemindersPlanned_ApprovalPushResolved()
    {
        var (id, _, accessCode) = await CreateDepositPendingAsync();
        await DispatchAsync();
        var before = await CreditsAsync();

        (await MarkAsync(id, (await DetailAsync(id)).ETag, Key())).Status.ShouldBe(200);
        await DispatchAsync();

        (await _api.ScalarAsync<string>(
            $"SELECT body FROM infra.dev_sms_log WHERE phone_e164 = '{RiskyPhone}' AND body LIKE '%kaporanız alındı%'"))
            .ShouldBe($"Kuaför Elif: 840 TL kaporanız alındı, Cum 27 Kas 14:30 randevunuz kesinleşti. omvila.com/r/{accessCode}");
        (await CreditsAsync()).ShouldBe(before - 1, "kapora SMS'i tek parça, salon kontöründen");
        (await _api.ScalarAsync<string>(
            "SELECT array_to_string(channel_plan, ',') || '|' || channel_used || '|' || credits_consumed || '|' || status FROM notifications.notification_outbox WHERE event_code = 'deposit.received'"))
            .ShouldBe("Sms|Sms|1|Sent", "V3: deposit.received yalnız SMS, hemen gider");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'appointment.reminder' AND status = 'Pending'"))
            .ShouldBe(2, "onay sayıldığı için hatırlatmalar planlanır (1440 ve 120 dk)");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'appointment.approved'")).ShouldBe(0);
        (await _api.ScalarAsync<long>(
            "SELECT count(*) FROM notifications.in_app_notifications WHERE event_code = 'staff.approval_needed' AND hold_expires_utc IS NOT NULL"))
            .ShouldBe(0, "yanıtlanan talebin kalan süresi boşalır");
    }

    // T-46 (3): "Randevu bilgilendirme SMS'leri" kapalı müşteriye kapora SMS'i ve hatırlatma yazılmaz; işaretleme yine geçerli.
    [Fact]
    public async Task MarkReceived_CustomerWithAppointmentSmsDisabled_NoSms()
    {
        var (id, _, _) = await CreateDepositPendingAsync();
        await _api.ScalarAsync<int>("UPDATE customers.brand_customers SET appointment_sms_enabled = false RETURNING 1");

        (await MarkAsync(id, (await DetailAsync(id)).ETag, Key())).Status.ShouldBe(200);
        await DispatchAsync();

        (await _api.ScalarAsync<long>(
            "SELECT count(*) FROM notifications.notification_outbox WHERE event_code IN ('deposit.received', 'appointment.reminder')")).ShouldBe(0);
    }

    [Fact]
    public async Task MarkReceived_SameIdempotencyKey_ReplaysFirstResponse_SinglePayment()
    {
        var (id, _, _) = await CreateDepositPendingAsync();
        var etag = (await DetailAsync(id)).ETag;
        var key = Key();

        var first = await MarkAsync(id, etag, key);
        var again = await MarkAsync(id, etag, key);

        first.Status.ShouldBe(200, first.Body.ToString());
        again.Status.ShouldBe(200, again.Body.ToString());
        again.Headers!["Idempotent-Replayed"].ShouldBe("true");
        again.Body.GetProperty("deposit").GetProperty("status").GetString().ShouldBe("Received");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM sales.payments")).ShouldBe(1);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM infra.outbox_events WHERE type LIKE '%.AppointmentDepositReceived'")).ShouldBe(1);
    }

    [Fact]
    public async Task MarkReceived_StaleIfMatch_Is412_NothingWritten()
    {
        var (id, _, _) = await CreateDepositPendingAsync();

        var stale = await MarkAsync(id, "W/\"1\"", Key());

        stale.Status.ShouldBe(412, stale.Body.ToString());
        (await _api.ScalarAsync<long>("SELECT count(*) FROM sales.payments")).ShouldBe(0);
        (await DetailAsync(id)).Body.GetProperty("displayStatus").GetString().ShouldBe("DepositPending");
    }

    // V5: personel v1'de kapora işaretleyemez; kaporasız onay da appointment.approve ister.
    [Fact]
    public async Task StaffRole_MarkReceivedAndWaive_AreForbidden()
    {
        var (id, _, _) = await CreateDepositPendingAsync();
        var etag = (await DetailAsync(id)).ETag;
        var staff = await _api.AddMemberAndSignInAsync(_token, "+905382741905", "Staff", _merkez);

        var marked = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/deposit/mark-received", null, staff, Headers(etag, Key()));
        marked.Status.ShouldBe(403, marked.Body.ToString());
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/approve", new { waiveDeposit = true }, staff, Headers(etag)))
            .Status.ShouldBe(403);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM sales.payments")).ShouldBe(0);
    }

    [Fact]
    public async Task OtherBrand_MarkReceived_Is404()
    {
        var (id, _, _) = await CreateDepositPendingAsync();
        var etag = (await DetailAsync(id)).ETag;
        var other = await _api.SignupAsync("+905427771122", "Berber Can");

        var marked = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/deposit/mark-received", null, other, Headers(etag, Key()));

        marked.Status.ShouldBe(404, marked.Body.ToString());
        (await _api.ScalarAsync<long>("SELECT count(*) FROM sales.payments")).ShouldBe(0);
    }

    [Fact]
    public async Task MarkReceived_AfterHold_IsHoldExpired_NoPayment()
    {
        var (id, _, _) = await CreateDepositPendingAsync();
        var etag = (await DetailAsync(id)).ETag;
        _factory.Clock.Advance(Duration.FromMinutes(120));

        var marked = await MarkAsync(id, etag, Key());

        marked.Status.ShouldBe(409, marked.Body.ToString());
        marked.Body.GetProperty("code").GetString().ShouldBe("hold_expired");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM sales.payments")).ShouldBe(0);
    }

    [Fact]
    public async Task MarkReceived_NoDepositOrAlreadyReceived_IsInvalidStateTransition()
    {
        var plain = await CreateAsync(OtherPhone, "Derya Şahin", "2026-11-27T13:30:00Z");
        plain.Body.GetProperty("status").GetString().ShouldBe("Confirmed");
        var plainId = plain.Body.GetProperty("id").GetString()!;
        var noDeposit = await MarkAsync(plainId, (await DetailAsync(plainId)).ETag, Key());
        noDeposit.Status.ShouldBe(409, noDeposit.Body.ToString());
        noDeposit.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");

        var (id, _, _) = await CreateDepositPendingAsync();
        var first = await MarkAsync(id, (await DetailAsync(id)).ETag, Key());
        first.Status.ShouldBe(200);
        var second = await MarkAsync(id, first.ETag, Key());
        second.Status.ShouldBe(409, second.Body.ToString());
        second.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM sales.payments")).ShouldBe(1);
    }

    // ───────── A2 "Onayla" / "Kaporasız onayla" ─────────

    [Fact]
    public async Task Approve_DepositAwaited_WithoutWaive_IsDepositRequired()
    {
        var (id, _, _) = await CreateDepositPendingAsync();
        var etag = (await DetailAsync(id)).ETag;

        foreach (object? body in new object?[] { null, new { waiveDeposit = false } })
        {
            var approved = await ApproveAsync(id, etag, body);
            approved.Status.ShouldBe(409, approved.Body.ToString());
            approved.Body.GetProperty("code").GetString().ShouldBe("deposit_required");
        }

        (await DetailAsync(id)).Body.GetProperty("displayStatus").GetString().ShouldBe("DepositPending");
    }

    [Fact]
    public async Task Criterion7_Waive_Confirmed_DepositWaived_AmountDueIsTotal_NoPayment_ApprovedSms()
    {
        var (id, _, _) = await CreateDepositPendingAsync();
        _factory.Clock.Advance(Duration.FromMinutes(10));

        var approved = await ApproveAsync(id, (await DetailAsync(id)).ETag, new { waiveDeposit = true });

        approved.Status.ShouldBe(200, approved.Body.ToString());
        var body = approved.Body;
        body.GetProperty("status").GetString().ShouldBe("Confirmed");
        body.GetProperty("deposit").GetProperty("status").GetString().ShouldBe("Waived");
        IsNullOrAbsent(body.GetProperty("deposit"), "receivedUtc").ShouldBeTrue();
        body.GetProperty("amountDue").GetProperty("amountMinor").GetInt64().ShouldBe(280000);
        body.GetProperty("history").EnumerateArray().Select(e => e.GetProperty("kind").GetString()).ShouldBe(["Created", "Approved", "DepositWaived"]);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM sales.payments")).ShouldBe(0);
        (await _api.ScalarAsync<long>(
            $"SELECT count(*) FROM scheduling.appointments WHERE id = '{id}' AND deposit_waived_utc IS NOT NULL AND deposit_waived_by_user_id IS NOT NULL"))
            .ShouldBe(1);

        await DispatchAsync();
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'appointment.approved'")).ShouldBe(1);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'deposit.received'")).ShouldBe(0);
    }

    [Fact]
    public async Task Approve_WithoutDeposit_WaiveIsIgnored()
    {
        await _api.ScalarAsync<int>("UPDATE tenancy.brand_settings SET requires_approval = true RETURNING 1");
        var created = await CreateAsync(OtherPhone, "Derya Şahin", FridayAt1430);
        created.Body.GetProperty("displayStatus").GetString().ShouldBe("PendingApproval");
        var id = created.Body.GetProperty("id").GetString()!;

        var approved = await ApproveAsync(id, (await DetailAsync(id)).ETag, new { waiveDeposit = true });

        approved.Status.ShouldBe(200, approved.Body.ToString());
        approved.Body.GetProperty("status").GetString().ShouldBe("Confirmed");
        IsNullOrAbsent(approved.Body, "deposit").ShouldBeTrue();
    }

    private async Task<(string Id, string TransferCode, string AccessCode)> CreateDepositPendingAsync()
    {
        var created = await CreateAsync(RiskyPhone, "Ayşe Yılmaz", FridayAt1430);
        created.Status.ShouldBe(201, created.Body.ToString());
        created.Body.GetProperty("displayStatus").GetString().ShouldBe("DepositPending");
        return (
            created.Body.GetProperty("id").GetString()!,
            created.Body.GetProperty("deposit").GetProperty("transferCode").GetString()!,
            created.Body.GetProperty("accessCode").GetString()!);
    }

    private Task<ManageApi.Response> DetailAsync(string id) => _api.GetAsync($"/api/v1/manage/appointments/{id}", _token);

    private Task<ManageApi.Response> MarkAsync(string id, string? etag, Dictionary<string, string> key) =>
        _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/deposit/mark-received", null, _token, Headers(etag, key));

    private Task<ManageApi.Response> ApproveAsync(string id, string? etag, object? body) =>
        _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/approve", body, _token, Headers(etag));

    private async Task<long> CreditsAsync() =>
        await _api.ScalarAsync<long>("SELECT coalesce(sum(amount), 0)::bigint FROM notifications.credit_transactions");

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
