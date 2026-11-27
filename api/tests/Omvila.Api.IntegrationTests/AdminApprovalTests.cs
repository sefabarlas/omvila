using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Omvila.Host.PlatformUsers;
using Omvila.Infrastructure.Email;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-06 KK2 (salon bildirir → kuyrukta → Destek "Kod tutuyor" hareketiyle eşleştirir → fatura Paid, abonelik Active, markaya
// bildirim, denetim), KK3 (uyuşmazlıkta "Kontrol edin", "Bulunamadı, markaya yaz" → NotFound), KK4 (Destek plan ve kontör paketi
// kaydedemez, CSV içe aktaramaz; Finans plan kaydeder; Finans SMS adına karar veremez); sözleşme admin-approvals, admin-finance,
// admin-audit, admin-plans; T-116 (3), (6), (14); T-124. Saat: 2026-11-25 09:00Z (12:00 İstanbul).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class AdminApprovalTests(PostgresFixture db) : IAsyncLifetime
{
    private const string AdminPhone = "+905550009911";
    private const string SupportPhone = "+905550009912";
    private const string FinancePhone = "+905550009913";
    private const string ElifPhone = "+905324187715";
    private const string KaanPhone = "+905324187716";
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _elifToken = null!;
    private string _kaanToken = null!;
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
        _kaanToken = await _api.SignupAsync(KaanPhone, "Berber Kaan");
        _kaanBrand = (await _api.GetAsync("/api/v1/manage/session", _kaanToken)).Body.GetProperty("brand").GetProperty("id").GetString()!;
        await _api.ScalarAsync<int>($"UPDATE tenancy.brands SET email = 'salon@example.com' WHERE id = '{_elifBrand}' RETURNING 1");
        (await CreatePlatformUserAsync(AdminPhone, "Deniz Yılmaz", "PlatformAdmin")).ShouldBe(0);
        (await CreatePlatformUserAsync(SupportPhone, "Ayşe Demir", "PlatformSupport")).ShouldBe(0);
        (await CreatePlatformUserAsync(FinancePhone, "Can Aydın", "PlatformFinance")).ShouldBe(0);
        Emails().Clear();
    }

    public async ValueTask DisposeAsync()
    {
        await _api.ScalarAsync<int>("""
            UPDATE platform.plans SET monthly_price_minor = NULL, yearly_price_minor = NULL, staff_limit = NULL, description = NULL,
              features = '{"walkInQueue":false,"waitlist":false,"packages":false,"crossBranchReports":false,"prioritySupport":false}'::jsonb;
            UPDATE platform.credit_packages SET price_minor = NULL, is_active = true RETURNING 1
            """);
        await _factory.DisposeAsync();
    }

    // ───────── KK2: salon bildirir → Destek eşleştirir ─────────

    [Fact]
    public async Task Criterion2_SalonNotifies_SupportMatchesCodeMatchingTransaction_InvoicePaid_SubscriptionActive_BrandNotified_Audited()
    {
        var (finance, _) = await AdminSignInAsync(FinancePhone);
        await SetPlanPriceAsync(finance, 100000);
        var notified = await NotifyAsync(_elifToken, "Subscription", receipt: ("dekont-ziraat.pdf", "%PDF-1.4 dekont"u8.ToArray()));
        notified.Status.ShouldBe(201, notified.Text);
        var noticeId = notified.Json.GetProperty("id").GetString()!;
        var code = notified.Json.GetProperty("transferCode").GetString()!;
        notified.Json.GetProperty("expectedAmount").GetProperty("amountMinor").GetInt64().ShouldBe(100000);

        // Finans dökümü içe aktarır: açıklamada kod küçük harf ve tiresiz; ikinci satır kodsuz aynı tutar.
        var csv = $"Tarih;Tutar;Gönderen;Açıklama;Referans\n25.11.2026 10:15;1.000,00;ELİF KAYA;{code.Replace("-", " ", StringComparison.Ordinal).ToLowerInvariant()} odeme;R-1\n24.11.2026;1.000,00;KAAN ÖZTÜRK;;R-2\n";
        (await ImportAsync(finance, csv, dryRun: false)).Json.GetProperty("imported").GetInt32().ShouldBe(2);

        var (support, _) = await AdminSignInAsync(SupportPhone);
        var queue = await _api.GetAsync("/api/v1/admin/approval-queue", support);
        queue.Status.ShouldBe(200, queue.Body.ToString());
        queue.Body.GetProperty("paymentNoticeCount").GetInt32().ShouldBe(1);
        var item = queue.Body.GetProperty("items").EnumerateArray().Single(i => string.Equals(i.GetProperty("type").GetString(), "PaymentNotice", StringComparison.Ordinal));
        item.GetProperty("brandName").GetString().ShouldBe("Kuaför Elif");
        var candidate = item.GetProperty("paymentNotice").GetProperty("candidate");
        candidate.GetProperty("verdict").GetString().ShouldBe("CodeMatches");
        item.GetProperty("paymentNotice").GetProperty("hasReceipt").GetBoolean().ShouldBeTrue();

        var detail = await _api.GetAsync($"/api/v1/admin/payment-notices/{noticeId}", support);
        detail.Status.ShouldBe(200, detail.Body.ToString());
        detail.Body.GetProperty("planName").GetString().ShouldBe("Salon Plus");
        detail.Body.GetProperty("candidateWindow").GetProperty("from").GetString().ShouldBe("2026-11-18");
        detail.Body.GetProperty("candidateWindow").GetProperty("to").GetString().ShouldBe("2026-11-28");
        detail.Body.GetProperty("receipt").GetProperty("fileName").GetString().ShouldBe("dekont-ziraat.pdf");
        var candidates = detail.Body.GetProperty("candidates").EnumerateArray().ToList();
        candidates.Select(c => c.GetProperty("verdict").GetString()).ShouldBe(["CodeMatches", "CheckRequired"]);
        candidates[0].GetProperty("receivedUtc").GetString().ShouldBe("2026-11-25T07:15:00Z");
        var bankTransactionId = candidate.GetProperty("bankTransactionId").GetString()!;
        candidates[0].GetProperty("bankTransactionId").GetString().ShouldBe(bankTransactionId);

        await ReceiptDownloadsAsync(support, noticeId);

        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/admin/payment-notices/{noticeId}/match", new { bankTransactionId }, support, "W/\"1\"")).Status.ShouldBe(412);
        var matched = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/admin/payment-notices/{noticeId}/match", new { bankTransactionId }, support, detail.ETag);
        matched.Status.ShouldBe(200, matched.Body.ToString());
        matched.Body.GetProperty("status").GetString().ShouldBe("Matched");
        matched.Body.GetProperty("resolution").GetProperty("resolvedByName").GetString().ShouldBe("Ayşe Demir");
        matched.Body.GetProperty("resolution").GetProperty("matchedBankTransactionId").GetString().ShouldBe(bankTransactionId);
        matched.Body.GetProperty("candidates").GetArrayLength().ShouldBe(0);

        await MatchedEffectsAsync(noticeId, bankTransactionId);
        await SecondMatchIsRefusedAsync(support, noticeId, bankTransactionId, matched.ETag);
    }

    // ───────── KK3: uyuşmazlık ve "Bulunamadı" ─────────

    [Fact]
    public async Task Criterion3_MismatchShowsCheckRequired_NotFoundAndReject_NotifyBrandWithMessage()
    {
        var (finance, _) = await AdminSignInAsync(FinancePhone);
        var medium = await PackageIdAsync("medium");
        var packages = await _api.GetAsync("/api/v1/admin/credit-packages", finance);
        var update = await _api.WriteAsync(HttpMethod.Put, "/api/v1/admin/credit-packages", new
        {
            packages = new[] { new { code = "medium", name = "Orta", credits = 5000, price = new { amountMinor = 50000, currency = "TRY" }, isActive = true } },
        }, finance, packages.ETag);
        update.Status.ShouldBe(200, update.Body.ToString());

        var notified = await NotifyAsync(_elifToken, "Credits", medium);
        var noticeId = notified.Json.GetProperty("id").GetString()!;
        var code = notified.Json.GetProperty("transferCode").GetString()!;
        await CreateBankTransactionAsync(finance, "R-30", 45000, code, "2026-11-24");
        await CreateBankTransactionAsync(finance, "R-31", 50000, null, "2026-11-25");
        await CreateBankTransactionAsync(finance, "R-32", 50000, code, "2026-11-10");

        var (support, _) = await AdminSignInAsync(SupportPhone);
        var detail = await _api.GetAsync($"/api/v1/admin/payment-notices/{noticeId}", support);
        var candidates = detail.Body.GetProperty("candidates").EnumerateArray().ToList();
        candidates.Count.ShouldBe(2, "aralık dışındaki hareket (10 Kasım) aday değildir");
        candidates.Select(c => $"{c.GetProperty("codeFound").GetBoolean()}/{c.GetProperty("amountMatches").GetBoolean()}/{c.GetProperty("verdict").GetString()}")
            .ShouldBe(["True/False/CheckRequired", "False/True/CheckRequired"]);
        detail.Body.GetProperty("credits").GetInt32().ShouldBe(5000);

        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/admin/payment-notices/{noticeId}/not-found", new { message = "x" }, support, detail.ETag))
            .Status.ShouldBe(400);
        var notFound = await _api.WriteAsync(
            HttpMethod.Post, $"/api/v1/admin/payment-notices/{noticeId}/not-found", new { message = "Havaleyi hesabımızda göremedik." }, support, detail.ETag);
        notFound.Status.ShouldBe(200, notFound.Body.ToString());
        notFound.Body.GetProperty("status").GetString().ShouldBe("NotFound");
        notFound.Body.GetProperty("resolution").GetProperty("message").GetString().ShouldBe("Havaleyi hesabımızda göremedik.");
        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/admin/payment-notices/{noticeId}/reject", new { message = "Yinelenen bildirim." }, support, notFound.ETag))
            .Status.ShouldBe(409);
        (await _api.ScalarAsync<long>($"SELECT count(*) FROM notifications.credit_transactions WHERE brand_id = '{_elifBrand}' AND type = 'Purchase'")).ShouldBe(0);

        // Salon yeniden bildirebilir; ret notu zorunludur.
        var renotified = await NotifyAsync(_elifToken, "Credits", medium);
        renotified.Status.ShouldBe(201, renotified.Text);
        var renoticeId = renotified.Json.GetProperty("id").GetString()!;
        var reDetail = await _api.GetAsync($"/api/v1/admin/payment-notices/{renoticeId}", support);
        var noMessage = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/admin/payment-notices/{renoticeId}/reject", new { }, support, reDetail.ETag);
        noMessage.Status.ShouldBe(400);
        noMessage.Body.GetProperty("errors").GetProperty("message")[0].GetString().ShouldBe("required");
        var rejected = await _api.WriteAsync(
            HttpMethod.Post, $"/api/v1/admin/payment-notices/{renoticeId}/reject", new { message = "Yinelenen bildirim." }, support, reDetail.ETag);
        rejected.Body.GetProperty("status").GetString().ShouldBe("Rejected");

        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        Emails().Sent.Single(e => string.Equals(e.EventCode, "owner.payment_notice_not_found", StringComparison.Ordinal)).TextBody.ShouldContain("OMVILA ekibinin notu: Havaleyi hesabımızda göremedik.");
        Emails().Sent.Single(e => string.Equals(e.EventCode, "owner.payment_notice_rejected", StringComparison.Ordinal)).TextBody.ShouldContain("Yinelenen bildirim.");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.in_app_notifications WHERE event_code IN ('owner.payment_notice_not_found', 'owner.payment_notice_rejected')"))
            .ShouldBe(2);
        (await _api.ScalarAsync<long>($"SELECT count(*) FROM platform.audit_logs WHERE action = 'Update' AND entity_type = 'PaymentNotice' AND brand_id = '{_elifBrand}'"))
            .ShouldBe(2);
        (await _api.ScalarAsync<string>($"SELECT status FROM platform.subscriptions WHERE brand_id = '{_elifBrand}'")).ShouldBe("Trialing", "abonelik durumu değişmez");
    }

    // ───────── KK4: rol sınırları ─────────

    [Fact]
    public async Task Criterion4_SupportCannotSavePlansPackagesOrImport_FinanceSavesPlans_AndIsAudited()
    {
        var (support, _) = await AdminSignInAsync(SupportPhone);
        var plans = await _api.GetAsync("/api/v1/admin/plans", support);
        plans.Status.ShouldBe(200, plans.Body.ToString());
        plans.Body.GetProperty("rules").GetProperty("trialDays").GetInt32().ShouldBe(14);
        plans.Body.GetProperty("rules").GetProperty("trialCredits").GetInt32().ShouldBe(100);
        var plan = plans.Body.GetProperty("plans").EnumerateArray().Single(p => string.Equals(p.GetProperty("code").GetString(), "salon_plus", StringComparison.Ordinal));
        plan.GetProperty("monthlyPrice").ValueKind.ShouldBe(JsonValueKind.Null);
        plan.GetProperty("brandCount").GetInt32().ShouldBe(2);
        (await _api.WriteAsync(HttpMethod.Put, "/api/v1/admin/plans", PlanBody(120000), support, plans.ETag)).Status.ShouldBe(403);
        var packages = await _api.GetAsync("/api/v1/admin/credit-packages", support);
        (await _api.WriteAsync(HttpMethod.Put, "/api/v1/admin/credit-packages", new { packages = Array.Empty<object>() }, support, packages.ETag)).Status.ShouldBe(403);
        (await ImportAsync(support, "Tarih;Tutar;Gönderen;Referans\n", dryRun: true)).Status.ShouldBe(403);
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/admin/bank-transactions", new { }, support, Key())).Status.ShouldBe(403);

        var (finance, _) = await AdminSignInAsync(FinancePhone);
        (await _api.WriteAsync(HttpMethod.Put, "/api/v1/admin/plans", PlanBody(120000), finance, "W/\"1\"")).Status.ShouldBe(412);
        var unknown = await _api.WriteAsync(HttpMethod.Put, "/api/v1/admin/plans", new { plans = new[] { new { code = "gold", name = "Altın" } } }, finance, plans.ETag);
        unknown.Status.ShouldBe(400);
        unknown.Body.GetProperty("errors").GetProperty("plans[0].code")[0].GetString().ShouldBe("invalid_format");
        var saved = await _api.WriteAsync(HttpMethod.Put, "/api/v1/admin/plans", PlanBody(120000), finance, plans.ETag);
        saved.Status.ShouldBe(200, saved.Body.ToString());
        var savedPlan = saved.Body.GetProperty("plans").EnumerateArray().Single(p => string.Equals(p.GetProperty("code").GetString(), "salon_plus", StringComparison.Ordinal));
        savedPlan.GetProperty("monthlyPrice").GetProperty("amountMinor").GetInt64().ShouldBe(120000);
        savedPlan.GetProperty("features").GetProperty("waitlist").GetBoolean().ShouldBeTrue();
        savedPlan.GetProperty("description").GetString().ShouldBe("Büyüyen salonlar");
        string.Equals(saved.ETag, plans.ETag, StringComparison.Ordinal).ShouldBeFalse();
        (await _api.ScalarAsync<string>("SELECT changes->>'code' || ':' || (changes->'monthlyPriceMinor'->>1) FROM platform.audit_logs WHERE entity_type = 'Plan' AND action = 'Update'"))
            .ShouldBe("salon_plus:120000");
    }

    // ───────── CSV içe aktarma ─────────

    [Fact]
    public async Task Import_PreviewWritesNothing_ApplyImports_ResendCreatesNoDuplicates_InvalidRowsReported()
    {
        var (finance, _) = await AdminSignInAsync(FinancePhone);
        const string csv = "﻿Tarih;Tutar;Gönderen;Açıklama;Referans\n"
            + "18.11.2026;1.250,50;ELİF KAYA;ABN-KE-1125;R-100\n"
            + "18.11.2026;-300,00;OMVILA;İade;R-101\n"
            + "31.02.2026;100,00;X;;R-102\n"
            + "18.11.2026;abc;Y;;R-103\n"
            + "19.11.2026;200;\"SPA MODA; LTD\";\"KNT \"\"SM\"\"\";R-104\n"
            + "19.11.2026;200;Z;;R-104\n";

        var preview = await ImportAsync(finance, csv, dryRun: true);
        preview.Status.ShouldBe(200, preview.Text);
        Summary(preview.Json).ShouldBe("True:6:2:1:1:2");
        preview.Json.GetProperty("errors").EnumerateArray().Select(e => $"{e.GetProperty("rowNumber").GetInt32()}:{e.GetProperty("column").GetString()}:{e.GetProperty("code").GetString()}")
            .ShouldBe(["4:Tarih:invalid_format", "5:Tutar:invalid_format"]);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM platform.bank_transactions")).ShouldBe(0);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM platform.audit_logs WHERE entity_type = 'PlatformBankTransaction'")).ShouldBe(0);

        Summary((await ImportAsync(finance, csv, dryRun: false)).Json).ShouldBe("False:6:2:1:1:2");
        (await _api.ScalarAsync<string>("SELECT amount_minor || '|' || sender_name || '|' || description || '|' || source FROM platform.bank_transactions WHERE bank_reference = 'R-104'"))
            .ShouldBe("20000|SPA MODA; LTD|KNT \"SM\"|Import");
        (await _api.ScalarAsync<long>("SELECT amount_minor FROM platform.bank_transactions WHERE bank_reference = 'R-100'")).ShouldBe(125050);
        Summary((await ImportAsync(finance, csv, dryRun: false)).Json).ShouldBe("False:6:0:3:1:2");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM platform.bank_transactions")).ShouldBe(2);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM platform.audit_logs WHERE entity_type = 'PlatformBankTransaction' AND action = 'Create'")).ShouldBe(2);

        var noHeader = await ImportAsync(finance, "a;b;c\n1;2;3\n", dryRun: true);
        noHeader.Status.ShouldBe(400);
        noHeader.Json.GetProperty("errors").GetProperty("file")[0].GetString().ShouldBe("invalid_format");
        var tooMany = new StringBuilder("Tarih;Tutar;Gönderen;Referans\n");
        for (var i = 0; i < 51; i++)
        {
            tooMany.Append(CultureInfoInvariant($"18.11.2026;10;A;T-{i}\n"));
        }

        (await ImportAsync(finance, tooMany.ToString(), dryRun: true)).Json.GetProperty("errors").GetProperty("file")[0].GetString().ShouldBe("too_large");

        // Elle ekleme: aynı referans 409 bank_transaction_duplicate; liste arama ve eşleşmemiş süzgeci.
        var duplicate = await _api.SendAsync(HttpMethod.Post, "/api/v1/admin/bank-transactions", new
        {
            valueDate = "2026-11-20",
            amount = new { amountMinor = 1000, currency = "TRY" },
            senderName = "ELİF KAYA",
            bankReference = "R-100",
        }, finance, Key());
        duplicate.Status.ShouldBe(409);
        duplicate.Body.GetProperty("code").GetString().ShouldBe("bank_transaction_duplicate");
        var list = await _api.GetAsync("/api/v1/admin/bank-transactions?q=spa%20moda&unmatchedOnly=true", finance);
        list.Status.ShouldBe(200, list.Body.ToString());
        list.Body.GetProperty("items").EnumerateArray().Select(i => i.GetProperty("bankReference").GetString()).ShouldBe(["R-104"]);
        (await _api.GetAsync("/api/v1/admin/bank-transactions?from=2026-01-01&to=2026-11-30", finance)).Status.ShouldBe(400, "aralık en fazla 92 gün");
    }

    // ───────── SMS gönderen adı ─────────

    [Fact]
    public async Task SmsSender_SupportApproves_FinanceSeesNoRows_RejectionKeepsPreviousName_BrandEmailed()
    {
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/brand/sms-sender", new { name = "KUAFORELIF" }, _elifToken, Key())).Status.ShouldBe(201);
        _factory.Clock.Advance(Duration.FromMinutes(1));
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/brand/sms-sender", new { name = "BERBERKAAN" }, _kaanToken, Key())).Status.ShouldBe(201);

        var (finance, _) = await AdminSignInAsync(FinancePhone);
        var financeQueue = (await _api.GetAsync("/api/v1/admin/approval-queue", finance)).Body;
        financeQueue.GetProperty("smsSenderCount").GetInt32().ShouldBe(0);
        financeQueue.GetProperty("items").GetArrayLength().ShouldBe(0);
        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/admin/sms-senders/{_elifBrand}/approve", new { }, finance, "W/\"1\"")).Status.ShouldBe(403);

        var (support, _) = await AdminSignInAsync(SupportPhone);
        var queue = (await _api.GetAsync("/api/v1/admin/approval-queue?type=SmsSender", support)).Body;
        queue.GetProperty("smsSenderCount").GetInt32().ShouldBe(2);
        var rows = queue.GetProperty("items").EnumerateArray().ToList();
        rows.Select(r => r.GetProperty("smsSender").GetProperty("requestedName").GetString()).ShouldBe(["KUAFORELIF", "BERBERKAAN"]);
        rows[0].GetProperty("smsSender").GetProperty("currentEffectiveName").GetString().ShouldBe("OMVILA");
        rows[0].GetProperty("id").GetString().ShouldBe(_elifBrand);

        var version = rows[0].GetProperty("smsSender").GetProperty("version").GetString()!;
        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/admin/sms-senders/{_elifBrand}/approve", new { }, support, "W/\"1\"")).Status.ShouldBe(412);
        var approved = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/admin/sms-senders/{_elifBrand}/approve", new { }, support, version);
        approved.Status.ShouldBe(200, approved.Body.ToString());
        approved.Body.GetProperty("status").GetString().ShouldBe("Approved");
        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/admin/sms-senders/{_elifBrand}/approve", new { }, support, version)).Status.ShouldBe(412);

        var kaanVersion = rows[1].GetProperty("smsSender").GetProperty("version").GetString()!;
        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/admin/sms-senders/{_kaanBrand}/reject", new { }, support, kaanVersion)).Status.ShouldBe(400);
        var rejected = await _api.WriteAsync(
            HttpMethod.Post, $"/api/v1/admin/sms-senders/{_kaanBrand}/reject", new { reason = "Başlık işletme unvanıyla uyuşmuyor." }, support, kaanVersion);
        rejected.Body.GetProperty("rejectionReason").GetString().ShouldBe("Başlık işletme unvanıyla uyuşmuyor.");

        var sms = (await _api.GetAsync("/api/v1/manage/brand/sms", _elifToken)).Body;
        sms.GetProperty("sender").GetProperty("effectiveName").GetString().ShouldBe("KUAFORELIF");
        (await _api.ScalarAsync<string>($"SELECT coalesce(approved_name, 'yok') || ':' || status FROM notifications.brand_sms_senders WHERE brand_id = '{_kaanBrand}'"))
            .ShouldBe("yok:Rejected");
        (await _api.ScalarAsync<string>("SELECT string_agg(action || ':' || entity_type, ',' ORDER BY id) FROM platform.audit_logs WHERE entity_type = 'BrandSmsSender'"))
            .ShouldBe("Approve:BrandSmsSender,Update:BrandSmsSender");

        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        var email = Emails().Sent.Single(e => string.Equals(e.EventCode, "owner.sms_sender_approved", StringComparison.Ordinal));
        email.Subject.ShouldBe("SMS gönderen adınız onaylandı");
        Emails().Sent.ShouldNotContain(e => string.Equals(e.EventCode, "owner.sms_sender_rejected", StringComparison.Ordinal), "Berber Kaan'ın işletme e-postası yok");

        // Bildirim paneli (T-132): karar markanın sahibine panel kaydı olarak da yazılır; e-postası olmayan marka da alır, push yok.
        await ShouldHaveSmsSenderBellAsync(_elifToken, "owner.sms_sender_approved", "SMS gönderen adınız onaylandı", "SMS'leriniz bundan sonra KUAFORELIF adıyla gider.");
        await ShouldHaveSmsSenderBellAsync(_kaanToken, "owner.sms_sender_rejected", "SMS gönderen adınız onaylanmadı", "BERBERKAAN onaylanmadı. Nedeni Ayarlar → Bildirimler ve SMS'te.");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.in_app_notifications WHERE event_code LIKE 'owner.sms_sender_%'")).ShouldBe(2L, "marka başına bir sahip");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code LIKE 'owner.sms_sender_%'")).ShouldBe(0L, "işletme push'u yok");
    }

    private async Task ShouldHaveSmsSenderBellAsync(string token, string eventCode, string title, string body)
    {
        var bell = (await _api.GetAsync("/api/v1/manage/notifications", token)).Body.GetProperty("items").EnumerateArray()
            .Single(n => string.Equals(n.GetProperty("event").GetString(), eventCode, StringComparison.Ordinal));
        bell.GetProperty("title").GetString().ShouldBe(title);
        bell.GetProperty("body").GetString().ShouldBe(body);
        bell.GetProperty("target").GetProperty("type").GetString().ShouldBe("SmsCredits");
    }

    // ───────── Abonelikler ─────────

    [Fact]
    public async Task Subscriptions_NextStepDerivedFromGraceDays_FiltersAndCounts()
    {
        await _api.ScalarAsync<int>($"UPDATE platform.subscriptions SET status = 'PastDue', past_due_since = '2026-11-19' WHERE brand_id = '{_kaanBrand}' RETURNING 1");
        var (support, _) = await AdminSignInAsync(SupportPhone);

        var page = await _api.GetAsync("/api/v1/admin/subscriptions", support);
        page.Status.ShouldBe(200, page.Body.ToString());
        var counts = page.Body.GetProperty("counts");
        $"{counts.GetProperty("trialing").GetInt32()}:{counts.GetProperty("pastDue").GetInt32()}:{counts.GetProperty("trialEndingThisWeek").GetInt32()}".ShouldBe("1:1:0");
        var items = page.Body.GetProperty("items").EnumerateArray().ToList();
        items.Select(i => i.GetProperty("brandName").GetString()).ShouldBe(["Berber Kaan", "Kuaför Elif"], "ödeme gecikti önce");
        items[0].GetProperty("pastDueDays").GetInt32().ShouldBe(6);
        Step(items[0]).ShouldBe("ReadOnlyOn:2026-11-26", "6. gün · yarın salt okunur (PastDueSince + 7)");
        Step(items[1]).ShouldBe("TrialEnds:2026-12-09");
        items[1].GetProperty("trialDaysLeft").GetInt32().ShouldBe(14);
        items[1].GetProperty("creditBalance").GetInt32().ShouldBe(100);
        items[1].GetProperty("creditsLow").GetBoolean().ShouldBeTrue();

        var pastDue = (await _api.GetAsync("/api/v1/admin/subscriptions?filter=PastDue", support)).Body.GetProperty("items");
        pastDue.GetArrayLength().ShouldBe(1);
        (await _api.GetAsync("/api/v1/admin/subscriptions?filter=CreditsLow&limit=1", support)).Body.GetProperty("nextCursor").GetString().ShouldBe("1");
        (await _api.GetAsync("/api/v1/admin/subscriptions?filter=Gold", support)).Status.ShouldBe(400);
    }

    // ───────── Denetim kaydı ─────────

    [Fact]
    public async Task AuditLog_FiltersSearchMasksAndExport_OnlyAdminExports()
    {
        await _api.ScalarAsync<int>($$"""
            INSERT INTO platform.audit_logs (actor_type, actor_id, brand_id, action, entity_type, entity_id, changes, occurred_utc) VALUES
              ('System', NULL, '{{_kaanBrand}}', 'Update', 'Subscription', NULL, '{"status": "ReadOnly"}', '2026-11-25 08:00:00+00'),
              ('System', NULL, '{{_elifBrand}}', 'Anonymize', 'BrandCustomer', NULL, '{"fullName": "Selin Öztürk", "note": "Arayın 0532 418 77 05"}', '2026-11-25 08:10:00+00'),
              ('System', NULL, NULL, 'Update', 'Plan', NULL, NULL, '2026-11-20 08:00:00+00')
            RETURNING 1
            """);
        var (support, _) = await AdminSignInAsync(SupportPhone);
        await _api.GetAsync($"/api/v1/admin/brands/{_elifBrand}", support);

        var all = await _api.GetAsync("/api/v1/admin/audit-log", support);
        all.Status.ShouldBe(200, all.Body.ToString());
        var entries = all.Body.GetProperty("items").EnumerateArray().ToList();
        entries.ShouldNotContain(e => string.Equals(e.GetProperty("entityType").GetString(), "Plan", StringComparison.Ordinal), "varsayılan son 24 saat");
        var view = entries.First(e => string.Equals(e.GetProperty("action").GetString(), "View", StringComparison.Ordinal));
        view.GetProperty("actor").GetProperty("name").GetString().ShouldBe("Ayşe Demir");
        view.GetProperty("actor").GetProperty("platformRole").GetString().ShouldBe("PlatformSupport");
        view.GetProperty("brand").GetProperty("name").GetString().ShouldBe("Kuaför Elif");
        var kvkk = (await _api.GetAsync("/api/v1/admin/audit-log?category=Kvkk", support)).Body.GetProperty("items").EnumerateArray().Single();
        kvkk.GetProperty("changes").GetProperty("fullName").GetString().ShouldBe("Selin Ö.");
        kvkk.GetProperty("changes").GetProperty("note").GetString().ShouldBe("Arayın 0532 *** ** 05");
        kvkk.GetProperty("actor").GetProperty("type").GetString().ShouldBe("System");

        Types(await _api.GetAsync("/api/v1/admin/audit-log?category=CreditsAndPlan", support)).ShouldBe(["Subscription"]);
        Types(await _api.GetAsync("/api/v1/admin/audit-log?category=View", support)).ShouldAllBe(t => string.Equals(t, "Brand", StringComparison.Ordinal));
        Types(await _api.GetAsync("/api/v1/admin/audit-log?q=Ayşe", support)).ShouldContain(t => string.Equals(t, "Brand", StringComparison.Ordinal));
        Types(await _api.GetAsync("/api/v1/admin/audit-log?q=Berber", support)).ShouldBe(["Subscription"]);
        Types(await _api.GetAsync("/api/v1/admin/audit-log?fromUtc=2026-11-19T00:00:00Z&toUtc=2026-11-21T00:00:00Z", support)).ShouldBe(["Plan"]);
        var paged = await _api.GetAsync("/api/v1/admin/audit-log?limit=1", support);
        var next = paged.Body.GetProperty("nextCursor").GetString()!;
        (await _api.GetAsync($"/api/v1/admin/audit-log?limit=1&cursor={next}", support)).Body.GetProperty("items")[0].GetProperty("id").GetInt64()
            .ShouldBeLessThan(paged.Body.GetProperty("items")[0].GetProperty("id").GetInt64());

        (await _api.SendRawAsync(HttpMethod.Get, "/api/v1/admin/audit-log/export", null, support)).Status.ShouldBe(403);
        var (admin, _) = await AdminSignInAsync(AdminPhone);
        (await _api.SendRawAsync(HttpMethod.Get, "/api/v1/admin/audit-log/export?fromUtc=2026-01-01T00:00:00Z", null, admin)).Status.ShouldBe(400);
        var export = await _api.SendRawAsync(HttpMethod.Get, "/api/v1/admin/audit-log/export?category=Kvkk", null, admin);
        export.Status.ShouldBe(200);
        export.ContentType!.ShouldStartWith("text/csv");
        export.ContentDisposition!.ShouldContain("denetim-kaydi-2026-11-25.csv");
        export.Body.Take(3).ShouldBe(Encoding.UTF8.GetPreamble());
        var lines = Encoding.UTF8.GetString(export.Body[3..]).Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        lines[0].ShouldBe("Zaman;Kim;Rol;İşlem;Varlık;Marka;Ayrıntı");
        lines[1].ShouldStartWith("\"25.11.2026 11:10\";\"Sistem\";\"\";\"Anonymize\";\"BrandCustomer\";\"Kuaför Elif\";");
        lines[1].ShouldContain("Selin Ö.");
        lines[1].ShouldNotContain("418 77");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM platform.audit_logs WHERE action = 'Export' AND entity_type = 'AuditLog'")).ShouldBe(1);
    }

    // Aynı hareket ikinci bildirime bağlanamaz; sonuçlanmış bildirim yeniden eşleşmez.
    private async Task SecondMatchIsRefusedAsync(string support, string noticeId, string bankTransactionId, string? matchedETag)
    {
        var second = await NotifyAsync(_kaanToken, "Subscription");
        var secondId = second.Json.GetProperty("id").GetString()!;
        var secondDetail = await _api.GetAsync($"/api/v1/admin/payment-notices/{secondId}", support);
        var conflict = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/admin/payment-notices/{secondId}/match", new { bankTransactionId }, support, secondDetail.ETag);
        conflict.Status.ShouldBe(409);
        conflict.Body.GetProperty("code").GetString().ShouldBe("bank_transaction_already_matched");
        var again = await _api.WriteAsync(HttpMethod.Post, $"/api/v1/admin/payment-notices/{noticeId}/match", new { bankTransactionId }, support, matchedETag);
        again.Status.ShouldBe(409);
        again.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");
    }

    // KK2: fatura Paid, abonelik Active, hareket bağlandı, denetim; olay işlenince markaya panel kaydı ve e-posta.
    private async Task MatchedEffectsAsync(string noticeId, string bankTransactionId)
    {
        (await _api.ScalarAsync<string>($"SELECT status FROM platform.subscriptions WHERE brand_id = '{_elifBrand}'")).ShouldBe("Active");
        (await _api.ScalarAsync<string>($"SELECT status || ':' || paid_via FROM platform.subscription_invoices WHERE brand_id = '{_elifBrand}'"))
            .ShouldBe("Paid:BankTransfer");
        (await _api.ScalarAsync<Guid>($"SELECT matched_notice_id FROM platform.bank_transactions WHERE id = '{bankTransactionId}'")).ToString().ShouldBe(noticeId);
        (await _api.ScalarAsync<long>($"SELECT count(*) FROM platform.audit_logs WHERE action = 'Match' AND entity_type = 'PaymentNotice' AND entity_id = '{noticeId}'"))
            .ShouldBe(1);

        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        (await _api.ScalarAsync<string>("SELECT title || ' · ' || body FROM notifications.in_app_notifications WHERE event_code = 'owner.payment_notice_matched'"))
            .ShouldBe("Havaleniz eşleşti · Abonelik ödemeniz alındı; aboneliğiniz aktif.");
        var email = Emails().Sent.Single(e => string.Equals(e.EventCode, "owner.payment_notice_matched", StringComparison.Ordinal));
        email.To.ShouldBe("salon@example.com");
        email.Subject.ShouldBe("Havaleniz eşleşti");

    }

    private async Task ReceiptDownloadsAsync(string support, string noticeId)
    {
        var receipt = await _api.SendRawAsync(HttpMethod.Get, $"/api/v1/admin/payment-notices/{noticeId}/receipt", null, support);
        receipt.Status.ShouldBe(200);
        receipt.ContentType.ShouldBe("application/pdf");
        receipt.Text.ShouldStartWith("%PDF-1.4");
        (await _api.ScalarAsync<string>($"SELECT changes->>'section' FROM platform.audit_logs WHERE entity_id = '{noticeId}' AND action = 'View' ORDER BY id DESC LIMIT 1"))
            .ShouldBe("Receipt");
    }

    private static string Summary(JsonElement r) =>
        $"{r.GetProperty("dryRun").GetBoolean()}:{r.GetProperty("totalRows").GetInt32()}:{r.GetProperty("imported").GetInt32()}:{r.GetProperty("duplicates").GetInt32()}:{r.GetProperty("skippedOutgoing").GetInt32()}:{r.GetProperty("invalidRows").GetInt32()}";

    private static string Step(JsonElement item) =>
        $"{item.GetProperty("nextStep").GetProperty("type").GetString()}:{item.GetProperty("nextStep").GetProperty("date").GetString()}";

    private static List<string> Types(ManageApi.Response response) =>
        [.. response.Body.GetProperty("items").EnumerateArray().Select(e => e.GetProperty("entityType").GetString()!)];

    private static string CultureInfoInvariant(FormattableString value) => FormattableString.Invariant(value);

    private static object PlanBody(long monthly) => new
    {
        plans = new[]
        {
            new
            {
                code = "salon_plus",
                name = "Salon Plus",
                description = "Büyüyen salonlar",
                monthlyPrice = new { amountMinor = monthly, currency = "TRY" },
                yearlyPrice = (object?)null,
                includedBranchCount = (int?)null,
                extraBranchPrice = (object?)null,
                staffLimit = (int?)null,
                includedSmsCredits = (int?)null,
                features = new { walkInQueue = true, waitlist = true, packages = true, crossBranchReports = true, prioritySupport = false },
                isPublic = true,
                isActive = true,
            },
        },
    };

    private async Task SetPlanPriceAsync(string token, long monthly)
    {
        var plans = await _api.GetAsync("/api/v1/admin/plans", token);
        var saved = await _api.WriteAsync(HttpMethod.Put, "/api/v1/admin/plans", PlanBody(monthly), token, plans.ETag);
        saved.Status.ShouldBe(200, saved.Body.ToString());
    }

    private async Task CreateBankTransactionAsync(string token, string reference, long amount, string? description, string valueDate)
    {
        var created = await _api.SendAsync(HttpMethod.Post, "/api/v1/admin/bank-transactions", new
        {
            valueDate,
            amount = new { amountMinor = amount, currency = "TRY" },
            senderName = "ELİF KAYA",
            description,
            bankReference = reference,
        }, token, Key());
        created.Status.ShouldBe(201, created.Body.ToString());
        created.Body.GetProperty("source").GetString().ShouldBe("Manual");
    }

    private async Task<string> PackageIdAsync(string code) =>
        (await _api.ScalarAsync<Guid>($"SELECT id FROM platform.credit_packages WHERE code = '{code}'")).ToString();

    private Task<ManageApi.RawResponse> ImportAsync(string token, string csv, bool dryRun)
    {
        var form = new MultipartFormDataContent();
        var file = new ByteArrayContent(Encoding.UTF8.GetBytes(csv));
        file.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
        form.Add(file, "file", "ekstre-2026-11-18.csv");
        form.Add(new StringContent(dryRun ? "true" : "false"), "dryRun");
        return _api.SendRawAsync(HttpMethod.Post, "/api/v1/admin/bank-transactions/import", form, token);
    }

    private Task<ManageApi.RawResponse> NotifyAsync(string token, string purpose, string? packageId = null, (string Name, byte[] Bytes)? receipt = null)
    {
        var form = new MultipartFormDataContent { { new StringContent(purpose), "purpose" } };
        if (packageId is not null)
        {
            form.Add(new StringContent(packageId), "creditPackageId");
        }

        if (receipt is { } r)
        {
            var content = new ByteArrayContent(r.Bytes);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            form.Add(content, "receipt", r.Name);
        }

        return _api.SendRawAsync(HttpMethod.Post, "/api/v1/manage/subscription/payment-notices", form, token, Key());
    }

    private FakeEmailSender Emails() => _factory.Services.GetRequiredService<FakeEmailSender>();

    private async Task<int> CreatePlatformUserAsync(string phone, string name, string role)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                ["platform-phone"] = phone,
                ["platform-name"] = name,
                ["platform-role"] = role,
            })
            .Build();
        return await PlatformUserCommand.RunAsync(_factory.Services, configuration, TextWriter.Null, Ct);
    }

    private async Task<(string Access, string Refresh)> AdminSignInAsync(string phone)
    {
        _factory.Clock.Advance(Duration.FromMinutes(2));
        var otp = await _api.SendAsync(HttpMethod.Post, "/api/v1/admin/auth/otp/request", new { phone });
        otp.Status.ShouldBe(202, otp.Body.ToString());
        var code = await _api.ScalarAsync<string>($"""
            SELECT payload->>'kod' FROM notifications.notification_outbox
            WHERE event_code = 'otp.code' AND recipient_phone = '{phone}' ORDER BY created_utc DESC, id DESC LIMIT 1
            """);
        var verified = await _api.SendAsync(HttpMethod.Post, "/api/v1/admin/auth/otp/verify", new { otpId = otp.Body.GetProperty("otpId").GetGuid(), code });
        verified.Status.ShouldBe(200, verified.Body.ToString());
        return (verified.Body.GetProperty("accessToken").GetString()!, verified.Body.GetProperty("refreshToken").GetString()!);
    }

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8124-{++_requests:D12}" };
}
