using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Omvila.Infrastructure.Email;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-06 kabul kriterleri 1 (deneme bitti → PastDue, 7 gün → ReadOnly: rezervasyon kapalı, yeni randevu 403, var olan
// hatırlatma gider) ve 2 (salon tarafı: "Havaleyi bildir"); durum-makinesi.md § 8; bildirimler.md § E-posta; roller-yetkiler.md
// subscription.manage (yalnız Owner); sözleşme manage-subscription; K1, K5; T-118.
// Saat: 2026-11-25 09:00Z (Çarşamba 12:00 İstanbul); deneme 14 gün → 2026-12-09.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class SubscriptionTests(PostgresFixture db) : IAsyncLifetime
{
    private const string OwnerPhone = "+905324187705";
    private const string AdvanceJob = "Omvila.Modules.Platform.Jobs.AdvanceSubscriptionStatuses, Omvila.Modules.Platform";
    private const string DispatchSmsJob = "Omvila.Modules.Notifications.Jobs.DispatchNotifications, Omvila.Modules.Notifications";
    private const string OmvilaIban = "TR330006100519786457841326";

    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private string _merkez = null!;
    private string _elif = null!;
    private int _requests;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db, new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["Platform:OmvilaBankAccount:AccountHolderName"] = "OMVILA Yazılım A.Ş.",
            ["Platform:OmvilaBankAccount:Iban"] = OmvilaIban,
            ["Platform:VatRatePercent"] = "20",
        });
        _api = new ManageApi(_factory, db);
        _token = await _api.SignupAsync(OwnerPhone, "Kuaför Elif");
        var session = (await _api.GetAsync("/api/v1/manage/session", _token)).Body;
        _merkez = session.GetProperty("branches")[0].GetProperty("id").GetString()!;
        _elif = session.GetProperty("membership").GetProperty("staffId").GetString()!;
        await _api.ScalarAsync<int>("UPDATE tenancy.brands SET email = 'salon@example.com' RETURNING 1");
    }

    public async ValueTask DisposeAsync()
    {
        await _api.ScalarAsync<int>("UPDATE platform.plans SET monthly_price_minor = NULL, included_branch_count = NULL, extra_branch_price_minor = NULL RETURNING 1");
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task Criterion1_TrialEnds_PastDue_ThenReadOnly_BookingClosed_NewAppointmentForbidden_ExistingReminderSent()
    {
        var (kesim, selin) = await PrepareCalendarAsync();
        var existing = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/appointments", new
        {
            branchId = _merkez,
            brandCustomerId = selin,
            channel = "Panel",
            startUtc = "2026-12-18T09:00:00Z",
            items = new[] { new { serviceId = kesim, staffId = _elif } },
            notifyCustomer = true,
        }, _token, Key());
        existing.Status.ShouldBe(201, existing.Body.ToString());
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);

        // 3 gün kala: owner.trial_ending (bir kez).
        await AdvanceToAsync(Instant.FromUtc(2026, 12, 6, 9, 0));
        await _api.RunJobAsync(AdvanceJob);
        await _api.RunJobAsync(AdvanceJob);
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        var trialEnding = Emails().ShouldHaveSingleItem();
        trialEnding.EventCode.ShouldBe("owner.trial_ending");
        trialEnding.To.ShouldBe("salon@example.com");
        trialEnding.Subject.ShouldBe("Deneme sürenizin bitmesine 3 gün kaldı");
        trialEnding.TextBody.ShouldContain("9 Aralık'ta bitiyor");

        await AssertPastDueAtTrialEndAsync();

        // 6. gün: hâlâ PastDue; her şey çalışır.
        await AdvanceToAsync(Instant.FromUtc(2026, 12, 15, 9, 0));
        await _api.RunJobAsync(AdvanceJob);
        (await _api.GetAsync("/api/v1/manage/subscription", _token)).Body.GetProperty("status").GetString().ShouldBe("PastDue");

        // 7 gün → ReadOnly: rezervasyon sayfası kapalı, panelde yeni randevu 403, ödeme uçları açık.
        await AdvanceToAsync(Instant.FromUtc(2026, 12, 16, 9, 0));
        await _api.RunJobAsync(AdvanceJob);
        var readOnly = (await _api.GetAsync("/api/v1/manage/subscription", _token)).Body;
        readOnly.GetProperty("status").GetString().ShouldBe("ReadOnly");
        readOnly.GetProperty("readOnlySince").GetString().ShouldBe("2026-12-16");
        (await _api.SendAsync(HttpMethod.Get, "/api/v1/public/brands/kuafor-elif", null)).Body.GetProperty("bookingEnabled").GetBoolean().ShouldBeFalse();
        var created = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/appointments", new
        {
            branchId = _merkez,
            brandCustomerId = selin,
            channel = "Panel",
            startUtc = "2026-12-21T09:00:00Z",
            items = new[] { new { serviceId = kesim, staffId = _elif } },
            notifyCustomer = true,
        }, _token, Key());
        created.Status.ShouldBe(403);
        created.Body.GetProperty("code").GetString().ShouldBe("subscription_read_only");
        (await NotifyAsync(Form("Subscription"))).Status.ShouldBe(201, "salt okunurda ödeme yolu açık");

        // Var olan randevunun hatırlatması gider (K5).
        await AdvanceToAsync(Instant.FromUtc(2026, 12, 17, 9, 30));
        await _api.RunJobAsync(DispatchSmsJob);
        (await _api.ScalarAsync<long>(
            "SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'appointment.reminder' AND status = 'Sent'")).ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task ReadOnly_CancelledAfterConfiguredDays_ThenBrandArchived()
    {
        await AdvanceToAsync(Instant.FromUtc(2026, 12, 9, 9, 0));
        await _api.RunJobAsync(AdvanceJob);
        await AdvanceToAsync(Instant.FromUtc(2026, 12, 16, 9, 0));
        await _api.RunJobAsync(AdvanceJob);

        await AdvanceToAsync(Instant.FromUtc(2027, 1, 15, 9, 0));
        await _api.RunJobAsync(AdvanceJob);
        (await _api.ScalarAsync<string>("SELECT status FROM platform.subscriptions")).ShouldBe("Cancelled");
        (await _api.ScalarAsync<string>("SELECT status FROM tenancy.brands")).ShouldNotBe("Archived", StringComparer.Ordinal);

        _factory.Clock.Advance(Duration.FromDays(30));
        await _api.RunJobAsync(AdvanceJob);
        (await _api.ScalarAsync<string>("SELECT status FROM tenancy.brands")).ShouldBe("Archived");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM platform.subscriptions WHERE brand_archived_utc IS NOT NULL")).ShouldBe(1);
    }

    [Fact]
    public async Task GetSubscription_Trialing_UnconfiguredPrice_NullAmount_BankAccountFromConfig()
    {
        var detail = await _api.GetAsync("/api/v1/manage/subscription", _token);

        detail.Status.ShouldBe(200);
        var body = detail.Body;
        body.GetProperty("status").GetString().ShouldBe("Trialing");
        body.GetProperty("plan").GetProperty("code").GetString().ShouldBe("salon_plus");
        body.GetProperty("billingCycle").GetString().ShouldBe("Monthly");
        body.GetProperty("paymentMethod").ValueKind.ShouldBe(JsonValueKind.Null);
        body.GetProperty("trialEndsOn").GetString().ShouldBe("2026-12-09");
        body.GetProperty("readOnlyOn").ValueKind.ShouldBe(JsonValueKind.Null);
        body.GetProperty("branches").GetProperty("count").GetInt32().ShouldBe(1);
        body.GetProperty("branches").GetProperty("included").ValueKind.ShouldBe(JsonValueKind.Null);
        body.GetProperty("staff").GetProperty("count").GetInt32().ShouldBe(1);
        body.GetProperty("amountDue").ValueKind.ShouldBe(JsonValueKind.Null, "fiyat tanımlı değil (S-14)");
        body.GetProperty("transferCode").GetString()!.ShouldMatch("^ABN-KE-[0-9]{4}$");
        body.GetProperty("omvilaBankAccount").GetProperty("iban").GetString().ShouldBe(OmvilaIban);
        body.GetProperty("omvilaBankAccount").GetProperty("bankName").ValueKind.ShouldBe(JsonValueKind.Null);
        body.GetProperty("pendingNotice").ValueKind.ShouldBe(JsonValueKind.Null);
    }

    [Fact]
    public async Task PricedPlan_InvoiceIssuedAtTrialEnd_ListedAndPdf_OtherBrand404()
    {
        await _api.ScalarAsync<int>("UPDATE platform.plans SET monthly_price_minor = 100000, included_branch_count = 1, extra_branch_price_minor = 25000 RETURNING 1");
        var amountDue = (await _api.GetAsync("/api/v1/manage/subscription", _token)).Body.GetProperty("amountDue");
        amountDue.GetProperty("amountMinor").GetInt64().ShouldBe(120000, "100.000 + %20 KDV");

        await AdvanceToAsync(Instant.FromUtc(2026, 12, 9, 9, 0));
        await _api.RunJobAsync(AdvanceJob);
        await _api.RunJobAsync(AdvanceJob);

        var page = await _api.GetAsync("/api/v1/manage/subscription/invoices", _token);
        page.Status.ShouldBe(200);
        var invoice = page.Body.GetProperty("items").EnumerateArray().ShouldHaveSingleItem();
        invoice.GetProperty("invoiceNumber").GetString().ShouldBe("OMV-2026-000001");
        invoice.GetProperty("periodStart").GetString().ShouldBe("2026-12-09");
        invoice.GetProperty("periodEnd").GetString().ShouldBe("2027-01-08");
        invoice.GetProperty("amount").GetProperty("amountMinor").GetInt64().ShouldBe(100000);
        invoice.GetProperty("tax").GetProperty("amountMinor").GetInt64().ShouldBe(20000);
        invoice.GetProperty("total").GetProperty("amountMinor").GetInt64().ShouldBe(120000);
        invoice.GetProperty("status").GetString().ShouldBe("Issued");
        invoice.GetProperty("paidUtc").ValueKind.ShouldBe(JsonValueKind.Null);
        page.Body.GetProperty("nextCursor").ValueKind.ShouldBe(JsonValueKind.Null);
        var invoiceId = invoice.GetProperty("id").GetString();

        var pdf = await _api.SendRawAsync(HttpMethod.Get, $"/api/v1/manage/subscription/invoices/{invoiceId}/pdf", null, _token);
        pdf.Status.ShouldBe(200);
        pdf.ContentType.ShouldBe("application/pdf");
        pdf.ContentDisposition!.ShouldContain("OMV-2026-000001.pdf");
        System.Text.Encoding.Latin1.GetString(pdf.Body).ShouldStartWith("%PDF-");

        var other = await _api.SignupAsync("+905324187706", "Berber Ali");
        (await _api.SendRawAsync(HttpMethod.Get, $"/api/v1/manage/subscription/invoices/{invoiceId}/pdf", null, other)).Status.ShouldBe(404);
        (await _api.GetAsync("/api/v1/manage/subscription/invoices", other)).Body.GetProperty("items").GetArrayLength().ShouldBe(0);
        (await _api.GetAsync("/api/v1/manage/subscription/invoices?limit=0", _token)).Status.ShouldBe(400);
    }

    [Fact]
    public async Task NotifyTransfer_Subscription_WithReceipt_ThenPendingConflict_ShownOnDetail()
    {
        var first = await NotifyAsync(Form("Subscription", senderName: "Elif Kaya", receipt: ("dekont.pdf", "%PDF-1.4 dekont"u8.ToArray())));

        first.Status.ShouldBe(201, first.Text);
        var notice = first.Json;
        notice.GetProperty("purpose").GetString().ShouldBe("Subscription");
        notice.GetProperty("transferCode").GetString()!.ShouldMatch("^ABN-KE-[0-9]{4}$");
        notice.GetProperty("hasReceipt").GetBoolean().ShouldBeTrue();
        notice.GetProperty("status").GetString().ShouldBe("Pending");
        notice.GetProperty("credits").ValueKind.ShouldBe(JsonValueKind.Null);
        notice.GetProperty("senderName").GetString().ShouldBe("Elif Kaya");
        (await _api.ScalarAsync<string>("SELECT channel || ':' || receipt_content_type FROM platform.payment_notices")).ShouldBe("Panel:application/pdf");

        var second = await NotifyAsync(Form("Subscription"));
        second.Status.ShouldBe(409);
        second.Json.GetProperty("code").GetString().ShouldBe("payment_notice_already_pending");
        second.Json.GetProperty("meta").GetProperty("paymentNoticeId").GetString().ShouldBe(notice.GetProperty("id").GetString());

        var detail = (await _api.GetAsync("/api/v1/manage/subscription", _token)).Body;
        detail.GetProperty("pendingNotice").GetProperty("id").GetString().ShouldBe(notice.GetProperty("id").GetString());
    }

    [Fact]
    public async Task NotifyTransfer_InvalidInput_400_UnknownPackage_404()
    {
        var tooLarge = await NotifyAsync(Form("Subscription", receipt: ("dekont.pdf", [.. "%PDF-"u8, .. new byte[5 * 1024 * 1024]])));
        tooLarge.Status.ShouldBe(400);
        tooLarge.Json.GetProperty("errors").GetProperty("receipt")[0].GetString().ShouldBe("too_large");

        var gif = await NotifyAsync(Form("Subscription", receipt: ("dekont.pdf", "GIF89a"u8.ToArray())));
        gif.Json.GetProperty("errors").GetProperty("receipt")[0].GetString().ShouldBe("invalid_format");

        var noPurpose = await NotifyAsync(Form(null));
        noPurpose.Json.GetProperty("errors").GetProperty("purpose")[0].GetString().ShouldBe("required");

        var noPackage = await NotifyAsync(Form("Credits"));
        noPackage.Json.GetProperty("errors").GetProperty("creditPackageId")[0].GetString().ShouldBe("required");

        (await NotifyAsync(Form("Credits", packageId: "0193a8f0-0000-7000-8000-00000000dead"))).Status.ShouldBe(404);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM platform.payment_notices")).ShouldBe(0);
    }

    [Fact]
    public async Task CreditPackages_ListAndPurchaseInstructions_ThenNotifyCredits()
    {
        var list = await _api.GetAsync("/api/v1/manage/credit-packages", _token);
        list.Status.ShouldBe(200);
        var items = list.Body.GetProperty("items").EnumerateArray().ToList();
        items.Select(i => i.GetProperty("credits").GetInt32()).ShouldBe([1000, 5000]);
        items[0].GetProperty("price").ValueKind.ShouldBe(JsonValueKind.Null);
        var medium = items[1].GetProperty("id").GetString()!;

        var purchase = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/credits/purchase", new { creditPackageId = medium }, _token);
        purchase.Status.ShouldBe(200);
        purchase.Body.GetProperty("credits").GetInt32().ShouldBe(5000);
        purchase.Body.GetProperty("amount").ValueKind.ShouldBe(JsonValueKind.Null);
        purchase.Body.GetProperty("transferCode").GetString()!.ShouldMatch("^KNT-KE-[0-9]{4}$");
        purchase.Body.GetProperty("omvilaBankAccount").GetProperty("accountHolderName").GetString().ShouldBe("OMVILA Yazılım A.Ş.");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM platform.payment_notices")).ShouldBe(0, "kontör siparişi kaydı yok (K1, T-116)");
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/credits/purchase", new { creditPackageId = "0193a8f0-0000-7000-8000-00000000dead" }, _token))
            .Status.ShouldBe(404);
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/credits/purchase", new { }, _token)).Status.ShouldBe(400);

        var credits = await NotifyAsync(Form("Credits", packageId: medium));
        credits.Status.ShouldBe(201);
        credits.Json.GetProperty("credits").GetInt32().ShouldBe(5000);
        credits.Json.GetProperty("transferCode").GetString().ShouldBe(purchase.Body.GetProperty("transferCode").GetString());
        (await NotifyAsync(Form("Subscription"))).Status.ShouldBe(201, "abonelik bildirimi kontör bildiriminden bağımsız");
    }

    [Fact]
    public async Task SubscriptionEndpoints_OwnerOnly_OthersForbidden()
    {
        var manager = await _api.AddMemberAndSignInAsync(_token, "+905324187711", "BranchManager", _merkez);

        (await _api.GetAsync("/api/v1/manage/subscription", manager)).Status.ShouldBe(403);
        (await _api.GetAsync("/api/v1/manage/subscription/invoices", manager)).Status.ShouldBe(403);
        (await _api.SendRawAsync(HttpMethod.Get, "/api/v1/manage/subscription/invoices/0193a8f0-0000-7000-8000-00000000dead/pdf", null, manager)).Status.ShouldBe(403);
        (await _api.GetAsync("/api/v1/manage/credit-packages", manager)).Status.ShouldBe(403);
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/credits/purchase", new { creditPackageId = "0193a8f0-0000-7000-8000-00000000dead" }, manager)).Status.ShouldBe(403);
        (await NotifyAsync(Form("Subscription"), manager)).Status.ShouldBe(403);
        (await _api.SendRawAsync(HttpMethod.Get, "/api/v1/manage/subscription/invoices/0193a8f0-0000-7000-8000-00000000dead/pdf", null, _token)).Status.ShouldBe(404);
    }

    [Fact]
    public async Task CreditsLowEmail_SentOnThresholdCrossing_NotWithoutBrandEmail()
    {
        var brandId = (await _api.GetAsync("/api/v1/manage/session", _token)).Body.GetProperty("brand").GetProperty("id").GetString();
        await EnqueueCreditsLowAsync(brandId!);
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);

        var email = Emails().Single(e => string.Equals(e.EventCode, "owner.credits_low", StringComparison.Ordinal));
        email.Subject.ShouldBe("SMS kontörünüz azaldı: 100 SMS kaldı");
        email.To.ShouldBe("salon@example.com");
        email.HtmlBody.ShouldContain("Kontör paketi al");

        _factory.Services.GetRequiredService<FakeEmailSender>().Clear();
        await _api.ScalarAsync<int>("UPDATE tenancy.brands SET email = NULL RETURNING 1");
        await EnqueueCreditsLowAsync(brandId!);
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        Emails().ShouldBeEmpty();
    }

    // Deneme bitti, ödeme yok → PastDue (turuncu şerit) + owner.subscription_payment_failed; fiyat yok → fatura kesilmez.
    private async Task AssertPastDueAtTrialEndAsync()
    {
        await AdvanceToAsync(Instant.FromUtc(2026, 12, 9, 9, 0));
        await _api.RunJobAsync(AdvanceJob);
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        var pastDue = (await _api.GetAsync("/api/v1/manage/subscription", _token)).Body;
        pastDue.GetProperty("status").GetString().ShouldBe("PastDue");
        pastDue.GetProperty("pastDueSince").GetString().ShouldBe("2026-12-09");
        pastDue.GetProperty("readOnlyOn").GetString().ShouldBe("2026-12-16");
        pastDue.GetProperty("trialEndsOn").ValueKind.ShouldBe(JsonValueKind.Null);
        pastDue.GetProperty("currentPeriodStart").GetString().ShouldBe("2026-12-09");
        (await _api.GetAsync("/api/v1/manage/session", _token)).Body.GetProperty("subscription").GetProperty("status").GetString().ShouldBe("PastDue");
        (await _api.GetAsync("/api/v1/manage/subscription/invoices", _token)).Body.GetProperty("items").GetArrayLength().ShouldBe(0);
        var failed = Emails().Single(e => string.Equals(e.EventCode, "owner.subscription_payment_failed", StringComparison.Ordinal));
        failed.Subject.ShouldBe("Abonelik ödemeniz alınamadı");
        failed.TextBody.ShouldContain("16 Aralık: Panel salt okunur olur");
        failed.HtmlBody.ShouldContain("Havale ile öde");
    }

    private IReadOnlyCollection<EmailMessage> Emails() => _factory.Services.GetRequiredService<FakeEmailSender>().Sent;

    private Task<int> EnqueueCreditsLowAsync(string brandId) =>
        _api.ScalarAsync<int>($$"""
            INSERT INTO infra.outbox_events (id, type, payload, occurred_utc, available_utc, attempts)
            VALUES (gen_random_uuid(), 'Omvila.Modules.Notifications.Contracts.SmsCreditsLow',
              '{"brandId":"{{brandId}}","balance":100,"threshold":200}', '2026-11-25T00:00:00Z', '2026-11-25T00:00:00Z', 0)
            RETURNING 1
            """);

    private async Task<(string Service, string Customer)> PrepareCalendarAsync()
    {
        var week = new
        {
            days = Enumerable.Range(1, 6).Select(d => new { dayOfWeek = d, intervals = new[] { new { start = "09:00", end = "19:00" } } }).ToArray(),
        };
        var hours = await _api.GetAsync($"/api/v1/manage/branches/{_merkez}/working-hours", _token);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/branches/{_merkez}/working-hours", week, _token, hours.ETag)).Status.ShouldBe(200);
        var elif = await _api.GetAsync($"/api/v1/manage/staff/{_elif}", _token);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{_elif}/branches/{_merkez}/working-hours", week, _token, elif.ETag)).Status.ShouldBe(200);
        var category = (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/service-categories", new { name = "Saç" }, _token, Key())).Body.GetProperty("id").GetString()!;
        var service = (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/services", new
        {
            categoryId = category,
            name = "Saç kesimi",
            durationMinutes = 45,
            price = new { amountMinor = 45000, currency = "TRY" },
            priceDisplay = "Fixed",
            deposit = new { mode = "Inherit" },
            isOnlineBookable = true,
            staffIds = new[] { _elif },
        }, _token, Key())).Body.GetProperty("id").GetString()!;
        var customer = (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/customers", new { fullName = "Selin Öztürk", phone = "+905550004418" }, _token, Key()))
            .Body.GetProperty("id").GetString()!;
        return (service, customer);
    }

    /// <summary>Saati verilen ana ilerletir ve erişim jetonu süresi dolmasın diye yeniden giriş yapar.</summary>
    private async Task AdvanceToAsync(Instant target)
    {
        var now = _factory.Clock.GetCurrentInstant();
        if (target > now)
        {
            _factory.Clock.Advance(target - now);
        }

        _token = (await _api.SignInAsync(OwnerPhone)).Body.GetProperty("accessToken").GetString()!;
    }

    private Task<ManageApi.RawResponse> NotifyAsync(MultipartFormDataContent form, string? token = null) =>
        _api.SendRawAsync(HttpMethod.Post, "/api/v1/manage/subscription/payment-notices", form, token ?? _token, Key());

    private static MultipartFormDataContent Form(string? purpose, string? packageId = null, string? senderName = null, (string Name, byte[] Bytes)? receipt = null)
    {
        var form = new MultipartFormDataContent();
        if (purpose is not null)
        {
            form.Add(new StringContent(purpose), "purpose");
        }

        if (packageId is not null)
        {
            form.Add(new StringContent(packageId), "creditPackageId");
        }

        if (senderName is not null)
        {
            form.Add(new StringContent(senderName), "senderName");
        }

        if (receipt is { } file)
        {
            var content = new ByteArrayContent(file.Bytes);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            form.Add(content, "receipt", file.Name);
        }

        return form;
    }

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{++_requests:D12}" };
}
