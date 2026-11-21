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
        (await CountAsync($"SELECT count(*) FROM tenancy.staff WHERE brand_id = {Brand}")).ShouldBe(4);
        (await CountAsync($"SELECT count(*) FROM identity.user_memberships WHERE brand_id = {Brand}")).ShouldBe(4);
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
        (await CountAsync("SELECT count(*) FROM tenancy.brands")).ShouldBe(1);
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
        (await RowsAsync(oldDemo)).Values.Sum().ShouldBe(0);
        (await RowsAsync(newDemo)).ShouldBe(demoBefore);
        (await RowsAsync(other)).ShouldBe(otherBefore);
        (await CountAsync("SELECT count(*) FROM tenancy.brands")).ShouldBe(2);
        (await _api.SignInAsync("+905550000001")).Status.ShouldBe(200);
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
