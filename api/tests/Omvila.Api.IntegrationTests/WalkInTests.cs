using NodaTime;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-05 kabul kriteri 4 (kapıda sırada 3 kişi; numaralar şube ve gün bazında 1'den; "Başlat" ile CheckedIn randevu,
// kanal WalkIn), durum-makinesi.md § 4 ve A1, alan-modeli.md § WalkInTicket (BranchQueueCounter UPDATE … RETURNING; telefonsuz
// bilette adla telefonsuz müşteri, T-86 W6), bildirimler.md walkin.almost_turn (önünde 1 kişi kalınca, telefon verildiyse, bir
// kez), sureler-ve-ayarlar.md WalkInAverageServiceMinutes, roller-yetkiler.md walkin.manage; sözleşme manage-walk-ins.
// Saat: 2026-11-25 09:00Z (Çarşamba 12:00 İstanbul; kayıt saati 2 dk ilerletir). Şube ve iki personel Pzt–Cmt 09:00–19:00.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class WalkInTests(PostgresFixture db) : IAsyncLifetime
{
    private const string ZeynepPhone = "+905382741905";
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private string _merkez = null!;
    private string _elif = null!;
    private string _zeynep = null!;
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
        _zeynep = (await PostAsync("/api/v1/manage/staff",
            new { fullName = "Zeynep Aksoy", phone = ZeynepPhone, role = "Staff", calendarColor = "#2F5D8A", branchIds = new[] { _merkez } })).Body.GetProperty("id").GetString()!;

        var hours = await _api.GetAsync($"/api/v1/manage/branches/{_merkez}/working-hours", _token);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/branches/{_merkez}/working-hours", Week(), _token, hours.ETag)).Status.ShouldBe(200);
        foreach (var staff in new[] { _elif, _zeynep })
        {
            var detail = await _api.GetAsync($"/api/v1/manage/staff/{staff}", _token);
            (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{staff}/branches/{_merkez}/working-hours", Week(), _token, detail.ETag)).Status.ShouldBe(200);
        }

        var category = (await PostAsync("/api/v1/manage/service-categories", new { name = "Saç" })).Body.GetProperty("id").GetString()!;
        _kesim = await ServiceAsync(category, "Saç kesimi", 45);
        _fon = await ServiceAsync(category, "Fön", 30);
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task Criterion4_ThreeInQueue_NumbersFromOne_StartCreatesCheckedInWalkInAppointment_NextDayRestarts()
    {
        var okan = await AddAsync("Okan Er", serviceId: _kesim, staffId: _zeynep, phone: "0533 000 00 40");
        var burak = await AddAsync("Burak");
        var ayse = await AddAsync("Ayşe Demir", serviceId: _fon);

        okan.Status.ShouldBe(201, okan.Body.ToString());
        okan.ETag.ShouldNotBeNull();
        okan.Body.GetProperty("phoneMasked").GetString().ShouldBe("0533 *** ** 40");
        okan.Body.GetProperty("preferredStaffName").GetString().ShouldBe("Zeynep Aksoy");
        okan.Body.GetProperty("serviceName").GetString().ShouldBe("Saç kesimi");
        new[] { okan, burak, ayse }.Select(t => t.Body.GetProperty("queueNumber").GetInt32()).ShouldBe([1, 2, 3]);

        var queue = await ListAsync();
        queue.GetProperty("date").GetString().ShouldBe("2026-11-25");
        queue.GetProperty("waitingCount").GetInt32().ShouldBe(3);
        queue.GetProperty("nextQueueNumber").GetInt32().ShouldBe(4);
        queue.GetProperty("servedCount").GetInt32().ShouldBe(0);
        queue.GetProperty("oldestJoinedUtc").GetString().ShouldBe(okan.Body.GetProperty("joinedUtc").GetString());
        // 2 müsait personel; hizmetsiz Burak bugünkü sürelerin ortalaması (45, 30 → 38 dk) sayılır.
        queue.GetProperty("items").EnumerateArray().Select(i => i.GetProperty("estimatedWaitMinutes").GetInt32()).ShouldBe([0, 23, 42]);
        queue.GetProperty("estimatedWaitMinutesForNew").GetInt32().ShouldBe(57);

        var started = await StartAsync(okan);

        started.Status.ShouldBe(200, started.Body.ToString());
        started.ETag.ShouldNotBeNull();
        started.Body.GetProperty("ticket").GetProperty("status").GetString().ShouldBe("Served");
        var appointment = started.Body.GetProperty("appointment");
        appointment.GetProperty("status").GetString().ShouldBe("CheckedIn");
        appointment.GetProperty("channel").GetString().ShouldBe("WalkIn");
        appointment.GetProperty("startUtc").GetString().ShouldBe(okan.Body.GetProperty("joinedUtc").GetString(), "şimdi (dakikaya yuvarlanmış) başlar");
        appointment.GetProperty("customer").GetProperty("fullName").GetString().ShouldBe("Okan Er");
        started.Body.GetProperty("ticket").GetProperty("appointmentId").GetString().ShouldBe(appointment.GetProperty("id").GetString());
        (await _api.ScalarAsync<string>("SELECT source FROM customers.brand_customers")).ShouldBe("WalkIn");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM infra.outbox_events WHERE type LIKE '%AppointmentConfirmed'")).ShouldBe(0, "kapıdaki müşteriye bildirim gitmez");

        var after = await ListAsync();
        after.GetProperty("waitingCount").GetInt32().ShouldBe(2);
        after.GetProperty("servedCount").GetInt32().ShouldBe(1);

        await AdvanceAsync(Duration.FromDays(1));
        var tomorrow = await AddAsync("Selin Öztürk");
        tomorrow.Body.GetProperty("queueNumber").GetInt32().ShouldBe(1, "ertesi gün numara 1'den");
        tomorrow.Body.GetProperty("queueDate").GetString().ShouldBe("2026-11-26");
        var next = await ListAsync();
        next.GetProperty("waitingCount").GetInt32().ShouldBe(1);
        next.GetProperty("servedCount").GetInt32().ShouldBe(0);
    }

    [Fact]
    public async Task Start_PhonelessTicket_OpensPhonelessCustomer_MissingServiceAndStaffIs400()
    {
        var burak = await AddAsync("Burak");

        var missing = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/walk-in-tickets/{Id(burak)}/start", new { }, _token, burak.ETag);
        missing.Status.ShouldBe(400);
        missing.Body.GetProperty("errors").GetProperty("serviceId")[0].GetString().ShouldBe("required");
        missing.Body.GetProperty("errors").GetProperty("staffId")[0].GetString().ShouldBe("required");

        var started = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/walk-in-tickets/{Id(burak)}/start",
            new { serviceId = _fon, staffId = _zeynep }, _token, burak.ETag);

        started.Status.ShouldBe(200, started.Body.ToString());
        var customer = started.Body.GetProperty("appointment").GetProperty("customer");
        customer.GetProperty("fullName").GetString().ShouldBe("Burak");
        customer.TryGetProperty("phoneMasked", out _).ShouldBeFalse();
        started.Body.GetProperty("ticket").GetProperty("brandCustomerId").GetString().ShouldBe(customer.GetProperty("brandCustomerId").GetString());
        (await _api.ScalarAsync<long>("SELECT count(*) FROM customers.customers WHERE phone_e164 IS NULL AND full_name = 'Burak'")).ShouldBe(1);

        var card = await _api.GetAsync($"/api/v1/manage/customers/{customer.GetProperty("brandCustomerId").GetString()}", _token);
        card.Status.ShouldBe(200, "telefonsuz müşteri kartı okunur");
        (await _api.GetAsync("/api/v1/manage/customers?q=0555", _token)).Status.ShouldBe(200, "telefon araması telefonsuz kayıtta bozulmaz");
    }

    [Fact]
    public async Task AlmostTurn_SmsOnce_ToTheTicketWithOnePersonAhead_OnlyWithPhone()
    {
        var a = await AddAsync("Ali Kaya", serviceId: _kesim, staffId: _elif);
        var b = await AddAsync("Berk Can", phone: "0555 000 00 02");
        var c = await AddAsync("Cem Ak", phone: "0555 000 00 03");
        var d = await AddAsync("Deniz Er");
        var e = await AddAsync("Ece Su", phone: "0555 000 00 05");
        (await SmsAsync()).ShouldBe(string.Empty, "katılımda gönderilmez");

        (await StartAsync(a)).Status.ShouldBe(200);
        (await SmsAsync()).ShouldBe("+905550000003|Kuaför Elif", "önünde 1 kişi kalan Cem");

        var leftE = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/walk-in-tickets/{Id(e)}/leave", new { }, _token, e.ETag);
        leftE.Status.ShouldBe(200, leftE.Body.ToString());
        leftE.Body.GetProperty("status").GetString().ShouldBe("Left");
        (await SmsAsync()).ShouldBe("+905550000003|Kuaför Elif", "Cem hâlâ ikinci; bir kez");

        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/walk-in-tickets/{Id(b)}/leave", new { }, _token, b.ETag)).Status.ShouldBe(200);
        (await SmsAsync()).ShouldBe("+905550000003|Kuaför Elif", "ikinci sıradaki Deniz'in telefonu yok");
        (await ListAsync()).GetProperty("items").EnumerateArray().Select(i => i.GetProperty("customerName").GetString()).ShouldBe(["Cem Ak", "Deniz Er"]);
        d.Status.ShouldBe(201);
        c.Status.ShouldBe(201);
    }

    [Fact]
    public async Task ConcurrentCreates_GetDistinctNumbers()
    {
        var created = await Task.WhenAll(Enumerable.Range(1, 8).Select(i => AddAsync($"Müşteri {i}")));

        created.Select(c => c.Status).ShouldAllBe(s => s == 201);
        created.Select(c => c.Body.GetProperty("queueNumber").GetInt32()).Order().ShouldBe(Enumerable.Range(1, 8));
        (await _api.ScalarAsync<int>($"SELECT last_number FROM scheduling.branch_queue_counters WHERE branch_id = '{_merkez}'")).ShouldBe(8);
    }

    [Fact]
    public async Task StaffManagesQueue_StartsOnlyForThemselves_ClosedTicket409_StaleVersion412_OtherBrand404()
    {
        var zeynep = (await _api.SignInAsync(ZeynepPhone)).Body.GetProperty("accessToken").GetString()!;
        var ticket = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/walk-in-tickets",
            new { branchId = _merkez, fullName = "Okan Er", serviceId = _kesim }, zeynep, Key());
        ticket.Status.ShouldBe(201, "personel sıraya ekler (walkin.manage)");
        (await _api.GetAsync($"/api/v1/manage/walk-in-tickets?branchId={_merkez}", zeynep)).Status.ShouldBe(200);

        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/walk-in-tickets/{Id(ticket)}/start", new { staffId = _elif }, zeynep, ticket.ETag))
            .Status.ShouldBe(403, "personel yalnız kendine randevu açar");
        var own = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/walk-in-tickets/{Id(ticket)}/start", new { staffId = _zeynep }, zeynep, ticket.ETag);
        own.Status.ShouldBe(200, own.Body.ToString());

        var again = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/walk-in-tickets/{Id(ticket)}/leave", new { }, _token, own.ETag);
        again.Status.ShouldBe(409);
        again.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");

        var other = await AddAsync("Burak");
        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/walk-in-tickets/{Id(other)}/leave", new { }, _token, "W/\"1\"")).Status.ShouldBe(412);
        (await _api.GetAsync("/api/v1/manage/walk-in-tickets", _token)).Status.ShouldBe(400);

        var kaan = await _api.SignupAsync("+905330000001", "Berber Kaan");
        (await _api.GetAsync($"/api/v1/manage/walk-in-tickets?branchId={_merkez}", kaan)).Status.ShouldBe(404);
        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/walk-in-tickets/{Id(other)}/leave", new { }, kaan, other.ETag)).Status.ShouldBe(404);
        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/walk-in-tickets/{Id(other)}/start", new { staffId = _elif, serviceId = _kesim }, kaan, other.ETag))
            .Status.ShouldBe(404);
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/walk-in-tickets", new { branchId = _merkez, fullName = "Okan" }, kaan, Key())).Status.ShouldBe(404);
    }

    private async Task<string> SmsAsync() => await _api.ScalarAsync<string?>("""
        SELECT coalesce(string_agg(recipient_phone || '|' || (payload->>'salon'), ',' ORDER BY created_utc), '')
        FROM notifications.notification_outbox WHERE event_code = 'walkin.almost_turn'
        """) ?? string.Empty;

    private async Task<System.Text.Json.JsonElement> ListAsync()
    {
        var list = await _api.GetAsync($"/api/v1/manage/walk-in-tickets?branchId={_merkez}", _token);
        list.Status.ShouldBe(200, list.Body.ToString());
        return list.Body;
    }

    private Task<ManageApi.Response> AddAsync(string fullName, string? serviceId = null, string? staffId = null, string? phone = null) =>
        PostAsync("/api/v1/manage/walk-in-tickets", new { branchId = _merkez, fullName, phone, serviceId, preferredStaffId = staffId });

    private Task<ManageApi.Response> StartAsync(ManageApi.Response ticket) =>
        _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/walk-in-tickets/{Id(ticket)}/start", new { }, _token, ticket.ETag);

    private async Task AdvanceAsync(Duration duration)
    {
        _factory.Clock.Advance(duration);
        _token = (await _api.SignInAsync("+905324187705")).Body.GetProperty("accessToken").GetString()!;
    }

    private static string Id(ManageApi.Response ticket) => ticket.Body.GetProperty("id").GetString()!;

    private Task<ManageApi.Response> PostAsync(string path, object body) => _api.SendAsync(HttpMethod.Post, path, body, _token, Key());

    private async Task<string> ServiceAsync(string category, string name, int minutes) =>
        (await PostAsync("/api/v1/manage/services", new
        {
            categoryId = category,
            name,
            durationMinutes = minutes,
            price = new { amountMinor = 45000, currency = "TRY" },
            priceDisplay = "Fixed",
            deposit = new { mode = "Inherit" },
            isOnlineBookable = true,
            staffIds = new[] { _elif, _zeynep },
        })).Body.GetProperty("id").GetString()!;

    private static object Week() => new
    {
        days = Enumerable.Range(1, 6).Select(d => new { dayOfWeek = d, intervals = new[] { new { start = "09:00", end = "19:00" } } }).ToArray(),
    };

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{++_requests:D12}" };
}
