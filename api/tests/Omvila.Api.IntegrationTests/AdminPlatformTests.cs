using System.Text.Json;
using Microsoft.Extensions.Configuration;
using NodaTime;
using Omvila.Host.PlatformUsers;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-06 KK4 (Destek kontör ekleyemez, plan ve durum değiştiremez: 403; Yönetici ekler, denetime yazılır) ve KK5 (marka
// detayının her açılışı AuditLog(View); telefonlar maskeli); K2 (platform girişi telefon + SMS kodu, kullanıcı komutla); K3 rol matrisi;
// ADR-0007 (aud=admin jetonu başka yüzeyde geçmez, yenileme jetonu gövdede ve döner); alan-modeli.md § User.PlatformRole (marka
// üyeliği olamaz); sözleşme admin-auth, admin-overview, admin-brands; T-116 (7), T-119. Saat: 2026-11-25 09:00Z (12:00 İstanbul).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class AdminPlatformTests(PostgresFixture db) : IAsyncLifetime
{
    private const string AdminPhone = "+905550009901";
    private const string SupportPhone = "+905550009902";
    private const string FinancePhone = "+905550009903";
    private const string ElifPhone = "+905324187705";
    private const string KaanPhone = "+905324187706";
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _elifToken = null!;
    private string _elifBrand = null!;
    private string _kaanBrand = null!;
    private int _requests;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db, new Dictionary<string, string?>(StringComparer.Ordinal) { ["Auth:AccessTokenMinutes"] = "600" });
        _api = new ManageApi(_factory, db);
        _elifToken = await _api.SignupAsync(ElifPhone, "Kuaför Elif");
        _elifBrand = (await _api.GetAsync("/api/v1/manage/session", _elifToken)).Body.GetProperty("brand").GetProperty("id").GetString()!;
        var kaanToken = await _api.SignupAsync(KaanPhone, "Berber Kaan");
        _kaanBrand = (await _api.GetAsync("/api/v1/manage/session", kaanToken)).Body.GetProperty("brand").GetProperty("id").GetString()!;
        (await CreatePlatformUserAsync(AdminPhone, "Deniz Yılmaz", "PlatformAdmin")).ShouldBe(0);
        (await CreatePlatformUserAsync(SupportPhone, "Ayşe Demir", "PlatformSupport")).ShouldBe(0);
        (await CreatePlatformUserAsync(FinancePhone, "Can Aydın", "PlatformFinance")).ShouldBe(0);
    }

    public async ValueTask DisposeAsync()
    {
        await _api.ScalarAsync<long>("""
            WITH s AS (DELETE FROM platform.subscriptions RETURNING 1), p AS (DELETE FROM platform.plans WHERE code IN ('solo', 'tiny', 'retired') RETURNING 1)
            SELECT (SELECT count(*) FROM s) + (SELECT count(*) FROM p)
            """);
        await _factory.DisposeAsync();
    }

    // ───────── Platform kullanıcısı komutu (K2) ─────────

    [Fact]
    public async Task Command_IsIdempotent_UpdatesRole_AndRefusesBrandMemberOrBadInput()
    {
        var output = new StringWriter();
        (await CreatePlatformUserAsync(SupportPhone, "Ayşe Demir", "PlatformFinance", output)).ShouldBe(0);
        output.ToString().ShouldContain("güncellendi: 0555 *** ** 02 (PlatformFinance)");
        output.ToString().ShouldNotContain("0009902");
        (await _api.ScalarAsync<string>($"SELECT platform_role || ':' || count(*) OVER () FROM identity.users WHERE phone_e164 = '{SupportPhone}'"))
            .ShouldBe("PlatformFinance:1");

        var refused = new StringWriter();
        (await CreatePlatformUserAsync(ElifPhone, "Elif Kaya", "PlatformAdmin", refused)).ShouldBe(1);
        refused.ToString().ShouldContain("bir markanın üyesi");
        (await _api.ScalarAsync<long>($"SELECT count(*) FROM identity.users WHERE phone_e164 = '{ElifPhone}' AND platform_role IS NOT NULL")).ShouldBe(0);

        (await CreatePlatformUserAsync("+905550009909", "Deniz Kara", "Owner")).ShouldBe(1);
        (await CreatePlatformUserAsync("02124445566", "Deniz Kara", "PlatformAdmin")).ShouldBe(1);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM identity.users WHERE platform_role IS NOT NULL")).ShouldBe(3);
    }

    // ───────── Giriş (K2, ADR-0007) ─────────

    [Fact]
    public async Task Login_IssuesAdminTokens_SessionHasRoleMatrix_AndAudienceDoesNotCrossSurfaces()
    {
        var (access, refresh) = await AdminSignInAsync(SupportPhone);

        var session = await _api.GetAsync("/api/v1/admin/session", access);
        session.Status.ShouldBe(200, session.Body.ToString());
        session.Body.GetProperty("role").GetString().ShouldBe("PlatformSupport");
        session.Body.GetProperty("user").GetProperty("fullName").GetString().ShouldBe("Ayşe Demir");
        session.Body.GetProperty("user").GetProperty("phoneMasked").GetString().ShouldBe("0555 *** ** 02");
        Strings(session.Body.GetProperty("permissions")).ShouldBe(
            ["platform.brands.view", "platform.payments.match", "platform.sms-senders.decide", "platform.plans.view", "platform.audit.view"]);
        (await _api.ScalarAsync<string>("SELECT audience || ':' || client_type FROM identity.refresh_tokens WHERE audience = 'admin'")).ShouldBe("admin:Web");
        (await _api.ScalarAsync<string>("SELECT purpose FROM identity.otp_challenges ORDER BY created_utc DESC LIMIT 1")).ShouldBe("AdminLogin");
        (await _api.ScalarAsync<string>("SELECT action || ':' || entity_type || ':' || actor_type FROM platform.audit_logs WHERE entity_type = 'PlatformSession'"))
            .ShouldBe("Create:PlatformSession:PlatformUser");

        // Bir yüzeyin jetonu diğerinde geçmez (ADR-0007).
        (await _api.GetAsync("/api/v1/manage/session", access)).Status.ShouldBe(401);
        (await _api.GetAsync("/api/v1/admin/session", _elifToken)).Status.ShouldBe(401);
        (await _api.GetAsync("/api/v1/admin/overview", _elifToken)).Status.ShouldBe(401);

        // Yenileme jetonu döner; kullanılmış jeton yeniden gelirse zincir iptal; salon yenileme ucu admin jetonunu kabul etmez.
        var rotated = await _api.SendAsync(HttpMethod.Post, "/api/v1/admin/auth/refresh", new { refreshToken = refresh });
        rotated.Status.ShouldBe(200, rotated.Body.ToString());
        var next = rotated.Body.GetProperty("refreshToken").GetString()!;
        string.Equals(next, refresh, StringComparison.Ordinal).ShouldBeFalse();
        (await _api.GetAsync("/api/v1/admin/session", rotated.Body.GetProperty("accessToken").GetString()!)).Status.ShouldBe(200);
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/admin/auth/refresh", new { refreshToken = refresh })).Status.ShouldBe(401);
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/admin/auth/refresh", new { refreshToken = next })).Status.ShouldBe(401, "zincir iptal edildi");
    }

    [Fact]
    public async Task Logout_RevokesChain_AndRemovedRoleCannotRefresh()
    {
        var (access, refresh) = await AdminSignInAsync(AdminPhone);
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/admin/auth/logout", new { refreshToken = refresh }, access)).Status.ShouldBe(204);
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/admin/auth/refresh", new { refreshToken = refresh })).Status.ShouldBe(401);

        var (_, second) = await AdminSignInAsync(FinancePhone);
        await _api.ScalarAsync<long>($"WITH u AS (UPDATE identity.users SET platform_role = NULL WHERE phone_e164 = '{FinancePhone}' RETURNING 1) SELECT count(*) FROM u");
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/admin/auth/refresh", new { refreshToken = second })).Status.ShouldBe(401);
    }

    [Fact]
    public async Task BrandMember_GetsNoAdminCode_AndPlatformUserCannotUseSalonPanel()
    {
        _factory.Clock.Advance(Duration.FromMinutes(2));
        var codesBefore = await OtpCodeCountAsync();
        var challenge = await _api.SendAsync(HttpMethod.Post, "/api/v1/admin/auth/otp/request", new { phone = "0532 418 77 05" });
        challenge.Status.ShouldBe(202, "numaranın platform kullanıcısı olup olmadığı açık edilmez");
        (await OtpCodeCountAsync()).ShouldBe(codesBefore, "platform kullanıcısı olmayan numaraya SMS gitmez");
        var verify = await _api.SendAsync(
            HttpMethod.Post, "/api/v1/admin/auth/otp/verify", new { otpId = challenge.Body.GetProperty("otpId").GetGuid(), code = "123456" });
        verify.Status.ShouldBe(422);
        verify.Body.GetProperty("code").GetString().ShouldBe("otp_invalid");

        // Salon kodu admin girişinde geçmez (amaç koda bağlı).
        _factory.Clock.Advance(Duration.FromMinutes(2));
        var manageOtp = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/auth/otp/request", new { phone = AdminPhone, purpose = "Login" });
        var manageCode = await LatestCodeAsync(AdminPhone);
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/admin/auth/otp/verify", new { otpId = manageOtp.Body.GetProperty("otpId").GetGuid(), code = manageCode }))
            .Body.GetProperty("code").GetString().ShouldBe("otp_invalid");

        // Platform kullanıcısı salon paneline giremez ve kayıtla marka açamaz.
        var salon = await _api.SignInAsync(AdminPhone);
        salon.Status.ShouldBe(200);
        salon.Body.GetProperty("result").GetString().ShouldBe("SignupRequired");
        var signup = await _api.SendAsync(
            HttpMethod.Post,
            "/api/v1/manage/auth/signup",
            new
            {
                signupToken = salon.Body.GetProperty("signupToken").GetString(),
                businessName = "Deniz Kuaför",
                industryCode = "hair_salon",
                acceptedTerms = true,
                privacyNoticeVersion = "2026-11-01",
            },
            headers: Key());
        signup.Status.ShouldBe(401);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM tenancy.brands")).ShouldBe(2);
    }

    // ───────── KK4: yetki ve denetim ─────────

    [Fact]
    public async Task Criterion4_SupportAndFinanceGet403_AdminGrantsCredits_AndEachWriteIsAudited()
    {
        var support = (await AdminSignInAsync(SupportPhone)).Access;
        var finance = (await AdminSignInAsync(FinancePhone)).Access;
        var admin = (await AdminSignInAsync(AdminPhone)).Access;
        var grant = new { credits = 500, reason = "Destek talebi #4821" };
        var etag = (await _api.GetAsync($"/api/v1/admin/brands/{_elifBrand}", support)).ETag!;

        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/admin/brands/{_elifBrand}/credits", grant, support, Key())).Status.ShouldBe(403);
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/admin/brands/{_elifBrand}/credits", grant, finance, Key())).Status.ShouldBe(403);
        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/admin/brands/{_elifBrand}/plan", new { planCode = "salon_plus", reason = "Deneme" }, support, etag))
            .Status.ShouldBe(403);
        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/admin/brands/{_elifBrand}/status", new { status = "Suspended", reason = "Deneme" }, support, etag))
            .Status.ShouldBe(403);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.credit_transactions WHERE type = 'Grant' AND reference_type = 'PlatformGrant'")).ShouldBe(0);

        var granted = await _api.SendAsync(HttpMethod.Post, $"/api/v1/admin/brands/{_elifBrand}/credits", grant, admin, Key());
        granted.Status.ShouldBe(201, granted.Body.ToString());
        granted.Body.GetProperty("balance").GetInt32().ShouldBe(600, "deneme hediyesi 100 + 500");
        var transaction = granted.Body.GetProperty("transaction");
        transaction.GetProperty("type").GetString().ShouldBe("Grant");
        transaction.GetProperty("credits").GetInt32().ShouldBe(500);
        transaction.GetProperty("note").GetString().ShouldBe("Destek talebi #4821");
        (await _api.ScalarAsync<string>("""
            SELECT action || ':' || actor_type || ':' || (changes->>'credits') || ':' || (changes->>'reason')
            FROM platform.audit_logs WHERE entity_type = 'CreditTransaction'
            """)).ShouldBe("Create:PlatformUser:500:Destek talebi #4821");

        var tooMuch = await _api.SendAsync(HttpMethod.Post, $"/api/v1/admin/brands/{_elifBrand}/credits", new { credits = -601, reason = "Düzeltme" }, admin, Key());
        tooMuch.Status.ShouldBe(409);
        tooMuch.Body.GetProperty("code").GetString().ShouldBe("insufficient_credits");
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/admin/brands/{_elifBrand}/credits", new { credits = 0, reason = "Düzeltme" }, admin, Key())).Status.ShouldBe(400);

        var credits = await _api.GetAsync($"/api/v1/admin/brands/{_elifBrand}/credit-transactions", support);
        credits.Status.ShouldBe(200, credits.Body.ToString());
        var rows = credits.Body.GetProperty("items").EnumerateArray().Select(r => $"{r.GetProperty("type").GetString()}:{r.GetProperty("credits").GetInt32()}:{r.GetProperty("balanceAfter").GetInt32()}:{Text(r.GetProperty("note"))}").ToList();
        rows.ShouldBe(["Grant:500:600:Destek talebi #4821", "TrialGift:100:100:-"]);
    }

    [Fact]
    public async Task Criterion4_AdminChangesStatusAndPlan_WithIfMatch_LimitsAndAudit()
    {
        var admin = (await AdminSignInAsync(AdminPhone)).Access;
        await _api.ScalarAsync<long>("""
            WITH p AS (
              INSERT INTO platform.plans (id, code, name, staff_limit, included_branch_count, is_public, is_active) VALUES
                ('0193a8f0-0000-7000-8000-0000000000a1', 'solo', 'Tekil', 1, 1, true, true),
                ('0193a8f0-0000-7000-8000-0000000000a2', 'tiny', 'Deneme sınırı', 0, 1, false, true),
                ('0193a8f0-0000-7000-8000-0000000000a3', 'retired', 'Eski plan', null, null, false, false)
              ON CONFLICT DO NOTHING RETURNING 1)
            SELECT count(*) FROM p
            """);
        var onboarding = await _api.GetAsync($"/api/v1/admin/brands/{_elifBrand}", admin);
        var notActive = await _api.WriteAsync(
            HttpMethod.Post, $"/api/v1/admin/brands/{_elifBrand}/status", new { status = "Suspended", reason = "Kullanım koşulları ihlali" }, admin, onboarding.ETag);
        notActive.Status.ShouldBe(409, "kurulumu bitmemiş marka askıya alınmaz");
        notActive.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");

        await ActivateBrandsAsync();
        var before = await _api.GetAsync($"/api/v1/admin/brands/{_elifBrand}", admin);
        var suspended = await _api.WriteAsync(
            HttpMethod.Post, $"/api/v1/admin/brands/{_elifBrand}/status", new { status = "Suspended", reason = "Kullanım koşulları ihlali" }, admin, before.ETag);
        suspended.Status.ShouldBe(200, suspended.Body.ToString());
        suspended.Body.GetProperty("brandStatus").GetString().ShouldBe("Suspended");
        string.Equals(suspended.ETag, before.ETag, StringComparison.Ordinal).ShouldBeFalse();
        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/admin/brands/{_elifBrand}/status", new { status = "Active", reason = "Yeniden açıldı" }, admin, before.ETag))
            .Status.ShouldBe(412);
        (await _api.ScalarAsync<string>("SELECT (changes->'status'->>1) || ':' || (changes->>'reason') FROM platform.audit_logs WHERE entity_type = 'Brand' AND action = 'Update'"))
            .ShouldBe("Suspended:Kullanım koşulları ihlali");

        var tiny = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/admin/brands/{_elifBrand}/plan", new { planCode = "tiny", reason = "Deneme" }, admin, suspended.ETag);
        tiny.Status.ShouldBe(409);
        tiny.Body.GetProperty("code").GetString().ShouldBe("plan_limit_exceeded");
        tiny.Body.GetProperty("meta").GetProperty("staffLimit").GetInt32().ShouldBe(0);
        tiny.Body.GetProperty("meta").GetProperty("staffCount").GetInt32().ShouldBe(1);
        var retired = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/admin/brands/{_elifBrand}/plan", new { planCode = "retired", reason = "Deneme" }, admin, suspended.ETag);
        retired.Status.ShouldBe(400);
        retired.Body.GetProperty("errors").GetProperty("planCode")[0].GetString().ShouldBe("invalid_format");

        var changed = await _api.WriteAsync(
            HttpMethod.Post, $"/api/v1/admin/brands/{_elifBrand}/plan", new { planCode = "solo", billingCycle = "Yearly", reason = "Tek kişi çalışıyor" }, admin, suspended.ETag);
        changed.Status.ShouldBe(200, changed.Body.ToString());
        changed.Body.GetProperty("subscription").GetProperty("planCode").GetString().ShouldBe("solo");
        changed.Body.GetProperty("subscription").GetProperty("planName").GetString().ShouldBe("Tekil");
        changed.Body.GetProperty("subscription").GetProperty("billingCycle").GetString().ShouldBe("Yearly");
        (await _api.ScalarAsync<string>("""
            SELECT (changes->'planCode'->>0) || '>' || (changes->'planCode'->>1) || ':' || (changes->>'reason')
            FROM platform.audit_logs WHERE entity_type = 'Subscription' AND action = 'Update'
            """)).ShouldBe("salon_plus>solo:Tek kişi çalışıyor");
        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/admin/brands/{_elifBrand}/plan", new { planCode = "salon_plus", reason = "Geri" }, admin, suspended.ETag))
            .Status.ShouldBe(412);
    }

    // ───────── KK5: görüntüleme denetimi, maskeli telefon ─────────

    [Fact]
    public async Task Criterion5_EveryBrandOpeningAndTabIsAudited_PhonesMasked()
    {
        var support = (await AdminSignInAsync(SupportPhone)).Access;
        var supportId = await _api.ScalarAsync<Guid>($"SELECT id FROM identity.users WHERE phone_e164 = '{SupportPhone}'");
        // Google Takvim bağlantısı (Integrations, IPlatformCalendarLinks): bir etkin, bir kesilmiş bağlantı.
        await _api.ScalarAsync<int>($"""
            INSERT INTO integrations.calendar_links (id, brand_id, staff_id, user_id, provider, external_calendar_id, status, connected_utc, connect_nonce)
            VALUES (gen_random_uuid(), '{_elifBrand}', gen_random_uuid(), gen_random_uuid(), 'Google', 'primary', 'Active', now(), gen_random_uuid()),
                   (gen_random_uuid(), '{_elifBrand}', gen_random_uuid(), gen_random_uuid(), 'Google', 'primary', 'Revoked', now(), gen_random_uuid())
            RETURNING 1
            """);

        var detail = await _api.GetAsync($"/api/v1/admin/brands/{_elifBrand}", support);
        detail.Status.ShouldBe(200, detail.Body.ToString());
        detail.Body.GetProperty("name").GetString().ShouldBe("Kuaför Elif");
        detail.Body.GetProperty("owner").GetProperty("phoneMasked").GetString().ShouldBe("0532 *** ** 05");
        detail.Body.ToString().ShouldNotContain("4187705");
        detail.Body.GetProperty("bookingUrl").GetString().ShouldBe("https://omvila.com/kuafor-elif");
        detail.Body.GetProperty("subscription").GetProperty("status").GetString().ShouldBe("Trialing");
        detail.Body.GetProperty("subscription").GetProperty("trialDaysLeft").GetInt32().ShouldBe(14);
        detail.Body.GetProperty("subscription").GetProperty("paymentMethod").ValueKind.ShouldBe(JsonValueKind.Null);
        detail.Body.GetProperty("credits").GetProperty("balance").GetInt32().ShouldBe(100);
        detail.Body.GetProperty("credits").GetProperty("estimatedDaysLeft").ValueKind.ShouldBe(JsonValueKind.Null);
        detail.Body.GetProperty("appointmentsThisMonth").GetProperty("onlinePercent").ValueKind.ShouldBe(JsonValueKind.Null);
        detail.Body.GetProperty("smsSender").GetProperty("status").GetString().ShouldBe("None");
        detail.Body.GetProperty("activeStaffCount").GetInt32().ShouldBe(1);
        detail.Body.GetProperty("googleCalendarConnectedStaffCount").GetInt32().ShouldBe(1);
        detail.ETag.ShouldBe(detail.Body.GetProperty("version").GetString());
        await _api.GetAsync($"/api/v1/admin/brands/{_elifBrand}", support);

        (await _api.GetAsync($"/api/v1/admin/brands/{_elifBrand}/invoices", support)).Body.GetProperty("items").GetArrayLength().ShouldBe(0);
        var notices = await _api.GetAsync($"/api/v1/admin/brands/{_elifBrand}/payment-notices", support);
        notices.Status.ShouldBe(200);
        notices.Body.GetProperty("nextCursor").ValueKind.ShouldBe(JsonValueKind.Null);
        (await _api.GetAsync($"/api/v1/admin/brands/{_elifBrand}/credit-transactions", support)).Status.ShouldBe(200);
        (await _api.GetAsync("/api/v1/admin/brands", support)).Status.ShouldBe(200);

        (await _api.ScalarAsync<string>($"""
            SELECT string_agg(changes->>'section', ',' ORDER BY id) FROM platform.audit_logs
            WHERE action = 'View' AND entity_type = 'Brand' AND brand_id = '{_elifBrand}' AND entity_id = '{_elifBrand}'
              AND actor_type = 'PlatformUser' AND actor_id = '{supportId}'
            """)).ShouldBe("Summary,Summary,Invoices,Payments,Credits", "liste görüntülemesi denetime yazılmaz");

        (await _api.GetAsync("/api/v1/admin/brands/0193a8f0-0000-7000-8000-00000000dead", support)).Status.ShouldBe(404);
        (await _api.GetAsync("/api/v1/admin/brands/0193a8f0-0000-7000-8000-00000000dead/invoices", support)).Status.ShouldBe(404);
    }

    // ───────── Markalar (filtreler) ve Genel bakış ─────────

    [Fact]
    public async Task ListBrands_FiltersSearchesAndPages()
    {
        await ActivateBrandsAsync();
        await SetSubscriptionAsync(_kaanBrand, "status = 'PastDue', past_due_since = '2026-11-19'");
        (await _api.WriteAsync(
            HttpMethod.Put,
            "/api/v1/manage/brand/bank-account",
            new { accountHolderName = "Elif Kaya", bankName = "Ziraat Bankası", iban = "TR29 0006 4000 0011 2345 6789 01", transferCodePrefix = "OMV", transferWindowMinutes = 120 },
            _elifToken,
            "W/\"0\"")).Status.ShouldBe(200);
        var finance = (await AdminSignInAsync(FinancePhone)).Access;

        var all = await _api.GetAsync("/api/v1/admin/brands", finance);
        all.Status.ShouldBe(200, all.Body.ToString());
        all.Body.GetProperty("counts").GetProperty("active").GetInt32().ShouldBe(2);
        all.Body.GetProperty("counts").GetProperty("trialing").GetInt32().ShouldBe(1);
        Names(all.Body).ShouldBe(["Berber Kaan", "Kuaför Elif"]);
        var kaan = all.Body.GetProperty("items")[0];
        kaan.GetProperty("subscriptionStatus").GetString().ShouldBe("PastDue");
        kaan.GetProperty("pastDueDays").GetInt32().ShouldBe(6);
        kaan.GetProperty("trialDaysLeft").ValueKind.ShouldBe(JsonValueKind.Null);
        kaan.GetProperty("planName").GetString().ShouldBe("Salon Plus");
        kaan.GetProperty("creditBalance").GetInt32().ShouldBe(100);
        kaan.GetProperty("depositEnabled").GetBoolean().ShouldBeFalse();
        all.Body.GetProperty("items")[1].GetProperty("depositEnabled").GetBoolean().ShouldBeTrue();

        Names((await _api.GetAsync("/api/v1/admin/brands?filter=Trialing", finance)).Body).ShouldBe(["Kuaför Elif"]);
        Names((await _api.GetAsync("/api/v1/admin/brands?filter=PastDue", finance)).Body).ShouldBe(["Berber Kaan"]);
        Names((await _api.GetAsync("/api/v1/admin/brands?filter=ReadOnly", finance)).Body).ShouldBeEmpty();
        Names((await _api.GetAsync("/api/v1/admin/brands?filter=DepositEnabled", finance)).Body).ShouldBe(["Kuaför Elif"]);
        Names((await _api.GetAsync("/api/v1/admin/brands?q=kuaf", finance)).Body).ShouldBe(["Kuaför Elif"]);
        Names((await _api.GetAsync("/api/v1/admin/brands?q=berber-kaan", finance)).Body).ShouldBe(["Berber Kaan"]);
        Names((await _api.GetAsync("/api/v1/admin/brands?q=7706", finance)).Body).ShouldBe(["Berber Kaan"], "sahibin telefonunun son 4 hanesi");
        (await _api.GetAsync("/api/v1/admin/brands?filter=Archived", finance)).Status.ShouldBe(400);
        (await _api.GetAsync("/api/v1/admin/brands?q=k", finance)).Status.ShouldBe(400);

        var first = await _api.GetAsync("/api/v1/admin/brands?limit=1", finance);
        Names(first.Body).ShouldBe(["Berber Kaan"]);
        var cursor = first.Body.GetProperty("nextCursor").GetString()!;
        var second = await _api.GetAsync($"/api/v1/admin/brands?limit=1&cursor={Uri.EscapeDataString(cursor)}", finance);
        Names(second.Body).ShouldBe(["Kuaför Elif"]);
        second.Body.GetProperty("nextCursor").ValueKind.ShouldBe(JsonValueKind.Null);
    }

    [Fact]
    public async Task Overview_CountsCardsFunnelAndAttentionBrands_AndSessionBadgeFollowsRole()
    {
        await ActivateBrandsAsync();
        await SetSubscriptionAsync(_elifBrand, "trial_ends_on = '2026-11-27'");
        await SetSubscriptionAsync(_kaanBrand, "status = 'PastDue', past_due_since = '2026-11-19'");
        var elifSession = (await _api.GetAsync("/api/v1/manage/session", _elifToken)).Body;
        var merkez = elifSession.GetProperty("branches")[0].GetProperty("id").GetString()!;
        var hours = await _api.GetAsync($"/api/v1/manage/branches/{merkez}/working-hours", _elifToken);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/branches/{merkez}/working-hours", Week(), _elifToken, hours.ETag)).Status.ShouldBe(200);
        var category = (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/service-categories", new { name = "Saç" }, _elifToken, Key())).Body.GetProperty("id").GetString()!;
        (await _api.SendAsync(
            HttpMethod.Post,
            "/api/v1/manage/services",
            new
            {
                categoryId = category,
                name = "Saç kesimi",
                durationMinutes = 45,
                price = new { amountMinor = 45000, currency = "TRY" },
                priceDisplay = "Fixed",
                deposit = new { mode = "Inherit" },
                isOnlineBookable = true,
                staffIds = new[] { elifSession.GetProperty("membership").GetProperty("staffId").GetString() },
            },
            _elifToken,
            Key())).Status.ShouldBe(201);
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/brand/sms-sender", new { name = "KUAFORELIF" }, _elifToken, Key())).Status.ShouldBe(201);
        var support = (await AdminSignInAsync(SupportPhone)).Access;

        var overview = await _api.GetAsync("/api/v1/admin/overview", support);
        overview.Status.ShouldBe(200, overview.Body.ToString());
        var body = overview.Body;
        Card(body, "activeBrands").ShouldBe("count=2,newThisMonth=2");
        Card(body, "pendingApprovals").ShouldBe("total=1,paymentNotices=0,smsSenders=1");
        Card(body, "trialing").ShouldBe("count=1,endingThisWeek=1");
        Card(body, "pastDue").ShouldBe("count=1,readOnlySoon=1");
        Card(body, "funnel").ShouldBe("windowDays=30,signedUp=2,addedServices=1,setWorkingHours=1,firstOnlineAppointment=0");
        body.GetProperty("attentionBrands").EnumerateArray()
            .Select(a => $"{a.GetProperty("brandName").GetString()}:{a.GetProperty("reason").GetString()}:{Text(a.GetProperty("pastDueDays"))}:{Text(a.GetProperty("trialDaysLeft"))}")
            .ShouldBe(["Berber Kaan:PastDue:6:-", "Kuaför Elif:TrialEnding:-:2"]);

        (await _api.GetAsync("/api/v1/admin/session", support)).Body.GetProperty("pendingApprovalCount").GetInt32().ShouldBe(1);
        var finance = (await AdminSignInAsync(FinancePhone)).Access;
        (await _api.GetAsync("/api/v1/admin/session", finance)).Body.GetProperty("pendingApprovalCount").GetInt32().ShouldBe(0, "Finans SMS başlığı onaylamaz");
    }

    private async Task<int> CreatePlatformUserAsync(string phone, string name, string role, TextWriter? output = null)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                ["platform-phone"] = phone,
                ["platform-name"] = name,
                ["platform-role"] = role,
            })
            .Build();
        return await PlatformUserCommand.RunAsync(_factory.Services, configuration, output ?? TextWriter.Null, Ct);
    }

    private async Task<(string Access, string Refresh)> AdminSignInAsync(string phone)
    {
        _factory.Clock.Advance(Duration.FromMinutes(2));
        var otp = await _api.SendAsync(HttpMethod.Post, "/api/v1/admin/auth/otp/request", new { phone });
        otp.Status.ShouldBe(202, otp.Body.ToString());
        var verified = await _api.SendAsync(
            HttpMethod.Post, "/api/v1/admin/auth/otp/verify", new { otpId = otp.Body.GetProperty("otpId").GetGuid(), code = await LatestCodeAsync(phone) });
        verified.Status.ShouldBe(200, verified.Body.ToString());
        verified.Body.GetProperty("expiresInSeconds").GetInt32().ShouldBe(36000);
        return (verified.Body.GetProperty("accessToken").GetString()!, verified.Body.GetProperty("refreshToken").GetString()!);
    }

    private Task<string> LatestCodeAsync(string phone) => _api.ScalarAsync<string>($"""
        SELECT payload->>'kod' FROM notifications.notification_outbox
        WHERE event_code = 'otp.code' AND recipient_phone = '{phone}' ORDER BY created_utc DESC, id DESC LIMIT 1
        """);

    private Task<long> OtpCodeCountAsync() => _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'otp.code'");

    private Task<long> ActivateBrandsAsync() =>
        _api.ScalarAsync<long>("WITH b AS (UPDATE tenancy.brands SET status = 'Active' RETURNING 1) SELECT count(*) FROM b");

    private Task<long> SetSubscriptionAsync(string brandId, string assignments) =>
        _api.ScalarAsync<long>($"WITH s AS (UPDATE platform.subscriptions SET {assignments} WHERE brand_id = '{brandId}' RETURNING 1) SELECT count(*) FROM s");

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{++_requests:D12}" };

    private static object Week() => new
    {
        days = Enumerable.Range(1, 7).Select(d => new { dayOfWeek = d, intervals = new[] { new { start = "09:00", end = "19:00" } } }).ToArray(),
    };

    private static List<string> Names(JsonElement page) =>
        [.. page.GetProperty("items").EnumerateArray().Select(i => i.GetProperty("name").GetString()!)];

    private static List<string> Strings(JsonElement array) => [.. array.EnumerateArray().Select(e => e.GetString()!)];

    private static string Card(JsonElement body, string name) =>
        string.Join(',', body.GetProperty(name).EnumerateObject().Select(p => $"{p.Name}={p.Value.GetInt32()}"));

    private static string Text(JsonElement value) => value.ValueKind switch
    {
        JsonValueKind.Null => "-",
        JsonValueKind.Number => value.GetInt32().ToString(System.Globalization.CultureInfo.InvariantCulture),
        _ => value.GetString() ?? "-",
    };
}
