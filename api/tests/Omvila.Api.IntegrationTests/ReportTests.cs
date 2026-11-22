using System.Text.Json;
using NodaTime;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-06 kabul kriteri 6 (peşin satılan paket tahsilatta görünür, hak edilen gelirde seans kullanıldıkça); sözleşme
// manageGetReportOverview (tahsilat: paket satışı ve kapora dahil, iadeler düşülmüş; gelmedi oranı ve kaybı; online pay; günlük
// tahsilat, kapalı gün) ve manageGetCommissionReport (Accrued + Approved + Paid + ters kayıt; gelmeyen prime dahil değil; personel
// yalnız kendi satırı); roller-yetkiler.md report.view (Owner, BranchManager Ş; personel 403), commission.view; T-120.
// Saat: 2026-11-25 09:00Z (Çarşamba 12:00 İstanbul).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class ReportTests(PostgresFixture db) : IAsyncLifetime
{
    private const string Today = "2026-11-25";
    private const string ZeynepPhone = "+905382741905";
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private string _merkez = null!;
    private string _brand = null!;
    private string _elif = null!;
    private string _kesim = null!;
    private string _fon = null!;
    private string _customer = null!;
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

        // Pazartesi–cumartesi 09:00–19:00, pazar kapalı.
        var week = new
        {
            days = Enumerable.Range(1, 6).Select(d => new { dayOfWeek = d, intervals = new[] { new { start = "09:00", end = "19:00" } } }).ToArray(),
        };
        var hours = await _api.GetAsync($"/api/v1/manage/branches/{_merkez}/working-hours", _token);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/branches/{_merkez}/working-hours", week, _token, hours.ETag)).Status.ShouldBe(200);
        var elif = await _api.GetAsync($"/api/v1/manage/staff/{_elif}", _token);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{_elif}/branches/{_merkez}/working-hours", week, _token, elif.ETag)).Status.ShouldBe(200);
        await _api.ScalarAsync<int>($"UPDATE tenancy.staff SET service_commission_percent = 24, sale_commission_percent = 5 WHERE id = '{_elif}' RETURNING 1");
        var category = (await PostAsync("/api/v1/manage/service-categories", new { name = "Saç" })).Body.GetProperty("id").GetString()!;
        _kesim = await ServiceAsync(category, "Saç kesimi", 45000);
        _fon = await ServiceAsync(category, "Fön", 19000);
        _customer = (await PostAsync("/api/v1/manage/customers", new { fullName = "Selin Öztürk", phone = "0532 123 45 67" })).Body.GetProperty("id").GetString()!;
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task EmptyRange_ZeroAmounts_NullRates_EveryDayListed()
    {
        var report = await OverviewAsync("2026-11-22", Today);

        report.Status.ShouldBe(200, report.Body.ToString());
        var body = report.Body;
        body.GetProperty("branchId").ValueKind.ShouldBe(JsonValueKind.Null, "şube verilmezse marka geneli");
        body.GetProperty("timeZone").GetString().ShouldBe("Europe/Istanbul");
        Amount(body, "earnedRevenue").ShouldBe(0);
        Amount(body, "collections").ShouldBe(0);
        body.GetProperty("completedAppointmentCount").GetInt32().ShouldBe(0);
        body.GetProperty("earnedRevenueChangePercent").ValueKind.ShouldBe(JsonValueKind.Null);
        body.GetProperty("noShow").GetProperty("percent").ValueKind.ShouldBe(JsonValueKind.Null);
        body.GetProperty("online").GetProperty("percent").ValueKind.ShouldBe(JsonValueKind.Null);
        body.GetProperty("currency").GetString().ShouldBe("TRY");
        body.GetProperty("dailyCollections").EnumerateArray().Select(d => $"{d.GetProperty("date").GetString()}/{d.GetProperty("isClosed").GetBoolean()}")
            .ShouldBe(["2026-11-22/True", "2026-11-23/False", "2026-11-24/False", "2026-11-25/False"], "pazar kapalı");
    }

    // KK6: 10 seanslık 1.500 ₺ Fön paketi peşin satılır → tahsilatta 1.500, hak edilende 0; bir seans kullanılınca 150 hak edilir.
    [Fact]
    public async Task Criterion6_PrepaidPackage_FullInCollections_EarnedAsSessionsAreUsed()
    {
        var package = (await PostAsync("/api/v1/manage/packages", new
        {
            name = "Fön paketi",
            price = Money(150000),
            items = new[] { new { serviceId = _fon, sessionCount = 10 } },
            validityDays = 180,
            saleCommissionPercent = 10,
            isAvailableAllBranches = true,
            isActive = true,
        })).Body.GetProperty("id").GetString()!;
        var sold = await PostAsync($"/api/v1/manage/customers/{_customer}/packages",
            new { packageId = package, branchId = _merkez, soldByStaffId = _elif, paymentMethod = "Card" });
        sold.Status.ShouldBe(201, sold.Body.ToString());
        var packageItem = sold.Body.GetProperty("items")[0].GetProperty("customerPackageItemId").GetString()!;

        var afterSale = (await OverviewAsync(Today, Today, _merkez)).Body;
        Amount(afterSale, "collections").ShouldBe(150000);
        Amount(afterSale, "earnedRevenue").ShouldBe(0, "peşin satış hak edilen gelire girmez");

        var (fonVisit, fonEtag, fonBody) = await WalkInAsync(_fon);
        var fonItem = fonBody.GetProperty("items")[0].GetProperty("id").GetString()!;
        (await CompleteAsync(fonVisit, fonEtag, new
        {
            payments = Array.Empty<object>(),
            packageRedemptions = new[] { new { appointmentItemId = fonItem, customerPackageItemId = packageItem } },
        })).Status.ShouldBe(200);
        var (kesimVisit, kesimEtag, _) = await WalkInAsync(_kesim);
        (await CompleteAsync(kesimVisit, kesimEtag, new { payments = new[] { new { method = "Cash", amount = Money(45000) } } })).Status.ShouldBe(200);

        var report = (await OverviewAsync(Today, Today, _merkez)).Body;
        Amount(report, "earnedRevenue").ShouldBe(15000 + 45000, "seans payı 150 + saç kesimi 450");
        report.GetProperty("completedAppointmentCount").GetInt32().ShouldBe(2);
        Amount(report, "collections").ShouldBe(150000 + 45000, "paketten düşülen seans tahsilat değildir");
        report.GetProperty("dailyCollections")[0].GetProperty("amountMinor").GetInt64().ShouldBe(195000);
        report.GetProperty("branchId").GetString().ShouldBe(_merkez);
    }

    // W3: iade işaretlendiği günün tahsilatından düşülür; kapora tahsilata dahildir.
    [Fact]
    public async Task Refunds_AreDeductedOnTheDayMarked_DepositsIncluded()
    {
        var yesterday = await InsertPaymentAsync("Cash", 50000, Instant.FromUtc(2026, 11, 24, 10, 0));
        await InsertPaymentAsync("Transfer", 15000, Instant.FromUtc(2026, 11, 25, 7, 0), isDeposit: true);
        await InsertPaymentAsync("Card", 100000, Instant.FromUtc(2026, 11, 25, 8, 0));
        await _api.ScalarAsync<int>(
            $"UPDATE sales.payments SET refunded_amount_minor = 20000, status = 'PartiallyRefunded', refunded_utc = '2026-11-25T08:30:00Z' WHERE id = '{yesterday}' RETURNING 1");
        await InsertPaymentAsync("Cash", 7000, Instant.FromUtc(2026, 11, 25, 8, 0), status: "Voided");

        var report = (await OverviewAsync("2026-11-24", Today)).Body;

        Amount(report, "collections").ShouldBe(50000 + 15000 + 100000 - 20000);
        report.GetProperty("dailyCollections").EnumerateArray().Select(d => d.GetProperty("amountMinor").GetInt64()).ShouldBe([50000L, 95000L]);
    }

    [Fact]
    public async Task NoShow_RateAndLoss_OnlineShare()
    {
        var noShow = await BookAsync(Instant.FromUtc(2026, 11, 25, 10, 0), _kesim);
        var online = await BookAsync(Instant.FromUtc(2026, 11, 25, 15, 0), _fon);
        await _api.ScalarAsync<int>($"UPDATE scheduling.appointments SET channel = 'Online' WHERE id = '{online}' RETURNING 1");
        var (visit, etag, _) = await WalkInAsync(_kesim);
        (await CompleteAsync(visit, etag, new { payments = new[] { new { method = "Cash", amount = Money(45000) } } })).Status.ShouldBe(200);
        _factory.Clock.Reset(Instant.FromUtc(2026, 11, 25, 12, 0));
        await _api.RunJobAsync(ManageApi.MarkNoShowsJob);
        (await _api.ScalarAsync<string>($"SELECT status FROM scheduling.appointments WHERE id = '{noShow}'")).ShouldBe("NoShow");

        var report = (await OverviewAsync(Today, Today, _merkez)).Body;

        var noShows = report.GetProperty("noShow");
        noShows.GetProperty("count").GetInt32().ShouldBe(1);
        noShows.GetProperty("percent").GetDouble().ShouldBe(50, "sonuçlanan: 1 tamamlanan + 1 gelmedi");
        noShows.GetProperty("lostRevenue").GetProperty("amountMinor").GetInt64().ShouldBe(45000);
        report.GetProperty("online").GetProperty("appointmentCount").GetInt32().ShouldBe(1);
        report.GetProperty("online").GetProperty("percent").GetDouble().ShouldBe(33.3);
        Amount(report, "earnedRevenue").ShouldBe(45000, "gelmeyen randevu hak edilmez");

        var commissions = (await CommissionsAsync(Today, Today, _token)).Body;
        var elif = commissions.GetProperty("items").EnumerateArray().ShouldHaveSingleItem();
        elif.GetProperty("appointmentCount").GetInt32().ShouldBe(1, "gelmeyen randevu sayılmaz");
        elif.GetProperty("serviceCommissionMinor").GetInt64().ShouldBe(10800);
    }

    [Fact]
    public async Task EarnedRevenueChange_AgainstPreviousEqualRange()
    {
        var (visit, etag, _) = await WalkInAsync(_kesim);
        (await CompleteAsync(visit, etag, new { payments = new[] { new { method = "Cash", amount = Money(45000) } } })).Status.ShouldBe(200);
        await _api.ScalarAsync<int>($"UPDATE scheduling.appointments SET start_utc = start_utc - interval '7 days', end_utc = end_utc - interval '7 days' WHERE id = '{visit}' RETURNING 1");
        var (now, nowEtag, _) = await WalkInAsync(_kesim, _fon);
        (await CompleteAsync(now, nowEtag, new { payments = new[] { new { method = "Cash", amount = Money(64000) } } })).Status.ShouldBe(200);

        var report = (await OverviewAsync("2026-11-19", Today)).Body;

        Amount(report, "earnedRevenue").ShouldBe(64000);
        report.GetProperty("earnedRevenueChangePercent").GetDouble().ShouldBe(42.2, "45.000 → 64.000");
    }

    [Fact]
    public async Task CommissionReport_EntriesWithReversals_RangeAndBranchScope_StaffSeesOwnRow()
    {
        var zeynep = (await PostAsync("/api/v1/manage/staff",
            new { fullName = "Zeynep Aksoy", phone = ZeynepPhone, role = "Staff", calendarColor = "#2F5D8A", branchIds = new[] { _merkez } })).Body.GetProperty("id").GetString()!;
        var kadikoy = (await PostAsync("/api/v1/manage/branches", new { name = "Kadıköy", addressLine = "Moda Cad. 1", district = "Kadıköy", city = "İstanbul" }))
            .Body.GetProperty("id").GetString()!;
        var (visit, etag, _) = await WalkInAsync(_kesim);
        (await CompleteAsync(visit, etag, new { payments = new[] { new { method = "Cash", amount = Money(45000) } } })).Status.ShouldBe(200);
        var accrued = await InsertCommissionAsync(zeynep, "Service", "Accrued", 20000, 5000, Instant.FromUtc(2026, 11, 24, 10, 0));
        await InsertCommissionAsync(zeynep, "Sale", "Approved", 40000, 2000, Instant.FromUtc(2026, 11, 24, 11, 0));
        await InsertCommissionAsync(zeynep, "Service", "Reversed", -4000, -1000, Instant.FromUtc(2026, 11, 25, 8, 0), reverses: accrued);
        await InsertCommissionAsync(zeynep, "Service", "Paid", 99900, 9990, Instant.FromUtc(2026, 11, 1, 10, 0));
        await InsertCommissionAsync(_elif, "Service", "Accrued", 30000, 7200, Instant.FromUtc(2026, 11, 24, 10, 0), branchId: kadikoy);

        var owner = await CommissionsAsync("2026-11-24", Today, _token);

        owner.Status.ShouldBe(200, owner.Body.ToString());
        var rows = owner.Body.GetProperty("items").EnumerateArray().ToList();
        rows.Select(r => r.GetProperty("staffId").GetString()).ShouldBe([_elif, zeynep], "toplam prime göre azalan");
        var elif = rows[0];
        elif.GetProperty("staffName").GetString().ShouldBe("Kuaför Elif");
        elif.GetProperty("appointmentCount").GetInt32().ShouldBe(1);
        elif.GetProperty("serviceRevenueMinor").GetInt64().ShouldBe(45000);
        elif.GetProperty("serviceCommissionMinor").GetInt64().ShouldBe(10800 + 7200);
        elif.GetProperty("totalCommissionMinor").GetInt64().ShouldBe(18000);
        var z = rows[1];
        z.GetProperty("appointmentCount").GetInt32().ShouldBe(0);
        z.GetProperty("serviceRevenueMinor").GetInt64().ShouldBe(0);
        z.GetProperty("serviceCommissionPercent").ValueKind.ShouldBe(JsonValueKind.Null, "ciro yoksa oran yok");
        z.GetProperty("serviceCommissionMinor").GetInt64().ShouldBe(4000, "ters kayıt düşülür; aralık dışı kayıt yok");
        z.GetProperty("saleCommissionMinor").GetInt64().ShouldBe(2000);
        z.GetProperty("totalCommissionMinor").GetInt64().ShouldBe(6000);
        owner.Body.GetProperty("totalCommissionMinor").GetInt64().ShouldBe(24000);

        var merkezOnly = (await CommissionsAsync("2026-11-24", Today, _token, _merkez)).Body;
        merkezOnly.GetProperty("items")[0].GetProperty("serviceCommissionMinor").GetInt64().ShouldBe(10800, "başka şubenin primi yok");
        merkezOnly.GetProperty("items")[0].GetProperty("serviceCommissionPercent").GetDouble().ShouldBe(24);

        var staff = (await _api.SignInAsync(ZeynepPhone)).Body.GetProperty("accessToken").GetString()!;
        var own = await CommissionsAsync("2026-11-24", Today, staff);
        own.Status.ShouldBe(200, own.Body.ToString());
        own.Body.GetProperty("items").EnumerateArray().Select(r => r.GetProperty("staffId").GetString()).ShouldBe([zeynep], "personel yalnız kendi satırı");
        own.Body.GetProperty("totalCommissionMinor").GetInt64().ShouldBe(6000);
        (await OverviewAsync(Today, Today, null, staff)).Status.ShouldBe(403, "report.view personelde yok");
    }

    [Fact]
    public async Task BranchManager_OwnBranchesOnly_OtherBranch404_UnlinkedStaffEmpty()
    {
        var kadikoy = (await PostAsync("/api/v1/manage/branches", new { name = "Kadıköy", addressLine = "Moda Cad. 1", district = "Kadıköy", city = "İstanbul" }))
            .Body.GetProperty("id").GetString()!;
        await InsertPaymentAsync("Cash", 45000, Instant.FromUtc(2026, 11, 25, 8, 0), branchId: kadikoy);
        await InsertPaymentAsync("Cash", 10000, Instant.FromUtc(2026, 11, 25, 8, 0));
        var manager = await _api.AddMemberAndSignInAsync(_token, "+905382741906", "BranchManager", _merkez);

        Amount((await OverviewAsync(Today, Today, null, manager)).Body, "collections").ShouldBe(10000, "kapsamdaki şubeler");
        Amount((await OverviewAsync(Today, Today, null, _token)).Body, "collections").ShouldBe(55000, "Owner marka geneli");
        (await OverviewAsync(Today, Today, kadikoy, manager)).Status.ShouldBe(404);
        (await CommissionsAsync(Today, Today, manager, kadikoy)).Status.ShouldBe(404);
        (await OverviewAsync(Today, Today, kadikoy, _token)).Status.ShouldBe(200);

        var unlinked = await _api.AddMemberAndSignInAsync(_token, "+905382741907", "Staff", _merkez);
        var empty = await CommissionsAsync(Today, Today, unlinked);
        empty.Status.ShouldBe(200, empty.Body.ToString());
        empty.Body.GetProperty("items").GetArrayLength().ShouldBe(0, "personel kaydı yok");
    }

    [Fact]
    public async Task OtherBrand_Is404_InvalidRange_Is400()
    {
        var other = await _api.SignupAsync("+905427771122", "Berber Can");
        (await OverviewAsync(Today, Today, _merkez, other)).Status.ShouldBe(404);
        (await CommissionsAsync(Today, Today, other, _merkez)).Status.ShouldBe(404);

        var missing = await _api.GetAsync("/api/v1/manage/reports/overview", _token);
        missing.Status.ShouldBe(400, missing.Body.ToString());
        missing.Body.GetProperty("errors").GetProperty("from")[0].GetString().ShouldBe("required");
        missing.Body.GetProperty("errors").GetProperty("to")[0].GetString().ShouldBe("required");
        (await OverviewAsync("25.11.2026", Today)).Body.GetProperty("errors").GetProperty("from")[0].GetString().ShouldBe("invalid_format");
        (await OverviewAsync(Today, "2026-11-24")).Body.GetProperty("errors").GetProperty("to")[0].GetString().ShouldBe("out_of_range");
        var tooLong = await OverviewAsync("2026-11-01", "2026-12-02");
        tooLong.Status.ShouldBe(400, "32 gün");
        tooLong.Body.GetProperty("errors").GetProperty("to")[0].GetString().ShouldBe("out_of_range");
        (await OverviewAsync("2026-11-01", "2026-12-01")).Status.ShouldBe(200, "31 gün");
        (await CommissionsAsync(Today, "2026-11-24", _token)).Status.ShouldBe(400);
    }

    private Task<ManageApi.Response> OverviewAsync(string from, string to, string? branchId = null, string? token = null) =>
        _api.GetAsync($"/api/v1/manage/reports/overview?from={from}&to={to}" + (branchId is null ? string.Empty : $"&branchId={branchId}"), token ?? _token);

    private Task<ManageApi.Response> CommissionsAsync(string from, string to, string token, string? branchId = null) =>
        _api.GetAsync($"/api/v1/manage/reports/commissions?from={from}&to={to}" + (branchId is null ? string.Empty : $"&branchId={branchId}"), token);

    /// <summary>Panelden telefonla randevu (Onaylandı).</summary>
    private async Task<string> BookAsync(Instant start, string service)
    {
        var created = await PostAsync("/api/v1/manage/appointments", new
        {
            branchId = _merkez,
            brandCustomerId = _customer,
            channel = "Phone",
            startUtc = Iso(start),
            items = new[] { new { serviceId = service, staffId = _elif } },
        });
        created.Status.ShouldBe(201, created.Body.ToString());
        return created.Body.GetProperty("appointment").GetProperty("id").GetString()!;
    }

    /// <summary>Kapıdan gelen (şimdi, doğrudan Geldi); saat 30 dk ilerler.</summary>
    private async Task<(string Id, string ETag, JsonElement Body)> WalkInAsync(params string[] services)
    {
        var created = await PostAsync("/api/v1/manage/appointments", new
        {
            branchId = _merkez,
            brandCustomerId = _customer,
            channel = "WalkIn",
            startUtc = Iso(_factory.Clock.GetCurrentInstant()),
            items = services.Select(s => new { serviceId = s, staffId = _elif }).ToArray(),
        });
        created.Status.ShouldBe(201, created.Body.ToString());
        var body = created.Body.GetProperty("appointment");
        _factory.Clock.Advance(Duration.FromMinutes(70));
        return (body.GetProperty("id").GetString()!, created.ETag!, body);
    }

    private Task<ManageApi.Response> CompleteAsync(string id, string etag, object body)
    {
        var headers = Key();
        headers["If-Match"] = etag;
        return _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/complete", body, _token, headers);
    }

    private async Task<string> InsertPaymentAsync(string method, long amountMinor, Instant paidUtc, bool isDeposit = false, string? branchId = null, string status = "Recorded")
    {
        var id = $"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _requests) + 500000:D12}";
        await _api.ScalarAsync<int>(
            $"""
            INSERT INTO sales.payments (id, brand_id, branch_id, brand_customer_id, method, amount_minor, currency_code, is_deposit, status, refunded_amount_minor, paid_utc)
              VALUES ('{id}', '{_brand}', '{branchId ?? _merkez}', NULL, '{method}', {amountMinor}, 'TRY', {isDeposit}, '{status}', 0, '{Iso(paidUtc)}');
            INSERT INTO sales.payment_allocations (id, brand_id, payment_id, target_type, target_id, amount_minor)
              VALUES (gen_random_uuid(), '{_brand}', '{id}', 'Appointment', gen_random_uuid(), {amountMinor});
            SELECT 1;
            """);
        return id;
    }

    private async Task<string> InsertCommissionAsync(
        string staffId, string type, string status, long basisMinor, long amountMinor, Instant createdUtc, string? reverses = null, string? branchId = null)
    {
        var id = $"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _requests) + 700000:D12}";
        var target = string.Equals(type, "Service", StringComparison.Ordinal) ? "gen_random_uuid(), NULL" : "NULL, gen_random_uuid()";
        await _api.ScalarAsync<int>(
            $"""
            INSERT INTO sales.commission_entries (id, brand_id, branch_id, staff_id, type, appointment_item_id, customer_package_id, basis_minor, percent,
                amount_minor, currency_code, period_year_month, status, reverses_entry_id, created_utc)
              VALUES ('{id}', '{_brand}', '{branchId ?? _merkez}', '{staffId}', '{type}', {target}, {basisMinor}, 25, {amountMinor}, 'TRY', 202611, '{status}',
                {(reverses is null ? "NULL" : $"'{reverses}'")}, '{Iso(createdUtc)}');
            SELECT 1;
            """);
        return id;
    }

    private async Task<string> ServiceAsync(string category, string name, long priceMinor) =>
        (await PostAsync("/api/v1/manage/services", new
        {
            categoryId = category,
            name,
            durationMinutes = 30,
            price = Money(priceMinor),
            priceDisplay = "Fixed",
            deposit = new { mode = "Never" },
            isOnlineBookable = true,
            staffIds = new[] { _elif },
        })).Body.GetProperty("id").GetString()!;

    private static long Amount(JsonElement body, string name) => body.GetProperty(name).GetProperty("amountMinor").GetInt64();

    private static object Money(long amountMinor) => new { amountMinor, currency = "TRY" };

    private Task<ManageApi.Response> PostAsync(string path, object body) => _api.SendAsync(HttpMethod.Post, path, body, _token, Key());

    private static string Iso(Instant at) => NodaTime.Text.InstantPattern.General.Format(at);

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _requests):D12}" };
}
