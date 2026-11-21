using Microsoft.AspNetCore.Mvc.Testing;
using NodaTime;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-02 kabul kriterleri 1 (online randevu mutlu yol), 2 (şube fiyatı + personel ek ücreti, sonradan fiyat
// değişmez), 4 (20 paralel istek), 5 (iki şube, geçiş süresi), 6 (fark etmez), 7 (dolu gün), 8 (pencere), 9 (güvenilir
// cihaz), 16 (kapalı gün / "Yalnızca bu gün için aç"), 17 (salt okunur → booking_disabled); § Alan kuralları (Source=Online,
// IsVerified, ConsentRecord, erişim kodu, customer_blocked); ADR-0007 (public-booking jetonu); sözleşme public-booking.
// Saat: 2026-11-25 09:00Z (Çarşamba 12:00 İstanbul). Şube Pzt–Cmt 09:00–19:00, Pazar kapalı.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class PublicBookingTests(PostgresFixture db) : IAsyncLifetime
{
    private const string AyşePhone = "+905321234567";
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private string _merkez = null!;
    private string _elif = null!;
    private string _zeynep = null!;
    private string _category = null!;
    private string _kesim = null!;
    private string _fon = null!;
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

        await SetBranchHoursAsync(_merkez, Week(1, 2, 3, 4, 5, 6));
        var elif = await _api.GetAsync($"/api/v1/manage/staff/{_elif}", _token);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{_elif}/branches/{_merkez}/working-hours", Week(1, 2, 3, 4, 5, 6), _token, elif.ETag))
            .Status.ShouldBe(200);
        _zeynep = (await PostAsync("/api/v1/manage/staff",
            new { fullName = "Zeynep Aksoy", phone = "+905382741905", role = "Staff", calendarColor = "#2F5D8A", branchIds = new[] { _merkez } })).Body.GetProperty("id").GetString()!;

        _category = (await PostAsync("/api/v1/manage/service-categories", new { name = "Saç" })).Body.GetProperty("id").GetString()!;
        _kesim = await ServiceAsync("Saç kesimi", 45, 45000, [_elif, _zeynep]);
        _fon = await ServiceAsync("Fön", 30, 30000, [_elif, _zeynep]);
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task Criterion1_BrandCardAndCatalog_ForBookingPageOpening()
    {
        var brand = await _api.SendAsync(HttpMethod.Get, "/api/v1/public/brands/kuafor-elif", null);
        brand.Status.ShouldBe(200, brand.Body.ToString());
        brand.Body.GetProperty("name").GetString().ShouldBe("Kuaför Elif");
        brand.Body.GetProperty("bookingEnabled").GetBoolean().ShouldBeTrue();
        brand.Body.GetProperty("industryLabel").GetString().ShouldBe("Kuaför");
        var branch = brand.Body.GetProperty("branches")[0];
        branch.GetProperty("id").GetString().ShouldBe(_merkez);
        branch.GetProperty("openNow").GetBoolean().ShouldBeTrue("Çarşamba 12:00");
        branch.GetProperty("todayClosesAt").GetString().ShouldBe("19:00");

        var catalog = await _api.SendAsync(HttpMethod.Get, $"/api/v1/public/branches/{_merkez}/catalog", null);
        catalog.Status.ShouldBe(200, catalog.Body.ToString());
        catalog.Body.GetProperty("services").EnumerateArray().Select(s => s.GetProperty("name").GetString()).ShouldBe(["Fön", "Saç kesimi"], ignoreOrder: true);
        catalog.Body.GetProperty("staff").GetArrayLength().ShouldBe(2);
        catalog.Body.GetProperty("allowAnyStaffOption").GetBoolean().ShouldBeTrue();
    }

    [Fact]
    public async Task Criterion1_HappyPath_SlotsOtpCreate_Confirmed_ManageUrl_ConsentAndOutbox()
    {
        var items = new[] { new { serviceId = _kesim, staffId = (string?)_elif }, new { serviceId = _fon, staffId = (string?)null } };
        var slots = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/availability/slots", new { items, date = "2026-11-26" });
        slots.Status.ShouldBe(200, slots.Body.ToString());
        var slot = slots.Body.GetProperty("slots").EnumerateArray().Single(s => string.Equals(s.GetProperty("localTime").GetString(), "14:30", StringComparison.Ordinal));
        slot.GetProperty("endLocalTime").GetString().ShouldBe("15:45");
        slot.GetProperty("period").GetString().ShouldBe("Afternoon");
        slot.GetProperty("total").GetProperty("amountMinor").GetInt64().ShouldBe(75000);

        var token = await VerifyAsync(AyşePhone);
        var created = await CreateAsync(token, items, slot.GetProperty("startUtc").GetString()!, marketing: true);

        created.Status.ShouldBe(201, created.Body.ToString());
        created.ETag.ShouldNotBeNull();
        var body = created.Body;
        body.GetProperty("status").GetString().ShouldBe("Confirmed");
        body.GetProperty("displayStatus").GetString().ShouldBe("Confirmed");
        var code = body.GetProperty("accessCode").GetString()!;
        code.Length.ShouldBe(10);
        body.GetProperty("manageUrl").GetString().ShouldBe($"https://omvila.com/r/{code}");
        body.GetProperty("startUtc").GetString().ShouldBe("2026-11-26T11:30:00Z");
        body.GetProperty("endUtc").GetString().ShouldBe("2026-11-26T12:45:00Z");
        body.GetProperty("items").EnumerateArray().Select(i => i.GetProperty("price").GetProperty("amountMinor").GetInt64()).ShouldBe([45000L, 30000L]);
        body.GetProperty("items")[1].GetProperty("staffName").GetString().ShouldBe(body.GetProperty("items")[0].GetProperty("staffName").GetString(), "aynı kişiden");
        body.GetProperty("total").GetProperty("amountMinor").GetInt64().ShouldBe(75000);
        body.GetProperty("canCancel").GetBoolean().ShouldBeTrue();
        body.GetProperty("cancelDeadlineUtc").GetString().ShouldBe("2026-11-25T11:30:00Z");
        body.GetProperty("brand").GetProperty("slug").GetString().ShouldBe("kuafor-elif");

        (await _api.ScalarAsync<string>("SELECT payload::jsonb->>'channel' FROM infra.outbox_events WHERE type LIKE '%AppointmentConfirmed'")).ShouldBe("Online");
        (await _api.ScalarAsync<string>("SELECT channel || '/' || client_app FROM scheduling.appointments")).ShouldBe("Online/BookingPage");
        (await _api.ScalarAsync<string>("SELECT source FROM customers.brand_customers")).ShouldBe("Online");
        (await _api.ScalarAsync<bool>("SELECT is_verified FROM customers.customers")).ShouldBeTrue();
        (await _api.ScalarAsync<string>("SELECT string_agg(consent_type || ':' || document_version || ':' || channel, ',' ORDER BY consent_type) FROM customers.consent_records"))
            .ShouldBe("MarketingEmail:2026-11-01:Web,MarketingSms:2026-11-01:Web,PrivacyNoticeAcknowledged:2026-11-01:Web");

        var calendar = await _api.GetAsync($"/api/v1/manage/calendar?branchId={_merkez}&from=2026-11-26&to=2026-11-26", _token);
        calendar.Body.GetProperty("appointments").EnumerateArray().ShouldContain(a => string.Equals(a.GetProperty("staffId").GetString(), _elif, StringComparison.Ordinal) && string.Equals(a.GetProperty("status").GetString(), "Confirmed", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Criterion2_BranchPriceAndStaffSurcharge_CopiedToItem_LaterPriceChangeDoesNotAffectAppointment()
    {
        var (nisantasi, mert) = await SecondBranchWithMertAsync();
        var balyaj = await ServiceAsync("Balyaj", 60, 180000, [mert]);
        (await _api.SendAsync(HttpMethod.Put, $"/api/v1/manage/services/{balyaj}/branch-prices/{nisantasi}",
            new { price = new { amountMinor = 200000, currency = "TRY" }, isAvailable = true }, _token)).Status.ShouldBe(200);
        await _api.ScalarAsync<int>($"UPDATE catalog.service_staff SET surcharge_minor = 30000 WHERE service_id = '{balyaj}' AND staff_id = '{mert}' RETURNING 1");

        var catalog = await _api.SendAsync(HttpMethod.Get, $"/api/v1/public/branches/{nisantasi}/catalog", null);
        var service = catalog.Body.GetProperty("services").EnumerateArray().Single(s => string.Equals(s.GetProperty("id").GetString(), balyaj, StringComparison.Ordinal));
        service.GetProperty("price").GetProperty("amountMinor").GetInt64().ShouldBe(200000, "şube fiyatı");
        var surcharge = catalog.Body.GetProperty("staff").EnumerateArray().Single(s => string.Equals(s.GetProperty("id").GetString(), mert, StringComparison.Ordinal)).GetProperty("surcharges")[0];
        surcharge.GetProperty("amount").GetProperty("amountMinor").GetInt64().ShouldBe(30000);

        var token = await VerifyAsync(AyşePhone);
        var created = await CreateAsync(token, new[] { new { serviceId = balyaj, staffId = (string?)mert } }, "2026-11-27T12:00:00Z", branchId: nisantasi);
        created.Status.ShouldBe(201, created.Body.ToString());
        created.Body.GetProperty("items")[0].GetProperty("price").GetProperty("amountMinor").GetInt64().ShouldBe(230000);
        created.Body.GetProperty("total").GetProperty("amountMinor").GetInt64().ShouldBe(230000);

        await _api.ScalarAsync<int>($"UPDATE catalog.services SET base_price_minor = 250000 WHERE id = '{balyaj}'; UPDATE catalog.branch_service_prices SET price_minor = 260000; SELECT 1");
        (await _api.ScalarAsync<long>("SELECT total_amount_minor FROM scheduling.appointments")).ShouldBe(230000);
        (await _api.ScalarAsync<long>("SELECT price_minor FROM scheduling.appointment_items")).ShouldBe(230000);
    }

    [Fact]
    public async Task Criterion4_TwentyParallelCreates_ExactlyOneCreated_OthersSlotTakenWithAlternatives()
    {
        var token = await VerifyAsync(AyşePhone);
        var items = new[] { new { serviceId = _kesim, staffId = (string?)_elif } };

        var results = await Task.WhenAll(Enumerable.Range(0, 20).Select(_ => CreateAsync(token, items, "2026-11-26T11:30:00Z")));

        results.Count(r => r.Status == 201).ShouldBe(1, string.Join(" | ", results.Where(r => r.Status is not (201 or 409)).Select(r => r.Body.ToString())));
        var conflicts = results.Where(r => r.Status == 409).ToList();
        conflicts.Count.ShouldBe(19);
        conflicts.ShouldAllBe(r => string.Equals(r.Body.GetProperty("code").GetString(), "slot_taken", StringComparison.Ordinal));
        conflicts.ShouldAllBe(r => r.Body.GetProperty("meta").GetProperty("alternatives").GetArrayLength() > 0);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM scheduling.appointments")).ShouldBe(1);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM customers.brand_customers")).ShouldBe(1);
    }

    [Fact]
    public async Task Criterion5_TwoBranches_TransferTimeDelaysFirstCandidateAtOtherBranch()
    {
        var (nisantasi, mert) = await SecondBranchWithMertAsync();
        var request = new { items = new[] { new { serviceId = _kesim, staffId = mert } }, date = "2026-11-27" };
        (await FirstSlotAsync(nisantasi, request)).ShouldBe("12:00");

        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/appointments", new
        {
            branchId = _merkez,
            newCustomer = new { fullName = "Selin Öztürk", phone = "+905550004418" },
            channel = "Panel",
            startUtc = "2026-11-27T08:15:00Z",
            items = new[] { new { serviceId = _kesim, staffId = mert } },
        }, _token, Key())).Status.ShouldBe(201);

        (await FirstSlotAsync(nisantasi, request)).ShouldBe("12:30", "Merkez'de 12:00'ye kadar dolu + 30 dk geçiş");
        var days = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{nisantasi}/availability/days",
            new { request.items, from = "2026-11-27", to = "2026-11-27" });
        days.Body.GetProperty("days")[0].GetProperty("firstSlotUtc").GetString().ShouldBe("2026-11-27T09:30:00Z");
    }

    [Fact]
    public async Task Criterion6_AnyStaff_AssignsLeastBusyDeterministically_AndNameShownOnCreate()
    {
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/appointments", new
        {
            branchId = _merkez,
            newCustomer = new { fullName = "Selin Öztürk", phone = "+905550004418" },
            channel = "Panel",
            startUtc = "2026-11-27T07:00:00Z",
            items = new[] { new { serviceId = _kesim, staffId = _zeynep } },
        }, _token, Key())).Status.ShouldBe(201);
        var items = new[] { new { serviceId = _kesim, staffId = (string?)null } };

        var slots = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/availability/slots", new { items, date = "2026-11-27" });
        var at14 = slots.Body.GetProperty("slots").EnumerateArray().Single(s => string.Equals(s.GetProperty("localTime").GetString(), "14:00", StringComparison.Ordinal));
        at14.GetProperty("items")[0].GetProperty("staffId").GetString().ShouldBe(_elif, "o gün doluluğu daha az olan");
        var again = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/availability/slots", new { items, date = "2026-11-27" });
        again.Body.GetProperty("slots").EnumerateArray().Single(s => string.Equals(s.GetProperty("localTime").GetString(), "14:00", StringComparison.Ordinal))
            .GetProperty("items")[0].GetProperty("staffId").GetString().ShouldBe(_elif, "deterministik");

        var created = await CreateAsync(await VerifyAsync(AyşePhone), items, "2026-11-27T11:00:00Z");
        created.Status.ShouldBe(201, created.Body.ToString());
        created.Body.GetProperty("items")[0].GetProperty("staffName").GetString().ShouldBe(at14.GetProperty("items")[0].GetProperty("staffName").GetString());
        (await _api.ScalarAsync<string>($"SELECT staff_id::text FROM scheduling.appointment_items WHERE appointment_id = '{created.Body.GetProperty("id").GetString()}'")).ShouldBe(_elif);
    }

    [Fact]
    public async Task Criterion7_FullDay_NoDotInStrip_NextAvailableReturned()
    {
        var gelin = await ServiceAsync("Gelin paketi", 600, 900000, [_elif]);
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/appointments", new
        {
            branchId = _merkez,
            newCustomer = new { fullName = "Selin Öztürk", phone = "+905550004418" },
            channel = "Panel",
            startUtc = "2026-11-26T06:00:00Z",
            items = new[] { new { serviceId = gelin, staffId = _elif } },
        }, _token, Key())).Status.ShouldBe(201);
        var items = new[] { new { serviceId = _kesim, staffId = _elif } };

        var strip = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/availability/days", new { items, from = "2026-11-26", to = "2026-11-28" });
        strip.Status.ShouldBe(200, strip.Body.ToString());
        var days = strip.Body.GetProperty("days").EnumerateArray().ToList();
        days[0].GetProperty("hasAvailability").GetBoolean().ShouldBeFalse("Perşembe dolu");
        days[0].GetProperty("isClosed").GetBoolean().ShouldBeFalse();
        days[1].GetProperty("firstSlotUtc").GetString().ShouldBe("2026-11-27T06:00:00Z");
        strip.Body.TryGetProperty("nextAvailableUtc", out _).ShouldBeFalse("aralıkta boş gün var");

        var thursday = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/availability/days", new { items, from = "2026-11-26", to = "2026-11-26" });
        thursday.Body.GetProperty("nextAvailableUtc").GetString().ShouldBe("2026-11-27T06:00:00Z", "en yakın uygun saat Cuma");
        var slots = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/availability/slots", new { items, date = "2026-11-26" });
        slots.Body.GetProperty("slots").GetArrayLength().ShouldBe(0);
        slots.Body.GetProperty("unavailableTimes").GetArrayLength().ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Criterion8_Window_MinNoticeFirstSlotToday_And61stDayNotSelectable()
    {
        _factory.Clock.Reset(Instant.FromUtc(2026, 11, 26, 7, 5));
        var items = new[] { new { serviceId = _kesim, staffId = _elif } };

        (await FirstSlotAsync(_merkez, new { items, date = "2026-11-26" })).ShouldBe("12:15", "10:05 + 120 dk → ızgarada 12:15");

        var far = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/availability/days", new { items, from = "2027-01-24", to = "2027-01-26" });
        far.Body.GetProperty("maxDate").GetString().ShouldBe("2027-01-25");
        var days = far.Body.GetProperty("days").EnumerateArray().ToList();
        days[1].GetProperty("hasAvailability").GetBoolean().ShouldBeTrue("60. gün");
        days[2].GetProperty("hasAvailability").GetBoolean().ShouldBeFalse("61. gün seçilemez");

        var token = await VerifyAsync(AyşePhone);
        var tooFar = await CreateAsync(token, items, "2027-01-26T07:00:00Z");
        tooFar.Status.ShouldBe(400);
        tooFar.Body.GetProperty("errors").GetProperty("startUtc")[0].GetString().ShouldBe("out_of_range");
    }

    [Fact]
    public async Task Criterion9_TrustedDevice_SecondRequestAlreadyVerified_WithFirstName_NoSms()
    {
        var first = await RequestOtpAsync(AyşePhone);
        var verified = await _api.SendAsync(HttpMethod.Post, "/api/v1/public/otp/verify",
            new { otpId = first.Body.GetProperty("challenge").GetProperty("otpId").GetGuid(), code = await LastCodeAsync(), rememberDevice = true });
        verified.Status.ShouldBe(200, verified.Body.ToString());
        verified.Body.TryGetProperty("knownCustomerFirstName", out _).ShouldBeFalse("salonda henüz kartı yok");
        var setCookie = verified.Header("Set-Cookie")!;
        setCookie.ShouldContain("omvila_td=");
        setCookie.ShouldContain("httponly", Case.Insensitive);
        setCookie.ShouldContain("path=/api/v1/public/otp", Case.Insensitive);
        var cookie = setCookie.Split(';')[0];
        (await CreateAsync(verified.Body.GetProperty("bookingToken").GetString()!, new[] { new { serviceId = _kesim, staffId = (string?)_elif } }, "2026-11-26T11:30:00Z"))
            .Status.ShouldBe(201);
        var smsBefore = await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'otp.code'");

        var second = await RequestOtpAsync("0532 123 45 67", cookie);

        second.Status.ShouldBe(200, second.Body.ToString());
        second.Body.GetProperty("status").GetString().ShouldBe("AlreadyVerified");
        var booking = second.Body.GetProperty("booking");
        booking.GetProperty("knownCustomerFirstName").GetString().ShouldBe("Ayşe");
        booking.GetProperty("phoneMasked").GetString().ShouldBe("0532 *** ** 67");
        booking.GetProperty("expiresInSeconds").GetInt32().ShouldBe(1800);
        (await CreateAsync(booking.GetProperty("bookingToken").GetString()!, new[] { new { serviceId = _fon, staffId = (string?)_elif } }, "2026-11-27T11:30:00Z"))
            .Status.ShouldBe(201);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'otp.code'")).ShouldBe(smsBefore);

        var otherPhone = await RequestOtpAsync("+905321112233", cookie);
        otherPhone.Body.GetProperty("status").GetString().ShouldBe("CodeSent", "çerez başka telefona ait");
    }

    [Fact]
    public async Task Criterion16_SundayClosed_ExtraOpeningOverrideOpensIt()
    {
        var items = new[] { new { serviceId = _kesim, staffId = _elif } };
        var closed = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/availability/days", new { items, from = "2026-11-29", to = "2026-11-29" });
        closed.Body.GetProperty("days")[0].GetProperty("isClosed").GetBoolean().ShouldBeTrue();
        closed.Body.GetProperty("days")[0].GetProperty("hasAvailability").GetBoolean().ShouldBeFalse();

        (await PostAsync($"/api/v1/manage/branches/{_merkez}/overrides",
            new { fromDate = "2026-11-29", kind = "ExtraOpening", isClosed = false, intervals = new[] { new { start = "10:00", end = "16:00" } } })).Status.ShouldBe(201);

        var open = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/availability/days", new { items, from = "2026-11-29", to = "2026-11-29" });
        open.Body.GetProperty("days")[0].GetProperty("isClosed").GetBoolean().ShouldBeFalse();
        open.Body.GetProperty("days")[0].GetProperty("firstSlotUtc").GetString().ShouldBe("2026-11-29T07:00:00Z");
    }

    [Fact]
    public async Task Criterion17_ReadOnlySubscription_BookingDisabled()
    {
        var token = await VerifyAsync(AyşePhone);
        await _api.ScalarAsync<int>("UPDATE platform.subscriptions SET status = 'ReadOnly' RETURNING 1");
        var items = new[] { new { serviceId = _kesim, staffId = (string?)_elif } };

        (await _api.SendAsync(HttpMethod.Get, "/api/v1/public/brands/kuafor-elif", null)).Body.GetProperty("bookingEnabled").GetBoolean().ShouldBeFalse();
        await ShouldBeBookingDisabledAsync(await _api.SendAsync(HttpMethod.Get, $"/api/v1/public/branches/{_merkez}/catalog", null));
        await ShouldBeBookingDisabledAsync(await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/availability/days", new { items, from = "2026-11-26", to = "2026-11-26" }));
        await ShouldBeBookingDisabledAsync(await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/availability/slots", new { items, date = "2026-11-26" }));
        await ShouldBeBookingDisabledAsync(await CreateAsync(token, items, "2026-11-26T11:30:00Z"));
    }

    [Fact]
    public async Task BlockedCustomer_IsForbidden_AndNothingWritten()
    {
        var customer = await PostAsync("/api/v1/manage/customers", new { fullName = "Ayşe Yılmaz", phone = AyşePhone });
        await _api.ScalarAsync<int>($"UPDATE customers.brand_customers SET is_blocked = true WHERE id = '{customer.Body.GetProperty("id").GetString()}' RETURNING 1");

        var result = await CreateAsync(await VerifyAsync(AyşePhone), new[] { new { serviceId = _kesim, staffId = (string?)_elif } }, "2026-11-26T11:30:00Z");

        result.Status.ShouldBe(403);
        result.Body.GetProperty("code").GetString().ShouldBe("customer_blocked");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM scheduling.appointments")).ShouldBe(0);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM customers.consent_records")).ShouldBe(0);
    }

    [Fact]
    public async Task IdempotentReplay_ReturnsFirstResponse_OneAppointment()
    {
        var token = await VerifyAsync(AyşePhone);
        var key = new Dictionary<string, string>(StringComparer.Ordinal) { ["Idempotency-Key"] = "0193a8f0-0000-7000-8000-00000000d201" };
        var body = Body(new[] { new { serviceId = _kesim, staffId = (string?)_elif } }, "2026-11-26T11:30:00Z", marketing: false);

        var first = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/appointments", body, token, key);
        var second = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/appointments", body, token, key);

        first.Status.ShouldBe(201, first.Body.ToString());
        second.Status.ShouldBe(201);
        second.Header("Idempotent-Replayed").ShouldBe("true");
        second.Body.GetProperty("id").GetString().ShouldBe(first.Body.GetProperty("id").GetString());
        (await _api.ScalarAsync<long>("SELECT count(*) FROM scheduling.appointments")).ShouldBe(1);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM customers.consent_records")).ShouldBe(1, "kampanya izni verilmedi");
    }

    [Fact]
    public async Task BookingToken_MissingInvalidExpiredOrManage_Is401()
    {
        var items = new[] { new { serviceId = _kesim, staffId = (string?)_elif } };
        var token = await VerifyAsync(AyşePhone);

        (await CreateAsync(null, items, "2026-11-26T11:30:00Z")).Status.ShouldBe(401);
        (await CreateAsync("not-a-token", items, "2026-11-26T11:30:00Z")).Status.ShouldBe(401);
        (await CreateAsync(_token, items, "2026-11-26T11:30:00Z")).Status.ShouldBe(401, "panel jetonu rezervasyonda geçmez");
        (await _api.SendAsync(HttpMethod.Get, $"/api/v1/public/branches/{_merkez}/my-packages", null, token)).Body.GetProperty("items").GetArrayLength().ShouldBe(0);

        _factory.Clock.Advance(Duration.FromMinutes(31));
        var expired = await CreateAsync(token, items, "2026-11-26T11:30:00Z");
        expired.Status.ShouldBe(401);
        expired.Body.GetProperty("code").GetString().ShouldBe("token_expired");
        (await _api.SendAsync(HttpMethod.Get, $"/api/v1/public/branches/{_merkez}/my-packages", null)).Status.ShouldBe(401);
    }

    [Fact]
    public async Task Validation_PrivacyNoticeRequired_StaffSelectionRules_UnknownBranch()
    {
        var token = await VerifyAsync(AyşePhone);
        var invalid = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/appointments",
            new { items = new[] { new { serviceId = _kesim } }, startUtc = "2026-11-26T11:30:00Z", fullName = "A", privacyNoticeAcknowledged = false }, token, Key());
        invalid.Status.ShouldBe(400);
        var errors = invalid.Body.GetProperty("errors");
        errors.GetProperty("privacyNoticeAcknowledged")[0].GetString().ShouldBe("required");
        errors.GetProperty("privacyNoticeVersion")[0].GetString().ShouldBe("required");
        errors.GetProperty("fullName")[0].GetString().ShouldBe("out_of_range");

        await _api.ScalarAsync<int>("UPDATE tenancy.brand_settings SET allow_staff_selection = false RETURNING 1");
        var withStaff = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/availability/slots",
            new { items = new[] { new { serviceId = _kesim, staffId = _elif } }, date = "2026-11-26" });
        withStaff.Status.ShouldBe(400);
        withStaff.Body.GetProperty("errors").GetProperty("items[0].staffId")[0].GetString().ShouldBe("out_of_range");

        (await _api.SendAsync(HttpMethod.Get, "/api/v1/public/branches/0193a8f0-0000-7000-8000-0000000000ff/catalog", null)).Status.ShouldBe(404);
        (await _api.SendAsync(HttpMethod.Get, "/api/v1/public/brands/olmayan-salon", null)).Status.ShouldBe(404);
    }

    [Fact]
    public async Task PublicBrand_OldSlugRedirects301_ShowPricesFalseHidesPrices()
    {
        var page = await _api.GetAsync("/api/v1/manage/brand/booking-page", _token);
        (await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/brand/booking-page",
            new { slug = "elif-salon", coverColor = "kum", showReviews = true, showPrices = false, preAppointmentNote = (string?)null }, _token, page.ETag)).Status.ShouldBe(200);

        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        using var old = await client.GetAsync(new Uri("/api/v1/public/brands/kuafor-elif", UriKind.Relative), TestContext.Current.CancellationToken);
        ((int)old.StatusCode).ShouldBe(301);
        old.Headers.Location!.OriginalString.ShouldBe("/api/v1/public/brands/elif-salon");
        var current = await _api.SendAsync(HttpMethod.Get, "/api/v1/public/brands/elif-salon", null);
        current.Body.GetProperty("coverColor").GetString().ShouldBe("kum");
        current.Body.GetProperty("showPrices").GetBoolean().ShouldBeFalse();

        var catalog = await _api.SendAsync(HttpMethod.Get, $"/api/v1/public/branches/{_merkez}/catalog", null);
        catalog.Body.GetProperty("services").EnumerateArray().All(s => !s.TryGetProperty("price", out _)).ShouldBeTrue("fiyatlar gizli");
    }

    private static Task ShouldBeBookingDisabledAsync(ManageApi.Response response)
    {
        response.Status.ShouldBe(403, response.Body.ToString());
        response.Body.GetProperty("code").GetString().ShouldBe("booking_disabled");
        return Task.CompletedTask;
    }

    private async Task<string> FirstSlotAsync(string branchId, object request)
    {
        var slots = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{branchId}/availability/slots", request);
        slots.Status.ShouldBe(200, slots.Body.ToString());
        return slots.Body.GetProperty("slots")[0].GetProperty("localTime").GetString()!;
    }

    /// <summary>Nişantaşı şubesi (Pzt–Cmt 09:00–19:00) ve iki şubede çalışan Mert: Cuma Merkez 09:00–12:00, Nişantaşı 12:00–19:00.</summary>
    private async Task<(string Branch, string Mert)> SecondBranchWithMertAsync()
    {
        var nisantasi = (await PostAsync("/api/v1/manage/branches",
            new { name = "Nişantaşı", addressLine = "Abdi İpekçi Cad. 1", district = "Şişli", city = "İstanbul" })).Body.GetProperty("id").GetString()!;
        await SetBranchHoursAsync(nisantasi, Week(1, 2, 3, 4, 5, 6));
        var mert = await PostAsync("/api/v1/manage/staff",
            new { fullName = "Mert Demir", role = "Staff", calendarColor = "#B4532A", branchIds = new[] { _merkez, nisantasi }, sendInvite = false, copyBranchWorkingHours = false });
        var mertId = mert.Body.GetProperty("id").GetString()!;
        var merkezHours = await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{mertId}/branches/{_merkez}/working-hours",
            new { days = new[] { new { dayOfWeek = 5, intervals = new[] { new { start = "09:00", end = "12:00" } } } } }, _token, mert.ETag);
        merkezHours.Status.ShouldBe(200, merkezHours.Body.ToString());
        var nisantasiHours = await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{mertId}/branches/{nisantasi}/working-hours",
            new { days = new[] { new { dayOfWeek = 5, intervals = new[] { new { start = "12:00", end = "19:00" } } } } }, _token, merkezHours.ETag);
        nisantasiHours.Status.ShouldBe(200, nisantasiHours.Body.ToString());
        var services = await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{mertId}/services",
            new { items = new[] { new { serviceId = _kesim } } }, _token, nisantasiHours.ETag);
        services.Status.ShouldBe(200, services.Body.ToString());
        return (nisantasi, mertId);
    }

    private async Task SetBranchHoursAsync(string branchId, object week)
    {
        var hours = await _api.GetAsync($"/api/v1/manage/branches/{branchId}/working-hours", _token);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/branches/{branchId}/working-hours", week, _token, hours.ETag)).Status.ShouldBe(200);
    }

    private Task<ManageApi.Response> RequestOtpAsync(string phone, string? cookie = null) =>
        _api.SendAsync(HttpMethod.Post, "/api/v1/public/otp/request", new { phone, branchId = _merkez }, headers: cookie is null
            ? null
            : new Dictionary<string, string>(StringComparer.Ordinal) { ["Cookie"] = cookie });

    private Task<string> LastCodeAsync(string phoneE164 = AyşePhone) => _api.ScalarAsync<string>(
        $"SELECT payload->>'kod' FROM notifications.notification_outbox WHERE event_code = 'otp.code' AND recipient_phone = '{phoneE164}' ORDER BY created_utc DESC, id DESC LIMIT 1");

    /// <summary>Kod iste → SMS kaydından kodu oku → doğrula; rezervasyon jetonunu döner.</summary>
    private async Task<string> VerifyAsync(string phone)
    {
        var requested = await RequestOtpAsync(phone);
        requested.Status.ShouldBe(200, requested.Body.ToString());
        requested.Body.GetProperty("status").GetString().ShouldBe("CodeSent");
        var verified = await _api.SendAsync(HttpMethod.Post, "/api/v1/public/otp/verify",
            new { otpId = requested.Body.GetProperty("challenge").GetProperty("otpId").GetGuid(), code = await LastCodeAsync(phone) });
        verified.Status.ShouldBe(200, verified.Body.ToString());
        return verified.Body.GetProperty("bookingToken").GetString()!;
    }

    private static object Body(object items, string startUtc, bool marketing) => new
    {
        items,
        sameStaffForAll = true,
        startUtc,
        fullName = "Ayşe Yılmaz",
        customerNote = "Uzun saç",
        privacyNoticeAcknowledged = true,
        privacyNoticeVersion = "2026-11-01",
        marketingConsent = marketing,
    };

    private Task<ManageApi.Response> CreateAsync(string? token, object items, string startUtc, bool marketing = false, string? branchId = null) =>
        _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{branchId ?? _merkez}/appointments", Body(items, startUtc, marketing), token, Key());

    private Task<ManageApi.Response> PostAsync(string path, object body) => _api.SendAsync(HttpMethod.Post, path, body, _token, Key());

    private async Task<string> ServiceAsync(string name, int minutes, long price, string[] staffIds) =>
        (await PostAsync("/api/v1/manage/services", new
        {
            categoryId = _category,
            name,
            durationMinutes = minutes,
            price = new { amountMinor = price, currency = "TRY" },
            priceDisplay = "Fixed",
            deposit = new { mode = "Inherit" },
            isOnlineBookable = true,
            staffIds,
        })).Body.GetProperty("id").GetString()!;

    private static object Week(params int[] days) => new
    {
        days = days.Select(d => new { dayOfWeek = d, intervals = new[] { new { start = "09:00", end = "19:00" } } }).ToArray(),
    };

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _requests):D12}" };
}
