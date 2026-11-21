using System.Text.Json;
using NodaTime;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-05 kabul kriteri 5 ("Tamamla ve ödeme al": Boya 1.250 + Fön 190 − kapora 200 = 1.240 ₺ Kart → Completed, Elif'e %24
// prim Accrued); durum-makinesi.md A11; alan-modeli.md § Payment, PaymentAllocation, CommissionEntry; docs/acik-sorular.md W2, T-88;
// roller-yetkiler.md appointment.complete (personel 403), payment.collect; sözleşme manageCompleteAppointment.
// Saat: 2026-11-25 09:00Z (Salı 12:00 İstanbul).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class CompletionTests(PostgresFixture db) : IAsyncLifetime
{
    private const string CustomerPhone = "+905321234567";
    private const string FridayAt1430 = "2026-11-27T11:30:00Z";
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private string _merkez = null!;
    private string _elif = null!;
    private string _boya = null!;
    private string _fon = null!;
    private int _requests;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db, new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["Auth:AccessTokenMinutes"] = "6000",
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
        // KK5 test verisi: Elif'in hizmet primi %24.
        await _api.ScalarAsync<int>($"UPDATE tenancy.staff SET service_commission_percent = 24 WHERE id = '{_elif}' RETURNING 1");

        (await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/brand/bank-account", new
        {
            accountHolderName = "Elif Kaya",
            bankName = "Ziraat Bankası",
            iban = "TR29 0006 4000 0011 2345 6789 01",
            transferCodePrefix = "OMV",
            transferWindowMinutes = 120,
        }, _token, "W/\"0\"")).Status.ShouldBe(200);
        var category = (await PostAsync("/api/v1/manage/service-categories", new { name = "Saç" })).Body.GetProperty("id").GetString()!;
        _boya = await ServiceAsync(category, "Boya", 60, 125000, new { mode = "Always", amount = new { amountMinor = 20000, currency = "TRY" } });
        _fon = await ServiceAsync(category, "Fön", 30, 19000, new { mode = "Never" });
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task Criterion5_DepositThenCard_Completed_PaymentsCommissionCountersEvent()
    {
        var (id, etag) = await CheckedInWithDepositAsync();
        _factory.Clock.Advance(Duration.FromMinutes(95));
        var at = Iso(_factory.Clock.GetCurrentInstant());

        var completed = await CompleteAsync(id, etag, new { payments = new[] { Line("Card", 124000) } });

        completed.Status.ShouldBe(200, completed.Body.ToString());
        completed.ETag.ShouldNotBeNull();
        var body = completed.Body;
        body.GetProperty("status").GetString().ShouldBe("Completed");
        body.GetProperty("displayStatus").GetString().ShouldBe("Completed");
        body.GetProperty("completedUtc").GetString().ShouldBe(at);
        body.GetProperty("total").GetProperty("amountMinor").GetInt64().ShouldBe(144000);
        body.GetProperty("amountDue").GetProperty("amountMinor").GetInt64().ShouldBe(0, "tamamlanmışta salonda ödenecek 0");
        body.GetProperty("items").EnumerateArray().Select(i => i.GetProperty("status").GetString()).ShouldBe(["Completed", "Completed"]);
        body.GetProperty("history").EnumerateArray().Select(e => e.GetProperty("kind").GetString()).Last().ShouldBe("Completed");
        var payments = body.GetProperty("payments").EnumerateArray().ToList();
        payments.Select(p => $"{p.GetProperty("method").GetString()}/{p.GetProperty("amount").GetProperty("amountMinor").GetInt64()}/{p.GetProperty("isDeposit").GetBoolean()}/{p.GetProperty("isBalanceAfterDeposit").GetBoolean()}/{p.GetProperty("purpose").GetString()}")
            .ShouldBe(["Transfer/20000/True/False/Appointment", "Card/124000/False/True/Appointment"], "kapora dahil, eskiden yeniye");
        payments[1].GetProperty("paidUtc").GetString().ShouldBe(at);
        payments[1].GetProperty("appointmentId").GetString().ShouldBe(id);
        payments[1].GetProperty("serviceNames").EnumerateArray().Select(s => s.GetString()).ShouldBe(["Boya", "Fön"]);

        (await _api.ScalarAsync<string>(
            $"""
            SELECT p.method || '/' || p.amount_minor || '/' || p.is_deposit || '/' || p.status || '/' || a.target_type || '/' || a.amount_minor
            FROM sales.payments p JOIN sales.payment_allocations a ON a.payment_id = p.id
            WHERE a.target_id = '{id}' AND NOT p.is_deposit AND p.collected_by_user_id IS NOT NULL
            """)).ShouldBe("Card/124000/false/Recorded/Appointment/124000");

        // Elif'e kalem başına %24: Boya 1.250 → 300 ₺, Fön 190 → 45,60 ₺ (taban tahsil edilen; kapora kalemlere dağıtılmış).
        (await _api.ScalarAsync<string>(
            $"""
            SELECT string_agg(c.type || '/' || c.status || '/' || c.basis_minor || '/' || c.percent || '/' || c.amount_minor || '/' || c.period_year_month, ',' ORDER BY c.basis_minor DESC)
            FROM sales.commission_entries c WHERE c.staff_id = '{_elif}' AND c.branch_id = '{_merkez}'
            """)).ShouldBe("Service/Accrued/125000/24.00/30000/202611,Service/Accrued/19000/24.00/4560/202611");
        (await _api.ScalarAsync<long>(
            $"SELECT count(*) FROM sales.commission_entries c JOIN scheduling.appointment_items i ON i.id = c.appointment_item_id WHERE i.appointment_id = '{id}'"))
            .ShouldBe(2);

        (await _api.ScalarAsync<string>(
            $"SELECT visit_count || '/' || last_visit_on || '/' || total_spent_minor FROM customers.brand_customers WHERE id = '{CustomerId(body)}'"))
            .ShouldBe("1/2026-11-27/144000");

        var payload = JsonDocument.Parse(await _api.ScalarAsync<string>(
            "SELECT payload::text FROM infra.outbox_events WHERE type LIKE '%.AppointmentCompleted'")).RootElement;
        payload.GetProperty("appointmentId").GetString().ShouldBe(id);
        payload.GetProperty("branchId").GetString().ShouldBe(_merkez);
        payload.GetProperty("brandCustomerId").GetString().ShouldBe(CustomerId(body));
        payload.GetProperty("completedUtc").GetString().ShouldBe(at);
        payload.GetProperty("staffIds").EnumerateArray().Select(s => s.GetString()).ShouldBe([_elif]);
        payload.GetProperty("brandId").GetString().ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task TwoPayments_EachRecordedSeparately_CommissionOnTotal()
    {
        var (id, etag, _) = await WalkInAsync(_boya, _fon);

        var completed = await CompleteAsync(id, etag, new { payments = new[] { Line("Cash", 100000), Line("Transfer", 44000) } });

        completed.Status.ShouldBe(200, completed.Body.ToString());
        (await _api.ScalarAsync<string>(
            $"SELECT string_agg(p.method || '/' || p.amount_minor, ',' ORDER BY p.amount_minor DESC) FROM sales.payments p JOIN sales.payment_allocations a ON a.payment_id = p.id WHERE a.target_id = '{id}'"))
            .ShouldBe("Cash/100000,Transfer/44000");
        (await _api.ScalarAsync<long>("SELECT sum(amount_minor)::bigint FROM sales.commission_entries")).ShouldBe(34560);
        completed.Body.GetProperty("payments").EnumerateArray().ShouldAllBe(p => !p.GetProperty("isBalanceAfterDeposit").GetBoolean());
    }

    // alan-modeli.md § ServiceStaff: personele özel hizmet oranı personelin oranını ezer; oranı 0 olan personele kayıt yazılmaz.
    [Fact]
    public async Task ServiceStaffOverride_BeatsStaffRate()
    {
        await _api.ScalarAsync<int>($"UPDATE catalog.service_staff SET commission_percent_override = 10 WHERE service_id = '{_fon}' RETURNING 1");
        var (id, etag, _) = await WalkInAsync(_boya, _fon);

        (await CompleteAsync(id, etag, new { payments = new[] { Line("Card", 144000) } })).Status.ShouldBe(200);

        (await _api.ScalarAsync<string>(
            "SELECT string_agg(percent || '/' || amount_minor, ',' ORDER BY basis_minor DESC) FROM sales.commission_entries"))
            .ShouldBe("24.00/30000,10.00/1900");
    }

    [Fact]
    public async Task TotalMismatch_Is409WithExpectedAmount_NothingWritten()
    {
        var (id, etag, _) = await WalkInAsync(_boya, _fon);

        foreach (var body in new object[] { new { payments = new[] { Line("Card", 124000) } }, new { payments = Array.Empty<object>() } })
        {
            var rejected = await CompleteAsync(id, etag, body);
            rejected.Status.ShouldBe(409, rejected.Body.ToString());
            rejected.Body.GetProperty("code").GetString().ShouldBe("payment_total_mismatch");
            rejected.Body.GetProperty("meta").GetProperty("expectedAmountMinor").GetInt64().ShouldBe(144000);
        }

        await NothingWrittenAsync(id, "CheckedIn");
    }

    [Fact]
    public async Task InvalidBody_Is400()
    {
        var (id, etag, _) = await WalkInAsync(_fon);

        var rejected = await CompleteAsync(id, etag, new
        {
            payments = new object[]
            {
                new { method = "PackageRedemption", amount = new { amountMinor = 19000, currency = "TRY" } },
                new { method = "Cash", amount = new { amountMinor = 0, currency = "TRY" } },
            },
        });

        rejected.Status.ShouldBe(400, rejected.Body.ToString());
        rejected.Body.GetProperty("code").GetString().ShouldBe("validation_failed");
        var errors = rejected.Body.GetProperty("errors");
        errors.GetProperty("payments[0].method")[0].GetString().ShouldBe("invalid_format");
        errors.GetProperty("payments[1].amount")[0].GetString().ShouldBe("out_of_range");
        await NothingWrittenAsync(id, "CheckedIn");
    }

    // Paketler başka PR'da (T-88): düşülecek paket kalemi bulunmaz.
    [Fact]
    public async Task PackageRedemption_IsPackageSessionUnavailable()
    {
        var (id, etag, body) = await WalkInAsync(_fon);
        var item = body.GetProperty("items")[0].GetProperty("id").GetString()!;
        var packageItem = "0193a8f0-0000-7000-8000-00000000aa01";

        var rejected = await CompleteAsync(id, etag, new
        {
            payments = Array.Empty<object>(),
            packageRedemptions = new[] { new { appointmentItemId = item, customerPackageItemId = packageItem } },
        });

        rejected.Status.ShouldBe(409, rejected.Body.ToString());
        rejected.Body.GetProperty("code").GetString().ShouldBe("package_session_unavailable");
        rejected.Body.GetProperty("meta").GetProperty("customerPackageItemId").GetString().ShouldBe(packageItem);
        await NothingWrittenAsync(id, "CheckedIn");
    }

    [Fact]
    public async Task NotCheckedIn_IsInvalidStateTransition()
    {
        var created = await PostAsync("/api/v1/manage/appointments", new
        {
            branchId = _merkez,
            newCustomer = new { fullName = "Derya Şahin", phone = "0533 111 22 18" },
            channel = "Phone",
            startUtc = "2026-11-26T11:30:00Z",
            items = new[] { new { serviceId = _fon, staffId = _elif } },
        });
        created.Status.ShouldBe(201, created.Body.ToString());
        var id = created.Body.GetProperty("appointment").GetProperty("id").GetString()!;

        var rejected = await CompleteAsync(id, created.ETag, new { payments = new[] { Line("Cash", 19000) } });

        rejected.Status.ShouldBe(409, rejected.Body.ToString());
        rejected.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");
        await NothingWrittenAsync(id, "Confirmed");
    }

    [Fact]
    public async Task CompletedTwice_SecondIsInvalidStateTransition()
    {
        var (id, etag, _) = await WalkInAsync(_fon);
        var first = await CompleteAsync(id, etag, new { payments = new[] { Line("Cash", 19000) } });
        first.Status.ShouldBe(200);

        var second = await CompleteAsync(id, first.ETag, new { payments = Array.Empty<object>() });

        second.Status.ShouldBe(409, second.Body.ToString());
        second.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM sales.payments")).ShouldBe(1);
    }

    [Fact]
    public async Task StaleIfMatch_Is412_NothingWritten()
    {
        var (id, _, _) = await WalkInAsync(_fon);

        var stale = await CompleteAsync(id, "W/\"1\"", new { payments = new[] { Line("Cash", 19000) } });

        stale.Status.ShouldBe(412, stale.Body.ToString());
        await NothingWrittenAsync(id, "CheckedIn");
    }

    [Fact]
    public async Task SameIdempotencyKey_ReplaysFirstResponse_SinglePaymentAndCommission()
    {
        var (id, etag, _) = await WalkInAsync(_fon);
        var key = Key();
        var body = new { payments = new[] { Line("Cash", 19000) } };

        var first = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/complete", body, _token, Headers(etag, key));
        var again = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/complete", body, _token, Headers(etag, key));

        first.Status.ShouldBe(200, first.Body.ToString());
        again.Status.ShouldBe(200, again.Body.ToString());
        again.Headers!["Idempotent-Replayed"].ShouldBe("true");
        again.Body.GetProperty("status").GetString().ShouldBe("Completed");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM sales.payments")).ShouldBe(1);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM sales.commission_entries")).ShouldBe(1);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM infra.outbox_events WHERE type LIKE '%.AppointmentCompleted'")).ShouldBe(1);
    }

    [Fact]
    public async Task StaffRole_Is403()
    {
        var (id, etag, _) = await WalkInAsync(_fon);
        var staff = await _api.AddMemberAndSignInAsync(_token, "+905382741905", "Staff", _merkez);

        var forbidden = await _api.SendAsync(
            HttpMethod.Post, $"/api/v1/manage/appointments/{id}/complete", new { payments = new[] { Line("Cash", 19000) } }, staff, Headers(etag, Key()));

        forbidden.Status.ShouldBe(403, forbidden.Body.ToString());
        await NothingWrittenAsync(id, "CheckedIn");
    }

    [Fact]
    public async Task OtherBrand_Is404()
    {
        var (id, etag, _) = await WalkInAsync(_fon);
        var other = await _api.SignupAsync("+905427771122", "Berber Can");

        var rejected = await _api.SendAsync(
            HttpMethod.Post, $"/api/v1/manage/appointments/{id}/complete", new { payments = new[] { Line("Cash", 19000) } }, other, Headers(etag, Key()));

        rejected.Status.ShouldBe(404, rejected.Body.ToString());
        await NothingWrittenAsync(id, "CheckedIn");
    }

    private async Task NothingWrittenAsync(string id, string status)
    {
        (await _api.ScalarAsync<long>("SELECT count(*) FROM sales.payments WHERE NOT is_deposit")).ShouldBe(0);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM sales.commission_entries")).ShouldBe(0);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM infra.outbox_events WHERE type LIKE '%.AppointmentCompleted'")).ShouldBe(0);
        (await _api.ScalarAsync<long>("SELECT coalesce(sum(visit_count), 0)::bigint FROM customers.brand_customers")).ShouldBe(0);
        (await _api.GetAsync($"/api/v1/manage/appointments/{id}", _token)).Body.GetProperty("status").GetString().ShouldBe(status);
    }

    /// <summary>Online: Boya (kapora 200 ₺, Always) + Fön; "Kapora geldi"; Cuma 14:00'te Geldi.</summary>
    private async Task<(string Id, string ETag)> CheckedInWithDepositAsync()
    {
        var created = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/appointments", new
        {
            items = new[] { new { serviceId = _boya, staffId = _elif }, new { serviceId = _fon, staffId = _elif } },
            sameStaffForAll = true,
            startUtc = FridayAt1430,
            fullName = "Selin Öztürk",
            privacyNoticeAcknowledged = true,
            privacyNoticeVersion = "2026-11-01",
        }, await VerifyAsync(CustomerPhone), Key());
        created.Status.ShouldBe(201, created.Body.ToString());
        created.Body.GetProperty("displayStatus").GetString().ShouldBe("DepositPending");
        var id = created.Body.GetProperty("id").GetString()!;

        var marked = await _api.SendAsync(
            HttpMethod.Post, $"/api/v1/manage/appointments/{id}/deposit/mark-received", null, _token, Headers((await DetailAsync(id)).ETag, Key()));
        marked.Status.ShouldBe(200, marked.Body.ToString());
        marked.Body.GetProperty("amountDue").GetProperty("amountMinor").GetInt64().ShouldBe(124000);

        _factory.Clock.Advance(Duration.FromHours(50));
        var checkedIn = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/check-in", new { }, _token, marked.ETag);
        checkedIn.Status.ShouldBe(200, checkedIn.Body.ToString());
        return (id, checkedIn.ETag!);
    }

    /// <summary>Panelden kapıdan gelen randevu: doğrudan Geldi.</summary>
    private async Task<(string Id, string ETag, JsonElement Body)> WalkInAsync(params string[] services)
    {
        var created = await PostAsync("/api/v1/manage/appointments", new
        {
            branchId = _merkez,
            newCustomer = new { fullName = "Selin Öztürk", phone = "0532 123 45 67" },
            channel = "WalkIn",
            startUtc = "2026-11-25T09:00:00Z",
            items = services.Select(s => new { serviceId = s, staffId = _elif }).ToArray(),
        });
        created.Status.ShouldBe(201, created.Body.ToString());
        var body = created.Body.GetProperty("appointment");
        body.GetProperty("status").GetString().ShouldBe("CheckedIn");
        return (body.GetProperty("id").GetString()!, created.ETag!, body);
    }

    private async Task<string> ServiceAsync(string category, string name, int minutes, long priceMinor, object deposit) =>
        (await PostAsync("/api/v1/manage/services", new
        {
            categoryId = category,
            name,
            durationMinutes = minutes,
            price = new { amountMinor = priceMinor, currency = "TRY" },
            priceDisplay = "Fixed",
            deposit,
            isOnlineBookable = true,
            staffIds = new[] { _elif },
        })).Body.GetProperty("id").GetString()!;

    private Task<ManageApi.Response> CompleteAsync(string id, string? etag, object body) =>
        _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/complete", body, _token, Headers(etag, Key()));

    private Task<ManageApi.Response> DetailAsync(string id) => _api.GetAsync($"/api/v1/manage/appointments/{id}", _token);

    private static object Line(string method, long amountMinor) => new { method, amount = new { amountMinor, currency = "TRY" } };

    private static string CustomerId(JsonElement body) => body.GetProperty("customer").GetProperty("brandCustomerId").GetString()!;

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
