using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Reviews.Contracts;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-05 kabul kriteri 8 (yalnız tamamlanan randevu, bir kez; ikinci deneme already_reviewed); alan-modeli.md § Review,
// § AppointmentAccessCode (/d/); bildirimler.md review.invite (Completed + 2 saat, Push → SMS), staff.review_received (Owner +
// şube yöneticisi; personel almaz, W8); roller-yetkiler.md review.view / review.reply (Owner, BranchManager Ş, personel 403);
// sureler-ve-ayarlar.md ReviewInviteEnabled, ReviewInviteDelayMinutes, ReviewPublicThreshold; sözleşme publicGetReviewInvite,
// publicCreateReview, manageListReviews, manageReplyReview, manageListCustomerReviews, PublicBrand.rating.
// Saat: 2026-11-25 09:00Z (12:00 İstanbul). Tamamla ucu (T-88) henüz yok: randevu SQL ile Completed yapılır.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class ReviewTests(PostgresFixture db) : IAsyncLifetime
{
    private const string CustomerPhone = "+905321118833";
    private const string DispatchSmsJob = "Omvila.Modules.Notifications.Jobs.DispatchNotifications, Omvila.Modules.Notifications";
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private Guid _brandId;
    private string _merkez = null!;
    private string _elif = null!;
    private string _boya = null!;
    private string _merve = null!;
    private static int _requests;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db, new Dictionary<string, string?>(StringComparer.Ordinal) { ["Auth:AccessTokenMinutes"] = "600" });
        _api = new ManageApi(_factory, db);
        await _api.ScalarAsync<long>($"WITH d AS (DELETE FROM infra.dev_sms_log WHERE phone_e164 = '{CustomerPhone}' RETURNING 1) SELECT count(*) FROM d");
        _token = await _api.SignupAsync("+905324187705", "Kuaför Elif");
        var session = (await _api.GetAsync("/api/v1/manage/session", _token)).Body;
        _brandId = session.GetProperty("brand").GetProperty("id").GetGuid();
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
        var category = (await PostAsync("/api/v1/manage/service-categories", new { name = "Saç" })).Body.GetProperty("id").GetString()!;
        _boya = (await PostAsync("/api/v1/manage/services", new
        {
            categoryId = category,
            name = "Boya ve fön",
            durationMinutes = 60,
            price = new { amountMinor = 125000, currency = "TRY" },
            priceDisplay = "Fixed",
            deposit = new { mode = "Inherit" },
            isOnlineBookable = true,
            staffIds = new[] { _elif },
        })).Body.GetProperty("id").GetString()!;
        _merve = (await PostAsync("/api/v1/manage/customers", new { fullName = "Merve Yıldız", phone = CustomerPhone })).Body.GetProperty("id").GetString()!;
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    // ───────── Davet (review.invite) ─────────

    [Fact]
    public async Task Invite_AfterCompletion_PlansPushThenSms_TwoHoursLater_Once()
    {
        var id = await CreateCompletedAsync("2026-11-25T06:00:00Z");
        var now = _factory.Clock.GetCurrentInstant();

        (await ScheduleInviteAsync(id, now)).ShouldBeTrue();
        (await ScheduleInviteAsync(id, now)).ShouldBeFalse("randevu başına bir davet");
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);

        var code = await ReviewCodeAsync(id);
        (await _api.ScalarAsync<string>(
            "SELECT to_char(scheduled_utc AT TIME ZONE 'UTC', 'MM-DD HH24:MI') || '|' || (payload->>'bağlantı') || '|' || (recipient_id IS NOT NULL) || '|' || reference_id "
            + "FROM notifications.notification_outbox WHERE event_code = 'review.invite'"))
            .ShouldBe($"{(now + Duration.FromMinutes(120)).ToDateTimeUtc():MM-dd HH:mm}|omvila.com/d/{code}|true|{id}", "Completed + 120 dk; müşteri hesabı verildi (Push → SMS)");

        _factory.Clock.Advance(Duration.FromMinutes(121));
        await _api.RunJobAsync(DispatchSmsJob);
        (await _api.ScalarAsync<string>($"SELECT body FROM infra.dev_sms_log WHERE phone_e164 = '{CustomerPhone}' AND body LIKE '%nasıldı%'"))
            .ShouldBe($"Kuaför Elif'teki randevunuz nasıldı? 10 saniyenizi alır: omvila.com/d/{code}");
    }

    [Fact]
    public async Task Invite_DisabledByBrand_OrNotCompleted_PlansNothing()
    {
        var confirmed = await CreateAsync("2026-11-26T08:00:00Z");
        (await ScheduleInviteAsync(confirmed, _factory.Clock.GetCurrentInstant())).ShouldBeFalse("yalnız tamamlanan randevu");

        await _api.ScalarAsync<int>($"UPDATE tenancy.brand_settings SET review_invite_enabled = false WHERE brand_id = '{_brandId}' RETURNING 1");
        var completed = await CreateCompletedAsync("2026-11-25T06:00:00Z");
        (await ScheduleInviteAsync(completed, _factory.Clock.GetCurrentInstant())).ShouldBeFalse("Değerlendirme daveti kapalı");

        (await _api.ScalarAsync<long>("SELECT count(*) FROM reviews.review_invitations")).ShouldBe(0);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM scheduling.appointment_access_codes WHERE purpose = 'Review'")).ShouldBe(0);
    }

    // ───────── /d/ bağlantısı (kabul kriteri 8) ─────────

    [Fact]
    public async Task Criterion8_Link_ShowsForm_Submit_ThenAlreadyReviewed()
    {
        var id = await CreateCompletedAsync("2026-11-25T06:00:00Z");
        await ScheduleInviteAsync(id, _factory.Clock.GetCurrentInstant());
        var code = await ReviewCodeAsync(id);

        var invite = await _api.GetAsync($"/api/v1/public/reviews/{code}", token: null!);
        invite.Status.ShouldBe(200, invite.Body.ToString());
        invite.Body.GetProperty("brand").GetProperty("name").GetString().ShouldBe("Kuaför Elif");
        invite.Body.GetProperty("branchName").GetString().ShouldBe("Merkez");
        invite.Body.GetProperty("appointmentStartUtc").GetString().ShouldBe("2026-11-25T06:00:00Z");
        invite.Body.GetProperty("timeZone").GetString().ShouldBe("Europe/Istanbul");
        invite.Body.GetProperty("serviceNames").EnumerateArray().Select(s => s.GetString()).ShouldBe(["Boya ve fön"]);
        invite.Body.GetProperty("staff").GetProperty("name").GetString().ShouldNotBeNullOrEmpty();
        invite.Body.GetProperty("customerDisplayName").GetString().ShouldBe("Merve Y.");
        invite.Body.GetProperty("alreadyReviewed").GetBoolean().ShouldBeFalse();
        invite.Body.GetProperty("review").ValueKind.ShouldBe(JsonValueKind.Null);

        var created = await SubmitAsync(code, new { rating = 4, comment = "  Boya tam istediğim gibi oldu. " });
        created.Status.ShouldBe(201, created.Body.ToString());
        created.Body.GetProperty("alreadyReviewed").GetBoolean().ShouldBeTrue();
        created.Body.GetProperty("review").GetProperty("rating").GetInt32().ShouldBe(4);
        created.Body.GetProperty("review").GetProperty("comment").GetString().ShouldBe("Boya tam istediğim gibi oldu.");
        (await _api.ScalarAsync<string>("SELECT is_published || '|' || staff_id || '|' || array_to_string(service_names, ',') FROM reviews.reviews"))
            .ShouldBe($"true|{_elif}|Boya ve fön", "yayınlanmış kaydedilir (S-15)");

        var again = await SubmitAsync(code, new { rating = 5 });
        again.Status.ShouldBe(409, again.Body.ToString());
        again.Body.GetProperty("code").GetString().ShouldBe("already_reviewed");
        (await _api.GetAsync($"/api/v1/public/reviews/{code}", token: null!)).Body.GetProperty("alreadyReviewed").GetBoolean().ShouldBeTrue();
        (await _api.ScalarAsync<long>("SELECT count(*) FROM reviews.reviews")).ShouldBe(1);
    }

    [Fact]
    public async Task Submit_NotCompleted_IsInvalidStateTransition_BadInput400_UnknownCode404()
    {
        var confirmed = await CreateAsync("2026-11-26T08:00:00Z");
        var code = await IssueCodeAsync(confirmed);

        var notCompleted = await SubmitAsync(code, new { rating = 5 });
        notCompleted.Status.ShouldBe(409, notCompleted.Body.ToString());
        notCompleted.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");

        var badRating = await SubmitAsync(code, new { rating = 6 });
        badRating.Status.ShouldBe(400);
        badRating.Body.GetProperty("errors").GetProperty("rating")[0].GetString().ShouldBe("out_of_range");
        var tooLong = await SubmitAsync(code, new { rating = 3, comment = new string('a', 1001) });
        tooLong.Body.GetProperty("errors").GetProperty("comment")[0].GetString().ShouldBe("too_long");

        foreach (var unknown in new[] { "AbCdEfGh12", "kısa" })
        {
            var missing = await _api.GetAsync($"/api/v1/public/reviews/{Uri.EscapeDataString(unknown)}", token: null!);
            missing.Status.ShouldBe(404);
            missing.Body.GetProperty("code").GetString().ShouldBe("access_code_invalid");
        }

        // /r/ kodu değerlendirme bağlantısı değildir.
        var manageCode = await _api.ScalarAsync<string>($"SELECT code FROM scheduling.appointment_access_codes WHERE appointment_id = '{confirmed}' AND purpose = 'Manage'");
        (await _api.GetAsync($"/api/v1/public/reviews/{manageCode}", token: null!)).Status.ShouldBe(404);
    }

    // ───────── staff.review_received (W8) ─────────

    [Fact]
    public async Task ReviewReceived_NotifiesOwnerAndBranchManager_NotStaff()
    {
        var staff = await _api.AddMemberAndSignInAsync(_token, "+905382741905", "Staff", _merkez);
        var manager = await _api.AddMemberAndSignInAsync(_token, "+905382741906", "BranchManager", _merkez);
        var id = await CreateCompletedAsync("2026-11-25T06:00:00Z");
        (await SubmitAsync(await IssueCodeAsync(id), new { rating = 4 })).Status.ShouldBe(201);

        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);

        var item = (await _api.GetAsync("/api/v1/manage/notifications", _token)).Body.GetProperty("items").EnumerateArray()
            .Single(i => string.Equals(i.GetProperty("event").GetString(), "staff.review_received", StringComparison.Ordinal));
        item.GetProperty("title").GetString().ShouldBe("Yeni değerlendirme");
        item.GetProperty("body").GetString().ShouldBe("4 yıldız · Merve Yıldız");
        item.GetProperty("target").GetProperty("type").GetString().ShouldBe("Reviews");
        (await UnreadAsync(manager, "staff.review_received")).ShouldBe(1);
        (await UnreadAsync(staff, "staff.review_received")).ShouldBe(0, "W8: personel almaz");
        (await _api.ScalarAsync<string>(
            "SELECT string_agg(DISTINCT (payload->>'müşteri') || '|' || (payload->>'puan'), ',') FROM notifications.notification_outbox WHERE event_code = 'staff.review_received'"))
            .ShouldBe("Merve Y.|4");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'staff.review_received'"))
            .ShouldBe(2, "Owner ve şubenin yöneticisi");
    }

    // ───────── Panel: liste, özet, yanıt, izinler ─────────

    [Fact]
    public async Task ManageList_AwaitingReplyOrder_Overview()
    {
        await SeedTwoReviewsAsync();

        var page = await _api.GetAsync("/api/v1/manage/reviews", _token);
        page.Status.ShouldBe(200, page.Body.ToString());
        page.Body.GetProperty("items").EnumerateArray().Select(i => i.GetProperty("rating").GetInt32()).ShouldBe([3, 5], "Yanıt bekleyen: düşük puan önce");
        var first = page.Body.GetProperty("items")[0];
        first.GetProperty("customer").GetProperty("fullName").GetString().ShouldBe("Merve Yıldız");
        first.GetProperty("customer").TryGetProperty("phone", out _).ShouldBeFalse();
        first.GetProperty("serviceNames")[0].GetString().ShouldBe("Boya ve fön");
        first.GetProperty("staffId").GetString().ShouldBe(_elif);
        first.GetProperty("staffName").GetString().ShouldNotBeNullOrEmpty();
        first.GetProperty("isPublished").GetBoolean().ShouldBeTrue();
        first.GetProperty("reply").ValueKind.ShouldBe(JsonValueKind.Null);
        page.Body.GetProperty("nextCursor").ValueKind.ShouldBe(JsonValueKind.Null);
        var overview = page.Body.GetProperty("overview");
        overview.GetProperty("averageRating").GetDouble().ShouldBe(4.0);
        overview.GetProperty("ratingCount").GetInt32().ShouldBe(2);
        overview.GetProperty("totalCount").GetInt32().ShouldBe(2);
        overview.GetProperty("awaitingReplyCount").GetInt32().ShouldBe(2);
        overview.GetProperty("thisMonthCount").GetInt32().ShouldBe(2);
        overview.GetProperty("thisMonthInviteResponsePercent").GetDouble().ShouldBe(100, "bu ay 1 davet, 1 yanıt");
        overview.GetProperty("topStaff").ValueKind.ShouldBe(JsonValueKind.Null, "eşik altı (5)");
    }

    [Fact]
    public async Task Reply_Overwrites_NoCustomerNotification_Filters_Cursor_CustomerTab()
    {
        var manager = await _api.AddMemberAndSignInAsync(_token, "+905382741906", "BranchManager", _merkez);
        var reviewId = await SeedTwoReviewsAsync();

        var replied = await ReplyAsync(reviewId, "Teşekkür ederiz, tekrar bekleriz.", manager);
        replied.Status.ShouldBe(200, replied.Body.ToString());
        replied.Body.GetProperty("reply").GetProperty("text").GetString().ShouldBe("Teşekkür ederiz, tekrar bekleriz.");
        _factory.Clock.Advance(Duration.FromMinutes(5));
        var repliedAt = _factory.Clock.GetCurrentInstant();
        var overwritten = await ReplyAsync(reviewId, "Geri bildiriminiz için teşekkür ederiz.", _token);
        overwritten.Body.GetProperty("reply").GetProperty("text").GetString().ShouldBe("Geri bildiriminiz için teşekkür ederiz.");
        overwritten.Body.GetProperty("reply").GetProperty("repliedUtc").GetString().ShouldBe(NodaTime.Text.InstantPattern.General.Format(repliedAt));
        overwritten.Body.GetProperty("reply").GetProperty("repliedByName").GetString().ShouldNotBeNullOrEmpty("sahibin personel adı");
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code LIKE 'review.%'")).ShouldBe(1, "yanıt müşteriye bildirilmez; yalnız davet");

        (await _api.GetAsync("/api/v1/manage/reviews?filter=AwaitingReply", _token)).Body.GetProperty("items").GetArrayLength().ShouldBe(1);
        (await _api.GetAsync("/api/v1/manage/reviews?filter=LowRating", _token)).Body.GetProperty("items").GetArrayLength().ShouldBe(1);
        var all = await _api.GetAsync("/api/v1/manage/reviews?filter=All&limit=1", _token);
        all.Body.GetProperty("items")[0].GetProperty("rating").GetInt32().ShouldBe(5, "Tümü: yeniden eskiye");
        var next = all.Body.GetProperty("nextCursor").GetString()!;
        (await _api.GetAsync($"/api/v1/manage/reviews?filter=All&limit=1&cursor={next}", _token)).Body.GetProperty("items")[0].GetProperty("rating").GetInt32().ShouldBe(3);
        (await _api.GetAsync($"/api/v1/manage/reviews?branchId={_merkez}", manager)).Status.ShouldBe(200);

        var tab = await _api.GetAsync($"/api/v1/manage/customers/{_merve}/reviews", _token);
        tab.Status.ShouldBe(200, tab.Body.ToString());
        tab.Body.GetProperty("items").GetArrayLength().ShouldBe(2);
        (await _api.GetAsync("/api/v1/manage/customers/0193a8f0-0000-7000-8000-00000000ffff/reviews", _token)).Status.ShouldBe(404);
    }

    /// <summary>Davetli 3 yıldız (yorumlu) ve davetsiz 5 yıldız; saat davet gönderiminden sonraya ilerler. Düşük puanlının kimliği.</summary>
    private async Task<string> SeedTwoReviewsAsync()
    {
        var low = await CreateCompletedAsync("2026-11-25T06:00:00Z");
        await ScheduleInviteAsync(low, _factory.Clock.GetCurrentInstant());
        (await SubmitAsync(await ReviewCodeAsync(low), new { rating = 3, comment = "İşlem güzeldi ama 20 dakika bekledim." })).Status.ShouldBe(201);
        _factory.Clock.Advance(Duration.FromMinutes(10));
        var high = await CreateCompletedAsync("2026-11-25T07:00:00Z");
        (await SubmitAsync(await IssueCodeAsync(high), new { rating = 5 })).Status.ShouldBe(201);
        _factory.Clock.Advance(Duration.FromHours(3));

        return (await _api.GetAsync("/api/v1/manage/reviews", _token)).Body.GetProperty("items")[0].GetProperty("id").GetString()!;
    }

    [Fact]
    public async Task ManageReviews_StaffForbidden_OtherBrandNotFound_ReplyValidated()
    {
        var staff = await _api.AddMemberAndSignInAsync(_token, "+905382741905", "Staff", _merkez);
        var id = await CreateCompletedAsync("2026-11-25T06:00:00Z");
        (await SubmitAsync(await IssueCodeAsync(id), new { rating = 2 })).Status.ShouldBe(201);
        var reviewId = (await _api.GetAsync("/api/v1/manage/reviews", _token)).Body.GetProperty("items")[0].GetProperty("id").GetString()!;

        (await _api.GetAsync("/api/v1/manage/reviews", staff)).Status.ShouldBe(403, "personel değerlendirmeleri görmez");
        (await _api.GetAsync($"/api/v1/manage/customers/{_merve}/reviews", staff)).Status.ShouldBe(403);
        (await ReplyAsync(reviewId, "Teşekkürler.", staff)).Status.ShouldBe(403);
        (await ReplyAsync(reviewId, "a", _token)).Body.GetProperty("errors").GetProperty("text")[0].GetString().ShouldBe("too_short");

        var other = await _api.SignupAsync("+905324187799", "Berber Ahmet");
        (await ReplyAsync(reviewId, "Başka marka.", other)).Status.ShouldBe(404, "başka markanın değerlendirmesi");
        (await _api.GetAsync("/api/v1/manage/reviews?filter=All", other)).Body.GetProperty("items").GetArrayLength().ShouldBe(0);
    }

    // ───────── Rezervasyon sayfası puanı ve "En çok övülen" (eşik 5) ─────────

    [Fact]
    public async Task PublicRating_And_TopStaff_AppearFromFiveReviews_AndRespectShowReviews()
    {
        var slug = await _api.ScalarAsync<string>($"SELECT slug FROM tenancy.brands WHERE id = '{_brandId}'");
        await InsertReviewsAsync(4, rating: 5);
        HasRating(await PublicBrandAsync(slug)).ShouldBeFalse("4 değerlendirme eşiğin altında");
        (await _api.GetAsync("/api/v1/manage/reviews", _token)).Body.GetProperty("overview").GetProperty("topStaff").ValueKind.ShouldBe(JsonValueKind.Null);

        await InsertReviewsAsync(1, rating: 4);
        var rating = (await PublicBrandAsync(slug)).GetProperty("rating");
        rating.GetProperty("average").GetDouble().ShouldBe(4.8);
        rating.GetProperty("count").GetInt32().ShouldBe(5);
        var top = (await _api.GetAsync("/api/v1/manage/reviews", _token)).Body.GetProperty("overview").GetProperty("topStaff");
        top.GetProperty("staffId").GetString().ShouldBe(_elif);
        top.GetProperty("averageRating").GetDouble().ShouldBe(4.8);
        top.GetProperty("count").GetInt32().ShouldBe(5);

        await _api.ScalarAsync<int>($"UPDATE tenancy.booking_page_settings SET show_reviews = false WHERE brand_id = '{_brandId}' RETURNING 1");
        HasRating(await PublicBrandAsync(slug)).ShouldBeFalse("salon puanı göstermiyor");
    }

    // Boş isteğe bağlı alan yazılmaz (rating zorunlu değil).
    private static bool HasRating(JsonElement brand) =>
        brand.TryGetProperty("rating", out var rating) && rating.ValueKind != JsonValueKind.Null;

    private async Task<JsonElement> PublicBrandAsync(string slug)
    {
        var response = await _api.GetAsync($"/api/v1/public/brands/{slug}", token: null!);
        response.Status.ShouldBe(200, response.Body.ToString());
        return response.Body;
    }

    private async Task InsertReviewsAsync(int count, int rating)
    {
        for (var i = 0; i < count; i++)
        {
            await _api.ScalarAsync<int>($"""
                INSERT INTO reviews.reviews (id, brand_id, branch_id, appointment_id, brand_customer_id, staff_id, rating, service_names, is_published, created_utc)
                VALUES (gen_random_uuid(), '{_brandId}', '{_merkez}', gen_random_uuid(), '{_merve}', '{_elif}', {rating}, ARRAY['Boya ve fön'], true, now())
                RETURNING 1
                """);
        }
    }

    private async Task<string> CreateAsync(string startUtc)
    {
        var created = await PostAsync("/api/v1/manage/appointments", new
        {
            branchId = _merkez,
            brandCustomerId = _merve,
            channel = "Phone",
            startUtc,
            items = new[] { new { serviceId = _boya, staffId = _elif } },
            notifyCustomer = false,
            allowOutsideWorkingHours = true,
        });
        created.Status.ShouldBe(201, created.Body.ToString());
        return created.Body.GetProperty("appointment").GetProperty("id").GetString()!;
    }

    /// <summary>Tamamla ucu (A11, T-88) gelene kadar durum doğrudan yazılır.</summary>
    private async Task<string> CreateCompletedAsync(string startUtc)
    {
        var id = await CreateAsync(startUtc);
        await _api.ScalarAsync<int>($"UPDATE scheduling.appointments SET status = 'Completed' WHERE id = '{id}' RETURNING 1");
        return id;
    }

    private async Task<bool> ScheduleInviteAsync(string appointmentId, Instant completedUtc) =>
        await InSessionAsync(sp => sp.GetRequiredService<IReviewInvitations>().ScheduleAsync(_brandId, Guid.Parse(appointmentId), completedUtc, Ct));

    private async Task<string> IssueCodeAsync(string appointmentId) =>
        (await InSessionAsync(sp => sp.GetRequiredService<IAppointmentReviewLinks>().IssueReviewCodeAsync(_brandId, Guid.Parse(appointmentId), Ct)))!;

    private async Task<T> InSessionAsync<T>(Func<IServiceProvider, Task<T>> work)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.ForBrand(_brandId), Ct);
        await session.BeginAsync(Ct);
        var result = await work(scope.ServiceProvider);
        await session.CommitAsync(Ct);
        return result;
    }

    private Task<string> ReviewCodeAsync(string appointmentId) =>
        _api.ScalarAsync<string>($"SELECT code FROM scheduling.appointment_access_codes WHERE appointment_id = '{appointmentId}' AND purpose = 'Review'");

    private Task<ManageApi.Response> SubmitAsync(string code, object body) =>
        _api.SendAsync(HttpMethod.Post, $"/api/v1/public/reviews/{code}", body, headers: Key());

    private Task<ManageApi.Response> ReplyAsync(string reviewId, string text, string token) =>
        _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/reviews/{reviewId}/reply", new { text }, token);

    private async Task<int> UnreadAsync(string token, string eventCode) =>
        (await _api.GetAsync("/api/v1/manage/notifications", token)).Body.GetProperty("items").EnumerateArray()
            .Count(i => string.Equals(i.GetProperty("event").GetString(), eventCode, StringComparison.Ordinal));

    private Task<ManageApi.Response> PostAsync(string path, object body) => _api.SendAsync(HttpMethod.Post, path, body, _token, Key());

    // Public uçların idempotency kapsamı markasızdır ("anonymous"): testler arasında anahtar çakışmasın diye sınıf genelinde sayaç.
    private static Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8d00-{Interlocked.Increment(ref _requests):D12}" };
}
