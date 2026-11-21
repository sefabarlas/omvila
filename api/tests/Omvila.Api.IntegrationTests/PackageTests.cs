using System.Text.Json;
using NodaTime;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-05 kabul kriteri 6 (10 seanslık Fön paketi, satış primi %10; online randevuda "Fön paketinizden düşülsün" → seans
// ayrılır; iptal → geri döner ve tarihli görünür; gelmedi → dönmez) ve 8 (paketten düşülen kalemde kapora yok); durum-makinesi.md
// A1, A4–A8, A11, § 5 (Active → Expired / Refunded), § 7 (Reversed); alan-modeli.md § CustomerPackage, PackageSessionUsage,
// CommissionEntry; docs/acik-sorular.md W1, W2, W9, T-95; roller-yetkiler.md package.manage (Owner), package.sell (Owner, BM Ş);
// sözleşme manage-packages, manageSellPackage, manageListCustomerPackages, publicListMyPackages, usePackageItems, packageRedemptions.
// Saat: 2026-11-25 09:00Z (Salı 12:00 İstanbul).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class PackageTests(PostgresFixture db) : IAsyncLifetime
{
    private const string CustomerPhone = "+905321234567";
    private const string FridayAt1430 = "2026-11-27T11:30:00Z";
    private const string FridayAt1600 = "2026-11-27T13:00:00Z";
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private string _merkez = null!;
    private string _elif = null!;
    private string _fon = null!;
    private string _kesim = null!;
    private string _customer = null!;
    private int _requests;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db, new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["Auth:AccessTokenMinutes"] = "600000",
            ["Auth:PublicBookingTokenMinutes"] = "600000",
        });
        _api = new ManageApi(_factory, db);
        _token = await _api.SignupAsync("+905324187705", "Kuaför Elif");
        var session = (await _api.GetAsync("/api/v1/manage/session", _token)).Body;
        _merkez = session.GetProperty("branches")[0].GetProperty("id").GetString()!;
        _elif = session.GetProperty("membership").GetProperty("staffId").GetString()!;

        var week = new
        {
            days = Enumerable.Range(1, 7).Select(d => new { dayOfWeek = d, intervals = new[] { new { start = "09:00", end = "19:00" } } }).ToArray(),
        };
        var hours = await _api.GetAsync($"/api/v1/manage/branches/{_merkez}/working-hours", _token);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/branches/{_merkez}/working-hours", week, _token, hours.ETag)).Status.ShouldBe(200);
        var elif = await _api.GetAsync($"/api/v1/manage/staff/{_elif}", _token);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{_elif}/branches/{_merkez}/working-hours", week, _token, elif.ETag)).Status.ShouldBe(200);
        await _api.ScalarAsync<int>($"UPDATE tenancy.staff SET service_commission_percent = 24, sale_commission_percent = 5 WHERE id = '{_elif}' RETURNING 1");

        // KK8: Fön her zaman kapora ister; paketten düşülünce istenmez.
        (await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/brand/bank-account", new
        {
            accountHolderName = "Elif Kaya",
            bankName = "Ziraat Bankası",
            iban = "TR29 0006 4000 0011 2345 6789 01",
            transferCodePrefix = "OMV",
            transferWindowMinutes = 120,
        }, _token, "W/\"0\"")).Status.ShouldBe(200);
        var category = (await PostAsync("/api/v1/manage/service-categories", new { name = "Saç" })).Body.GetProperty("id").GetString()!;
        _fon = await ServiceAsync(category, "Fön", 30, 19000, new { mode = "Always", amount = new { amountMinor = 5000, currency = "TRY" } });
        _kesim = await ServiceAsync(category, "Saç kesimi", 45, 45000, new { mode = "Never" });
        _customer = (await PostAsync("/api/v1/manage/customers", new { fullName = "Selin Öztürk", phone = "0532 123 45 67" })).Body.GetProperty("id").GetString()!;
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task Criterion6_SellTenSessionFon_PaymentAndTenPercentSaleCommission()
    {
        var package = await CreatePackageAsync("Fön paketi", 150000, 10, saleCommissionPercent: 10);
        var sold = await SellAsync(package, "Card");
        sold.Status.ShouldBe(201, sold.Body.ToString());
        sold.ETag.ShouldNotBeNull();
        var body = sold.Body;
        body.GetProperty("packageName").GetString().ShouldBe("Fön paketi");
        body.GetProperty("status").GetString().ShouldBe("Active");
        body.GetProperty("price").GetProperty("amountMinor").GetInt64().ShouldBe(150000);
        body.GetProperty("expiresOn").GetString().ShouldBe("2027-02-23");
        body.GetProperty("soldByStaffId").GetString().ShouldBe(_elif);
        body.GetProperty("customer").GetProperty("fullName").GetString().ShouldBe("Selin Öztürk");
        body.GetProperty("remainingSessions").GetInt32().ShouldBe(10);
        var packageItem = body.GetProperty("items")[0].GetProperty("customerPackageItemId").GetString()!;
        body.GetProperty("items")[0].GetProperty("serviceName").GetString().ShouldBe("Fön");

        (await _api.ScalarAsync<string>(
            "SELECT p.method || '/' || p.amount_minor || '/' || a.target_type FROM sales.payments p JOIN sales.payment_allocations a ON a.payment_id = p.id"))
            .ShouldBe("Card/150000/CustomerPackage");
        (await _api.ScalarAsync<string>("SELECT type || '/' || status || '/' || basis_minor || '/' || percent || '/' || amount_minor || '/' || staff_id FROM sales.commission_entries"))
            .ShouldBe($"Sale/Accrued/150000/10.00/15000/{_elif}", "pakete özel %10, personelin %5'ini ezer");
    }

    [Fact]
    public async Task Criterion6_OnlineReserve_CancelReturnsDated_NoShowBurns()
    {
        var package = await CreatePackageAsync("Fön paketi", 150000, 10, saleCommissionPercent: 10);
        var packageItem = (await SellAsync(package, "Card")).Body.GetProperty("items")[0].GetProperty("customerPackageItemId").GetString()!;
        var token = await VerifyAsync(CustomerPhone);
        var usable = (await MyPackagesAsync(token)).GetProperty("items").EnumerateArray().ToList();
        usable.ShouldHaveSingleItem().GetProperty("remainingSessions").GetInt32().ShouldBe(10);
        usable[0].GetProperty("customerPackageItemId").GetString().ShouldBe(packageItem);

        // Online randevuda "Fön paketinizden düşülsün": seans ayrılır, kapora istenmez (KK8), salonda ödenecek 0.
        var booked = await BookAsync(token, FridayAt1430, packageItem);
        booked.Status.ShouldBe(201, booked.Body.ToString());
        booked.Body.GetProperty("displayStatus").GetString().ShouldBe("Confirmed");
        booked.Body.GetProperty("items")[0].GetProperty("paidFromPackage").GetBoolean().ShouldBeTrue();
        booked.Body.GetProperty("amountDue").GetProperty("amountMinor").GetInt64().ShouldBe(0);
        IsNullOrMissing(booked.Body, "deposit").ShouldBeTrue("paketten düşülen kalemde kapora yok (KK8)");
        var appointment = booked.Body.GetProperty("id").GetString()!;
        (await CardAsync()).GetProperty("items")[0].GetProperty("remainingSessions").GetInt32().ShouldBe(9);

        // Salon iptali → seans geri döner, kullanım tarihli görünür.
        _factory.Clock.Advance(Duration.FromHours(1));
        var cancelledAt = Iso(_factory.Clock.GetCurrentInstant());
        var detail = await _api.GetAsync($"/api/v1/manage/appointments/{appointment}", _token);
        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{appointment}/cancel", new { reason = "CustomerRequested" }, _token, detail.ETag))
            .Status.ShouldBe(200);
        var card = (await CardAsync()).GetProperty("items")[0];
        card.GetProperty("remainingSessions").GetInt32().ShouldBe(10);
        var usage = card.GetProperty("usages").EnumerateArray().ShouldHaveSingleItem();
        usage.GetProperty("appointmentId").GetString().ShouldBe(appointment);
        usage.GetProperty("serviceName").GetString().ShouldBe("Fön");
        usage.GetProperty("appointmentStartUtc").GetString().ShouldBe(FridayAt1430);
        usage.GetProperty("revertedUtc").GetString().ShouldBe(cancelledAt);

        // Gelmedi → seans geri dönmez.
        var second = await BookAsync(token, FridayAt1600, packageItem);
        second.Status.ShouldBe(201, second.Body.ToString());
        _factory.Clock.Advance(Duration.FromDays(3));
        await _api.RunJobAsync(ManageApi.MarkNoShowsJob);
        (await _api.GetAsync($"/api/v1/manage/appointments/{second.Body.GetProperty("id").GetString()}", _token)).Body.GetProperty("status").GetString().ShouldBe("NoShow");
        card = (await CardAsync()).GetProperty("items")[0];
        card.GetProperty("remainingSessions").GetInt32().ShouldBe(9);
        card.GetProperty("usages").EnumerateArray().Count(u => !u.TryGetProperty("revertedUtc", out var r) || r.ValueKind == JsonValueKind.Null).ShouldBe(1);
    }

    // durum-makinesi.md A4 (red), A5 (süre dolumu), A6 (müşteri iptali): seans geri döner.
    [Fact]
    public async Task DeclineExpiryAndCustomerCancel_ReturnSessions()
    {
        var package = await CreatePackageAsync("Fön paketi", 150000, 10, saleCommissionPercent: 10);
        var packageItem = (await SellAsync(package, "Cash")).Body.GetProperty("items")[0].GetProperty("customerPackageItemId").GetString()!;
        var token = await VerifyAsync(CustomerPhone);

        var cancelled = await BookAsync(token, FridayAt1430, packageItem);
        cancelled.Status.ShouldBe(201, cancelled.Body.ToString());
        var code = cancelled.Body.GetProperty("accessCode").GetString();
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/appointments/{code}/cancel", new { reason = "PlanChanged" }, headers: Headers(cancelled.ETag)))
            .Status.ShouldBe(200);
        (await CardAsync()).GetProperty("items")[0].GetProperty("remainingSessions").GetInt32().ShouldBe(10, "müşteri iptali (A6)");

        await _api.ScalarAsync<int>("UPDATE tenancy.brand_settings SET requires_approval = true RETURNING 1");
        var declined = await BookAsync(token, FridayAt1430, packageItem);
        declined.Body.GetProperty("status").GetString().ShouldBe("PendingApproval");
        (await CardAsync()).GetProperty("items")[0].GetProperty("remainingSessions").GetInt32().ShouldBe(9, "onay bekleyen talepte de seans ayrılır");
        var id = declined.Body.GetProperty("id").GetString();
        var detail = await _api.GetAsync($"/api/v1/manage/appointments/{id}", _token);
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/decline", null, _token, Headers(detail.ETag))).Status.ShouldBe(200);
        (await CardAsync()).GetProperty("items")[0].GetProperty("remainingSessions").GetInt32().ShouldBe(10, "red (A4)");

        (await BookAsync(token, FridayAt1600, packageItem)).Status.ShouldBe(201);
        _factory.Clock.Advance(Duration.FromHours(3));
        await _api.RunJobAsync("Omvila.Modules.Scheduling.Jobs.ExpireAppointmentHolds, Omvila.Modules.Scheduling");
        var card = (await CardAsync()).GetProperty("items")[0];
        card.GetProperty("remainingSessions").GetInt32().ShouldBe(10, "süre dolumu (A5)");
        card.GetProperty("usages").EnumerateArray().ShouldAllBe(u => u.GetProperty("revertedUtc").ValueKind == JsonValueKind.String);
        card.GetProperty("usages").GetArrayLength().ShouldBe(3);
    }

    [Fact]
    public async Task CompleteReservedOnlineAppointment_ZeroRedemptionPayment_SessionShareCommission()
    {
        var package = await CreatePackageAsync("Fön paketi", 150000, 10, saleCommissionPercent: 10);
        var packageItem = (await SellAsync(package, "Cash")).Body.GetProperty("items")[0].GetProperty("customerPackageItemId").GetString()!;
        var booked = await BookAsync(await VerifyAsync(CustomerPhone), FridayAt1430, packageItem);
        var id = booked.Body.GetProperty("id").GetString()!;
        _factory.Clock.Advance(Duration.FromHours(50));
        var detail = await _api.GetAsync($"/api/v1/manage/appointments/{id}", _token);
        var checkedIn = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/check-in", new { }, _token, detail.ETag);
        checkedIn.Status.ShouldBe(200, checkedIn.Body.ToString());

        var completed = await CompleteAsync(id, checkedIn.ETag, new { payments = Array.Empty<object>() });

        completed.Status.ShouldBe(200, completed.Body.ToString());
        completed.Body.GetProperty("payments").EnumerateArray()
            .Select(p => $"{p.GetProperty("method").GetString()}/{p.GetProperty("amount").GetProperty("amountMinor").GetInt64()}")
            .ShouldBe(["PackageRedemption/0"]);
        // W2: taban paket fiyatının seans payı 15.000; Elif %24 → 3.600.
        (await _api.ScalarAsync<string>("SELECT basis_minor || '/' || amount_minor FROM sales.commission_entries WHERE type = 'Service'")).ShouldBe("15000/3600");
        (await _api.ScalarAsync<long>($"SELECT total_spent_minor FROM customers.brand_customers WHERE id = '{_customer}'")).ShouldBe(0);
        (await CardAsync()).GetProperty("items")[0].GetProperty("remainingSessions").GetInt32().ShouldBe(9);
    }

    [Fact]
    public async Task CompleteWithPackageRedemption_ReservesSession_ReducesAmountDue()
    {
        var package = await CreatePackageAsync("Fön paketi", 150000, 10, saleCommissionPercent: null);
        var packageItem = (await SellAsync(package, "Transfer")).Body.GetProperty("items")[0].GetProperty("customerPackageItemId").GetString()!;
        (await _api.ScalarAsync<string>("SELECT percent || '/' || amount_minor FROM sales.commission_entries")).ShouldBe("5.00/7500", "personelin satış primi");
        var (id, etag, body) = await WalkInAsync(_kesim, _fon);
        var fonItem = body.GetProperty("items").EnumerateArray().Single(i => string.Equals(i.GetProperty("serviceId").GetString(), _fon, StringComparison.Ordinal)).GetProperty("id").GetString()!;
        var redemption = new[] { new { appointmentItemId = fonItem, customerPackageItemId = packageItem } };

        var mismatch = await CompleteAsync(id, etag, new { payments = new[] { Line("Cash", 64000) }, packageRedemptions = redemption });
        mismatch.Status.ShouldBe(409);
        mismatch.Body.GetProperty("meta").GetProperty("expectedAmountMinor").GetInt64().ShouldBe(45000);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM sales.package_session_usages")).ShouldBe(0, "toplam tutmazsa seans da geri alınır");

        var completed = await CompleteAsync(id, etag, new { payments = new[] { Line("Cash", 45000) }, packageRedemptions = redemption });

        completed.Status.ShouldBe(200, completed.Body.ToString());
        completed.Body.GetProperty("items").EnumerateArray().Single(i => string.Equals(i.GetProperty("id").GetString(), fonItem, StringComparison.Ordinal))
            .GetProperty("paidFromPackage").GetBoolean().ShouldBeTrue();
        (await _api.ScalarAsync<string>(
            $"SELECT string_agg(p.method || '/' || p.amount_minor, ',' ORDER BY p.amount_minor DESC) FROM sales.payments p JOIN sales.payment_allocations a ON a.payment_id = p.id WHERE a.target_id = '{id}'"))
            .ShouldBe("Cash/45000,PackageRedemption/0");
        (await _api.ScalarAsync<string>("SELECT string_agg(basis_minor || '/' || amount_minor, ',' ORDER BY basis_minor DESC) FROM sales.commission_entries WHERE type = 'Service'"))
            .ShouldBe("45000/10800,15000/3600");
        (await CardAsync()).GetProperty("items")[0].GetProperty("remainingSessions").GetInt32().ShouldBe(9);

        var (other, otherEtag, otherBody) = await WalkInAsync(_kesim);
        var kesimItem = otherBody.GetProperty("items")[0].GetProperty("id").GetString()!;
        var wrongService = await CompleteAsync(other, otherEtag, new
        {
            payments = Array.Empty<object>(),
            packageRedemptions = new[] { new { appointmentItemId = kesimItem, customerPackageItemId = packageItem } },
        });
        wrongService.Status.ShouldBe(409, wrongService.Body.ToString());
        wrongService.Body.GetProperty("code").GetString().ShouldBe("package_session_unavailable");
        wrongService.Body.GetProperty("meta").GetProperty("customerPackageItemId").GetString().ShouldBe(packageItem);
    }

    [Fact]
    public async Task InsufficientSessions_IsPackageSessionUnavailable_NothingWritten()
    {
        var package = await CreatePackageAsync("Fön · tek seans", 19000, 1, saleCommissionPercent: 0);
        var packageItem = (await SellAsync(package, "Cash")).Body.GetProperty("items")[0].GetProperty("customerPackageItemId").GetString()!;
        (await _api.ScalarAsync<long>("SELECT count(*) FROM sales.commission_entries")).ShouldBe(0, "oran 0 ise prim yazılmaz");
        var token = await VerifyAsync(CustomerPhone);
        (await BookAsync(token, FridayAt1430, packageItem)).Status.ShouldBe(201);

        var rejected = await BookAsync(token, FridayAt1600, packageItem);

        rejected.Status.ShouldBe(409, rejected.Body.ToString());
        rejected.Body.GetProperty("code").GetString().ShouldBe("package_session_unavailable");
        rejected.Body.GetProperty("meta").GetProperty("customerPackageItemId").GetString().ShouldBe(packageItem);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM scheduling.appointments")).ShouldBe(1);
        (await MyPackagesAsync(token)).GetProperty("items").GetArrayLength().ShouldBe(0, "kalan seansı olmayan kalem listelenmez");

        var unknown = await BookAsync(token, FridayAt1600, "0193a8f0-0000-7000-8000-00000000aa01");
        unknown.Status.ShouldBe(409);
        unknown.Body.GetProperty("code").GetString().ShouldBe("package_session_unavailable");
    }

    [Fact]
    public async Task ExpiryJob_ActiveToExpired_AfterLastLocalDay_ThenUnusable()
    {
        var package = await CreatePackageAsync("Fön paketi", 150000, 10, saleCommissionPercent: 10, validityDays: 2);
        var sold = await SellAsync(package, "Cash");
        sold.Body.GetProperty("expiresOn").GetString().ShouldBe("2026-11-27");
        var packageItem = sold.Body.GetProperty("items")[0].GetProperty("customerPackageItemId").GetString()!;
        var token = await VerifyAsync(CustomerPhone);

        // 27 Kas 23:30 İstanbul: son gün, henüz geçerli.
        _factory.Clock.Advance(Duration.FromHours(59.5));
        await _api.RunJobAsync(ExpireJob);
        (await CardAsync()).GetProperty("items")[0].GetProperty("status").GetString().ShouldBe("Active");

        // 28 Kas 00:30 İstanbul (27 Kas 21:30Z): yerel gün geçti.
        _factory.Clock.Advance(Duration.FromHours(1));
        await _api.RunJobAsync(ExpireJob);
        (await CardAsync()).GetProperty("items")[0].GetProperty("status").GetString().ShouldBe("Expired");
        (await MyPackagesAsync(token)).GetProperty("items").GetArrayLength().ShouldBe(0);
        var list = (await _api.GetAsync("/api/v1/manage/packages", _token)).Body;
        list.GetProperty("overview").GetProperty("activeCustomerPackages").GetInt32().ShouldBe(0);
        list.GetProperty("overview").GetProperty("usageRatePercent").GetDecimal().ShouldBe(0m);
        (await _api.GetAsync("/api/v1/manage/customer-packages?filter=Expired", _token)).Body.GetProperty("items").GetArrayLength().ShouldBe(1);

        var rejected = await BookAsync(token, "2026-12-01T11:30:00Z", packageItem);
        rejected.Status.ShouldBe(409, rejected.Body.ToString());
        rejected.Body.GetProperty("code").GetString().ShouldBe("package_session_unavailable");
    }

    // Kasa (T-96): paket satışı satırı ad, seans ve satan personelle; "Paketten" 0 TL satırı yalnız sayıya girer; paket iadesi
    // işaretlendiği günün nakit iadesi olarak düşülür (W3).
    [Fact]
    public async Task CashDay_PackageSaleRow_RedemptionZeroRow_RefundOnRefundDay()
    {
        var package = await CreatePackageAsync("Fön paketi", 150000, 10, saleCommissionPercent: 10);
        var sold = await SellAsync(package, "Cash");
        var packageItem = sold.Body.GetProperty("items")[0].GetProperty("customerPackageItemId").GetString()!;
        var saleDay = (await _api.GetAsync($"/api/v1/manage/cash?branchId={_merkez}&date=2026-11-25", _token)).Body;
        var saleRow = saleDay.GetProperty("payments").EnumerateArray().ShouldHaveSingleItem();
        saleRow.GetProperty("purpose").GetString().ShouldBe("CustomerPackage");
        saleRow.GetProperty("customerPackageId").GetString().ShouldBe(sold.Body.GetProperty("id").GetString());
        saleRow.GetProperty("packageName").GetString().ShouldBe("Fön paketi");
        saleRow.GetProperty("packageSessionCount").GetInt32().ShouldBe(10);
        saleRow.GetProperty("staffName").GetString().ShouldNotBeNullOrEmpty("satan personel");
        saleDay.GetProperty("expectedCash").GetProperty("amountMinor").GetInt64().ShouldBe(150000);

        var booked = await BookAsync(await VerifyAsync(CustomerPhone), FridayAt1430, packageItem);
        var id = booked.Body.GetProperty("id").GetString()!;
        _factory.Clock.Advance(Duration.FromHours(50));
        var detail = await _api.GetAsync($"/api/v1/manage/appointments/{id}", _token);
        var checkedIn = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/check-in", new { }, _token, detail.ETag);
        (await CompleteAsync(id, checkedIn.ETag, new { payments = Array.Empty<object>() })).Status.ShouldBe(200);
        var refunded = await _api.SendAsync(
            HttpMethod.Post, $"/api/v1/manage/customer-packages/{sold.Body.GetProperty("id").GetString()}/refund", new { amount = Amount(90000) }, _token,
            Headers(sold.ETag, Key()));
        refunded.Status.ShouldBe(200, refunded.Body.ToString());

        var serviceDay = (await _api.GetAsync($"/api/v1/manage/cash?branchId={_merkez}&date=2026-11-27", _token)).Body;
        var redemption = serviceDay.GetProperty("payments").EnumerateArray().ShouldHaveSingleItem();
        redemption.GetProperty("method").GetString().ShouldBe("PackageRedemption");
        redemption.GetProperty("amount").GetProperty("amountMinor").GetInt64().ShouldBe(0);
        redemption.GetProperty("appointmentId").GetString().ShouldBe(id);
        redemption.GetProperty("isBalanceAfterDeposit").GetBoolean().ShouldBeFalse();
        serviceDay.GetProperty("total").GetProperty("count").GetInt32().ShouldBe(1);
        serviceDay.GetProperty("total").GetProperty("amount").GetProperty("amountMinor").GetInt64().ShouldBe(0);
        serviceDay.GetProperty("cashRefunded").GetProperty("amountMinor").GetInt64().ShouldBe(90000, "paket iadesi iade gününde düşülür");
        (await _api.GetAsync($"/api/v1/manage/cash?branchId={_merkez}&date=2026-11-25", _token)).Body
            .GetProperty("cashRefunded").GetProperty("amountMinor").GetInt64().ShouldBe(0);
    }

    [Fact]
    public async Task Refund_W9_ManualAmount_PaymentPartiallyRefunded_SaleCommissionReversed()
    {
        var package = await CreatePackageAsync("Fön paketi", 150000, 10, saleCommissionPercent: 10);
        var sold = await SellAsync(package, "Cash");
        var id = sold.Body.GetProperty("id").GetString()!;
        var path = $"/api/v1/manage/customer-packages/{id}/refund";

        var tooMuch = await _api.SendAsync(HttpMethod.Post, path, new { amount = Amount(150001) }, _token, Headers(sold.ETag, Key()));
        tooMuch.Status.ShouldBe(400, tooMuch.Body.ToString());
        tooMuch.Body.GetProperty("errors").GetProperty("amount")[0].GetString().ShouldBe("out_of_range");
        (await _api.SendAsync(HttpMethod.Post, path, new { amount = Amount(90000) }, _token, Headers("W/\"1\"", Key()))).Status.ShouldBe(412);

        var refunded = await _api.SendAsync(HttpMethod.Post, path, new { amount = Amount(90000), note = "3 seans kullanıldı" }, _token, Headers(sold.ETag, Key()));

        refunded.Status.ShouldBe(200, refunded.Body.ToString());
        refunded.Body.GetProperty("status").GetString().ShouldBe("Refunded");
        refunded.Body.GetProperty("refundedAmount").GetProperty("amountMinor").GetInt64().ShouldBe(90000);
        refunded.Body.GetProperty("refundedUtc").GetString().ShouldBe(Iso(_factory.Clock.GetCurrentInstant()));
        (await _api.ScalarAsync<string>("SELECT status || '/' || refunded_amount_minor FROM sales.payments")).ShouldBe("PartiallyRefunded/90000");
        (await _api.ScalarAsync<string>(
            "SELECT string_agg(status || '/' || basis_minor || '/' || amount_minor || '/' || (reverses_entry_id IS NOT NULL), ',' ORDER BY amount_minor DESC) FROM sales.commission_entries"))
            .ShouldBe("Accrued/150000/15000/false,Reversed/-90000/-9000/true");

        var again = await _api.SendAsync(HttpMethod.Post, path, new { amount = Amount(1000) }, _token, Headers(refunded.ETag, Key()));
        again.Status.ShouldBe(409);
        again.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");
        (await MyPackagesAsync(await VerifyAsync(CustomerPhone))).GetProperty("items").GetArrayLength().ShouldBe(0);
    }

    [Fact]
    public async Task Definitions_CreateGetUpdateList_Overview_AndValidation()
    {
        var invalid = await PostAsync("/api/v1/manage/packages", new
        {
            name = "Fön",
            price = Amount(150000),
            items = new[] { new { serviceId = _fon, sessionCount = 5 }, new { serviceId = _fon, sessionCount = 2 } },
            validityDays = 90,
            isAvailableAllBranches = true,
            isActive = true,
        });
        invalid.Status.ShouldBe(400);
        invalid.Body.GetProperty("errors").GetProperty("items")[0].GetString().ShouldBe("invalid_format");

        var id = await CreatePackageAsync("Fön paketi", 150000, 10, saleCommissionPercent: 10);
        var got = await _api.GetAsync($"/api/v1/manage/packages/{id}", _token);
        got.Status.ShouldBe(200);
        got.ETag.ShouldNotBeNull();
        got.Body.GetProperty("totalSessions").GetInt32().ShouldBe(10);
        got.Body.GetProperty("items")[0].GetProperty("serviceName").GetString().ShouldBe("Fön");
        await SellAsync(id, "Card");

        var update = new
        {
            name = "Fön ve kesim",
            price = Amount(200000),
            items = new[] { new { serviceId = _fon, sessionCount = 5 }, new { serviceId = _kesim, sessionCount = 2 } },
            validityDays = 60,
            saleCommissionPercent = (decimal?)null,
            isAvailableAllBranches = false,
            isActive = false,
        };
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/packages/{id}", update, _token, "W/\"1\"")).Status.ShouldBe(412);
        var updated = await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/packages/{id}", update, _token, got.ETag);
        updated.Status.ShouldBe(200, updated.Body.ToString());
        updated.Body.GetProperty("totalSessions").GetInt32().ShouldBe(7);
        updated.Body.GetProperty("isActive").GetBoolean().ShouldBeFalse();
        updated.Body.GetProperty("soldCount").GetInt32().ShouldBe(1);
        IsNullOrMissing(updated.Body, "saleCommissionPercent").ShouldBeTrue();

        var card = (await CardAsync()).GetProperty("items")[0];
        card.GetProperty("packageName").GetString().ShouldBe("Fön paketi", "satılmış paket değişmez");
        card.GetProperty("totalSessions").GetInt32().ShouldBe(10);

        var notForSale = await SellAsync(id, "Cash");
        notForSale.Status.ShouldBe(409);
        notForSale.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");

        var list = (await _api.GetAsync($"/api/v1/manage/packages?branchId={_merkez}", _token)).Body;
        list.GetProperty("items").EnumerateArray().Select(i => (i.GetProperty("name").GetString(), i.GetProperty("activeCount").GetInt32()))
            .ShouldBe([("Fön ve kesim", 1)]);
        var overview = list.GetProperty("overview");
        overview.GetProperty("activeCustomerPackages").GetInt32().ShouldBe(1);
        overview.GetProperty("remainingSessions").GetInt32().ShouldBe(10);
        overview.GetProperty("soldThisMonth").GetInt32().ShouldBe(1);
        overview.GetProperty("soldThisMonthAmount").GetProperty("amountMinor").GetInt64().ShouldBe(150000);
        overview.GetProperty("expiringSoon").GetInt32().ShouldBe(0);
        IsNullOrMissing(overview, "usageRatePercent").ShouldBeTrue("süresi biten paket yok");
        (await _api.GetAsync("/api/v1/manage/customer-packages", _token)).Body.GetProperty("items").GetArrayLength().ShouldBe(1);
        (await _api.GetAsync("/api/v1/manage/customer-packages?filter=ExpiringSoon", _token)).Body.GetProperty("items").GetArrayLength().ShouldBe(0);
        (await _api.GetAsync("/api/v1/manage/customer-packages?filter=Nope", _token)).Status.ShouldBe(400);
    }

    [Fact]
    public async Task Permissions_StaffAndBranchManager()
    {
        var package = await CreatePackageAsync("Fön paketi", 150000, 10, saleCommissionPercent: 10);
        var staff = await _api.AddMemberAndSignInAsync(_token, "+905382741905", "Staff", _merkez);
        var manager = await _api.AddMemberAndSignInAsync(_token, "+905382741906", "BranchManager", _merkez);
        var write = new
        {
            name = "Kesim paketi",
            price = Amount(100000),
            items = new[] { new { serviceId = _kesim, sessionCount = 3 } },
            validityDays = 90,
            isAvailableAllBranches = true,
            isActive = true,
        };

        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/packages", write, staff, Key())).Status.ShouldBe(403);
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/packages", write, manager, Key())).Status.ShouldBe(403, "package.manage yalnız Owner");
        (await _api.GetAsync("/api/v1/manage/packages", staff)).Status.ShouldBe(403);
        (await _api.GetAsync("/api/v1/manage/packages", manager)).Status.ShouldBe(200);
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/customers/{_customer}/packages", SaleBody(package, "Cash"), staff, Key())).Status.ShouldBe(403);
        (await _api.GetAsync($"/api/v1/manage/customers/{_customer}/packages", staff)).Status.ShouldBe(200, "customer.view personelde de var");

        var sold = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/customers/{_customer}/packages", SaleBody(package, "Cash"), manager, Key());
        sold.Status.ShouldBe(201, sold.Body.ToString());
    }

    [Fact]
    public async Task SoldByStaffNotInBranch_Is400_AndOtherBrand_Is404()
    {
        var package = await CreatePackageAsync("Fön paketi", 150000, 10, saleCommissionPercent: 10);
        var badSeller = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/customers/{_customer}/packages", new
        {
            packageId = package,
            branchId = _merkez,
            soldByStaffId = "0193a8f0-0000-7000-8000-00000000ee01",
            paymentMethod = "Cash",
        }, _token, Key());
        badSeller.Status.ShouldBe(400);
        badSeller.Body.GetProperty("errors").GetProperty("soldByStaffId")[0].GetString().ShouldBe("out_of_range");

        var sold = await SellAsync(package, "Cash");
        var other = await _api.SignupAsync("+905427771122", "Berber Can");

        (await _api.GetAsync($"/api/v1/manage/packages/{package}", other)).Status.ShouldBe(404);
        (await _api.GetAsync($"/api/v1/manage/customers/{_customer}/packages", other)).Status.ShouldBe(404);
        (await _api.GetAsync("/api/v1/manage/customer-packages", other)).Body.GetProperty("items").GetArrayLength().ShouldBe(0);
        (await _api.GetAsync("/api/v1/manage/packages", other)).Body.GetProperty("items").GetArrayLength().ShouldBe(0);
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/customer-packages/{sold.Body.GetProperty("id").GetString()}/refund",
            new { amount = Amount(1000) }, other, Headers(sold.ETag, Key()))).Status.ShouldBe(404);
        var otherBranch = (await _api.GetAsync("/api/v1/manage/session", other)).Body.GetProperty("branches")[0].GetProperty("id").GetString();
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/customers/{_customer}/packages",
            new { packageId = package, branchId = otherBranch, soldByStaffId = _elif, paymentMethod = "Cash" }, other, Key())).Status.ShouldBe(404);
    }

    // alan-modeli.md § Birleştirme (paketler hedefe taşınır), § Anonimleştirme ("Aktif paket varsa arayüz önce uyarır"); T-92, T-95.
    [Fact]
    public async Task CustomerMerge_MovesPackages_AndDeletionPreview_ListsActivePackages()
    {
        var package = await CreatePackageAsync("Fön paketi", 150000, 10, saleCommissionPercent: 10);
        var packageItem = (await SellAsync(package, "Cash")).Body.GetProperty("items")[0].GetProperty("customerPackageItemId").GetString()!;
        (await BookAsync(await VerifyAsync(CustomerPhone), FridayAt1430, packageItem)).Status.ShouldBe(201);

        var preview = await _api.GetAsync($"/api/v1/manage/customers/{_customer}/deletion", _token);
        preview.Status.ShouldBe(200, preview.Body.ToString());
        var active = preview.Body.GetProperty("activePackages").EnumerateArray().ShouldHaveSingleItem();
        active.GetProperty("packageName").GetString().ShouldBe("Fön paketi");
        active.GetProperty("remainingSessions").GetInt32().ShouldBe(9);

        var target = (await PostAsync("/api/v1/manage/customers", new { fullName = "Selin Ö.", phone = "0533 111 22 18" })).Body.GetProperty("id").GetString()!;
        var merged = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/customers/{target}/merge", new { sourceBrandCustomerId = _customer }, _token);
        merged.Status.ShouldBe(200, merged.Body.ToString());

        var moved = (await _api.GetAsync($"/api/v1/manage/customers/{target}/packages", _token)).Body.GetProperty("items").EnumerateArray().ShouldHaveSingleItem();
        moved.GetProperty("customer").GetProperty("brandCustomerId").GetString().ShouldBe(target);
        moved.GetProperty("remainingSessions").GetInt32().ShouldBe(9, "seans defteri paketle gelir");
        moved.GetProperty("usages").GetArrayLength().ShouldBe(1);
        (await _api.ScalarAsync<long>($"SELECT count(*) FROM sales.customer_packages WHERE brand_customer_id = '{_customer}'")).ShouldBe(0);
    }

    [Fact]
    public async Task SameIdempotencyKey_SellsOnce()
    {
        var package = await CreatePackageAsync("Fön paketi", 150000, 10, saleCommissionPercent: 10);
        var key = Key();

        var first = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/customers/{_customer}/packages", SaleBody(package, "Cash"), _token, key);
        var again = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/customers/{_customer}/packages", SaleBody(package, "Cash"), _token, key);

        first.Status.ShouldBe(201);
        again.Status.ShouldBe(201);
        again.Headers!["Idempotent-Replayed"].ShouldBe("true");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM sales.customer_packages")).ShouldBe(1);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM sales.payments")).ShouldBe(1);
    }

    private const string ExpireJob = "Omvila.Modules.Sales.Jobs.ExpireCustomerPackages, Omvila.Modules.Sales";

    private async Task<string> CreatePackageAsync(string name, long price, int sessions, decimal? saleCommissionPercent, int validityDays = 90)
    {
        var created = await PostAsync("/api/v1/manage/packages", new
        {
            name,
            price = Amount(price),
            items = new[] { new { serviceId = _fon, sessionCount = sessions } },
            validityDays,
            saleCommissionPercent,
            isAvailableAllBranches = true,
            isActive = true,
        });
        created.Status.ShouldBe(201, created.Body.ToString());
        created.ETag.ShouldNotBeNull();
        return created.Body.GetProperty("id").GetString()!;
    }

    private object SaleBody(string package, string method) => new { packageId = package, branchId = _merkez, soldByStaffId = _elif, paymentMethod = method };

    private Task<ManageApi.Response> SellAsync(string package, string method) =>
        PostAsync($"/api/v1/manage/customers/{_customer}/packages", SaleBody(package, method));

    private async Task<JsonElement> CardAsync()
    {
        var card = await _api.GetAsync($"/api/v1/manage/customers/{_customer}/packages", _token);
        card.Status.ShouldBe(200, card.Body.ToString());
        return card.Body;
    }

    private async Task<JsonElement> MyPackagesAsync(string token)
    {
        var listed = await _api.SendAsync(HttpMethod.Get, $"/api/v1/public/branches/{_merkez}/my-packages", null, token);
        listed.Status.ShouldBe(200, listed.Body.ToString());
        return listed.Body;
    }

    private Task<ManageApi.Response> BookAsync(string token, string startUtc, string packageItem) =>
        _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/appointments", new
        {
            items = new[] { new { serviceId = _fon, staffId = _elif } },
            sameStaffForAll = true,
            startUtc,
            fullName = "Selin Öztürk",
            usePackageItems = new[] { new { serviceId = _fon, customerPackageItemId = packageItem } },
            privacyNoticeAcknowledged = true,
            privacyNoticeVersion = "2026-11-01",
        }, token, Key());

    /// <summary>Panelden kapıdan gelen randevu: doğrudan Geldi.</summary>
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
        _factory.Clock.Advance(Duration.FromHours(2));
        return (body.GetProperty("id").GetString()!, created.ETag!, body);
    }

    private Task<ManageApi.Response> CompleteAsync(string id, string? etag, object body) =>
        _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/complete", body, _token, Headers(etag, Key()));

    private async Task<string> ServiceAsync(string category, string name, int minutes, long priceMinor, object deposit) =>
        (await PostAsync("/api/v1/manage/services", new
        {
            categoryId = category,
            name,
            durationMinutes = minutes,
            price = Amount(priceMinor),
            priceDisplay = "Fixed",
            deposit,
            isOnlineBookable = true,
            staffIds = new[] { _elif },
        })).Body.GetProperty("id").GetString()!;

    private async Task<string> VerifyAsync(string phone)
    {
        _factory.Clock.Advance(Duration.FromMinutes(2));
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

    private static bool IsNullOrMissing(JsonElement body, string name) =>
        !body.TryGetProperty(name, out var value) || value.ValueKind == JsonValueKind.Null;

    private static object Amount(long amountMinor) => new { amountMinor, currency = "TRY" };

    private static object Line(string method, long amountMinor) => new { method, amount = Amount(amountMinor) };

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
