using System.Text.Json;
using NodaTime;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-05 KK2 ("Aynısını tekrar al": kalem serviceId/staffId, canRebook), KK6 (müşteri tarafı paketler), KK8 (değerlendirme
// yalnız Completed ve bir kez), KK9 (hesap silme: bütün markalarda anonim, randevu/ödeme kayıtları kalır); dilim-04 KK9 (Ödemelerim:
// AwaitingConfirmation randevudan); durum-makinesi.md A6 (müşteri iptali, CancellationCutoffHours), A14 (saat değişikliği); sözleşme
// me-appointments, me-wallet, me-profile (rızalar, hesap silme), publicCreateAppointment / publicListMyPackages / publicJoinWaitlist meBearer;
// T-101, T-104. Saat: 2026-11-25 09:00Z (Çarşamba 12:00 İstanbul).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class MeAppointmentTests(PostgresFixture db) : IAsyncLifetime
{
    private const string MervePhone = "+905551112233";
    private const string NurPhone = "+905552223344";
    private const string OwnerPhone = "+905324187705";
    private const string OtherOwnerPhone = "+905427771122";
    private const string FridayAt1430 = "2026-11-27T11:30:00Z";
    private const string BankPath = "/api/v1/manage/brand/bank-account";
    private static readonly string[] WaitlistDates = ["2026-11-28"];
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _owner = null!;
    private string _merkez = null!;
    private string _elif = null!;
    private string _fon = null!;
    private string _balyaj = null!;
    private int _requests;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db, new Dictionary<string, string?>(StringComparer.Ordinal) { ["Auth:AccessTokenMinutes"] = "600" });
        _api = new ManageApi(_factory, db);
        _owner = await _api.SignupAsync(OwnerPhone, "Kuaför Elif");
        var session = (await _api.GetAsync("/api/v1/manage/session", _owner)).Body;
        _merkez = session.GetProperty("branches")[0].GetProperty("id").GetString()!;
        _elif = session.GetProperty("membership").GetProperty("staffId").GetString()!;
        await OpenWeekAsync(_owner, _merkez, _elif);
        var category = (await PostAsync("/api/v1/manage/service-categories", new { name = "Saç" })).Body.GetProperty("id").GetString()!;
        _fon = await ServiceAsync(_owner, category, "Fön", 45000, _elif);
        _balyaj = await ServiceAsync(_owner, category, "Balyaj", 280000, _elif, depositPercent: 30);
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    // ───────── meBearer ile randevu (T-101 (3)) ─────────

    [Fact]
    public async Task MeBearer_CreatesAppointment_FillsProfileName_SavesBrandAsBooking_AndAppearsInUpcoming()
    {
        var merve = await SignInAsync(MervePhone);

        var created = await BookAsync(merve, _fon, FridayAt1430);

        created.Status.ShouldBe(201, created.Body.ToString());
        var id = created.Body.GetProperty("id").GetString()!;
        (await _api.ScalarAsync<string>($"SELECT full_name FROM customers.customers WHERE phone_e164 = '{MervePhone}'")).ShouldBe("Merve Yıldız", "adsız profile ilk randevunun adı yazılır");
        (await _api.ScalarAsync<string>($"SELECT client_app FROM scheduling.appointments WHERE id = '{id}'")).ShouldBe("CustomerApp");
        (await _api.ScalarAsync<string>("SELECT source FROM customers.customer_saved_brands")).ShouldBe("Booking");
        (await _api.ScalarAsync<string>("SELECT channel FROM customers.consent_records LIMIT 1")).ShouldBe("CustomerApp");
        var brand = (await _api.GetAsync("/api/v1/me/brands", merve)).Body.GetProperty("items").EnumerateArray().ShouldHaveSingleItem();
        brand.GetProperty("hasBooked").GetBoolean().ShouldBeTrue();
        brand.GetProperty("savedUtc").ValueKind.ShouldBe(JsonValueKind.Null, "randevuyla gelen markada savedUtc yok");

        var upcoming = (await ListAsync(merve, "Upcoming")).Body;
        var row = upcoming.GetProperty("items").EnumerateArray().ShouldHaveSingleItem();
        row.GetProperty("id").GetString().ShouldBe(id);
        row.GetProperty("status").GetString().ShouldBe("Confirmed");
        row.GetProperty("brand").GetProperty("slug").GetString().ShouldNotBeNullOrEmpty();
        row.GetProperty("branch").GetProperty("id").GetString().ShouldBe(_merkez);
        row.GetProperty("timeZone").GetString().ShouldBe("Europe/Istanbul");
        var item = row.GetProperty("items").EnumerateArray().ShouldHaveSingleItem();
        item.GetProperty("serviceId").GetString().ShouldBe(_fon);
        item.GetProperty("staffId").GetString().ShouldBe(_elif);
        row.GetProperty("canRebook").GetBoolean().ShouldBeTrue();
        row.GetProperty("canReview").GetBoolean().ShouldBeFalse();
        upcoming.GetProperty("nextCursor").ValueKind.ShouldBe(JsonValueKind.Null);
        (await ListAsync(merve, "Past")).Body.GetProperty("items").GetArrayLength().ShouldBe(0);
    }

    [Fact]
    public async Task MeBearer_ProfileNameWins_AndMyPackagesAndWaitlistAcceptTheAppToken()
    {
        await PostAsync("/api/v1/manage/customers", new { fullName = "Merve Yıldız", phone = MervePhone });
        var merve = await SignInAsync(MervePhone);

        var created = await BookAsync(merve, _fon, FridayAt1430, fullName: "Başka Ad");
        created.Status.ShouldBe(201, created.Body.ToString());
        (await _api.ScalarAsync<string>($"SELECT full_name FROM customers.customers WHERE phone_e164 = '{MervePhone}'")).ShouldBe("Merve Yıldız");

        var packages = await _api.SendAsync(HttpMethod.Get, $"/api/v1/public/branches/{_merkez}/my-packages", null, merve);
        packages.Status.ShouldBe(200, packages.Body.ToString());
        packages.Body.GetProperty("items").GetArrayLength().ShouldBe(0);

        var joined = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/waitlist", new
        {
            preferences = new { serviceIds = new[] { _fon }, preferredStaffId = _elif, acceptOtherStaff = false, dates = WaitlistDates, timeWindows = Array.Empty<string>() },
            privacyNoticeAcknowledged = true,
            privacyNoticeVersion = "2026-11-01",
        }, merve, Key());
        joined.Status.ShouldBe(201, joined.Body.ToString());
        (await _api.ScalarAsync<long>("SELECT count(*) FROM scheduling.waitlist_entries")).ShouldBe(1);

        (await _api.SendAsync(HttpMethod.Get, $"/api/v1/public/branches/{_merkez}/my-packages", null)).Status.ShouldBe(401);
        (await _api.SendAsync(HttpMethod.Get, $"/api/v1/public/branches/{_merkez}/my-packages", null, _owner)).Status.ShouldBe(401, "panel jetonu geçmez");
    }

    // ───────── Randevularım, detay, izolasyon ─────────

    [Fact]
    public async Task GetAppointment_Detail_WithETag_AndOtherCustomersAppointmentIs404Everywhere()
    {
        var merve = await SignInAsync(MervePhone);
        var nur = await SignInAsync(NurPhone);
        var id = (await BookAsync(merve, _fon, FridayAt1430)).Body.GetProperty("id").GetString()!;

        var detail = await _api.GetAsync($"/api/v1/me/appointments/{id}", merve);

        detail.Status.ShouldBe(200, detail.Body.ToString());
        detail.ETag.ShouldNotBeNullOrEmpty();
        detail.Body.GetProperty("canCancel").GetBoolean().ShouldBeTrue();
        detail.Body.GetProperty("cancelDeadlineUtc").GetString().ShouldBe("2026-11-26T11:30:00Z");
        detail.Body.GetProperty("rescheduleDeadlineUtc").GetString().ShouldBe("2026-11-26T23:30:00Z");
        detail.Body.GetProperty("deposit").ValueKind.ShouldBe(JsonValueKind.Null);
        detail.Body.GetProperty("cancellation").ValueKind.ShouldBe(JsonValueKind.Null);
        detail.Body.GetProperty("branch").GetProperty("addressLine").ValueKind.ShouldBe(JsonValueKind.String);
        detail.Body.GetProperty("total").GetProperty("amountMinor").GetInt64().ShouldBe(45000);

        (await _api.GetAsync($"/api/v1/me/appointments/{id}", nur)).Status.ShouldBe(404);
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/me/appointments/{id}/cancel", new { reason = "PlanChanged" }, nur, IfMatch(detail.ETag))).Status.ShouldBe(404);
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/me/appointments/{id}/availability/slots", new { date = "2026-11-28" }, nur)).Status.ShouldBe(404);
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/me/appointments/{id}/reschedule", new { startUtc = "2026-11-28T08:00:00Z" }, nur, IfMatch(detail.ETag))).Status.ShouldBe(404);
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/me/appointments/{id}/deposit/report", null, nur)).Status.ShouldBe(404);
        (await ListAsync(nur, "Upcoming")).Body.GetProperty("items").GetArrayLength().ShouldBe(0);
        (await _api.GetAsync($"/api/v1/me/appointments/{id}", _owner)).Status.ShouldBe(401, "panel jetonu me yüzeyinde geçmez");
        (await _api.GetAsync("/api/v1/me/appointments?scope=Later", merve)).Status.ShouldBe(400);
        (await _api.GetAsync("/api/v1/me/appointments", merve)).Status.ShouldBe(400);
        (await _api.GetAsync("/api/v1/me/appointments?scope=Upcoming&notReviewed=true", merve)).Status.ShouldBe(400, "notReviewed yalnız Past");
    }

    // ───────── İptal (A6) ─────────

    [Fact]
    public async Task Cancel_ReasonRequired_CancelsWithEvent_AndReturnsThePackageSession()
    {
        var card = (await PostAsync("/api/v1/manage/customers", new { fullName = "Merve Yıldız", phone = MervePhone })).Body.GetProperty("id").GetString()!;
        await SellFonPackageAsync(card);
        var merve = await SignInAsync(MervePhone);
        var usable = (await _api.SendAsync(HttpMethod.Get, $"/api/v1/public/branches/{_merkez}/my-packages", null, merve)).Body.GetProperty("items");
        var packageItem = usable.EnumerateArray().ShouldHaveSingleItem().GetProperty("customerPackageItemId").GetString()!;
        var id = (await BookAsync(merve, _fon, FridayAt1430, packageItem: packageItem)).Body.GetProperty("id").GetString()!;
        RemainingSessions(await _api.GetAsync("/api/v1/me/packages", merve)).ShouldBe(4);
        var etag = (await _api.GetAsync($"/api/v1/me/appointments/{id}", merve)).ETag;

        (await CancelAsync(merve, id, new { }, etag)).Status.ShouldBe(400);
        (await CancelAsync(merve, id, new { reason = "Other" }, etag)).Status.ShouldBe(400, "Diğer'de not zorunlu");
        var cancelled = await CancelAsync(merve, id, new { reason = "Sick" }, etag);

        cancelled.Status.ShouldBe(200, cancelled.Body.ToString());
        cancelled.Body.GetProperty("status").GetString().ShouldBe("Cancelled");
        cancelled.Body.GetProperty("cancellation").GetProperty("cancelledBy").GetString().ShouldBe("Customer");
        cancelled.Body.GetProperty("cancellation").GetProperty("reason").GetString().ShouldBe("Sick");
        cancelled.Body.GetProperty("canCancel").GetBoolean().ShouldBeFalse();
        (await _api.ScalarAsync<string>("SELECT payload::jsonb->>'reason' FROM infra.outbox_events WHERE type LIKE '%AppointmentCancelledByCustomer'")).ShouldBe("Sick");
        RemainingSessions(await _api.GetAsync("/api/v1/me/packages", merve)).ShouldBe(5, "iptalde seans geri döner");
        (await CancelAsync(merve, id, new { reason = "Sick" }, cancelled.ETag)).Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");
        var past = (await ListAsync(merve, "Past")).Body.GetProperty("items").EnumerateArray().ShouldHaveSingleItem();
        past.GetProperty("status").GetString().ShouldBe("Cancelled");
    }

    [Fact]
    public async Task Cancel_InsideCutoff_Is409CutoffPassed_AndStaleVersionIs412()
    {
        var merve = await SignInAsync(MervePhone);
        // Bugün 18:00 İstanbul: başlangıca 24 saatten az.
        var soon = (await BookAsync(merve, _fon, "2026-11-25T15:00:00Z")).Body.GetProperty("id").GetString()!;
        var later = (await BookAsync(merve, _fon, FridayAt1430)).Body.GetProperty("id").GetString()!;

        var detail = await _api.GetAsync($"/api/v1/me/appointments/{soon}", merve);
        detail.Body.GetProperty("canCancel").GetBoolean().ShouldBeFalse();
        var refused = await CancelAsync(merve, soon, new { reason = "PlanChanged" }, detail.ETag);
        refused.Status.ShouldBe(409);
        refused.Body.GetProperty("code").GetString().ShouldBe("cutoff_passed");

        (await CancelAsync(merve, later, new { reason = "PlanChanged" }, "W/\"1\"")).Status.ShouldBe(412);
        var upcoming = (await ListAsync(merve, "Upcoming")).Body.GetProperty("items");
        upcoming.GetArrayLength().ShouldBe(2);
        upcoming[0].GetProperty("id").GetString().ShouldBe(soon, "başlangıca göre artan");
    }

    // ───────── Saat değişikliği (A14) ─────────

    [Fact]
    public async Task Reschedule_SlotsThenMove_KeepsStaff_AndTakenSlotIs409()
    {
        var merve = await SignInAsync(MervePhone);
        var nur = await SignInAsync(NurPhone);
        var id = (await BookAsync(merve, _fon, FridayAt1430)).Body.GetProperty("id").GetString()!;
        (await BookAsync(nur, _fon, "2026-11-28T08:00:00Z")).Status.ShouldBe(201);

        var slots = await _api.SendAsync(HttpMethod.Post, $"/api/v1/me/appointments/{id}/availability/slots", new { date = "2026-11-28" }, merve);
        slots.Status.ShouldBe(200, slots.Body.ToString());
        var starts = slots.Body.GetProperty("slots").EnumerateArray().Select(s => s.GetProperty("startUtc").GetString()).ToList();
        starts.Contains("2026-11-28T08:00:00Z", StringComparer.Ordinal).ShouldBeFalse("Nur'un saati dolu");
        var etag = (await _api.GetAsync($"/api/v1/me/appointments/{id}", merve)).ETag;

        var taken = await _api.SendAsync(HttpMethod.Post, $"/api/v1/me/appointments/{id}/reschedule", new { startUtc = "2026-11-28T08:00:00Z" }, merve, IfMatch(etag));
        taken.Status.ShouldBe(409);
        taken.Body.GetProperty("code").GetString().ShouldBe("slot_taken");

        var moved = await _api.SendAsync(HttpMethod.Post, $"/api/v1/me/appointments/{id}/reschedule", new { startUtc = starts[0] }, merve, IfMatch(etag));
        moved.Status.ShouldBe(200, moved.Body.ToString());
        moved.Body.GetProperty("startUtc").GetString().ShouldBe(starts[0]);
        moved.Body.GetProperty("items")[0].GetProperty("staffId").GetString().ShouldBe(_elif);
        (await _api.ScalarAsync<string>("SELECT payload::jsonb->>'changedBy' FROM infra.outbox_events WHERE type LIKE '%AppointmentRescheduled'")).ShouldBe("Customer");
    }

    // ───────── Kapora ve Ödemelerim (dilim-04 KK4, KK9) ─────────

    [Fact]
    public async Task Deposit_ReportedFromApp_ShowsAwaitingConfirmation_ThenRecordedAfterSalonMarksIt()
    {
        (await _api.WriteAsync(HttpMethod.Put, BankPath, new
        {
            accountHolderName = "Elif Kaya",
            bankName = "Ziraat Bankası",
            iban = "TR29 0006 4000 0011 2345 6789 01",
            transferCodePrefix = "OMV",
            transferWindowMinutes = 120,
        }, _owner, "W/\"0\"")).Status.ShouldBe(200);
        var card = (await PostAsync("/api/v1/manage/customers", new { fullName = "Merve Yıldız", phone = MervePhone })).Body.GetProperty("id").GetString()!;
        await _api.ScalarAsync<int>($"UPDATE customers.brand_customers SET no_show_count = 2 WHERE id = '{card}' RETURNING 1");
        var merve = await SignInAsync(MervePhone);
        var nur = await SignInAsync(NurPhone);
        var id = (await BookAsync(merve, _balyaj, FridayAt1430)).Body.GetProperty("id").GetString()!;

        var detail = (await _api.GetAsync($"/api/v1/me/appointments/{id}", merve)).Body;
        detail.GetProperty("displayStatus").GetString().ShouldBe("DepositPending");
        detail.GetProperty("deposit").GetProperty("instructions").GetProperty("iban").GetString().ShouldBe("TR29 0006 4000 0011 2345 6789 01");
        (await _api.GetAsync("/api/v1/me/payments", merve)).Body.GetProperty("items").GetArrayLength().ShouldBe(0, "bildirilmemiş kapora listelenmez");

        var reported = await _api.SendAsync(HttpMethod.Post, $"/api/v1/me/appointments/{id}/deposit/report", null, merve);
        reported.Status.ShouldBe(200, reported.Body.ToString());
        reported.Body.GetProperty("deposit").GetProperty("status").GetString().ShouldBe("CustomerReported");
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/me/appointments/{id}/deposit/report", null, merve)).Status.ShouldBe(200, "kendiliğinden idempotent");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM infra.outbox_events WHERE type LIKE '%AppointmentDepositReported'")).ShouldBe(1);

        var awaiting = (await _api.GetAsync("/api/v1/me/payments", merve)).Body.GetProperty("items").EnumerateArray().ShouldHaveSingleItem();
        awaiting.GetProperty("id").GetString().ShouldBe(id);
        awaiting.GetProperty("kind").GetString().ShouldBe("Deposit");
        awaiting.GetProperty("status").GetString().ShouldBe("AwaitingConfirmation");
        awaiting.GetProperty("amount").GetProperty("amountMinor").GetInt64().ShouldBe(84000);
        awaiting.GetProperty("method").GetString().ShouldBe("Transfer");
        (await _api.GetAsync("/api/v1/me/payments", nur)).Body.GetProperty("items").GetArrayLength().ShouldBe(0);

        var etag = (await _api.GetAsync($"/api/v1/manage/appointments/{id}", _owner)).ETag!;
        var headers = new Dictionary<string, string>(Key(), StringComparer.Ordinal) { ["If-Match"] = etag };
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/deposit/mark-received", null, _owner, headers)).Status.ShouldBe(200);

        var recorded = (await _api.GetAsync("/api/v1/me/payments", merve)).Body.GetProperty("items").EnumerateArray().ShouldHaveSingleItem();
        recorded.GetProperty("status").GetString().ShouldBe("Recorded");
        recorded.GetProperty("appointmentId").GetString().ShouldBe(id);
        recorded.GetProperty("appointmentStartUtc").GetString().ShouldBe(FridayAt1430);
        recorded.GetProperty("serviceNames").EnumerateArray().Select(s => s.GetString()).ShouldBe(["Balyaj"]);
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/me/appointments/{id}/deposit/report", null, merve)).Body.GetProperty("code").GetString()
            .ShouldBe("invalid_state_transition", "kapora artık beklenmiyor");
    }

    [Fact]
    public async Task Packages_ListAllBrands_UsableFirst_RunningLow_AndPackagePurchaseInPayments()
    {
        var card = (await PostAsync("/api/v1/manage/customers", new { fullName = "Merve Yıldız", phone = MervePhone })).Body.GetProperty("id").GetString()!;
        await SellFonPackageAsync(card, sessions: 2);
        var merve = await SignInAsync(MervePhone);
        var nur = await SignInAsync(NurPhone);

        var list = (await _api.GetAsync("/api/v1/me/packages", merve)).Body;

        list.GetProperty("activeCount").GetInt32().ShouldBe(1);
        var package = list.GetProperty("items").EnumerateArray().ShouldHaveSingleItem();
        package.GetProperty("packageName").GetString().ShouldBe("Fön paketi");
        package.GetProperty("isUsable").GetBoolean().ShouldBeTrue();
        package.GetProperty("isRunningLow").GetBoolean().ShouldBeTrue("kalan 2 ≤ PackageLowSessionThreshold");
        package.GetProperty("finishedOn").ValueKind.ShouldBe(JsonValueKind.Null);
        package.GetProperty("items")[0].GetProperty("serviceName").GetString().ShouldBe("Fön");
        (await _api.GetAsync("/api/v1/me/packages", nur)).Body.GetProperty("items").GetArrayLength().ShouldBe(0);

        var payment = (await _api.GetAsync("/api/v1/me/payments", merve)).Body.GetProperty("items").EnumerateArray().ShouldHaveSingleItem();
        payment.GetProperty("kind").GetString().ShouldBe("PackagePurchase");
        payment.GetProperty("method").GetString().ShouldBe("Cash");
        payment.GetProperty("packageName").GetString().ShouldBe("Fön paketi");
        payment.GetProperty("packageSessionCount").GetInt32().ShouldBe(2);
        (await _api.GetAsync("/api/v1/me/payments?limit=0", merve)).Status.ShouldBe(400);
    }

    // ───────── Geçmiş, "Aynısını tekrar al" ve değerlendirme (KK2, KK8) ─────────

    [Fact]
    public async Task Criterion2And8_PastRow_CanRebook_NotReviewed_ReviewOnceOnlyWhenCompleted()
    {
        var merve = await SignInAsync(MervePhone);
        var nur = await SignInAsync(NurPhone);
        var done = (await BookAsync(merve, _fon, FridayAt1430)).Body.GetProperty("id").GetString()!;
        var open = (await BookAsync(merve, _fon, "2026-11-28T08:00:00Z")).Body.GetProperty("id").GetString()!;
        await _api.ScalarAsync<int>($"UPDATE scheduling.appointments SET status = 'Completed' WHERE id = '{done}' RETURNING 1");

        var past = (await ListAsync(merve, "Past", limit: 1)).Body;
        var row = past.GetProperty("items").EnumerateArray().ShouldHaveSingleItem();
        row.GetProperty("id").GetString().ShouldBe(done);
        row.GetProperty("canReview").GetBoolean().ShouldBeTrue();
        row.GetProperty("canRebook").GetBoolean().ShouldBeTrue();
        row.GetProperty("review").ValueKind.ShouldBe(JsonValueKind.Null);
        row.GetProperty("items")[0].GetProperty("serviceId").GetString().ShouldBe(_fon);
        (await ListAsync(merve, "Past", notReviewed: true)).Body.GetProperty("items").GetArrayLength().ShouldBe(1);

        (await ReviewAsync(nur, done, 5)).Status.ShouldBe(404, "başka müşterinin randevusu");
        var notCompleted = await ReviewAsync(merve, open, 5);
        notCompleted.Status.ShouldBe(409);
        notCompleted.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");
        (await ReviewAsync(merve, done, 6)).Status.ShouldBe(400);

        var created = await ReviewAsync(merve, done, 4, "Boya tam istediğim gibi oldu.");
        created.Status.ShouldBe(201, created.Body.ToString());
        created.Body.GetProperty("rating").GetInt32().ShouldBe(4);
        created.Body.GetProperty("appointmentId").GetString().ShouldBe(done);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM infra.outbox_events WHERE type LIKE '%ReviewReceived'")).ShouldBe(1);
        var again = await ReviewAsync(merve, done, 5);
        again.Status.ShouldBe(409);
        again.Body.GetProperty("code").GetString().ShouldBe("already_reviewed");

        var reviewed = (await ListAsync(merve, "Past")).Body.GetProperty("items")[0];
        reviewed.GetProperty("review").GetProperty("rating").GetInt32().ShouldBe(4);
        reviewed.GetProperty("canReview").GetBoolean().ShouldBeFalse();
        (await ListAsync(merve, "Past", notReviewed: true)).Body.GetProperty("items").GetArrayLength().ShouldBe(0);

        // Hizmet online randevuya kapanınca "Aynısını tekrar al" gösterilmez.
        await _api.ScalarAsync<int>($"UPDATE catalog.services SET is_online_bookable = false WHERE id = '{_fon}' RETURNING 1");
        (await ListAsync(merve, "Past")).Body.GetProperty("items")[0].GetProperty("canRebook").GetBoolean().ShouldBeFalse();
    }

    [Fact]
    public async Task ListAppointments_SpansBrands_AndPaginatesWithCursor()
    {
        var merve = await SignInAsync(MervePhone);
        await BookAsync(merve, _fon, FridayAt1430);
        var (_, otherBranch, otherStaff, otherFon) = await OtherBrandAsync();
        (await BookOnBranchAsync(merve, otherBranch, otherStaff, otherFon, "2026-11-28T08:00:00Z")).Status.ShouldBe(201);

        var first = (await ListAsync(merve, "Upcoming", limit: 1)).Body;
        first.GetProperty("items").GetArrayLength().ShouldBe(1);
        var cursor = first.GetProperty("nextCursor").GetString()!;
        var second = (await _api.GetAsync($"/api/v1/me/appointments?scope=Upcoming&limit=1&cursor={Uri.EscapeDataString(cursor)}", merve)).Body;
        second.GetProperty("items").GetArrayLength().ShouldBe(1);
        second.GetProperty("nextCursor").ValueKind.ShouldBe(JsonValueKind.Null);
        second.GetProperty("items")[0].GetProperty("brand").GetProperty("name").GetString().ShouldBe("Berber Can");
        (await _api.GetAsync("/api/v1/me/appointments?scope=Upcoming&cursor=bozuk!", merve)).Status.ShouldBe(400);
    }

    // ───────── İzinlerim ─────────

    [Fact]
    public async Task Consents_OnlyBookedBrands_ToggleAppendsLedgerOnce_AndUnknownBrandIs404()
    {
        var merve = await SignInAsync(MervePhone);
        await BookAsync(merve, _fon, FridayAt1430);
        await _api.SignupAsync(OtherOwnerPhone, "Berber Can");
        const string otherSlug = "berber-can";
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/me/brands", new { brandSlug = otherSlug, source = "Qr" }, merve)).Status.ShouldBe(200);

        var list = (await _api.GetAsync("/api/v1/me/consents", merve)).Body.GetProperty("items");
        var row = list.EnumerateArray().ShouldHaveSingleItem("yalnız kaydedilen salon listede yok");
        row.GetProperty("marketingGranted").GetBoolean().ShouldBeFalse();
        row.GetProperty("updatedUtc").ValueKind.ShouldBe(JsonValueKind.Null);
        var brandId = row.GetProperty("brand").GetProperty("id").GetString()!;

        var granted = await _api.SendAsync(HttpMethod.Put, "/api/v1/me/consents", new { brandId, granted = true }, merve);
        granted.Status.ShouldBe(200, granted.Body.ToString());
        granted.Body.GetProperty("items")[0].GetProperty("marketingGranted").GetBoolean().ShouldBeTrue();
        granted.Body.GetProperty("items")[0].GetProperty("updatedUtc").ValueKind.ShouldBe(JsonValueKind.String);
        (await MarketingRowsAsync()).ShouldBe(3, "SMS, e-posta ve push birlikte");
        (await _api.SendAsync(HttpMethod.Put, "/api/v1/me/consents", new { brandId, granted = true }, merve)).Status.ShouldBe(200);
        (await MarketingRowsAsync()).ShouldBe(3, "aynı değer yeni satır yazmaz");
        (await _api.SendAsync(HttpMethod.Put, "/api/v1/me/consents", new { brandId, granted = false }, merve)).Body.GetProperty("items")[0]
            .GetProperty("marketingGranted").GetBoolean().ShouldBeFalse();
        (await MarketingRowsAsync()).ShouldBe(6);
        (await _api.ScalarAsync<string>("SELECT DISTINCT channel FROM customers.consent_records WHERE consent_type LIKE 'Marketing%'")).ShouldBe("CustomerApp");

        var otherBrandId = (await _api.GetAsync("/api/v1/me/brands", merve)).Body.GetProperty("items").EnumerateArray()
            .Single(b => string.Equals(b.GetProperty("slug").GetString(), otherSlug, StringComparison.Ordinal)).GetProperty("id").GetString();
        (await _api.SendAsync(HttpMethod.Put, "/api/v1/me/consents", new { brandId = otherBrandId, granted = true }, merve)).Status.ShouldBe(404);
        (await _api.SendAsync(HttpMethod.Put, "/api/v1/me/consents", new { granted = true }, merve)).Status.ShouldBe(400);
    }

    // ───────── Hesap silme (KK9) ─────────

    [Fact]
    public async Task Criterion9_AccountDeletion_AnonymizesEveryBrand_KeepsRecords_CancelsUpcoming_AndSignInFindsANewAccount()
    {
        var card = (await PostAsync("/api/v1/manage/customers", new { fullName = "Merve Yıldız", phone = MervePhone })).Body.GetProperty("id").GetString()!;
        await SellFonPackageAsync(card);
        var signedIn = await SignInResponseAsync(MervePhone);
        var merve = signedIn.GetProperty("accessToken").GetString()!;
        var oldCustomerId = await _api.ScalarAsync<Guid>($"SELECT id FROM customers.customers WHERE phone_e164 = '{MervePhone}'");
        var mine = (await BookAsync(merve, _fon, FridayAt1430)).Body.GetProperty("id").GetString()!;
        var (_, otherBranch, otherStaff, otherFon) = await OtherBrandAsync();
        var theirs = (await BookOnBranchAsync(merve, otherBranch, otherStaff, otherFon, "2026-11-28T08:00:00Z")).Body.GetProperty("id").GetString()!;
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/me/devices", new { platform = "iOS", pushToken = "fcm-token-me-0123456789-abcdefghij", appVersion = "1.0.0" }, merve))
            .Status.ShouldBe(200);
        (await _api.SendAsync(HttpMethod.Put, "/api/v1/me/consents",
            new { brandId = await _api.ScalarAsync<Guid>($"SELECT brand_id FROM customers.brand_customers WHERE id = '{card}'"), granted = true }, merve)).Status.ShouldBe(200);

        _factory.Clock.Advance(Duration.FromMinutes(2));
        var otp = await _api.SendAsync(HttpMethod.Post, "/api/v1/me/account/deletion/otp", null, merve);
        otp.Status.ShouldBe(202, otp.Body.ToString());
        (await _api.ScalarAsync<string>("SELECT purpose FROM identity.otp_challenges ORDER BY created_utc DESC LIMIT 1")).ShouldBe("AccountDeletion");
        var otpId = otp.Body.GetProperty("otpId").GetGuid();
        var wrong = await _api.SendAsync(HttpMethod.Post, "/api/v1/me/account/deletion", new { otpId, code = "000000" }, merve);
        wrong.Status.ShouldBe(422);
        (await _api.ScalarAsync<long>($"SELECT count(*) FROM customers.customers WHERE phone_e164 = '{MervePhone}'")).ShouldBe(1);

        var deleted = await _api.SendAsync(HttpMethod.Post, "/api/v1/me/account/deletion", new { otpId, code = await LatestCodeAsync() }, merve);

        deleted.Status.ShouldBe(200, deleted.Body.ToString());
        deleted.Body.GetProperty("cancelledAppointmentCount").GetInt32().ShouldBe(2);
        await ShouldBeErasedAsync(oldCustomerId, mine, theirs);

        (await _api.GetAsync("/api/v1/me/profile", merve)).Status.ShouldBe(401);
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/me/auth/refresh", new { refreshToken = signedIn.GetProperty("refreshToken").GetString() })).Status.ShouldBe(401);
        var again = await SignInResponseAsync(MervePhone);
        again.GetProperty("isNewAccount").GetBoolean().ShouldBeTrue("giriş eski kimliği bulmaz");
        var fresh = again.GetProperty("accessToken").GetString()!;
        (await _api.ScalarAsync<Guid>($"SELECT id FROM customers.customers WHERE phone_e164 = '{MervePhone}'")).ShouldNotBe(oldCustomerId);
        (await ListAsync(fresh, "Past")).Body.GetProperty("items").GetArrayLength().ShouldBe(0);
        (await _api.GetAsync("/api/v1/me/brands", fresh)).Body.GetProperty("items").GetArrayLength().ShouldBe(0);
        (await _api.GetAsync("/api/v1/me/packages", fresh)).Body.GetProperty("items").GetArrayLength().ShouldBe(0);
    }

    // ───────── Yardımcılar ─────────

    /// <summary>KK9: bütün markalarda anonim, randevu ve ödeme kayıtları kalır, cihaz ve kayıtlı markalar silinir, denetim yazılır.</summary>
    private async Task ShouldBeErasedAsync(Guid oldCustomerId, string mine, string theirs)
    {
        (await _api.ScalarAsync<string>($"SELECT full_name || ':' || has_app_account || ':' || (phone_e164 LIKE '+000%') FROM customers.customers WHERE id = '{oldCustomerId}'"))
            .ShouldBe("Silinmiş müşteri:false:true");
        (await _api.ScalarAsync<long>($"SELECT count(*) FROM customers.customers WHERE phone_e164 = '{MervePhone}'")).ShouldBe(0);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM customers.brand_customers WHERE anonymized_utc IS NOT NULL AND display_name = 'Silinmiş müşteri'")).ShouldBe(2, "bütün markalarda");
        (await _api.ScalarAsync<long>($"SELECT count(*) FROM customers.brand_customers WHERE customer_id = '{oldCustomerId}'")).ShouldBe(0);
        (await _api.ScalarAsync<string>($"SELECT string_agg(status || '/' || cancellation_reason, ',' ORDER BY start_utc) FROM scheduling.appointments WHERE id IN ('{mine}', '{theirs}')"))
            .ShouldBe("Cancelled/AccountDeleted,Cancelled/AccountDeleted", "randevu kayıtları kalır");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM sales.payments")).ShouldBe(1, "ödeme kayıtları kalır");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM infra.outbox_events WHERE type LIKE '%AppointmentCancelledByCustomer'")).ShouldBe(2);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.devices")).ShouldBe(0);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM customers.customer_saved_brands")).ShouldBe(0);
        (await _api.ScalarAsync<string>("SELECT requested_via || '/' || status || '/' || (brand_id IS NULL) FROM customers.data_deletion_requests")).ShouldBe("CustomerApp/Completed/true");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM platform.audit_logs WHERE action = 'Anonymize'")).ShouldBe(3, "iki marka + platform geneli");
        (await _api.ScalarAsync<bool>(
            "SELECT is_granted FROM customers.consent_records WHERE consent_type = 'MarketingSms' ORDER BY occurred_utc DESC, id DESC LIMIT 1")).ShouldBeFalse("kampanya izni geri çekilir");
    }


    private async Task<JsonElement> SignInResponseAsync(string phone)
    {
        _factory.Clock.Advance(Duration.FromMinutes(2));
        var otp = await _api.SendAsync(HttpMethod.Post, "/api/v1/me/auth/otp/request", new { phone });
        otp.Status.ShouldBe(202, otp.Body.ToString());
        var verified = await _api.SendAsync(HttpMethod.Post, "/api/v1/me/auth/otp/verify", new { otpId = otp.Body.GetProperty("otpId").GetGuid(), code = await LatestCodeAsync() });
        verified.Status.ShouldBe(200, verified.Body.ToString());
        return verified.Body;
    }

    private async Task<string> SignInAsync(string phone) => (await SignInResponseAsync(phone)).GetProperty("accessToken").GetString()!;

    private Task<string> LatestCodeAsync() =>
        _api.ScalarAsync<string>("SELECT payload->>'kod' FROM notifications.notification_outbox WHERE event_code = 'otp.code' ORDER BY created_utc DESC, id DESC LIMIT 1");

    private Task<ManageApi.Response> BookAsync(string token, string serviceId, string startUtc, string fullName = "Merve Yıldız", string? packageItem = null) =>
        BookOnBranchAsync(token, _merkez, _elif, serviceId, startUtc, fullName, packageItem);

    private Task<ManageApi.Response> BookOnBranchAsync(
        string token, string branchId, string staffId, string serviceId, string startUtc, string fullName = "Merve Yıldız", string? packageItem = null)
    {
        var headers = new Dictionary<string, string>(Key(), StringComparer.Ordinal) { ["X-Client"] = "customer-app" };
        return _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{branchId}/appointments", new
        {
            items = new[] { new { serviceId, staffId } },
            sameStaffForAll = true,
            startUtc,
            fullName,
            usePackageItems = packageItem is null ? null : new[] { new { serviceId, customerPackageItemId = packageItem } },
            privacyNoticeAcknowledged = true,
            privacyNoticeVersion = "2026-11-01",
            marketingConsent = false,
        }, token, headers);
    }

    private Task<ManageApi.Response> ListAsync(string token, string scope, int? limit = null, bool notReviewed = false) =>
        _api.GetAsync($"/api/v1/me/appointments?scope={scope}{(limit is { } l ? $"&limit={l}" : string.Empty)}{(notReviewed ? "&notReviewed=true" : string.Empty)}", token);

    private Task<ManageApi.Response> CancelAsync(string token, string id, object body, string? etag) =>
        _api.SendAsync(HttpMethod.Post, $"/api/v1/me/appointments/{id}/cancel", body, token, IfMatch(etag));

    private Task<ManageApi.Response> ReviewAsync(string token, string appointmentId, int rating, string? comment = null) =>
        _api.SendAsync(HttpMethod.Post, "/api/v1/me/reviews", new { appointmentId, rating, comment }, token, Key());

    private Task<long> MarketingRowsAsync() => _api.ScalarAsync<long>("SELECT count(*) FROM customers.consent_records WHERE consent_type LIKE 'Marketing%'");

    private static int RemainingSessions(ManageApi.Response packages) =>
        packages.Body.GetProperty("items").EnumerateArray().ShouldHaveSingleItem().GetProperty("remainingSessions").GetInt32();

    private async Task SellFonPackageAsync(string card, int sessions = 5)
    {
        var package = await PostAsync("/api/v1/manage/packages", new
        {
            name = "Fön paketi",
            price = new { amountMinor = 150000, currency = "TRY" },
            items = new[] { new { serviceId = _fon, sessionCount = sessions } },
            validityDays = 90,
            isAvailableAllBranches = true,
            isActive = true,
        });
        package.Status.ShouldBe(201, package.Body.ToString());
        var sold = await PostAsync($"/api/v1/manage/customers/{card}/packages",
            new { packageId = package.Body.GetProperty("id").GetString(), branchId = _merkez, soldByStaffId = _elif, paymentMethod = "Cash" });
        sold.Status.ShouldBe(201, sold.Body.ToString());
    }

    /// <summary>İkinci marka (Berber Can): açık hafta, online "Fön".</summary>
    private async Task<(string Owner, string Branch, string Staff, string Fon)> OtherBrandAsync()
    {
        var owner = await _api.SignupAsync(OtherOwnerPhone, "Berber Can");
        var session = (await _api.GetAsync("/api/v1/manage/session", owner)).Body;
        var branch = session.GetProperty("branches")[0].GetProperty("id").GetString()!;
        var staff = session.GetProperty("membership").GetProperty("staffId").GetString()!;
        await OpenWeekAsync(owner, branch, staff);
        var category = (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/service-categories", new { name = "Saç" }, owner, Key())).Body.GetProperty("id").GetString()!;
        return (owner, branch, staff, await ServiceAsync(owner, category, "Fön", 30000, staff));
    }

    private async Task OpenWeekAsync(string owner, string branch, string staff)
    {
        var week = new
        {
            days = Enumerable.Range(1, 7).Select(d => new { dayOfWeek = d, intervals = new[] { new { start = "09:00", end = "19:00" } } }).ToArray(),
        };
        var hours = await _api.GetAsync($"/api/v1/manage/branches/{branch}/working-hours", owner);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/branches/{branch}/working-hours", week, owner, hours.ETag)).Status.ShouldBe(200);
        var member = await _api.GetAsync($"/api/v1/manage/staff/{staff}", owner);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{staff}/branches/{branch}/working-hours", week, owner, member.ETag)).Status.ShouldBe(200);
    }

    private async Task<string> ServiceAsync(string owner, string category, string name, long price, string staff, int? depositPercent = null)
    {
        var created = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/services", new
        {
            categoryId = category,
            name,
            durationMinutes = 45,
            price = new { amountMinor = price, currency = "TRY" },
            priceDisplay = "Fixed",
            deposit = depositPercent is { } p ? (object)new { mode = "Inherit", percent = p } : new { mode = "Inherit" },
            isOnlineBookable = true,
            staffIds = new[] { staff },
        }, owner, Key());
        created.Status.ShouldBe(201, created.Body.ToString());
        return created.Body.GetProperty("id").GetString()!;
    }

    private Task<ManageApi.Response> PostAsync(string path, object body) => _api.SendAsync(HttpMethod.Post, path, body, _owner, Key());

    private static Dictionary<string, string>? IfMatch(string? etag) =>
        etag is null ? null : new Dictionary<string, string>(StringComparer.Ordinal) { ["If-Match"] = etag };

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _requests):D12}" };
}
