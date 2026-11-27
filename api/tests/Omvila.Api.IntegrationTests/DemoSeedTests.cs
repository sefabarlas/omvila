using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Omvila.Host.Seeding;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: acik-sorular.md T-74 (demo verisi komutu `--role=seed [--reset]`): idempotent, Production'da red, --reset yalnız
// demo markasını siler ve yeniden kurar, sahip ve personel panele girebilir, kurulum SMS üretmez.
// Saat: 2026-11-25 09:00Z (Çarşamba 12:00 İstanbul); onay bekleyen talepler Perşembe ve Cuma, Elif ve Zeynep (4 talep).
// T-85: kapora hesabı ve politikası, aynı günlerin ikinci randevusunda dört kapora durumu (Awaiting, CustomerReported, Received
// tahsilatla, Waived); --reset Sales tahsilatlarını da siler (RowsAsync brand_id taşıyan bütün tablolar).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class DemoSeedTests(PostgresFixture db) : IAsyncLifetime
{
    private const string Brand = "(SELECT id FROM tenancy.brands WHERE slug = 'kuafor-elif')";
    private const string AuditTable = "platform.audit_logs";
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db);
        _api = new ManageApi(_factory, db);
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task Seed_CreatesDemoBrand_AndSecondRunChangesNothing()
    {
        var first = await SeedAsync(reset: false);
        first.Code.ShouldBe(0);
        first.Output.ShouldContain("kuafor-elif");
        first.Output.ShouldContain("0555 000 00 01");

        (await CountAsync($"SELECT count(*) FROM tenancy.brands WHERE id = {Brand} AND status = 'Active'")).ShouldBe(1);
        (await CountAsync($"SELECT count(*) FROM tenancy.branches WHERE brand_id = {Brand} AND time_zone_id = 'Europe/Istanbul'")).ShouldBe(2);
        (await CountAsync($"SELECT count(*) FROM tenancy.staff WHERE brand_id = {Brand}")).ShouldBe(5);
        (await CountAsync($"SELECT count(*) FROM identity.user_memberships WHERE brand_id = {Brand}")).ShouldBe(5);
        (await CountAsync($"SELECT count(*) FROM catalog.service_categories WHERE brand_id = {Brand}")).ShouldBe(4);
        (await CountAsync($"SELECT count(*) FROM catalog.services WHERE brand_id = {Brand} AND base_price_minor > 0")).ShouldBe(10);
        (await CountAsync($"SELECT count(*) FROM customers.brand_customers WHERE brand_id = {Brand}")).ShouldBe(15);
        (await CountAsync($"SELECT coalesce(sum(amount), 0) FROM notifications.credit_transactions WHERE brand_id = {Brand}")).ShouldBe(1000);
        (await CountAsync($"SELECT count(*) FROM platform.subscriptions WHERE brand_id = {Brand} AND status = 'Trialing'")).ShouldBe(1);

        // Durumlar: Onaylandı, Onay bekliyor (süresi dolmamış), Geldi, Gelmedi, İptal; geçmiş ve gelecek iki hafta.
        foreach (var status in new[] { "Confirmed", "CheckedIn", "NoShow", "Cancelled" })
        {
            (await CountAsync($"SELECT count(*) FROM scheduling.appointments WHERE brand_id = {Brand} AND status = '{status}'")).ShouldBeGreaterThan(0, status);
        }

        (await CountAsync($"SELECT count(*) FROM scheduling.appointments WHERE brand_id = {Brand} AND status = 'PendingApproval' AND deposit_required_minor IS NULL AND hold_expires_utc > '2026-11-25T09:00:00Z'"))
            .ShouldBe(4);
        (await CountAsync($"SELECT count(*) FROM scheduling.appointments WHERE brand_id = {Brand} AND status = 'Confirmed' AND start_utc < '2026-11-25T09:00:00Z'"))
            .ShouldBe(0, "geçmişte onaylı randevu kalırsa MarkNoShows işi onları Gelmedi yapar");
        (await CountAsync($"SELECT count(*) FROM scheduling.appointments WHERE brand_id = {Brand} AND start_utc < '2026-11-10T21:00:00Z'")).ShouldBe(0);
        (await CountAsync($"SELECT count(*) FROM scheduling.appointments WHERE brand_id = {Brand} AND start_utc >= '2026-12-09T21:00:00Z'")).ShouldBe(0);
        (await CountAsync($"SELECT count(*) FROM customers.brand_customers WHERE brand_id = {Brand} AND no_show_count = 2")).ShouldBe(2);

        // Kurulum bildirim üretmez: SMS kuyruğu ve entegrasyon olayı yok.
        (await CountAsync($"SELECT count(*) FROM notifications.notification_outbox WHERE brand_id = {Brand}")).ShouldBe(0);
        (await CountAsync("SELECT count(*) FROM infra.outbox_events")).ShouldBe(0);

        var before = await RowsAsync(await BrandIdAsync());
        var second = await SeedAsync(reset: false);
        second.Code.ShouldBe(0);
        second.Output.ShouldContain("zaten var");
        (await RowsAsync(await BrandIdAsync())).ShouldBe(before);
        (await CountAsync("SELECT count(*) FROM tenancy.brands")).ShouldBe(3, "Kuaför Elif ve iki küçük demo markası (T-131)");
    }

    [Fact]
    public async Task Seed_AddsDepositAccountAndDepositAppointments()
    {
        (await SeedAsync(reset: false)).Code.ShouldBe(0);

        // Aktif hesap, RiskyCustomersOnly + eşik 2, Balyaj %30 ve Keratin 150 ₺; dört kapora durumu, yalnız riskli müşterilerde.
        (await CountAsync($"SELECT count(*) FROM tenancy.brand_bank_accounts WHERE brand_id = {Brand} AND is_active AND iban_last4 = '8901' AND transfer_window_minutes = 120"))
            .ShouldBe(1);
        (await CountAsync($"SELECT count(*) FROM tenancy.brand_settings WHERE brand_id = {Brand} AND deposit_policy = 'RiskyCustomersOnly' AND risky_customer_no_show_threshold = 2"))
            .ShouldBe(1);
        (await CountAsync($"SELECT count(*) FROM catalog.services WHERE brand_id = {Brand} AND deposit_mode = 'Inherit' AND (deposit_percent = 30 OR deposit_amount_minor = 15000)"))
            .ShouldBe(2);
        const string Deposit = "FROM scheduling.appointments WHERE brand_id = " + Brand + " AND deposit_required_minor > 0 AND channel = 'Online'";
        (await CountAsync($"SELECT count(*) {Deposit}")).ShouldBe(4);
        (await CountAsync($"SELECT count(*) {Deposit} AND status = 'PendingApproval' AND deposit_customer_reported_utc IS NULL AND hold_expires_utc = '2026-11-25T11:00:00Z'"))
            .ShouldBe(1, "Kapora bekleniyor (Awaiting), havale süresi seed anından");
        (await CountAsync($"SELECT count(*) {Deposit} AND status = 'PendingApproval' AND deposit_customer_reported_utc IS NOT NULL")).ShouldBe(1, "Havale bildirildi");
        (await CountAsync($"SELECT count(*) {Deposit} AND status = 'Confirmed' AND deposit_received_utc IS NOT NULL AND deposit_payment_id IS NOT NULL")).ShouldBe(1, "Kapora alındı");
        (await CountAsync($"SELECT count(*) {Deposit} AND status = 'Confirmed' AND deposit_waived_utc IS NOT NULL")).ShouldBe(1, "Kaporasız onay");
        (await CountAsync($"SELECT count(*) {Deposit} AND brand_customer_id NOT IN (SELECT id FROM customers.brand_customers WHERE no_show_count >= 2)"))
            .ShouldBe(0, "kapora yalnız riskli müşterilerde");
        (await CountAsync(
            "SELECT count(*) FROM sales.payments p JOIN sales.payment_allocations a ON a.payment_id = p.id JOIN scheduling.appointments s ON s.deposit_payment_id = p.id "
            + $"WHERE p.brand_id = {Brand} AND p.is_deposit AND p.method = 'Transfer' AND p.amount_minor = s.deposit_required_minor AND p.reference = s.deposit_transfer_code"))
            .ShouldBe(1);
        (await CountAsync("SELECT count(*) FROM infra.outbox_events")).ShouldBe(0, "kapora geçişleri de olay yazmaz");
    }

    // T-114: Dilim 5 verisi. Saat Çarşamba 12:00 İstanbul: dün (Salı) Kadıköy'de nakit tahsilat var.
    [Fact]
    public async Task Seed_AddsDilim5Data()
    {
        var seeded = await SeedAsync(reset: false);
        seeded.Code.ShouldBe(0);
        seeded.Output.ShouldContain("Kasa: 2026-11-24 kapatıldı");

        // Geçmişte biten randevular tamamlanmıştır: yöntem başına tahsilat, Elif'e %24 hizmet primi; Geldi yalnız şu an süren.
        (await CountAsync($"SELECT count(*) FROM scheduling.appointments WHERE brand_id = {Brand} AND status = 'CheckedIn' AND end_utc <= '2026-11-25T09:00:00Z'")).ShouldBe(0);
        (await CountAsync($"SELECT count(*) FROM scheduling.appointments WHERE brand_id = {Brand} AND status = 'Completed' AND end_utc <= '2026-11-25T09:00:00Z'")).ShouldBeGreaterThan(20);
        foreach (var method in new[] { "Cash", "Card", "Transfer" })
        {
            (await CountAsync($"SELECT count(*) FROM sales.payments WHERE brand_id = {Brand} AND method = '{method}' AND NOT is_deposit")).ShouldBeGreaterThan(0, method);
        }

        (await CountAsync(
            "SELECT count(*) FROM scheduling.appointments s WHERE s.brand_id = " + Brand + " AND s.status = 'Completed' AND NOT EXISTS "
            + "(SELECT 1 FROM sales.payment_allocations a WHERE a.target_id = s.id)")).ShouldBe(0, "her tamamlanan randevunun tahsilatı (veya paket kaydı) var");
        const string Elif = "(SELECT id FROM tenancy.staff WHERE full_name = 'Elif Kaya')";
        (await CountAsync($"SELECT count(*) FROM sales.commission_entries WHERE staff_id = {Elif} AND type = 'Service' AND percent = 24 AND status = 'Accrued' AND amount_minor > 0"))
            .ShouldBeGreaterThan(0);
        (await CountAsync($"SELECT count(*) FROM customers.brand_customers WHERE brand_id = {Brand} AND visit_count > 0")).ShouldBeGreaterThan(5);

        // Paketler: Fön paketi %10 satış primi (KK6); Selin'de 2 seans kaldı, Derya'nın paketi 10 gün sonra biter, Melis'e dün satıldı.
        (await CountAsync($"SELECT count(*) FROM sales.packages WHERE brand_id = {Brand} AND is_active")).ShouldBe(2);
        (await CountAsync($"SELECT count(*) FROM sales.packages WHERE brand_id = {Brand} AND name = 'Fön paketi' AND sale_commission_percent_override = 10")).ShouldBe(1);
        (await CountAsync($"SELECT count(*) FROM sales.customer_packages WHERE brand_id = {Brand} AND status = 'Active'")).ShouldBe(3);
        const string Remaining =
            "SELECT i.total_sessions - (SELECT count(*) FROM sales.package_session_usages u WHERE u.customer_package_item_id = i.id AND u.reverted_utc IS NULL) "
            + "FROM sales.customer_package_items i JOIN sales.customer_packages p ON p.id = i.customer_package_id "
            + "JOIN customers.brand_customers c ON c.id = p.brand_customer_id WHERE c.display_name = ";
        (await CountAsync(Remaining + "'Selin Öztürk'")).ShouldBe(2);
        (await CountAsync(Remaining + "'Derya Şahin'")).ShouldBe(9);
        (await CountAsync(Remaining + "'Melis Tunç'")).ShouldBe(10);
        (await CountAsync($"SELECT count(*) FROM sales.customer_packages WHERE brand_id = {Brand} AND expires_on = '2026-12-05'")).ShouldBe(1, "süresi yaklaşan");
        (await CountAsync($"SELECT count(*) FROM sales.payments WHERE brand_id = {Brand} AND method = 'PackageRedemption' AND amount_minor = 0")).ShouldBe(3);
        (await CountAsync(
            $"SELECT count(*) FROM sales.commission_entries WHERE staff_id = {Elif} AND type = 'Sale' AND percent = 10 AND basis_minor = 225000 AND amount_minor = 22500 AND period_year_month = 202611"))
            .ShouldBe(1, "Fön paketi: pakete özel %10 (Melis, dün)");
        (await CountAsync($"SELECT count(*) FROM sales.commission_entries WHERE staff_id = {Elif} AND type = 'Sale' AND percent = 5 AND basis_minor = 216000"))
            .ShouldBe(1, "bakım paketi: satan personelin oranı (Selin)");

        // Kasa: dün Kadıköy kapatıldı, fark −50 ₺ ve açıklama; beklenen o günün nakit tahsilatları (KK7 örneği).
        const string Close = "FROM sales.cash_day_closes WHERE brand_id = " + Brand;
        (await CountAsync($"SELECT count(*) {Close}")).ShouldBe(1);
        (await CountAsync($"SELECT count(*) {Close} AND business_date = '2026-11-24' AND difference_minor = -5000 AND difference_note IS NOT NULL AND expected_cash_minor > 0")).ShouldBe(1);
        (await CountAsync(
            "SELECT count(*) FROM sales.cash_day_closes c WHERE c.expected_cash_minor = (SELECT sum(p.amount_minor) FROM sales.payments p WHERE p.branch_id = c.branch_id AND p.method = 'Cash' "
            + "AND p.paid_utc >= '2026-11-23T21:00:00Z' AND p.paid_utc < '2026-11-24T21:00:00Z')")).ShouldBe(1);

    }

    [Fact]
    public async Task Seed_AddsReviewsWaitlistAndWalkIns()
    {
        (await SeedAsync(reset: false)).Code.ShouldBe(0);

        // Değerlendirmeler: en az 5, biri yanıtlı, düşük puanlı biri yanıt bekliyor; hepsi tamamlanan randevuda.
        (await CountAsync($"SELECT count(*) FROM reviews.reviews WHERE brand_id = {Brand} AND is_published")).ShouldBeGreaterThanOrEqualTo(5);
        (await CountAsync($"SELECT count(*) FROM reviews.reviews WHERE brand_id = {Brand} AND reply_text IS NOT NULL")).ShouldBe(1);
        (await CountAsync($"SELECT count(*) FROM reviews.reviews WHERE brand_id = {Brand} AND rating <= 2 AND reply_text IS NULL")).ShouldBe(1);
        (await CountAsync(
            $"SELECT count(*) FROM reviews.reviews r WHERE r.brand_id = {Brand} AND NOT EXISTS (SELECT 1 FROM scheduling.appointments s WHERE s.id = r.appointment_id AND s.status = 'Completed')"))
            .ShouldBe(0);

        // Bekleme listesinde 2 kişi; bugün kapıda sırada 1 ve 2 numara.
        (await CountAsync($"SELECT count(*) FROM scheduling.waitlist_entries WHERE brand_id = {Brand} AND status = 'Waiting'")).ShouldBe(2);
        (await CountAsync($"SELECT count(*) FROM scheduling.walk_in_tickets WHERE brand_id = {Brand} AND status = 'Waiting' AND queue_date = '2026-11-25' AND queue_number IN (1, 2)"))
            .ShouldBe(2);

        // Personel bildirim tercihleri varsayılan (kayıt yok).
        (await CountAsync($"SELECT count(*) FROM notifications.notification_preferences WHERE brand_id = {Brand}")).ShouldBe(0);
    }

    [Fact]
    public async Task Seed_PanelReadsDilim5Data()
    {
        (await SeedAsync(reset: false)).Code.ShouldBe(0);
        var token = (await _api.SignInAsync("+905550000001")).Body.GetProperty("accessToken").GetString()!;
        var branch = (Guid)(await ScalarAsync($"SELECT id FROM tenancy.branches WHERE brand_id = {Brand} AND name = 'Kadıköy'"))!;

        var cash = await _api.GetAsync($"/api/v1/manage/cash?branchId={branch}&date=2026-11-24", token);
        cash.Status.ShouldBe(200);
        cash.Body.GetProperty("status").GetString().ShouldBe("Closed");

        var queue = await _api.GetAsync($"/api/v1/manage/walk-in-tickets?branchId={branch}", token);
        queue.Status.ShouldBe(200);
        queue.Body.GetProperty("items").GetArrayLength().ShouldBe(2);

        var waitlist = await _api.GetAsync($"/api/v1/manage/waitlist?branchId={branch}", token);
        waitlist.Status.ShouldBe(200);
        waitlist.Body.GetProperty("items").GetArrayLength().ShouldBe(2);

        var reviews = await _api.GetAsync("/api/v1/manage/reviews", token);
        reviews.Status.ShouldBe(200);

        var commission = await _api.GetAsync("/api/v1/manage/me/commission-summary", token);
        commission.Status.ShouldBe(200);

        var preferences = await _api.GetAsync("/api/v1/manage/me/notification-preferences", token);
        preferences.Status.ShouldBe(200);
    }

    [Fact]
    public async Task Seed_OwnerAndStaffCanSignIn_AndPanelOpensActive()
    {
        (await SeedAsync(reset: false)).Code.ShouldBe(0);

        var owner = await _api.SignInAsync("+905550000001");
        owner.Status.ShouldBe(200);
        var session = (await _api.GetAsync("/api/v1/manage/session", owner.Body.GetProperty("accessToken").GetString()!)).Body;
        session.GetProperty("brand").GetProperty("slug").GetString().ShouldBe("kuafor-elif");
        session.GetProperty("brand").GetProperty("status").GetString().ShouldBe("Active");
        session.GetProperty("membership").GetProperty("role").GetString().ShouldBe("Owner");
        session.GetProperty("branches").GetArrayLength().ShouldBe(2);
        session.GetProperty("smsCredits").GetProperty("balance").GetInt32().ShouldBe(1000);

        // Kapora hesabı API sürecinde çözülür (aynı şifreleme anahtarı); tam IBAN dönmez.
        var account = await _api.GetAsync("/api/v1/manage/brand/bank-account", owner.Body.GetProperty("accessToken").GetString()!);
        account.Status.ShouldBe(200);
        account.Body.GetProperty("configured").GetBoolean().ShouldBeTrue();
        account.Body.GetProperty("ibanMasked").GetString().ShouldBe("TR29 •••• 6789 01");
        account.Body.GetProperty("accountHolderName").GetString().ShouldBe("Elif Kaya");

        var manager = await _api.SignInAsync("+905550000004");
        manager.Status.ShouldBe(200);
        var managerSession = (await _api.GetAsync("/api/v1/manage/session", manager.Body.GetProperty("accessToken").GetString()!)).Body;
        managerSession.GetProperty("membership").GetProperty("role").GetString().ShouldBe("BranchManager");
    }

    [Fact]
    public async Task Seed_InProduction_FailsWithoutWriting()
    {
        var output = new StringWriter();

        var code = await DemoSeedCommand.RunAsync(_factory.Services, new Environment("Production"), reset: false, output, Ct);

        code.ShouldBe(1);
        output.ToString().ShouldContain("Production");
        (await CountAsync("SELECT count(*) FROM tenancy.brands")).ShouldBe(0);
        (await CountAsync("SELECT count(*) FROM identity.users")).ShouldBe(0);
    }

    [Fact]
    public void IbanKey_RequiredOutsideTesting()
    {
        var empty = new ConfigurationBuilder().Build();
        var keyed = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.Ordinal) { ["Tenancy:IbanEncryptionKey"] = "anahtar" }).Build();

        DemoSeedCommand.IbanKeyError(new Environment("Development"), empty).ShouldNotBeNull().ShouldContain("Tenancy__IbanEncryptionKey");
        DemoSeedCommand.IbanKeyError(new Environment("Staging"), empty).ShouldNotBeNull();
        DemoSeedCommand.IbanKeyError(new Environment("Development"), keyed).ShouldBeNull();
        DemoSeedCommand.IbanKeyError(new Environment("Testing"), empty).ShouldBeNull("seed ve API aynı süreçte, geçici anahtar paylaşılır");
    }

    [Fact]
    public async Task Reset_RecreatesOnlyDemoBrand()
    {
        await _api.SignupAsync("+905324187799", "Berber Kaan");
        var other = (Guid)(await ScalarAsync("SELECT id FROM tenancy.brands WHERE name = 'Berber Kaan'"))!;
        (await SeedAsync(reset: false)).Code.ShouldBe(0);
        var oldDemo = await BrandIdAsync();
        var otherBefore = await RowsAsync(other);
        var demoBefore = await RowsAsync(oldDemo);

        (await SeedAsync(reset: true)).Code.ShouldBe(0);

        var newDemo = await BrandIdAsync();
        newDemo.ShouldNotBe(oldDemo);
        // Denetim kaydı silinmez (yalnız INSERT izni): eski markanın satırları kalır, diğer bütün tabloları boşalır.
        var leftovers = await RowsAsync(oldDemo);
        leftovers.Remove(AuditTable, out var oldAudits).ShouldBeTrue();
        oldAudits.ShouldBe(demoBefore[AuditTable]);
        leftovers.Values.Sum().ShouldBe(0);
        (await RowsAsync(newDemo)).ShouldBe(demoBefore);
        (await RowsAsync(other)).ShouldBe(otherBefore);
        (await CountAsync("SELECT count(*) FROM tenancy.brands")).ShouldBe(4, "üç demo markası ve Berber Kaan");
        (await CountAsync("SELECT count(*) FROM tenancy.brands WHERE name IN ('Berber Kemal', 'Nar Güzellik')")).ShouldBe(2);
        (await CountAsync("SELECT count(*) FROM platform.bank_transactions")).ShouldBe(3, "önceki seed'in banka hareketleri silinir");
        (await _api.SignInAsync("+905550000001")).Status.ShouldBe(200);
    }

    // T-131: Dilim 6 verisi. Plan fiyatı tanımlı değil (S-14): fatura kesilmez, bildirimlerin beklenen tutarı boş.
    [Fact]
    public async Task Seed_AddsDilim6Data()
    {
        var seeded = await SeedAsync(reset: false);
        seeded.Code.ShouldBe(0);
        seeded.Output.ShouldContain("Berber Kemal PastDue");
        seeded.Output.ShouldContain("Nar Güzellik Active");
        seeded.Output.ShouldContain("0555 000 99 02");

        // Abonelik durumları: Denemede, Gecikti (deneme 3 gün önce bitti; 7 günlük süre dolmadı), Aktif (dönem bugünü kapsar).
        const string Kemal = "(SELECT id FROM tenancy.brands WHERE name = 'Berber Kemal')";
        const string Nar = "(SELECT id FROM tenancy.brands WHERE name = 'Nar Güzellik')";
        (await CountAsync($"SELECT count(*) FROM tenancy.brands WHERE id IN ({Kemal}, {Nar}) AND status = 'Active'")).ShouldBe(2);
        (await CountAsync($"SELECT count(*) FROM platform.subscriptions WHERE brand_id = {Brand} AND status = 'Trialing'")).ShouldBe(1);
        (await CountAsync(
            $"SELECT count(*) FROM platform.subscriptions WHERE brand_id = {Kemal} AND status = 'PastDue' AND past_due_since = '2026-11-22' AND current_period_start = '2026-11-22'"))
            .ShouldBe(1);
        (await CountAsync(
            $"SELECT count(*) FROM platform.subscriptions WHERE brand_id = {Nar} AND status = 'Active' AND payment_method = 'BankTransfer' AND trial_ends_on IS NULL "
            + "AND current_period_start = '2026-11-15' AND current_period_end >= '2026-11-25'")).ShouldBe(1);
        (await CountAsync("SELECT count(*) FROM platform.subscription_invoices")).ShouldBe(0, "plan fiyatı tanımlı değil: fatura uydurulmaz");

        // Onay kuyruğu: bekleyen abonelik havalesi (kodu taşıyan ve taşımayan birer aday), kontör havalesi (paket varsa), SMS adı.
        (await CountAsync($"SELECT count(*) FROM platform.payment_notices WHERE brand_id = {Kemal} AND purpose = 'Subscription' AND status = 'Pending' AND expected_amount_minor IS NULL"))
            .ShouldBe(1);
        (await CountAsync(
            "SELECT count(*) FROM platform.bank_transactions t WHERE t.matched_notice_id IS NULL AND t.bank_reference LIKE 'DEMO-%' AND position("
            + $"(SELECT transfer_code FROM platform.subscriptions WHERE brand_id = {Kemal}) in t.description) > 0")).ShouldBe(1, "kodu taşıyan aday");
        (await CountAsync("SELECT count(*) FROM platform.bank_transactions WHERE matched_notice_id IS NULL")).ShouldBe(2);
        (await CountAsync(
            $"SELECT count(*) FROM platform.payment_notices n JOIN platform.bank_transactions t ON t.matched_notice_id = n.id WHERE n.brand_id = {Nar} AND n.status = 'Matched' "
            + "AND n.matched_bank_transaction_id = t.id")).ShouldBe(1);
        var packages = await CountAsync("SELECT count(*) FROM platform.credit_packages WHERE is_active");
        (await CountAsync($"SELECT count(*) FROM platform.payment_notices WHERE brand_id = {Brand} AND purpose = 'Credits' AND status = 'Pending'")).ShouldBe(Math.Min(1, packages));
        (await CountAsync($"SELECT count(*) FROM notifications.brand_sms_senders WHERE brand_id = {Brand} AND status = 'Pending' AND name = 'KUAFORELIF'")).ShouldBe(1);

        // Platform kullanıcıları (üç rol), denetim kaydı (giriş, görüntüleme, eşleştirme), Google Takvim, yeni personel.
        (await CountAsync("SELECT count(DISTINCT platform_role) FROM identity.users WHERE phone_e164 LIKE '+9055500099%'")).ShouldBe(3);
        (await CountAsync("SELECT count(*) FROM platform.audit_logs")).ShouldBe(6);
        (await CountAsync($"SELECT count(*) FROM platform.audit_logs WHERE brand_id = {Nar} AND action = 'Match' AND actor_type = 'PlatformUser'")).ShouldBe(1);
        (await CountAsync("SELECT count(*) FROM platform.audit_logs WHERE action = 'View' AND entity_type = 'Brand' AND occurred_utc < '2026-11-25T09:00:00Z'")).ShouldBe(3);
        (await CountAsync(
            $"SELECT count(*) FROM integrations.calendar_links l JOIN tenancy.staff s ON s.id = l.staff_id WHERE l.brand_id = {Brand} AND l.status = 'Active' AND s.full_name = 'Elif Kaya'"))
            .ShouldBe(1);
        (await CountAsync(
            "SELECT count(*) FROM identity.user_memberships m JOIN identity.users u ON u.id = m.user_id "
            + $"WHERE m.brand_id = {Brand} AND u.phone_e164 = '+905550000005' AND m.role = 'Staff'")).ShouldBe(1);
        (await CountAsync("SELECT count(*) FROM infra.outbox_events")).ShouldBe(0, "Dilim 6 verisi de olay yazmaz");

        // Gecikti markasının sahibi girer; panel aboneliği ve bekleyen bildirimi okur. Platform kullanıcısı salon panelinde üye değildir.
        var owner = await _api.SignInAsync("+905550000011");
        owner.Status.ShouldBe(200);
        var subscription = await _api.GetAsync("/api/v1/manage/subscription", owner.Body.GetProperty("accessToken").GetString()!);
        subscription.Status.ShouldBe(200);
        subscription.Body.GetProperty("status").GetString().ShouldBe("PastDue");
    }

    // T-131: plan fiyatı tanımlıysa dönem faturaları kesilir; kodu taşıyan hareket fatura tutarıyladır ("Kod tutuyor").
    [Fact]
    public async Task Seed_WithPlanPrice_IssuesInvoicesAndMatchingTransfer()
    {
        await ScalarAsync("UPDATE platform.plans SET monthly_price_minor = 100000 WHERE code = 'salon_plus'");
        try
        {
            (await SeedAsync(reset: false)).Code.ShouldBe(0);

            const string Invoices = "FROM platform.subscription_invoices i JOIN tenancy.brands b ON b.id = i.brand_id WHERE b.name = ";
            (await CountAsync($"SELECT count(*) {Invoices}'Berber Kemal' AND i.status = 'Issued' AND i.period_start = '2026-11-22' AND i.due_on = '2026-11-22'")).ShouldBe(1);
            (await CountAsync($"SELECT count(*) {Invoices}'Nar Güzellik' AND i.status = 'Paid' AND i.paid_via = 'BankTransfer' AND i.period_start = '2026-11-15'")).ShouldBe(1);
            (await CountAsync("SELECT count(DISTINCT invoice_number) FROM platform.subscription_invoices WHERE invoice_number LIKE 'OMV-2026-%'")).ShouldBe(2);
            (await CountAsync(
                "SELECT count(*) FROM platform.payment_notices n JOIN platform.bank_transactions t ON t.amount_minor = n.expected_amount_minor "
                + "AND position(n.transfer_code in t.description) > 0 WHERE n.status = 'Pending' AND n.purpose = 'Subscription' AND t.matched_notice_id IS NULL"))
                .ShouldBe(1, "kod ve tutar tutan aday");
        }
        finally
        {
            await ScalarAsync("UPDATE platform.plans SET monthly_price_minor = NULL WHERE code = 'salon_plus'");
        }
    }

    private async Task<(int Code, string Output)> SeedAsync(bool reset)
    {
        var output = new StringWriter();
        var environment = _factory.Services.GetRequiredService<IHostEnvironment>();
        var code = await DemoSeedCommand.RunAsync(_factory.Services, environment, reset, output, Ct);
        return (code, output.ToString());
    }

    private async Task<Guid> BrandIdAsync() => (Guid)(await ScalarAsync($"SELECT {Brand}"))!;

    /// <summary>Markanın her tablodaki satır sayısı (brand_id taşıyan bütün tablolar ve marka satırı).</summary>
    private async Task<Dictionary<string, long>> RowsAsync(Guid brandId)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        var tables = new List<string>();
        await using (var list = new NpgsqlCommand(
            "SELECT table_schema || '.' || table_name FROM information_schema.columns WHERE column_name = 'brand_id' AND table_schema NOT IN ('pg_catalog', 'information_schema') ORDER BY 1",
            connection))
        await using (var reader = await list.ExecuteReaderAsync(Ct))
        {
            while (await reader.ReadAsync(Ct))
            {
                tables.Add(reader.GetString(0));
            }
        }

        var rows = new Dictionary<string, long>(StringComparer.Ordinal);
        foreach (var table in tables.Append("tenancy.brands"))
        {
            var column = string.Equals(table, "tenancy.brands", StringComparison.Ordinal) ? "id" : "brand_id";
            await using var count = new NpgsqlCommand($"SELECT count(*) FROM {table} WHERE {column} = $1", connection);
            count.Parameters.Add(new NpgsqlParameter { Value = brandId });
            rows[table] = (long)(await count.ExecuteScalarAsync(Ct))!;
        }

        return rows;
    }

    private async Task<long> CountAsync(string sql) => Convert.ToInt64(await ScalarAsync(sql), System.Globalization.CultureInfo.InvariantCulture);

    private async Task<object?> ScalarAsync(string sql)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand(sql, connection);
        return await command.ExecuteScalarAsync(Ct);
    }

    private sealed class Environment(string name) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = name;

        public string ApplicationName { get; set; } = "Omvila.Host";

        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;

        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
