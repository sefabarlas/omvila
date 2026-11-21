using System.Text.Json;
using System.Text.RegularExpressions;
using NodaTime;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-04 kabul kriterleri 2 (riskli müşteri, Balyaj %30 2.800 ₺ → 840 ₺, DepositPending, +2 sa; diğer müşteri
// kaporasız), 3 (havale talimatı ve deposit.awaiting SMS'i), 6 (süre dolunca DepositNotReceived), 8 (paket: Catalog birim
// testleri); durum-makinesi.md § Oluşturma kuralı (panel kapora istemez; kapora + onay tek süre TransferWindowMinutes);
// sureler-ve-ayarlar.md § Kapora (aktif hesap yoksa kapora yok); V10 (talimat güncel hesaptan); T-81.
// Saat: 2026-11-25 09:00Z (Çarşamba 12:00 İstanbul). Randevu Cuma 14:30 (11:30Z), Balyaj 60 dk 2.800 ₺.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed partial class DepositBookingTests(PostgresFixture db) : IAsyncLifetime
{
    private const string RiskyPhone = "+905321234567";
    private const string OtherPhone = "+905339876543";
    private const string FridayAt1430 = "2026-11-27T11:30:00Z";
    private const string BankPath = "/api/v1/manage/brand/bank-account";
    private const string ExpireJob = "Omvila.Modules.Scheduling.Jobs.ExpireAppointmentHolds, Omvila.Modules.Scheduling";
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
        // Havale süresi jeton ömürlerinden uzun: saat ilerletildiğinde oturum düşmesin.
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

        (await _api.WriteAsync(HttpMethod.Put, BankPath, Account("tr29 0006 4000 0011 2345 6789 01"), _token, "W/\"0\"")).Status.ShouldBe(200);
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

        // Marka varsayılanı: RiskyCustomersOnly, eşik 2. Riskli müşteri 2 kez gelmemiş.
        var risky = (await PostAsync("/api/v1/manage/customers", new { fullName = "Ayşe Yılmaz", phone = RiskyPhone })).Body.GetProperty("id").GetString()!;
        await _api.ScalarAsync<int>($"UPDATE customers.brand_customers SET no_show_count = 2 WHERE id = '{risky}' RETURNING 1");
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task Criterion2And3_RiskyCustomer_Deposit840_DepositPending_Hold2h_Instructions_Sms()
    {
        var hold = Iso(_factory.Clock.GetCurrentInstant() + Duration.FromMinutes(120));
        var created = await CreateAsync(RiskyPhone, "Ayşe Yılmaz");

        created.Status.ShouldBe(201, created.Body.ToString());
        var body = created.Body;
        body.GetProperty("status").GetString().ShouldBe("PendingApproval");
        body.GetProperty("displayStatus").GetString().ShouldBe("DepositPending");
        body.GetProperty("holdExpiresUtc").GetString().ShouldBe(hold);
        body.GetProperty("total").GetProperty("amountMinor").GetInt64().ShouldBe(280000);
        body.GetProperty("amountDue").GetProperty("amountMinor").GetInt64().ShouldBe(280000, "kapora gelmeden salonda ödenecek toplamdır");
        var deposit = body.GetProperty("deposit");
        deposit.GetProperty("required").GetProperty("amountMinor").GetInt64().ShouldBe(84000);
        deposit.GetProperty("status").GetString().ShouldBe("Awaiting");
        var code = deposit.GetProperty("transferCode").GetString()!;
        TransferCodePattern().IsMatch(code).ShouldBeTrue(code);
        var instructions = deposit.GetProperty("instructions");
        instructions.GetProperty("accountHolderName").GetString().ShouldBe("Elif Kaya");
        instructions.GetProperty("bankName").GetString().ShouldBe("Ziraat Bankası");
        instructions.GetProperty("iban").GetString().ShouldBe("TR29 0006 4000 0011 2345 6789 01");
        instructions.GetProperty("amount").GetProperty("amountMinor").GetInt64().ShouldBe(84000);
        instructions.GetProperty("transferCode").GetString().ShouldBe(code);
        instructions.GetProperty("dueUtc").GetString().ShouldBe(hold);
        (await _api.ScalarAsync<string>("SELECT hold_reason || '/' || deposit_required_minor || '/' || deposit_transfer_code FROM scheduling.appointments"))
            .ShouldBe($"Deposit/84000/{code}");

        // T-81: kaporalı talepte "Talebiniz alındı" yerine yalnız deposit.awaiting (her zaman SMS), personele onay bildirimi.
        await DispatchAsync();
        (await SentBodiesAsync(RiskyPhone, code)).ShouldBe(
            $"Kuaför Elif: Cum 27 Kas 14:30 için 840 TL kaporayı 2 saat içinde gönderin. IBAN TR290006400000112345678901, Elif Kaya, açıklama {code}");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'appointment.requested'")).ShouldBe(0);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.in_app_notifications WHERE event_code = 'staff.approval_needed'")).ShouldBe(1);
    }

    // Panel: yan panel, takvim bloğu ve onay kuyruğu kapora durumunu taşır; havale talimatı panelde yok.
    [Fact]
    public async Task Criterion2_PanelViews_CarryDepositStatus_WithoutInstructions()
    {
        var created = (await CreateAsync(RiskyPhone, "Ayşe Yılmaz")).Body;
        var id = created.GetProperty("id").GetString()!;
        var code = created.GetProperty("deposit").GetProperty("transferCode").GetString()!;

        var detail = await _api.GetAsync($"/api/v1/manage/appointments/{id}", _token);
        detail.Body.GetProperty("displayStatus").GetString().ShouldBe("DepositPending");
        detail.Body.GetProperty("deposit").GetProperty("status").GetString().ShouldBe("Awaiting");
        detail.Body.GetProperty("deposit").GetProperty("transferCode").GetString().ShouldBe(code);
        detail.Body.GetProperty("deposit").TryGetProperty("instructions", out _).ShouldBeFalse();
        detail.Body.GetProperty("amountDue").GetProperty("amountMinor").GetInt64().ShouldBe(280000);
        var block = (await _api.GetAsync($"/api/v1/manage/calendar?branchId={_merkez}&from=2026-11-27&to=2026-11-27", _token))
            .Body.GetProperty("appointments")[0];
        block.GetProperty("displayStatus").GetString().ShouldBe("DepositPending");
        block.GetProperty("depositStatus").GetString().ShouldBe("Awaiting");
        block.GetProperty("depositAmount").GetProperty("amountMinor").GetInt64().ShouldBe(84000);
        var queue = (await _api.GetAsync("/api/v1/manage/approval-queue", _token)).Body;
        queue.GetProperty("totalCount").GetInt32().ShouldBe(1);
        queue.GetProperty("items")[0].GetProperty("depositStatus").GetString().ShouldBe("Awaiting");
        queue.GetProperty("items")[0].GetProperty("depositAmount").GetProperty("amountMinor").GetInt64().ShouldBe(84000);

    }

    [Fact]
    public async Task Criterion2_CustomerWithoutNoShows_NoDeposit_Confirmed()
    {
        var created = await CreateAsync(OtherPhone, "Derya Şahin");

        created.Status.ShouldBe(201, created.Body.ToString());
        created.Body.GetProperty("status").GetString().ShouldBe("Confirmed");
        created.Body.TryGetProperty("deposit", out var deposit).ShouldBe(deposit.ValueKind == JsonValueKind.Null);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM scheduling.appointments WHERE deposit_required_minor IS NOT NULL")).ShouldBe(0);
    }

    [Fact]
    public async Task DepositAndApproval_SingleHold_IsTransferWindow_NotApprovalTimeout()
    {
        await _api.ScalarAsync<int>("UPDATE tenancy.brand_settings SET requires_approval = true RETURNING 1");
        var account = await _api.GetAsync(BankPath, _token);
        (await _api.WriteAsync(HttpMethod.Put, BankPath, Account(iban: null, window: 240), _token, account.ETag)).Status.ShouldBe(200);
        var hold = Iso(_factory.Clock.GetCurrentInstant() + Duration.FromMinutes(240));

        var created = await CreateAsync(RiskyPhone, "Ayşe Yılmaz");

        created.Body.GetProperty("holdExpiresUtc").GetString().ShouldBe(hold);
        (await _api.ScalarAsync<string>("SELECT hold_reason FROM scheduling.appointments")).ShouldBe("ApprovalAndDeposit");
        await DispatchAsync();
        var code = created.Body.GetProperty("deposit").GetProperty("transferCode").GetString()!;
        (await SentBodiesAsync(RiskyPhone, code)).ShouldContain("kaporayı 4 saat içinde gönderin");
    }

    [Fact]
    public async Task NoActiveBankAccount_NoDeposit_EvenForAlwaysService()
    {
        await _api.ScalarAsync<int>($"UPDATE catalog.services SET deposit_mode = 'Always' WHERE id = '{_balyaj}' RETURNING 1");
        await _api.ScalarAsync<int>("UPDATE tenancy.brand_bank_accounts SET is_active = false RETURNING 1");

        var created = await CreateAsync(RiskyPhone, "Ayşe Yılmaz");

        created.Status.ShouldBe(201, created.Body.ToString());
        created.Body.GetProperty("status").GetString().ShouldBe("Confirmed");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM scheduling.appointments WHERE deposit_required_minor IS NOT NULL")).ShouldBe(0);
    }

    [Fact]
    public async Task PanelChannel_NoDeposit_ForRiskyCustomer()
    {
        var risky = await _api.ScalarAsync<Guid>("SELECT id FROM customers.brand_customers WHERE no_show_count = 2");
        var created = await PostAsync("/api/v1/manage/appointments", new
        {
            branchId = _merkez,
            brandCustomerId = risky,
            channel = "Phone",
            startUtc = FridayAt1430,
            items = new[] { new { serviceId = _balyaj, staffId = _elif } },
        });

        created.Status.ShouldBe(201, created.Body.ToString());
        created.Body.GetProperty("appointment").GetProperty("status").GetString().ShouldBe("Confirmed");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM scheduling.appointments WHERE deposit_required_minor IS NOT NULL")).ShouldBe(0);
    }

    // V10: kapora bekleyen randevuda salon IBAN'ı değiştirirse SMS bağlantısı güncel IBAN'ı gösterir; hesap pasifse talimat yok.
    [Fact]
    public async Task AppointmentLink_ShowsCurrentAccount_NoInstructionsWhenAccountInactive()
    {
        var accessCode = (await CreateAsync(RiskyPhone, "Ayşe Yılmaz")).Body.GetProperty("accessCode").GetString()!;
        var account = await _api.GetAsync(BankPath, _token);
        (await _api.WriteAsync(HttpMethod.Put, BankPath, Account("TR33 0006 1005 1978 6457 8413 26"), _token, account.ETag)).Status.ShouldBe(200);

        var shown = await _api.SendAsync(HttpMethod.Get, $"/api/v1/public/appointments/{accessCode}", null);
        shown.Status.ShouldBe(200, shown.Body.ToString());
        shown.Body.GetProperty("deposit").GetProperty("instructions").GetProperty("iban").GetString().ShouldBe("TR33 0006 1005 1978 6457 8413 26");

        await _api.ScalarAsync<int>("UPDATE tenancy.brand_bank_accounts SET is_active = false RETURNING 1");
        var inactive = await _api.SendAsync(HttpMethod.Get, $"/api/v1/public/appointments/{accessCode}", null);
        inactive.Body.GetProperty("deposit").GetProperty("status").GetString().ShouldBe("Awaiting");
        inactive.Body.GetProperty("deposit").TryGetProperty("instructions", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task Criterion6_HoldExpires_CancelledDepositNotReceived_SlotFreed()
    {
        var accessCode = (await CreateAsync(RiskyPhone, "Ayşe Yılmaz")).Body.GetProperty("accessCode").GetString()!;
        _factory.Clock.Advance(Duration.FromMinutes(120));

        await _api.RunJobAsync(ExpireJob);

        (await _api.ScalarAsync<string>("SELECT status || '/' || cancellation_reason || '/' || cancelled_by FROM scheduling.appointments"))
            .ShouldBe("Cancelled/DepositNotReceived/System");
        var shown = await _api.SendAsync(HttpMethod.Get, $"/api/v1/public/appointments/{accessCode}", null);
        shown.Body.GetProperty("displayStatus").GetString().ShouldBe("Cancelled");
        shown.Body.GetProperty("deposit").TryGetProperty("instructions", out _).ShouldBeFalse("iptal edilen randevuda havale talimatı yok");
        (await CreateAsync(OtherPhone, "Derya Şahin")).Status.ShouldBe(201, "saat boşaldı");
    }

    [GeneratedRegex("^OMV-[2-9A-HJ-NP-Z]{4}$", RegexOptions.None, 1000)]
    private static partial Regex TransferCodePattern();

    private static string Iso(Instant at) => NodaTime.Text.InstantPattern.General.Format(at);

    private static object Account(string? iban, int window = 120) => iban is null
        ? new { accountHolderName = "Elif Kaya", bankName = "Ziraat Bankası", transferCodePrefix = "OMV", transferWindowMinutes = window }
        : new { accountHolderName = "Elif Kaya", bankName = "Ziraat Bankası", iban, transferCodePrefix = "OMV", transferWindowMinutes = window };

    private async Task DispatchAsync()
    {
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        await _api.RunJobAsync(DispatchSmsJob);
    }

    /// <summary>Bu randevunun kapora SMS'leri (geliştirme SMS günlüğü testler arasında temizlenmez; açıklama koduyla süzülür).</summary>
    private async Task<string> SentBodiesAsync(string phone, string transferCode) =>
        await _api.ScalarAsync<object>(
            $"SELECT string_agg(body, E'\\n') FROM infra.dev_sms_log WHERE phone_e164 = '{phone}' AND body LIKE '%{transferCode}'") as string ?? string.Empty;

    private async Task<ManageApi.Response> CreateAsync(string phone, string fullName) =>
        await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/appointments", new
        {
            items = new[] { new { serviceId = _balyaj, staffId = _elif } },
            sameStaffForAll = true,
            startUtc = FridayAt1430,
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

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _requests):D12}" };
}
