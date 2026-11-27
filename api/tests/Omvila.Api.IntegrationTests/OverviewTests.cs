using System.Text.Json;
using NodaTime;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: sözleşme manageGetOverview (bugünkü ciro iade düşülmüş; haftalık doluluk = dolu dakika / çalışma dakikası, Draft ve
// Cancelled sayılmaz; çalışma dakikası olmayan personel listede yok); bildirimler.md staff.daily_summary (tercihe göre, kapalı
// varsayılan) ve sureler-ve-ayarlar.md DailySummaryLocalTime (20:00, şube yerel); roller-yetkiler.md report.view; T-105 (6), T-120.
// Saat: 2026-11-25 09:00Z (Çarşamba 12:00 İstanbul); hafta 23–29 Kasım.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class OverviewTests(PostgresFixture db) : IAsyncLifetime
{
    private const string Path = "/api/v1/manage/overview";
    private const string ZeynepPhone = "+905382741905";
    private const string DailySummaryJob = "Omvila.Modules.Scheduling.Jobs.SendDailySummaries, Omvila.Modules.Scheduling";
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private string _merkez = null!;
    private string _brand = null!;
    private string _elif = null!;
    private string _zeynep = null!;
    private string _kesim = null!;
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
        _zeynep = (await PostAsync("/api/v1/manage/staff",
            new { fullName = "Zeynep Aksoy", phone = ZeynepPhone, role = "Staff", calendarColor = "#2F5D8A", branchIds = new[] { _merkez } })).Body.GetProperty("id").GetString()!;

        await PutHoursAsync($"/api/v1/manage/branches/{_merkez}/working-hours", $"/api/v1/manage/branches/{_merkez}/working-hours",
            Enumerable.Range(1, 6).Select(d => (d, "09:00", "19:00")));
        // Elif yalnız perşembe 10:00–12:00, Zeynep yalnız çarşamba 14:00–16:00 çalışır (haftada 120'şer dakika).
        await PutHoursAsync($"/api/v1/manage/staff/{_elif}", $"/api/v1/manage/staff/{_elif}/branches/{_merkez}/working-hours", [(4, "10:00", "12:00")]);
        await PutHoursAsync($"/api/v1/manage/staff/{_zeynep}", $"/api/v1/manage/staff/{_zeynep}/branches/{_merkez}/working-hours", [(3, "14:00", "16:00")]);
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
            staffIds = new[] { _elif, _zeynep },
        })).Body.GetProperty("id").GetString()!;
        _customer = (await PostAsync("/api/v1/manage/customers", new { fullName = "Selin Öztürk", phone = "0532 123 45 67" })).Body.GetProperty("id").GetString()!;
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task Overview_TodayRevenueAndCount_WeeklyOccupancyByStaff()
    {
        await BookAsync(_elif, Instant.FromUtc(2026, 11, 26, 7, 0));
        await BookAsync(_elif, Instant.FromUtc(2026, 11, 26, 7, 30));
        await BookAsync(_zeynep, Instant.FromUtc(2026, 11, 25, 11, 0));
        var cancelled = await BookAsync(_zeynep, Instant.FromUtc(2026, 11, 25, 12, 0));
        await _api.ScalarAsync<int>(
            $"UPDATE scheduling.appointments SET status = 'Cancelled', cancelled_utc = now(), cancelled_by = 'Salon', cancellation_reason = 'Other' WHERE id = '{cancelled}' RETURNING 1");
        var paid = await InsertPaymentAsync(100000, Instant.FromUtc(2026, 11, 25, 7, 0));
        await _api.ScalarAsync<int>(
            $"UPDATE sales.payments SET refunded_amount_minor = 1600, status = 'PartiallyRefunded', refunded_utc = '2026-11-25T08:00:00Z' WHERE id = '{paid}' RETURNING 1");
        await InsertPaymentAsync(55500, Instant.FromUtc(2026, 11, 24, 20, 59));

        var overview = await _api.GetAsync(Path, _token);

        overview.Status.ShouldBe(200, overview.Body.ToString());
        var body = overview.Body;
        body.GetProperty("branchId").GetString().ShouldBe(_merkez, "birincil şube");
        body.GetProperty("timeZone").GetString().ShouldBe("Europe/Istanbul");
        body.GetProperty("date").GetString().ShouldBe("2026-11-25");
        body.GetProperty("todayRevenue").GetProperty("amountMinor").GetInt64().ShouldBe(98400, "dünün tahsilatı yok, iade düşülür");
        body.GetProperty("todayAppointmentCount").GetInt32().ShouldBe(1, "iptal sayılmaz");
        body.GetProperty("pendingApproval").GetProperty("count").GetInt32().ShouldBe(0);
        body.GetProperty("pendingApproval").GetProperty("earliestHoldExpiresUtc").ValueKind.ShouldBe(JsonValueKind.Null);
        body.GetProperty("walkIns").GetProperty("waitingCount").GetInt32().ShouldBe(0);
        var weekly = body.GetProperty("weeklyOccupancy");
        weekly.GetProperty("weekStart").GetString().ShouldBe("2026-11-23");
        weekly.GetProperty("percent").GetInt32().ShouldBe(38, "90 / 240 dakika");
        weekly.GetProperty("previousWeekPercent").GetInt32().ShouldBe(0);
        weekly.GetProperty("staff").EnumerateArray()
            .Select(s => $"{s.GetProperty("staffName").GetString()}/{s.GetProperty("percent").GetInt32()}/{s.GetProperty("calendarColor").GetString()}")
            .ShouldBe(["Elif Kaya/50/" + await ColorAsync(_elif), "Zeynep Aksoy/25/#2F5D8A"], "doluluğa göre azalan");
    }

    [Fact]
    public async Task Overview_NoWorkingMinutes_PercentNull_StaffEmpty()
    {
        await _api.ScalarAsync<int>("WITH d AS (DELETE FROM tenancy.staff_working_hours RETURNING 1) SELECT count(*)::int FROM d");

        var weekly = (await _api.GetAsync(Path, _token)).Body.GetProperty("weeklyOccupancy");

        weekly.GetProperty("percent").ValueKind.ShouldBe(JsonValueKind.Null);
        weekly.GetProperty("staff").GetArrayLength().ShouldBe(0);
    }

    [Fact]
    public async Task Overview_Staff403_BranchManagerOtherBranch404_OtherBrand404()
    {
        var kadikoy = (await PostAsync("/api/v1/manage/branches", new { name = "Kadıköy", addressLine = "Moda Cad. 1", district = "Kadıköy", city = "İstanbul" }))
            .Body.GetProperty("id").GetString()!;
        var manager = await _api.AddMemberAndSignInAsync(_token, "+905382741906", "BranchManager", _merkez);
        var staff = (await _api.SignInAsync(ZeynepPhone)).Body.GetProperty("accessToken").GetString()!;
        var other = await _api.SignupAsync("+905427771122", "Berber Can");

        (await _api.GetAsync(Path, staff)).Status.ShouldBe(403);
        (await _api.GetAsync(Path, manager)).Body.GetProperty("branchId").GetString().ShouldBe(_merkez);
        (await _api.GetAsync($"{Path}?branchId={kadikoy}", manager)).Status.ShouldBe(404);
        (await _api.GetAsync($"{Path}?branchId={kadikoy}", _token)).Status.ShouldBe(200);
        (await _api.GetAsync($"{Path}?branchId={_merkez}", other)).Status.ShouldBe(404);
    }

    // Gün sonu özeti: şube yerel 20:00'den önce yazılmaz, sonra günde bir kez; alıcı report.view + tercihi açık olan.
    [Fact]
    public async Task DailySummary_AtBranchLocal2000_OncePerDay_OnlyToOptedInReportViewers()
    {
        (await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/me/notification-preferences", Preferences(dailySummary: true), _token, "W/\"0\"")).Status.ShouldBe(200);
        var managerToken = await _api.AddMemberAndSignInAsync(_token, "+905382741906", "BranchManager", _merkez);
        managerToken.ShouldNotBeNullOrEmpty();
        var ownerUser = await _api.ScalarAsync<string>("SELECT user_id::text FROM identity.user_memberships WHERE role = 'Owner'");
        await CompletedVisitAsync();
        await InsertPaymentAsync(150000, Instant.FromUtc(2026, 11, 25, 10, 0));

        _factory.Clock.Reset(Instant.FromUtc(2026, 11, 25, 16, 59));
        await _api.RunJobAsync(DailySummaryJob);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM scheduling.branch_daily_summaries")).ShouldBe(0, "19:59 İstanbul");

        _factory.Clock.Reset(Instant.FromUtc(2026, 11, 25, 17, 0));
        await _api.RunJobAsync(DailySummaryJob);
        await _api.RunJobAsync(DailySummaryJob);
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);

        (await _api.ScalarAsync<string>("SELECT business_date || '/' || completed_count || '/' || collected_minor FROM scheduling.branch_daily_summaries"))
            .ShouldBe("2026-11-25/1/195000");
        (await _api.ScalarAsync<string>(
            "SELECT string_agg(recipient_id::text || '|' || (payload->>'n') || '|' || (payload->>'tutar'), ',') FROM notifications.notification_outbox WHERE event_code = 'staff.daily_summary'"))
            .ShouldBe($"{ownerUser}|1|1.950 ₺", "yönetici tercihini açmadı (varsayılan kapalı)");

        _factory.Clock.Reset(Instant.FromUtc(2026, 11, 26, 17, 30));
        await _api.RunJobAsync(DailySummaryJob);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM scheduling.branch_daily_summaries")).ShouldBe(1, "ertesi gün tamamlanan yok");
    }

    [Fact]
    public async Task DailySummary_UsesBranchTimeZone()
    {
        await CompletedVisitAsync();
        await _api.ScalarAsync<int>($"UPDATE tenancy.branches SET time_zone_id = 'Europe/London' WHERE id = '{_merkez}' RETURNING 1");

        _factory.Clock.Reset(Instant.FromUtc(2026, 11, 25, 17, 0));
        await _api.RunJobAsync(DailySummaryJob);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM scheduling.branch_daily_summaries")).ShouldBe(0, "Londra'da 17:00");

        _factory.Clock.Reset(Instant.FromUtc(2026, 11, 25, 20, 0));
        await _api.RunJobAsync(DailySummaryJob);
        (await _api.ScalarAsync<string>("SELECT business_date || '/' || completed_count FROM scheduling.branch_daily_summaries")).ShouldBe("2026-11-25/1");
    }

    /// <summary>Zeynep'le bugün 14:00 (İstanbul) saç kesimi: Geldi + tamamla, nakit 450 ₺.</summary>
    private async Task CompletedVisitAsync()
    {
        var id = await BookAsync(_zeynep, Instant.FromUtc(2026, 11, 25, 11, 0));
        _factory.Clock.Reset(Instant.FromUtc(2026, 11, 25, 11, 0));
        var appointment = await _api.GetAsync($"/api/v1/manage/appointments/{id}", _token);
        var checkedIn = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/check-in", null, _token, IfMatch(appointment.ETag!));
        checkedIn.Status.ShouldBe(200, checkedIn.Body.ToString());
        var headers = Key();
        headers["If-Match"] = checkedIn.ETag!;
        var completed = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/complete",
            new { payments = new[] { new { method = "Cash", amount = new { amountMinor = 45000, currency = "TRY" } } } }, _token, headers);
        completed.Status.ShouldBe(200, completed.Body.ToString());
    }

    private async Task<string> BookAsync(string staffId, Instant start)
    {
        var created = await PostAsync("/api/v1/manage/appointments", new
        {
            branchId = _merkez,
            brandCustomerId = _customer,
            channel = "Phone",
            startUtc = NodaTime.Text.InstantPattern.General.Format(start),
            items = new[] { new { serviceId = _kesim, staffId } },
        });
        created.Status.ShouldBe(201, created.Body.ToString());
        return created.Body.GetProperty("appointment").GetProperty("id").GetString()!;
    }

    private async Task PutHoursAsync(string etagPath, string path, IEnumerable<(int Day, string Start, string End)> days)
    {
        var current = await _api.GetAsync(etagPath, _token);
        var body = new { days = days.Select(d => new { dayOfWeek = d.Day, intervals = new[] { new { start = d.Start, end = d.End } } }).ToArray() };
        (await _api.WriteAsync(HttpMethod.Put, path, body, _token, current.ETag)).Status.ShouldBe(200);
    }

    private Task<string> ColorAsync(string staffId) => _api.ScalarAsync<string>($"SELECT calendar_color FROM tenancy.staff WHERE id = '{staffId}'");

    private async Task<string> InsertPaymentAsync(long amountMinor, Instant paidUtc)
    {
        var id = $"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _requests) + 500000:D12}";
        await _api.ScalarAsync<int>(
            $"""
            INSERT INTO sales.payments (id, brand_id, branch_id, brand_customer_id, method, amount_minor, currency_code, is_deposit, status, refunded_amount_minor, paid_utc)
              VALUES ('{id}', '{_brand}', '{_merkez}', NULL, 'Card', {amountMinor}, 'TRY', false, 'Recorded', 0, '{NodaTime.Text.InstantPattern.General.Format(paidUtc)}');
            INSERT INTO sales.payment_allocations (id, brand_id, payment_id, target_type, target_id, amount_minor)
              VALUES (gen_random_uuid(), '{_brand}', '{id}', 'CustomerPackage', gen_random_uuid(), {amountMinor});
            SELECT 1;
            """);
        return id;
    }

    private static object Preferences(bool dailySummary) => new
    {
        newOnlineAppointment = true,
        cancellationAndReschedule = true,
        customerCheckedIn = false,
        walkInQueue = true,
        newReview = true,
        dailySummary,
        quietHoursEnabled = true,
    };

    private static Dictionary<string, string> IfMatch(string etag) => new(StringComparer.Ordinal) { ["If-Match"] = etag };

    private Task<ManageApi.Response> PostAsync(string path, object body) => _api.SendAsync(HttpMethod.Post, path, body, _token, Key());

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _requests):D12}" };
}
