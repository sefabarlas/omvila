using System.Text.Json;
using NodaTime;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-05 kabul kriteri 3 (iptal edilen 16:00 → Merve'ye teklif, push → SMS, 15 dk tutma; kabul → randevu; süre
// dolar → sıradakine, Merve listede kalır); durum-makinesi.md § 3 bekleme listesi ve A4–A7 "bekleme listesi tetiklenir";
// musaitlik.md § 4 ve M18 (teklifin tuttuğu saat başkasına kapalı); bildirimler.md waitlist.offer; roller-yetkiler.md
// waitlist.manage (personel yok) ve erişim kodu kuralı; sözleşme manage-waitlist, public-waitlist; T-86 (5), T-90.
// Saat: 2026-11-25 09:00Z (Çarşamba 12:00 İstanbul). Boşalan saat Perşembe 26 Kasım 16:00 (13:00Z), Fön 45 dk.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class WaitlistTests(PostgresFixture db) : IAsyncLifetime
{
    private const string Thursday1600 = "2026-11-26T13:00:00Z";
    private const string Thursday = "2026-11-26";
    private const string MervePhone = "+905551112233";
    private const string NurPhone = "+905552223344";
    private const string OtherPhone = "+905553334455";
    private const string DispatchSmsJob = "Omvila.Modules.Notifications.Jobs.DispatchNotifications, Omvila.Modules.Notifications";
    private const string ExpireOffersJob = "Omvila.Modules.Scheduling.Jobs.ExpireWaitlistOffers, Omvila.Modules.Scheduling";
    private const string ExpireEntriesJob = "Omvila.Modules.Scheduling.Jobs.ExpireWaitlistEntries, Omvila.Modules.Scheduling";
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private string _merkez = null!;
    private string _elif = null!;
    private string _zeynep = null!;
    private string _elifName = null!;
    private string _fon = null!;
    private string _selin = null!;
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
        await _api.ScalarAsync<long>(
            $"WITH d AS (DELETE FROM infra.dev_sms_log WHERE phone_e164 IN ('{MervePhone}', '{NurPhone}', '{OtherPhone}') RETURNING 1) SELECT count(*) FROM d");
        _token = await _api.SignupAsync("+905324187705", "Kuaför Elif");
        var session = (await _api.GetAsync("/api/v1/manage/session", _token)).Body;
        _merkez = session.GetProperty("branches")[0].GetProperty("id").GetString()!;
        _elif = session.GetProperty("membership").GetProperty("staffId").GetString()!;
        _elifName = (await _api.GetAsync($"/api/v1/manage/staff/{_elif}", _token)).Body.GetProperty("fullName").GetString()!;
        _zeynep = (await PostAsync("/api/v1/manage/staff",
            new { fullName = "Zeynep Aksoy", phone = "+905382741905", role = "Staff", calendarColor = "#2F5D8A", branchIds = new[] { _merkez } })).Body.GetProperty("id").GetString()!;

        var hours = await _api.GetAsync($"/api/v1/manage/branches/{_merkez}/working-hours", _token);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/branches/{_merkez}/working-hours", Week(), _token, hours.ETag)).Status.ShouldBe(200);
        foreach (var staff in new[] { _elif, _zeynep })
        {
            var current = await _api.GetAsync($"/api/v1/manage/staff/{staff}", _token);
            (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{staff}/branches/{_merkez}/working-hours", Week(), _token, current.ETag)).Status.ShouldBe(200);
        }

        var category = (await PostAsync("/api/v1/manage/service-categories", new { name = "Saç" })).Body.GetProperty("id").GetString()!;
        _fon = (await PostAsync("/api/v1/manage/services", new
        {
            categoryId = category,
            name = "Fön",
            durationMinutes = 45,
            price = new { amountMinor = 45000, currency = "TRY" },
            priceDisplay = "Fixed",
            deposit = new { mode = "Inherit" },
            isOnlineBookable = true,
            staffIds = new[] { _elif, _zeynep },
        })).Body.GetProperty("id").GetString()!;
        _selin = (await PostAsync("/api/v1/manage/customers", new { fullName = "Selin Öztürk", phone = "+905550004418" })).Body.GetProperty("id").GetString()!;
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    // ───────── Kabul kriteri 3 ─────────

    [Fact]
    public async Task Criterion3_CancelledSlot_OfferedToMerve_HeldFor15Minutes_AcceptCreatesAppointment()
    {
        var appointment = await BookAsync(_elif, Thursday1600);
        var merve = await JoinAsync(MervePhone, "Merve Yıldız", Preferences(preferredStaffId: _elif, timeWindows: ["Afternoon"]));
        merve.Status.ShouldBe(201, merve.Body.ToString());
        merve.Body.GetProperty("status").GetString().ShouldBe("Waiting");
        merve.Body.GetProperty("serviceNames").EnumerateArray().Select(s => s.GetString()).ShouldBe(["Fön"]);
        merve.Body.GetProperty("preferredStaffName").GetString().ShouldBe(_elifName);

        (await CancelAsync(appointment)).Status.ShouldBe(200);
        await DispatchAsync();

        var code = await PendingOfferCodeAsync(EntryId(merve));
        var offer = await GetOfferAsync(code);
        offer.Status.ShouldBe(200, offer.Body.ToString());
        offer.ETag.ShouldNotBeNull();
        offer.Body.GetProperty("status").GetString().ShouldBe("Pending");
        offer.Body.GetProperty("startUtc").GetString().ShouldBe(Thursday1600);
        offer.Body.GetProperty("endUtc").GetString().ShouldBe("2026-11-26T13:45:00Z");
        offer.Body.GetProperty("holdExpiresUtc").GetString().ShouldBe(Iso(_factory.Clock.GetCurrentInstant() + Duration.FromMinutes(15)));
        offer.Body.GetProperty("customerFirstName").GetString().ShouldBe("Merve");
        offer.Body.GetProperty("brand").GetProperty("name").GetString().ShouldBe("Kuaför Elif");
        offer.Body.GetProperty("items")[0].GetProperty("serviceName").GetString().ShouldBe("Fön");
        offer.Body.GetProperty("items")[0].GetProperty("staffName").GetString().ShouldBe(_elifName);
        offer.Body.GetProperty("total").GetProperty("amountMinor").GetInt64().ShouldBe(45000);
        offer.Body.GetProperty("requiresApproval").GetBoolean().ShouldBeFalse();
        offer.Body.GetProperty("waitlistAccessCode").GetString().ShouldBe(merve.Body.GetProperty("accessCode").GetString());

        // waitlist.offer: push → SMS planı, sessiz saate kaydırılmaz; cihaz yok → SMS.
        (await _api.ScalarAsync<string>(
            "SELECT array_to_string(channel_plan, ',') || '|' || status FROM notifications.notification_outbox WHERE event_code = 'waitlist.offer'"))
            .ShouldBe("Push,Sms|Sent");
        (await _api.ScalarAsync<string>($"SELECT body FROM infra.dev_sms_log WHERE phone_e164 = '{MervePhone}' AND body LIKE '%boşaldı%'"))
            .ShouldBe($"Kuaför Elif: Beklediğiniz Per 26 Kas 16:00 boşaldı. 15 dk içinde onaylarsanız sizin: omvila.com/b/{code}");

        var accepted = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/waitlist-offers/{code}/accept", null, headers: IfMatch(offer.ETag));

        accepted.Status.ShouldBe(200, accepted.Body.ToString());
        accepted.Body.GetProperty("status").GetString().ShouldBe("Confirmed");
        accepted.Body.GetProperty("startUtc").GetString().ShouldBe(Thursday1600);
        (await EntryStatusAsync(EntryId(merve))).ShouldBe("Converted");
        (await _api.ScalarAsync<string>("SELECT channel FROM scheduling.appointments WHERE status = 'Confirmed'")).ShouldBe("Online");
        var after = await GetOfferAsync(code);
        after.Body.GetProperty("status").GetString().ShouldBe("Accepted");
        after.Body.GetProperty("appointmentAccessCode").GetString().ShouldBe(accepted.Body.GetProperty("accessCode").GetString());

        var again = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/waitlist-offers/{code}/accept", null, headers: IfMatch(offer.ETag));
        again.Status.ShouldBe(200, "kendiliğinden idempotent");
        again.Body.GetProperty("id").GetString().ShouldBe(accepted.Body.GetProperty("id").GetString());
    }

    [Fact]
    public async Task Criterion3_OfferExpires_NextInLineGetsOffer_MerveStaysOnList()
    {
        var appointment = await BookAsync(_elif, Thursday1600);
        var merve = await JoinAsync(MervePhone, "Merve Yıldız", Preferences());
        _factory.Clock.Advance(Duration.FromMinutes(1));
        var nur = await JoinAsync(NurPhone, "Nur Ateş", Preferences());
        await CancelAsync(appointment);
        await DispatchAsync();
        var merveCode = await PendingOfferCodeAsync(EntryId(merve));
        (await PendingOfferCountAsync()).ShouldBe(1, "yalnız ilk kişiye teklif gider (FIFO)");

        _factory.Clock.Advance(Duration.FromMinutes(15));
        await _api.RunJobAsync(ExpireOffersJob);
        await DispatchAsync();

        (await GetOfferAsync(merveCode)).Body.GetProperty("status").GetString().ShouldBe("Expired");
        (await EntryStatusAsync(EntryId(merve))).ShouldBe("Waiting", "kişi listede kalır");
        var nurCode = await PendingOfferCodeAsync(EntryId(nur));
        (await GetOfferAsync(nurCode)).Body.GetProperty("startUtc").GetString().ShouldBe(Thursday1600);
        var late = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/waitlist-offers/{merveCode}/accept", null, headers: IfMatch((await GetOfferAsync(merveCode)).ETag));
        late.Status.ShouldBe(409);
        late.Body.GetProperty("code").GetString().ShouldBe("hold_expired");
    }

    // W4: onay isteyen salonda kabul A1 online kurallarıyla Onay bekliyor oluşturur.
    [Fact]
    public async Task Accept_InApprovalSalon_CreatesPendingApproval_RequestedEvent()
    {
        await _api.ScalarAsync<int>("UPDATE tenancy.brand_settings SET requires_approval = true RETURNING 1");
        var appointment = await BookAsync(_elif, Thursday1600);
        var merve = await JoinAsync(MervePhone, "Merve Yıldız", Preferences());
        await CancelAsync(appointment);
        await DispatchAsync();
        var code = await PendingOfferCodeAsync(EntryId(merve));
        var offer = await GetOfferAsync(code);
        offer.Body.GetProperty("requiresApproval").GetBoolean().ShouldBeTrue();

        var accepted = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/waitlist-offers/{code}/accept", null, headers: IfMatch(offer.ETag));

        accepted.Status.ShouldBe(200, accepted.Body.ToString());
        accepted.Body.GetProperty("status").GetString().ShouldBe("PendingApproval");
        accepted.Body.GetProperty("holdExpiresUtc").GetString().ShouldNotBeNullOrEmpty();
        (await _api.ScalarAsync<long>("SELECT count(*) FROM infra.outbox_events WHERE type LIKE '%.AppointmentRequested'")).ShouldBe(1);
        (await EntryStatusAsync(EntryId(merve))).ShouldBe("Converted");
    }

    [Fact]
    public async Task HeldSlot_IsClosedToOthers_OpenToOfferee()
    {
        var appointment = await BookAsync(_elif, Thursday1600);
        await JoinAsync(MervePhone, "Merve Yıldız", Preferences(preferredStaffId: _elif));
        await CancelAsync(appointment);
        await DispatchAsync();

        var other = await PublicBookAsync(OtherPhone, "Ece Koç", _elif, Thursday1600);
        other.Status.ShouldBe(409, other.Body.ToString());
        other.Body.GetProperty("code").GetString().ShouldBe("slot_taken");
        var slots = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/availability/slots",
            new { items = new[] { new { serviceId = _fon, staffId = _elif } }, sameStaffForAll = true, date = Thursday });
        slots.Body.GetProperty("slots").EnumerateArray()
            .Any(x => string.Equals(x.GetProperty("startUtc").GetString(), Thursday1600, StringComparison.Ordinal))
            .ShouldBeFalse("teklif süresince saat online gösterilmez");

        _factory.Clock.Advance(Duration.FromMinutes(15));
        (await PublicBookAsync(OtherPhone, "Ece Koç", _elif, Thursday1600)).Status.ShouldBe(201, "tutma bitince saat boşalır");
    }

    [Fact]
    public async Task Decline_OffersToNext_DeclinerStaysWaiting_SameSlotNotReofferedToDecliner()
    {
        var appointment = await BookAsync(_elif, Thursday1600);
        var merve = await JoinAsync(MervePhone, "Merve Yıldız", Preferences());
        _factory.Clock.Advance(Duration.FromMinutes(1));
        var nur = await JoinAsync(NurPhone, "Nur Ateş", Preferences());
        await CancelAsync(appointment);
        await DispatchAsync();
        var code = await PendingOfferCodeAsync(EntryId(merve));
        var offer = await GetOfferAsync(code);

        var declined = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/waitlist-offers/{code}/decline", null, headers: IfMatch(offer.ETag));

        declined.Status.ShouldBe(200, declined.Body.ToString());
        declined.Body.GetProperty("status").GetString().ShouldBe("Declined");
        (await EntryStatusAsync(EntryId(merve))).ShouldBe("Waiting");
        (await PendingOfferCodeAsync(EntryId(nur))).ShouldNotBeNullOrEmpty();
        var twice = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/waitlist-offers/{code}/decline", null, headers: IfMatch(offer.ETag));
        twice.Status.ShouldBe(200, "zaten reddedildi: aynı sonuç");
        var accept = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/waitlist-offers/{code}/accept", null, headers: IfMatch(declined.ETag));
        accept.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");
    }

    [Fact]
    public async Task Eligibility_ByStaffDayAndWindow_FifoAmongMatching()
    {
        var appointment = await BookAsync(_elif, Thursday1600);
        var morning = await JoinAsync(MervePhone, "Merve Yıldız", Preferences(timeWindows: ["Morning"]));
        _factory.Clock.Advance(Duration.FromMinutes(1));
        var zeynepOnly = await JoinAsync(NurPhone, "Nur Ateş", Preferences(preferredStaffId: _zeynep));
        _factory.Clock.Advance(Duration.FromMinutes(1));
        var otherDay = await JoinAsync(OtherPhone, "Ece Koç", Preferences(dates: ["2026-11-27"]));
        var anyStaff = await AddAsync(new { fullName = "Derya Şahin", phone = "+905554445566" }, Preferences(preferredStaffId: _zeynep, acceptOtherStaff: true, dates: []));
        anyStaff.Status.ShouldBe(201, anyStaff.Body.ToString());

        await CancelAsync(appointment);
        await DispatchAsync();

        (await PendingOfferCountAsync()).ShouldBe(1);
        (await PendingOfferCodeAsync(anyStaff.Body.GetProperty("id").GetString()!)).ShouldNotBeNullOrEmpty("başka personel de olur, her gün, her saat");
        foreach (var skipped in new[] { morning, zeynepOnly, otherDay })
        {
            (await _api.ScalarAsync<long>($"SELECT count(*) FROM scheduling.waitlist_offers WHERE waitlist_entry_id = '{EntryId(skipped)}'")).ShouldBe(0);
        }
    }

    [Fact]
    public async Task SalonCancel_WithOfferToWaitlistFalse_DoesNotOffer()
    {
        var appointment = await BookAsync(_elif, Thursday1600);
        await JoinAsync(MervePhone, "Merve Yıldız", Preferences());

        (await CancelAsync(appointment, offerToWaitlist: false)).Status.ShouldBe(200);
        await DispatchAsync();

        (await _api.ScalarAsync<long>("SELECT count(*) FROM scheduling.waitlist_offers")).ShouldBe(0);
    }

    [Fact]
    public async Task CustomerCancelFromLink_TriggersOffer()
    {
        var booked = await PublicBookAsync(OtherPhone, "Ece Koç", _elif, Thursday1600);
        var merve = await JoinAsync(MervePhone, "Merve Yıldız", Preferences());
        var code = booked.Body.GetProperty("accessCode").GetString()!;

        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/appointments/{code}/cancel", new { reason = "PlanChanged" }, headers: IfMatch(booked.ETag)))
            .Status.ShouldBe(200);
        await DispatchAsync();

        (await PendingOfferCodeAsync(EntryId(merve))).ShouldNotBeNullOrEmpty("A6 bekleme listesini tetikler");
    }

    // ───────── Panel ─────────

    [Fact]
    public async Task Panel_ListOverview_OfferSlot_Withdraw_SlotFreedWithoutAutoOffer()
    {
        var nur = await AddAsync(new { fullName = "Nur Ateş", phone = NurPhone }, Preferences(preferredStaffId: _elif), note: "Sabah erken arayın");
        nur.Status.ShouldBe(201, nur.Body.ToString());
        nur.ETag.ShouldNotBeNull();
        nur.Body.GetProperty("source").GetString().ShouldBe("Panel");
        nur.Body.GetProperty("note").GetString().ShouldBe("Sabah erken arayın");
        nur.Body.GetProperty("pendingOffer").ValueKind.ShouldBe(JsonValueKind.Null);
        _factory.Clock.Advance(Duration.FromMinutes(1));
        var merve = await JoinAsync(MervePhone, "Merve Yıldız", Preferences());

        var list = await _api.GetAsync($"/api/v1/manage/waitlist?branchId={_merkez}", _token);
        list.Status.ShouldBe(200, list.Body.ToString());
        list.Body.GetProperty("items").EnumerateArray().Select(i => i.GetProperty("customer").GetProperty("fullName").GetString()).ShouldBe(["Nur Ateş", "Merve Yıldız"]);
        list.Body.GetProperty("overview").GetProperty("waitingCount").GetInt32().ShouldBe(2);
        list.Body.GetProperty("overview").GetProperty("offerHoldMinutes").GetInt32().ShouldBe(15);
        list.Body.GetProperty("overview").GetProperty("convertedThisMonth").GetInt32().ShouldBe(0);

        var id = nur.Body.GetProperty("id").GetString()!;
        var offered = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/waitlist/{id}/offer", new { startUtc = Thursday1600, staffId = _elif }, _token, IfMatch(nur.ETag));
        offered.Status.ShouldBe(200, offered.Body.ToString());
        offered.Body.GetProperty("pendingOffer").GetProperty("status").GetString().ShouldBe("Pending");
        offered.Body.GetProperty("pendingOffer").GetProperty("staffName").GetString().ShouldBe(_elifName);
        var twice = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/waitlist/{id}/offer", new { startUtc = "2026-11-26T14:00:00Z", staffId = _elif }, _token, IfMatch(offered.ETag));
        twice.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/waitlist/{id}/offer/withdraw", null, _token, IfMatch(nur.ETag))).Status.ShouldBe(412);

        var withdrawn = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/waitlist/{id}/offer/withdraw", null, _token, IfMatch(offered.ETag));

        withdrawn.Status.ShouldBe(200, withdrawn.Body.ToString());
        withdrawn.Body.GetProperty("pendingOffer").ValueKind.ShouldBe(JsonValueKind.Null);
        withdrawn.Body.GetProperty("status").GetString().ShouldBe("Waiting");
        (await _api.ScalarAsync<long>($"SELECT count(*) FROM scheduling.waitlist_offers WHERE waitlist_entry_id = '{EntryId(merve)}'")).ShouldBe(0, "geri alınan saat sıradakine gitmez");
        (await PublicBookAsync(OtherPhone, "Ece Koç", _elif, Thursday1600)).Status.ShouldBe(201, "saat yeniden online");
        var again = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/waitlist/{id}/offer/withdraw", null, _token, IfMatch(withdrawn.ETag));
        again.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");
        await DispatchAsync();
        (await _api.ScalarAsync<long>($"SELECT count(*) FROM infra.dev_sms_log WHERE phone_e164 = '{NurPhone}' AND body LIKE '%boşaldı%'")).ShouldBe(1, "geri almada müşteriye bildirim yok");
    }

    [Fact]
    public async Task Panel_OfferSlot_TakenSlot_IsSlotTaken_NoCredits_IsInsufficientCredits()
    {
        await BookAsync(_elif, Thursday1600);
        var nur = await AddAsync(new { fullName = "Nur Ateş", phone = NurPhone }, Preferences());
        var id = nur.Body.GetProperty("id").GetString()!;

        var taken = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/waitlist/{id}/offer", new { startUtc = Thursday1600, staffId = _elif }, _token, IfMatch(nur.ETag));
        taken.Status.ShouldBe(409, taken.Body.ToString());
        taken.Body.GetProperty("code").GetString().ShouldBe("slot_taken");

        await _api.ScalarAsync<int>("DELETE FROM notifications.credit_transactions RETURNING 1");
        var broke = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/waitlist/{id}/offer", new { startUtc = "2026-11-26T14:00:00Z", staffId = _elif }, _token, IfMatch(nur.ETag));
        broke.Status.ShouldBe(409, broke.Body.ToString());
        broke.Body.GetProperty("code").GetString().ShouldBe("insufficient_credits");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM scheduling.waitlist_offers")).ShouldBe(0);
    }

    [Fact]
    public async Task Panel_Add_ExistingPhone_IsPhoneAlreadyExistsWithCustomerId()
    {
        var added = await AddAsync(new { fullName = "Selin Öztürk", phone = "+905550004418" }, Preferences());

        added.Status.ShouldBe(409, added.Body.ToString());
        added.Body.GetProperty("code").GetString().ShouldBe("phone_already_exists");
        added.Body.GetProperty("meta").GetProperty("brandCustomerId").GetString().ShouldBe(_selin);
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/waitlist", new { branchId = _merkez, brandCustomerId = _selin, preferences = Fill(Preferences()) }, _token, Key()))
            .Status.ShouldBe(201);
    }

    [Fact]
    public async Task Permissions_StaffIs403_ManagerOfOtherBranchIs404()
    {
        var staff = await _api.AddMemberAndSignInAsync(_token, "+905382741977", "Staff", _merkez);
        (await _api.GetAsync($"/api/v1/manage/waitlist?branchId={_merkez}", staff)).Status.ShouldBe(403);
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/waitlist", new { branchId = _merkez, brandCustomerId = _selin, preferences = Preferences() }, staff, Key()))
            .Status.ShouldBe(403);

        var otherBranch = (await PostAsync("/api/v1/manage/branches", new { name = "Moda", addressLine = "Moda Cad. 1", district = "Kadıköy", city = "İstanbul" })).Body;
        var otherId = otherBranch.GetProperty("id").GetString()!;
        var manager = await _api.AddMemberAndSignInAsync(_token, "+905382741978", "BranchManager", otherId);
        (await _api.GetAsync($"/api/v1/manage/waitlist?branchId={_merkez}", manager)).Status.ShouldBe(404);
        var nur = await AddAsync(new { fullName = "Nur Ateş", phone = NurPhone }, Preferences());
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/waitlist/{nur.Body.GetProperty("id").GetString()}/offer/withdraw", null, manager, IfMatch(nur.ETag)))
            .Status.ShouldBe(404);
    }

    // ───────── Public ─────────

    [Fact]
    public async Task Join_Again_SameServices_UpdatesPreferences_Returns200()
    {
        var first = await JoinAsync(MervePhone, "Merve Yıldız", Preferences(timeWindows: ["Afternoon"]));
        var again = await JoinAsync(MervePhone, "Merve Yıldız", Preferences(timeWindows: ["Afternoon", "Evening"]));

        again.Status.ShouldBe(200, again.Body.ToString());
        again.Body.GetProperty("id").GetString().ShouldBe(EntryId(first));
        again.Body.GetProperty("timeWindows").EnumerateArray().Select(w => w.GetString()).ShouldBe(["Afternoon", "Evening"]);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM scheduling.waitlist_entries")).ShouldBe(1);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM customers.consent_records WHERE consent_type = 'PrivacyNoticeAcknowledged'")).ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Join_Validation_And_Blocked()
    {
        var invalid = await JoinAsync(MervePhone, "M", new { serviceIds = Array.Empty<string>(), acceptOtherStaff = false, dates = Array.Empty<string>(), timeWindows = NightOnly });
        invalid.Status.ShouldBe(400);
        invalid.Body.GetProperty("errors").GetProperty("preferences.serviceIds").ShouldNotBe(default);
        invalid.Body.GetProperty("errors").GetProperty("preferences.timeWindows").ShouldNotBe(default);
        invalid.Body.GetProperty("errors").GetProperty("fullName").ShouldNotBe(default);

        await _api.ScalarAsync<int>($"UPDATE customers.brand_customers SET is_blocked = true WHERE id = '{_selin}' RETURNING 1");
        var blocked = await JoinAsync("+905550004418", "Selin Öztürk", Preferences());
        blocked.Status.ShouldBe(403);
        blocked.Body.GetProperty("code").GetString().ShouldBe("customer_blocked");
    }

    [Fact]
    public async Task Leave_WithPendingOffer_Cancelled_OfferPassesToNext_Idempotent()
    {
        var appointment = await BookAsync(_elif, Thursday1600);
        var merve = await JoinAsync(MervePhone, "Merve Yıldız", Preferences());
        _factory.Clock.Advance(Duration.FromMinutes(1));
        var nur = await JoinAsync(NurPhone, "Nur Ateş", Preferences());
        await CancelAsync(appointment);
        await DispatchAsync();
        var leaveCode = merve.Body.GetProperty("accessCode").GetString()!;

        var left = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/waitlist/{leaveCode}/leave", null);

        left.Status.ShouldBe(200, left.Body.ToString());
        left.Body.GetProperty("status").GetString().ShouldBe("Cancelled");
        (await PendingOfferCodeAsync(EntryId(nur))).ShouldNotBeNullOrEmpty();
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/waitlist/{leaveCode}/leave", null)).Status.ShouldBe(200);
    }

    [Fact]
    public async Task UnknownCodes_Are404AccessCodeInvalid_OfferCodeDoesNotGrantLeave()
    {
        var unknown = await GetOfferAsync("ZZZZZZZZZZ");
        unknown.Status.ShouldBe(404);
        unknown.Body.GetProperty("code").GetString().ShouldBe("access_code_invalid");
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/public/waitlist/short/leave", null)).Body.GetProperty("code").GetString().ShouldBe("access_code_invalid");

        var appointment = await BookAsync(_elif, Thursday1600);
        var merve = await JoinAsync(MervePhone, "Merve Yıldız", Preferences());
        await CancelAsync(appointment);
        await DispatchAsync();
        var offerCode = await PendingOfferCodeAsync(EntryId(merve));
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/waitlist/{offerCode}/leave", null)).Status.ShouldBe(404, "teklif kodu yalnız kabul ve ret yetkisi verir");
        (await GetOfferAsync(merve.Body.GetProperty("accessCode").GetString()!)).Status.ShouldBe(404, "kayıt kodu teklifi göstermez");
    }

    [Fact]
    public async Task ExpireEntriesJob_AllRequestedDaysPast_Expired_AnyDayStaysWaiting()
    {
        var dated = await JoinAsync(MervePhone, "Merve Yıldız", Preferences());
        var anyDay = await JoinAsync(NurPhone, "Nur Ateş", Preferences(dates: []));

        _factory.Clock.Advance(Duration.FromDays(2));
        await _api.RunJobAsync(ExpireEntriesJob);

        (await EntryStatusAsync(EntryId(dated))).ShouldBe("Expired");
        (await EntryStatusAsync(EntryId(anyDay))).ShouldBe("Waiting");
        var leave = await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/waitlist/{dated.Body.GetProperty("accessCode").GetString()}/leave", null);
        leave.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");
    }

    private static readonly string[] NightOnly = ["Night"];

    private static object Preferences(
        string? preferredStaffId = null, bool acceptOtherStaff = false, string[]? dates = null, string[]? timeWindows = null) => new
        {
            serviceIds = new[] { "{fon}" },
            preferredStaffId,
            acceptOtherStaff,
            dates = dates ?? [Thursday],
            timeWindows = timeWindows ?? [],
        };

    private JsonElement Fill(object preferences) =>
        JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(preferences).Replace("{fon}", _fon, StringComparison.Ordinal));

    private async Task<ManageApi.Response> JoinAsync(string phone, string fullName, object preferences) =>
        await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/waitlist", new
        {
            preferences = Fill(preferences),
            fullName,
            privacyNoticeAcknowledged = true,
            privacyNoticeVersion = "2026-11-01",
        }, await VerifyAsync(phone), Key());

    private Task<ManageApi.Response> AddAsync(object newCustomer, object preferences, string? note = null) =>
        _api.SendAsync(HttpMethod.Post, "/api/v1/manage/waitlist", new { branchId = _merkez, newCustomer, preferences = Fill(preferences), note }, _token, Key());

    private async Task<string> BookAsync(string staffId, string startUtc)
    {
        var created = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/appointments", new
        {
            branchId = _merkez,
            brandCustomerId = _selin,
            channel = "Panel",
            startUtc,
            items = new[] { new { serviceId = _fon, staffId } },
            notifyCustomer = false,
        }, _token, Key());
        created.Status.ShouldBe(201, created.Body.ToString());
        return created.Body.GetProperty("appointment").GetProperty("id").GetString()!;
    }

    private async Task<ManageApi.Response> PublicBookAsync(string phone, string fullName, string staffId, string startUtc) =>
        await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/appointments", new
        {
            items = new[] { new { serviceId = _fon, staffId } },
            sameStaffForAll = true,
            startUtc,
            fullName,
            privacyNoticeAcknowledged = true,
            privacyNoticeVersion = "2026-11-01",
        }, await VerifyAsync(phone), Key());

    private async Task<ManageApi.Response> CancelAsync(string id, bool? offerToWaitlist = null)
    {
        var current = await _api.GetAsync($"/api/v1/manage/appointments/{id}", _token);
        var cancelled = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/cancel",
            new { reason = "CustomerRequested", notifyCustomer = false, offerToWaitlist }, _token, current.ETag);
        cancelled.Status.ShouldBe(200, cancelled.Body.ToString());
        return cancelled;
    }

    private Task<ManageApi.Response> GetOfferAsync(string code) => _api.SendAsync(HttpMethod.Get, $"/api/v1/public/waitlist-offers/{code}", null);

    private Task<string> PendingOfferCodeAsync(string entryId) =>
        _api.ScalarAsync<string>($"SELECT access_code FROM scheduling.waitlist_offers WHERE waitlist_entry_id = '{entryId}' AND status = 'Pending'");

    private Task<long> PendingOfferCountAsync() => _api.ScalarAsync<long>("SELECT count(*) FROM scheduling.waitlist_offers WHERE status = 'Pending'");

    private Task<string> EntryStatusAsync(string entryId) => _api.ScalarAsync<string>($"SELECT status FROM scheduling.waitlist_entries WHERE id = '{entryId}'");

    /// <summary>Olaylar iki tur işlenir (iptal → teklif → waitlist.offer), sonra SMS gönderilir.</summary>
    private async Task DispatchAsync()
    {
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        await _api.RunJobAsync(DispatchSmsJob);
    }

    /// <summary>Kod iste → kodu outbox'tan oku → doğrula; rezervasyon jetonunu döner.</summary>
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

    private static string EntryId(ManageApi.Response response) => response.Body.GetProperty("id").GetString()!;

    private static string Iso(Instant at) => NodaTime.Text.InstantPattern.General.Format(at);

    private static Dictionary<string, string> IfMatch(string? etag) => new(StringComparer.Ordinal) { ["If-Match"] = etag ?? "W/\"0\"" };

    private static object Week() => new
    {
        days = Enumerable.Range(1, 6).Select(d => new { dayOfWeek = d, intervals = new[] { new { start = "09:00", end = "19:00" } } }).ToArray(),
    };

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _requests):D12}" };
}
