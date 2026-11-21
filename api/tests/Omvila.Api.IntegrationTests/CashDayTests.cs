using System.Text.Json;
using NodaTime;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-05 kabul kriteri 7 (gün kapatma: beklenen 3.450, sayılan 3.400 → fark −50 ve açıklama); alan-modeli.md § CashDayClose
// ("kapanıştan sonra" işareti); docs/acik-sorular.md W3 (beklenen nakit = nakit tahsilatlar − nakit iadeler, gün şubenin saat
// diliminde), T-86, T-96; roller-yetkiler.md payment.collect (Kasa) ve cash.close (Owner, BranchManager Ş; personel 403);
// sözleşme manageGetCashDay, manageCloseCashDay. Saat: 2026-11-25 09:00Z (Salı 12:00 İstanbul).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class CashDayTests(PostgresFixture db) : IAsyncLifetime
{
    private const string Today = "2026-11-25";
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private string _merkez = null!;
    private string _brand = null!;
    private string _elif = null!;
    private string _kesim = null!;
    private int _requests;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db, new Dictionary<string, string?>(StringComparer.Ordinal) { ["Auth:AccessTokenMinutes"] = "6000" });
        _api = new ManageApi(_factory, db);
        _token = await _api.SignupAsync("+905324187705", "Kuaför Elif");
        var session = (await _api.GetAsync("/api/v1/manage/session", _token)).Body;
        _merkez = session.GetProperty("branches")[0].GetProperty("id").GetString()!;
        _brand = session.GetProperty("brand").GetProperty("id").GetString()!;
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
            durationMinutes = 30,
            price = new { amountMinor = 45000, currency = "TRY" },
            priceDisplay = "Fixed",
            deposit = new { mode = "Never" },
            isOnlineBookable = true,
            staffIds = new[] { _elif },
        })).Body.GetProperty("id").GetString()!;
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task EmptyDay_IsOpen_ZeroTotals_TodayByDefault()
    {
        var day = await GetAsync(null);

        day.Status.ShouldBe(200, day.Body.ToString());
        var body = day.Body;
        body.GetProperty("branchId").GetString().ShouldBe(_merkez);
        body.GetProperty("date").GetString().ShouldBe(Today);
        body.GetProperty("timeZone").GetString().ShouldBe("Europe/Istanbul");
        body.GetProperty("status").GetString().ShouldBe("Open");
        Amount(body, "total").ShouldBe(0);
        body.GetProperty("total").GetProperty("count").GetInt32().ShouldBe(0);
        Amount(body, "expectedCash").ShouldBe(0);
        body.GetProperty("payments").GetArrayLength().ShouldBe(0);
        body.GetProperty("closing").ValueKind.ShouldBe(JsonValueKind.Null);
    }

    [Fact]
    public async Task Day_TotalsByMethod_RowsWithCustomerServiceStaffAndPackage()
    {
        var appointmentId = await SeedCriterion7Async();

        var day = (await GetAsync(Today)).Body;
        Amount(day, "total").ShouldBe(534000);
        day.GetProperty("total").GetProperty("count").GetInt32().ShouldBe(5);
        Amount(day, "cash").ShouldBe(345000);
        day.GetProperty("cash").GetProperty("count").GetInt32().ShouldBe(2);
        Amount(day, "card").ShouldBe(124000);
        Amount(day, "transfer").ShouldBe(65000);
        day.GetProperty("transfer").GetProperty("depositAmount").GetProperty("amountMinor").GetInt64().ShouldBe(15000);
        day.GetProperty("transfer").GetProperty("otherAmount").GetProperty("amountMinor").GetInt64().ShouldBe(50000);
        Amount(day, "expectedCash").ShouldBe(345000);
        var rows = day.GetProperty("payments").EnumerateArray().ToList();
        rows.Select(r => r.GetProperty("method").GetString()).ShouldBe(["Cash", "Cash", "Card", "Transfer", "Transfer"], "yeniden eskiye");
        var walkIn = rows[0];
        walkIn.GetProperty("purpose").GetString().ShouldBe("Appointment");
        walkIn.GetProperty("appointmentId").GetString().ShouldBe(appointmentId);
        walkIn.GetProperty("serviceNames").EnumerateArray().Select(s => s.GetString()).ShouldBe(["Saç kesimi"]);
        walkIn.GetProperty("staffName").GetString().ShouldNotBeNullOrEmpty();
        walkIn.GetProperty("customer").GetProperty("fullName").GetString().ShouldBe("Okan Er");
        walkIn.GetProperty("customer").TryGetProperty("phone", out _).ShouldBeFalse("kasada tam telefon dönmez");
        walkIn.GetProperty("recordedAfterDayClose").GetBoolean().ShouldBeFalse();
        var package = rows[4];
        package.GetProperty("purpose").GetString().ShouldBe("CustomerPackage");
        package.GetProperty("customerPackageId").GetString().ShouldNotBeNullOrEmpty();
        package.TryGetProperty("appointmentId", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task Criterion7_Expected3450Counted3400_DifferenceMinus50_NoteRequired()
    {
        await SeedCriterion7Async();
        var at = _factory.Clock.GetCurrentInstant();
        Amount((await GetAsync(Today)).Body, "expectedCash").ShouldBe(345000);

        var noNote = await CloseAsync(Today, 340000, null);
        noNote.Status.ShouldBe(400, noNote.Body.ToString());
        noNote.Body.GetProperty("code").GetString().ShouldBe("validation_failed");
        noNote.Body.GetProperty("errors").GetProperty("differenceNote")[0].GetString().ShouldBe("required");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM sales.cash_day_closes")).ShouldBe(0);

        var closed = await CloseAsync(Today, 340000, "Bozuk para verildi");

        closed.Status.ShouldBe(200, closed.Body.ToString());
        closed.Body.GetProperty("status").GetString().ShouldBe("Closed");
        var closing = closed.Body.GetProperty("closing");
        closing.GetProperty("expectedCash").GetProperty("amountMinor").GetInt64().ShouldBe(345000);
        closing.GetProperty("countedCash").GetProperty("amountMinor").GetInt64().ShouldBe(340000);
        closing.GetProperty("differenceMinor").GetInt64().ShouldBe(-5000);
        closing.GetProperty("difference").GetString().ShouldBe("Shortage");
        closing.GetProperty("differenceNote").GetString().ShouldBe("Bozuk para verildi");
        closing.GetProperty("closedUtc").GetString().ShouldBe(Iso(at));
        (await _api.ScalarAsync<string>(
            "SELECT business_date || '/' || expected_cash_minor || '/' || counted_cash_minor || '/' || difference_minor || '/' || (closed_by_user_id IS NOT NULL) FROM sales.cash_day_closes"))
            .ShouldBe($"{Today}/345000/340000/-5000/true");
        (await GetAsync(Today)).Body.GetProperty("status").GetString().ShouldBe("Closed");
    }

    // İstanbul gece yarısı = 21:00Z: 24 Kasım 23:59:59 önceki güne, 25 Kasım 00:00 bugüne düşer.
    [Fact]
    public async Task DayBoundary_IsBranchLocalMidnight()
    {
        await InsertPaymentAsync("Cash", 11100, Instant.FromUtc(2026, 11, 24, 20, 59, 59));
        await InsertPaymentAsync("Cash", 22200, Instant.FromUtc(2026, 11, 24, 21, 0, 0));
        await InsertPaymentAsync("Cash", 33300, Instant.FromUtc(2026, 11, 25, 20, 59, 59));
        await InsertPaymentAsync("Cash", 44400, Instant.FromUtc(2026, 11, 25, 21, 0, 0));

        Amount((await GetAsync("2026-11-24")).Body, "cash").ShouldBe(11100);
        Amount((await GetAsync(Today)).Body, "cash").ShouldBe(55500);
        Amount((await GetAsync("2026-11-26")).Body, "cash").ShouldBe(44400);
    }

    // W3: iade, işaretlendiği günün beklenen nakdinden düşülür (tahsilat önceki günün olsa da).
    [Fact]
    public async Task CashRefundMarkedToday_IsDeductedFromExpected()
    {
        var yesterday = await InsertPaymentAsync("Cash", 50000, Instant.FromUtc(2026, 11, 24, 10, 0));
        await InsertPaymentAsync("Cash", 200000, Instant.FromUtc(2026, 11, 25, 8, 0));
        await _api.ScalarAsync<int>(
            $"UPDATE sales.payments SET refunded_amount_minor = 20000, status = 'PartiallyRefunded', refunded_utc = '2026-11-25T08:30:00Z' WHERE id = '{yesterday}' RETURNING 1");
        // Havale (kapora) iadesi nakit kasasını etkilemez.
        var deposit = await InsertPaymentAsync("Transfer", 15000, Instant.FromUtc(2026, 11, 25, 7, 0), isDeposit: true);
        await _api.ScalarAsync<int>(
            $"UPDATE sales.payments SET refunded_amount_minor = 15000, status = 'Refunded', refunded_utc = '2026-11-25T08:40:00Z' WHERE id = '{deposit}' RETURNING 1");

        var today = (await GetAsync(Today)).Body;
        Amount(today, "cash").ShouldBe(200000);
        Amount(today, "cashRefunded").ShouldBe(20000);
        Amount(today, "expectedCash").ShouldBe(180000);
        var refundedRow = today.GetProperty("payments").EnumerateArray().Single(p => string.Equals(p.GetProperty("id").GetString(), deposit, StringComparison.Ordinal));
        refundedRow.GetProperty("refundedAmount").GetProperty("amountMinor").GetInt64().ShouldBe(15000);
        refundedRow.GetProperty("status").GetString().ShouldBe("Refunded");
        var previous = (await GetAsync("2026-11-24")).Body;
        Amount(previous, "cashRefunded").ShouldBe(0);
        Amount(previous, "expectedCash").ShouldBe(50000);

        var closed = await CloseAsync(Today, 180000, null);
        closed.Status.ShouldBe(200, closed.Body.ToString());
        closed.Body.GetProperty("closing").GetProperty("difference").GetString().ShouldBe("None");
    }

    [Fact]
    public async Task CloseTwice_Is409_SameKeyReplays()
    {
        var key = Key();
        var body = CloseBody(Today, 0, null);
        var first = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/cash/close", body, _token, key);
        var replay = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/cash/close", body, _token, key);
        var again = await CloseAsync(Today, 0, null);

        first.Status.ShouldBe(200, first.Body.ToString());
        replay.Status.ShouldBe(200, replay.Body.ToString());
        replay.Header("Idempotent-Replayed").ShouldBe("true");
        again.Status.ShouldBe(409, again.Body.ToString());
        again.Body.GetProperty("code").GetString().ShouldBe("cash_day_already_closed");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM sales.cash_day_closes")).ShouldBe(1);
    }

    [Fact]
    public async Task PaymentAfterClose_IsMarked_ClosingKeepsSnapshot()
    {
        await InsertPaymentAsync("Cash", 45000, Instant.FromUtc(2026, 11, 25, 8, 0));
        (await CloseAsync(Today, 45000, null)).Status.ShouldBe(200);
        _factory.Clock.Advance(Duration.FromHours(1));

        await CompletedWalkInAsync("Cash");

        var day = (await GetAsync(Today)).Body;
        day.GetProperty("status").GetString().ShouldBe("Closed");
        Amount(day, "expectedCash").ShouldBe(90000);
        day.GetProperty("closing").GetProperty("expectedCash").GetProperty("amountMinor").GetInt64().ShouldBe(45000);
        day.GetProperty("payments").EnumerateArray().Select(p => p.GetProperty("recordedAfterDayClose").GetBoolean()).ShouldBe([true, false]);
    }

    [Fact]
    public async Task InvalidRequests_Are400()
    {
        var missing = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/cash/close", new { }, _token, Key());
        missing.Status.ShouldBe(400, missing.Body.ToString());
        var errors = missing.Body.GetProperty("errors");
        errors.GetProperty("branchId")[0].GetString().ShouldBe("required");
        errors.GetProperty("date")[0].GetString().ShouldBe("required");
        errors.GetProperty("countedCash")[0].GetString().ShouldBe("required");

        var bad = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/cash/close", new
        {
            branchId = _merkez,
            date = "25.11.2026",
            countedCash = new { amountMinor = 100, currency = "USD" },
        }, _token, Key());
        bad.Status.ShouldBe(400, bad.Body.ToString());
        bad.Body.GetProperty("errors").GetProperty("date")[0].GetString().ShouldBe("invalid_format");
        bad.Body.GetProperty("errors").GetProperty("countedCash.currency")[0].GetString().ShouldBe("invalid_format");

        var negative = await CloseAsync(Today, -1, "x");
        negative.Body.GetProperty("errors").GetProperty("countedCash")[0].GetString().ShouldBe("out_of_range");

        var future = await CloseAsync("2026-11-26", 0, null);
        future.Status.ShouldBe(400, future.Body.ToString());
        future.Body.GetProperty("errors").GetProperty("date")[0].GetString().ShouldBe("out_of_range");

        (await _api.GetAsync("/api/v1/manage/cash", _token)).Body.GetProperty("errors").GetProperty("branchId")[0].GetString().ShouldBe("required");
        (await GetAsync("2026/11/25")).Body.GetProperty("errors").GetProperty("date")[0].GetString().ShouldBe("invalid_format");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM sales.cash_day_closes")).ShouldBe(0);
    }

    [Fact]
    public async Task StaffRole_Is403()
    {
        var staff = await _api.AddMemberAndSignInAsync(_token, "+905382741905", "Staff", _merkez);

        (await _api.GetAsync($"/api/v1/manage/cash?branchId={_merkez}", staff)).Status.ShouldBe(403);
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/cash/close", CloseBody(Today, 0, null), staff, Key())).Status.ShouldBe(403);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM sales.cash_day_closes")).ShouldBe(0);
    }

    [Fact]
    public async Task BranchManager_OwnBranchOnly()
    {
        var kadikoy = (await PostAsync("/api/v1/manage/branches", new { name = "Kadıköy", addressLine = "Moda Cad. 1", district = "Kadıköy", city = "İstanbul" }))
            .Body.GetProperty("id").GetString()!;
        await InsertPaymentAsync("Cash", 45000, Instant.FromUtc(2026, 11, 25, 8, 0), branchId: kadikoy);
        var manager = await _api.AddMemberAndSignInAsync(_token, "+905382741906", "BranchManager", _merkez);

        var own = await _api.GetAsync($"/api/v1/manage/cash?branchId={_merkez}", manager);
        own.Status.ShouldBe(200, own.Body.ToString());
        Amount(own.Body, "cash").ShouldBe(0, "başka şubenin tahsilatı görünmez");
        (await _api.GetAsync($"/api/v1/manage/cash?branchId={kadikoy}", manager)).Status.ShouldBe(404);
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/cash/close", CloseBody(Today, 0, null, kadikoy), manager, Key())).Status.ShouldBe(404);

        var closed = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/cash/close", CloseBody(Today, 0, null), manager, Key());
        closed.Status.ShouldBe(200, closed.Body.ToString());
        Amount((await _api.GetAsync($"/api/v1/manage/cash?branchId={kadikoy}", _token)).Body, "cash").ShouldBe(45000);
        (await _api.GetAsync($"/api/v1/manage/cash?branchId={kadikoy}", _token)).Body.GetProperty("status").GetString().ShouldBe("Open", "kapanış şube bazında");
    }

    [Fact]
    public async Task OtherBrand_Is404_UnknownBranch_Is404()
    {
        await InsertPaymentAsync("Cash", 45000, Instant.FromUtc(2026, 11, 25, 8, 0));
        var other = await _api.SignupAsync("+905427771122", "Berber Can");

        (await _api.GetAsync($"/api/v1/manage/cash?branchId={_merkez}", other)).Status.ShouldBe(404);
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/cash/close", CloseBody(Today, 0, null), other, Key())).Status.ShouldBe(404);
        (await _api.GetAsync("/api/v1/manage/cash?branchId=0193a8f0-0000-7000-8000-0000000000ff", _token)).Status.ShouldBe(404);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM sales.cash_day_closes")).ShouldBe(0);
    }

    // Müşteri kartı Ödemeler sekmesi (sözleşme manageListCustomerPayments; T-96): kartın tahsilatları yeniden eskiye, imleçle.
    [Fact]
    public async Task CustomerPayments_NewestFirst_Paged_PackageAndAfterClose()
    {
        var appointmentId = await CompletedWalkInAsync("Cash");
        var customer = await CustomerIdAsync();
        var deposit = await InsertPaymentAsync("Transfer", 15000, Instant.FromUtc(2026, 11, 24, 10, 0), isDeposit: true, customerId: customer);
        var package = await InsertPaymentAsync("Card", 50000, Instant.FromUtc(2026, 11, 20, 10, 0), targetType: "CustomerPackage", customerId: customer);
        await InsertPaymentAsync("Cash", 99900, Instant.FromUtc(2026, 11, 25, 8, 0));
        (await CloseAsync(Today, 144900, null)).Status.ShouldBe(200);
        _factory.Clock.Advance(Duration.FromMinutes(5));
        var late = await InsertPaymentAsync("Cash", 1000, _factory.Clock.GetCurrentInstant(), customerId: customer);

        var first = await _api.GetAsync($"/api/v1/manage/customers/{customer}/payments?limit=2", _token);

        first.Status.ShouldBe(200, first.Body.ToString());
        var items = first.Body.GetProperty("items").EnumerateArray().ToList();
        items.Select(i => i.GetProperty("id").GetString()).First().ShouldBe(late);
        items[0].GetProperty("recordedAfterDayClose").GetBoolean().ShouldBeTrue();
        items[1].GetProperty("appointmentId").GetString().ShouldBe(appointmentId);
        items[1].GetProperty("recordedAfterDayClose").GetBoolean().ShouldBeFalse();
        items[1].GetProperty("customer").GetProperty("fullName").GetString().ShouldBe("Okan Er");
        var cursor = first.Body.GetProperty("nextCursor").GetString()!;

        var second = await _api.GetAsync($"/api/v1/manage/customers/{customer}/payments?limit=2&cursor={Uri.EscapeDataString(cursor)}", _token);

        second.Status.ShouldBe(200, second.Body.ToString());
        var rest = second.Body.GetProperty("items").EnumerateArray().ToList();
        rest.Select(i => i.GetProperty("id").GetString()).ShouldBe([deposit, package], "başka müşterinin tahsilatı yok");
        rest[0].GetProperty("isDeposit").GetBoolean().ShouldBeTrue();
        rest[1].GetProperty("purpose").GetString().ShouldBe("CustomerPackage");
        second.Body.GetProperty("nextCursor").ValueKind.ShouldBe(JsonValueKind.Null);

        var bad = await _api.GetAsync($"/api/v1/manage/customers/{customer}/payments?limit=0&cursor=bozuk", _token);
        bad.Status.ShouldBe(400, bad.Body.ToString());
        bad.Body.GetProperty("errors").GetProperty("limit")[0].GetString().ShouldBe("out_of_range");
        bad.Body.GetProperty("errors").GetProperty("cursor")[0].GetString().ShouldBe("invalid_format");
    }

    [Fact]
    public async Task CustomerPayments_Staff403_OtherBrand404_BranchManagerSeesOwnBranch()
    {
        await CompletedWalkInAsync("Cash");
        var customer = await CustomerIdAsync();
        var kadikoy = (await PostAsync("/api/v1/manage/branches", new { name = "Kadıköy", addressLine = "Moda Cad. 1", district = "Kadıköy", city = "İstanbul" }))
            .Body.GetProperty("id").GetString()!;
        await InsertPaymentAsync("Card", 30000, Instant.FromUtc(2026, 11, 20, 10, 0), branchId: kadikoy, customerId: customer);
        var path = $"/api/v1/manage/customers/{customer}/payments";

        (await _api.GetAsync(path, _token)).Body.GetProperty("items").GetArrayLength().ShouldBe(2);
        var manager = await _api.AddMemberAndSignInAsync(_token, "+905382741906", "BranchManager", _merkez);
        (await _api.GetAsync(path, manager)).Body.GetProperty("items").GetArrayLength().ShouldBe(1, "başka şubenin tahsilatı görünmez");
        var staff = await _api.AddMemberAndSignInAsync(_token, "+905382741905", "Staff", _merkez);
        (await _api.GetAsync(path, staff)).Status.ShouldBe(403);
        var other = await _api.SignupAsync("+905427771122", "Berber Can");
        (await _api.GetAsync(path, other)).Status.ShouldBe(404);
        (await _api.GetAsync("/api/v1/manage/customers/0193a8f0-0000-7000-8000-0000000000ff/payments", _token)).Status.ShouldBe(404);
    }

    private Task<string> CustomerIdAsync() =>
        _api.ScalarAsync<string>("SELECT brand_customer_id::text FROM sales.payments WHERE brand_customer_id IS NOT NULL LIMIT 1");

    /// <summary>KK7 günü: nakit 450 (randevu) + 3.000; kart 1.240; havale 150 kapora + 500 paket satışı.</summary>
    private async Task<string> SeedCriterion7Async()
    {
        var appointmentId = await CompletedWalkInAsync("Cash");
        var at = _factory.Clock.GetCurrentInstant();
        await InsertPaymentAsync("Cash", 300000, at.Minus(Duration.FromMinutes(30)));
        await InsertPaymentAsync("Card", 124000, at.Minus(Duration.FromMinutes(40)));
        await InsertPaymentAsync("Transfer", 15000, at.Minus(Duration.FromMinutes(50)), isDeposit: true);
        await InsertPaymentAsync("Transfer", 50000, at.Minus(Duration.FromMinutes(60)), targetType: "CustomerPackage");
        return appointmentId;
    }

    /// <summary>Kapıdan gelen (Okan Er, Saç kesimi 450 ₺) şimdi Geldi; tamamla ve ödeme al.</summary>
    private async Task<string> CompletedWalkInAsync(string method)
    {
        var created = await PostAsync("/api/v1/manage/appointments", new
        {
            branchId = _merkez,
            newCustomer = new { fullName = "Okan Er", phone = "0532 123 45 67" },
            channel = "WalkIn",
            startUtc = Iso(_factory.Clock.GetCurrentInstant()),
            items = new[] { new { serviceId = _kesim, staffId = _elif } },
        });
        created.Status.ShouldBe(201, created.Body.ToString());
        var id = created.Body.GetProperty("appointment").GetProperty("id").GetString()!;
        var headers = Key();
        headers["If-Match"] = created.ETag!;
        var completed = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/complete", new
        {
            payments = new[] { new { method, amount = new { amountMinor = 45000, currency = "TRY" } } },
        }, _token, headers);
        completed.Status.ShouldBe(200, completed.Body.ToString());
        return id;
    }

    /// <summary>Tahsilat kaydını doğrudan yazar (paket satışı ve geçmiş günler için; tahsis tek hedefe).</summary>
    private async Task<string> InsertPaymentAsync(
        string method,
        long amountMinor,
        Instant paidUtc,
        bool isDeposit = false,
        string targetType = "Appointment",
        string? branchId = null,
        string? customerId = null)
    {
        var id = $"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _requests) + 500000:D12}";
        var target = $"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _requests) + 600000:D12}";
        await _api.ScalarAsync<int>(
            $"""
            INSERT INTO sales.payments (id, brand_id, branch_id, brand_customer_id, method, amount_minor, currency_code, is_deposit, status, refunded_amount_minor, paid_utc)
              VALUES ('{id}', '{_brand}', '{branchId ?? _merkez}', {(customerId is null ? "NULL" : $"'{customerId}'")}, '{method}', {amountMinor}, 'TRY', {isDeposit}, 'Recorded', 0, '{Iso(paidUtc)}');
            INSERT INTO sales.payment_allocations (id, brand_id, payment_id, target_type, target_id, amount_minor)
              VALUES (gen_random_uuid(), '{_brand}', '{id}', '{targetType}', '{target}', {amountMinor});
            SELECT 1;
            """);
        return id;
    }

    private Task<ManageApi.Response> GetAsync(string? date) =>
        _api.GetAsync($"/api/v1/manage/cash?branchId={_merkez}" + (date is null ? string.Empty : $"&date={date}"), _token);

    private Task<ManageApi.Response> CloseAsync(string date, long counted, string? note) =>
        _api.SendAsync(HttpMethod.Post, "/api/v1/manage/cash/close", CloseBody(date, counted, note), _token, Key());

    private object CloseBody(string date, long counted, string? note, string? branchId = null) =>
        new { branchId = branchId ?? _merkez, date, countedCash = new { amountMinor = counted, currency = "TRY" }, differenceNote = note };

    private static long Amount(JsonElement body, string name) =>
        body.GetProperty(name) is var e && e.TryGetProperty("amount", out var nested)
            ? nested.GetProperty("amountMinor").GetInt64()
            : e.GetProperty("amountMinor").GetInt64();

    private Task<ManageApi.Response> PostAsync(string path, object body) => _api.SendAsync(HttpMethod.Post, path, body, _token, Key());

    private static string Iso(Instant at) => NodaTime.Text.InstantPattern.General.Format(at);

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _requests):D12}" };
}
