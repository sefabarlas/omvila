using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using NodaTime;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: sözleşme manageMergeCustomers, managePreviewImport, manageCommitImport, manageExportCustomers,
// manageGetCustomerDeletionPreview, manageRequestCustomerDeletion, manageListDeletionRequests (T-86 (7), T-92); alan-modeli.md
// § Birleştirme, § CustomerImport, § DataDeletionRequest; guvenlik-kvkk.md § Anonimleştirme (Silinmiş müşteri, +000…,
// AuditLog(Anonymize)), § Haklar (CSV dışa aktarma); roller-yetkiler.md customer.merge (Owner, BranchManager),
// customer.import / customer.export / customer.kvkk (yalnız Owner); ADR-0006 (başka markanın kartı 404); kök AGENTS.md
// § Gizlilik (telefon ve ad günlüğe yazılmaz). Saat: 2026-11-25 09:00Z; randevu Cuma 14:30 (11:30Z).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class CustomerRecordsTests(PostgresFixture db) : IAsyncLifetime
{
    private const string AysePhone = "+905321234567";
    private const string AyseOtherPhone = "+905339876543";
    private const string FridayAt1430 = "2026-11-27T11:30:00Z";
    private readonly CapturingLoggerProvider _logs = new();
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private string _merkez = null!;
    private string _elif = null!;
    private string _balyaj = null!;
    private int _requests;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(
            db,
            new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                ["Auth:AccessTokenMinutes"] = "600",
                ["Auth:PublicBookingTokenMinutes"] = "600",
                ["Customers:Import:MaxRows"] = "5",
                ["Customers:Import:MaxFileBytes"] = "4096",
            },
            _logs);
        _api = new ManageApi(_factory, db);
        _token = await _api.SignupAsync("+905324187705", "Kuaför Elif");
        var session = (await _api.GetAsync("/api/v1/manage/session", _token)).Body;
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
        (await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/brand/bank-account", new
        {
            accountHolderName = "Elif Kaya",
            bankName = "Ziraat Bankası",
            iban = "TR29 0006 4000 0011 2345 6789 01",
            transferCodePrefix = "OMV",
            transferWindowMinutes = 120,
        }, _token, "W/\"0\"")).Status.ShouldBe(200);
        var category = (await PostAsync("/api/v1/manage/service-categories", new { name = "Saç" })).Body.GetProperty("id").GetString()!;
        _balyaj = (await PostAsync("/api/v1/manage/services", new
        {
            categoryId = category,
            name = "Balyaj",
            durationMinutes = 60,
            price = new { amountMinor = 280000, currency = "TRY" },
            priceDisplay = "Fixed",
            deposit = new { mode = "Inherit", percent = 30 },
            isOnlineBookable = true,
            staffIds = new[] { _elif },
        })).Body.GetProperty("id").GetString()!;
    }

    public async ValueTask DisposeAsync()
    {
        await _factory.DisposeAsync();
        _logs.Dispose();
    }

    // ───────── Birleştirme ─────────

    [Fact]
    public async Task Merge_MovesAppointmentAndPayment_SumsCounters_MergesNotes_ArchivesSource()
    {
        var (source, appointmentId) = await AyseWithReceivedDepositAsync();
        await _api.ScalarAsync<int>($"UPDATE customers.brand_customers SET notes = 'Online kart notu', visit_count = 2, cancel_count = 1, total_spent_minor = 90000, first_visit_on = '2026-03-01', last_visit_on = '2026-10-01' WHERE id = '{source}' RETURNING 1");
        _factory.Clock.Advance(Duration.FromMinutes(1));
        var target = await CreateCustomerAsync("Ayşe Y.", AyseOtherPhone, notes: "Kısa saç sever");
        await _api.ScalarAsync<int>($"UPDATE customers.brand_customers SET visit_count = 3, no_show_count = 1, total_spent_minor = 150000, first_visit_on = '2025-05-01', last_visit_on = '2026-09-01' WHERE id = '{target}' RETURNING 1");

        var merged = await MergeAsync(target, new { sourceBrandCustomerId = source, mergeNotes = true });

        merged.Status.ShouldBe(200, merged.Body.ToString());
        merged.Body.GetProperty("id").GetString().ShouldBe(target);
        merged.Body.GetProperty("fullName").GetString().ShouldBe("Ayşe Y.");
        merged.Body.GetProperty("visitCount").GetInt32().ShouldBe(5);
        merged.Body.GetProperty("noShowCount").GetInt32().ShouldBe(3, "kaynakta 2 + hedefte 1");
        merged.Body.GetProperty("totalSpent").GetProperty("amountMinor").GetInt64().ShouldBe(240000);
        merged.Body.GetProperty("lastVisitOn").GetString().ShouldBe("2026-10-01");
        merged.Body.GetProperty("notes").GetString().ShouldBe("Online kart notu\n\nKısa saç sever", "notlar kart sırasıyla, eskisi önce");
        merged.Body.GetProperty("nextAppointmentUtc").GetString().ShouldBe(FridayAt1430, "randevu kalan karta taşındı");
        (await _api.ScalarAsync<string>($"SELECT cancel_count || '/' || first_visit_on FROM customers.brand_customers WHERE id = '{target}'")).ShouldBe("1/2025-05-01");

        (await _api.ScalarAsync<string>($"SELECT brand_customer_id::text FROM scheduling.appointments WHERE id = '{appointmentId}'")).ShouldBe(target);
        (await _api.ScalarAsync<long>($"""
            SELECT count(*) FROM scheduling.appointments a JOIN customers.brand_customers b ON b.id = a.brand_customer_id
            WHERE a.id = '{appointmentId}' AND a.customer_id = b.customer_id
            """)).ShouldBe(1, "randevunun global kişisi de kalan kartınki");
        (await _api.ScalarAsync<string>("SELECT brand_customer_id::text FROM sales.payments")).ShouldBe(target, "tahsilat da taşınır");

        // Kaynak silinmez, arşivlenir; listede, aramada ve kart olarak görünmez.
        (await _api.ScalarAsync<string>($"SELECT merged_into_id::text FROM customers.brand_customers WHERE id = '{source}'")).ShouldBe(target);
        (await _api.GetAsync($"/api/v1/manage/customers/{source}", _token)).Status.ShouldBe(404);
        Names((await _api.GetAsync("/api/v1/manage/customers", _token)).Body).ShouldBe(["Ayşe Y."]);
        Names((await _api.GetAsync("/api/v1/manage/customers?q=4567", _token)).Body).ShouldBeEmpty("arşiv kartın numarası listede aranmaz");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM customers.customers")).ShouldBe(2, "global kişiler birleştirilmez");
        (await _api.ScalarAsync<string>("SELECT action || '/' || entity_type || '/' || (changes->>'operation') FROM platform.audit_logs")).ShouldBe("Update/BrandCustomer/merge");

        // Aynı numarayla gelen yeni randevu kalan karta yazılır (markadaki görünüm birleşti).
        var again = await BookAsync(AysePhone, "Ayşe Yılmaz", "2026-11-28T08:00:00Z");
        again.Status.ShouldBe(201, again.Body.ToString());
        (await _api.ScalarAsync<string>($"SELECT brand_customer_id::text FROM scheduling.appointments WHERE id = '{again.Body.GetProperty("id").GetString()}'")).ShouldBe(target);
        AssertNoPersonalDataInLogs("Ayşe", AysePhone, AyseOtherPhone, "Kısa saç sever");
    }

    [Fact]
    public async Task Merge_WithoutNotes_KeepsTargetNotes_Errors_AndPermissions()
    {
        var a = await CreateCustomerAsync("Selin Öztürk", "+905550004418", notes: "Hedef notu");
        var b = await CreateCustomerAsync("Selin Ö.", "+905550004419", notes: "Kaynak notu");
        var c = await CreateCustomerAsync("Derya Şahin", "+905550002118");

        (await MergeAsync(a, new { sourceBrandCustomerId = a })).Body.GetProperty("code").GetString().ShouldBe("validation_failed");
        var staff = await _api.AddMemberAndSignInAsync(_token, "+905382741905", "Staff", _merkez);
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/customers/{a}/merge", new { sourceBrandCustomerId = b }, staff)).Status.ShouldBe(403);
        var kaan = await _api.SignupAsync("+905330000001", "Berber Kaan");
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/customers/{a}/merge", new { sourceBrandCustomerId = b }, kaan)).Status.ShouldBe(404, "başka markanın kartı");

        var manager = await _api.AddMemberAndSignInAsync(_token, "+905382741906", "BranchManager", _merkez);
        var merged = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/customers/{a}/merge", new { sourceBrandCustomerId = b, mergeNotes = false }, manager);
        merged.Status.ShouldBe(200, merged.Body.ToString());
        merged.Body.GetProperty("notes").GetString().ShouldBe("Hedef notu");

        var twice = await MergeAsync(c, new { sourceBrandCustomerId = b });
        twice.Status.ShouldBe(409);
        twice.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition", "kaynak zaten birleştirilmiş");
        (await MergeAsync(b, new { sourceBrandCustomerId = c })).Status.ShouldBe(404, "arşiv kart kalacak kayıt olamaz");
    }

    // ───────── İçe aktarma ─────────

    [Fact]
    public async Task Import_Preview_ClassifiesRows_WritesNothing_Commit_CreatesAndMerges_Idempotent()
    {
        var existing = await CreateCustomerAsync("Selin Öztürk", "+905550004418");
        var csv = "﻿Ad Soyad;Telefon;Not;Doğum yılı;Kampanya izni\r\n"
            + "Derya Şahin;0555 000 21 18;Kısa saç;1990;Evet\r\n"
            + "Selin Ö.;5550004418;Yeni not;;Evet\r\n"
            + "Pınar Yalçın;+90 555 000 0941;;abc;\r\n"
            + "Ece Koç;12345;;;\r\n"
            + ";0555 000 11 11;;;\r\n";

        var preview = await PreviewAsync(csv, "musteriler.csv");

        preview.Status.ShouldBe(201, preview.Text);
        var body = preview.Json;
        body.GetProperty("fileName").GetString().ShouldBe("musteriler.csv");
        body.GetProperty("totalRows").GetInt32().ShouldBe(5);
        body.GetProperty("toCreate").GetInt32().ShouldBe(2);
        body.GetProperty("toMerge").GetInt32().ShouldBe(1, "numarası markada kayıtlı");
        body.GetProperty("invalidRows").GetInt32().ShouldBe(2);
        body.GetProperty("status").GetString().ShouldBe("Previewed");
        body.GetProperty("mappedColumns").EnumerateArray().Select(c => c.GetString()).ShouldBe(["FullName", "Phone", "Notes", "BirthYear"]);
        body.GetProperty("errors").EnumerateArray().Select(e => $"{e.GetProperty("rowNumber").GetInt32()}/{e.GetProperty("column").GetString()}/{e.GetProperty("code").GetString()}")
            .ShouldBe(["5/Telefon/invalid_phone", "6/Ad Soyad/missing_name"]);
        body.GetProperty("expiresUtc").GetString().ShouldBe(Iso(_factory.Clock.GetCurrentInstant() + Duration.FromMinutes(60)), "önizleme 60 dakika geçerli");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM customers.brand_customers")).ShouldBe(1, "önizleme kayıt değiştirmez");

        var importId = body.GetProperty("importId").GetString()!;
        var committed = await CommitAsync(importId, new { sendInfoSms = true });

        committed.Status.ShouldBe(200, committed.Body.ToString());
        committed.Body.GetProperty("status").GetString().ShouldBe("Committed");
        committed.Body.GetProperty("created").GetInt32().ShouldBe(2);
        committed.Body.GetProperty("merged").GetInt32().ShouldBe(1);
        committed.Body.GetProperty("skipped").GetInt32().ShouldBe(2);
        (await _api.ScalarAsync<string>("""
            SELECT string_agg(b.display_name || '/' || b.source || '/' || b.appointment_sms_enabled || '/' || coalesce(b.notes, '-') || '/' || coalesce(b.birth_year::text, '-'), ',' ORDER BY b.display_name)
            FROM customers.brand_customers b WHERE b.source = 'Import'
            """)).ShouldBe("Derya Şahin/Import/true/Kısa saç/1990,Pınar Yalçın/Import/true/-/-");
        (await _api.ScalarAsync<string>($"SELECT display_name || '/' || source || '/' || notes FROM customers.brand_customers WHERE id = '{existing}'"))
            .ShouldBe("Selin Öztürk/Panel/Yeni not", "mevcut ad korunur, boş not dolar");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM customers.consent_records")).ShouldBe(0, "kampanya izni içe aktarılmaz");
        (await _api.ScalarAsync<bool>($"SELECT rows IS NULL FROM customers.customer_imports WHERE id = '{importId}'")).ShouldBeTrue("uygulanınca satırlar silinir");
        (await _api.ScalarAsync<string>("SELECT action || '/' || entity_type FROM platform.audit_logs")).ShouldBe("Create/CustomerImport");

        var again = await CommitAsync(importId, new { sendInfoSms = false });
        again.Status.ShouldBe(200);
        again.Body.GetProperty("created").GetInt32().ShouldBe(2, "kendiliğinden idempotent");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM customers.brand_customers")).ShouldBe(3);
        AssertNoPersonalDataInLogs("Derya", "Pınar", "+905550002118", "Kısa saç");
    }

    [Fact]
    public async Task Import_DuplicateInFile_Expiry_Limits_Format_AndPermissions()
    {
        var duplicate = await PreviewAsync("Ad,Telefon\nDerya Şahin,05550002118\nDerya Ş.,+905550002118\n", "a.csv");
        duplicate.Status.ShouldBe(201, duplicate.Text);
        duplicate.Json.GetProperty("toCreate").GetInt32().ShouldBe(1);
        duplicate.Json.GetProperty("errors")[0].GetProperty("code").GetString().ShouldBe("duplicate_in_file");
        duplicate.Json.GetProperty("errors")[0].GetProperty("rowNumber").GetInt32().ShouldBe(3);

        _factory.Clock.Advance(Duration.FromMinutes(60));
        var expired = await CommitAsync(duplicate.Json.GetProperty("importId").GetString()!, null);
        expired.Status.ShouldBe(409);
        expired.Body.GetProperty("code").GetString().ShouldBe("hold_expired");

        var rows = await PreviewAsync("Ad;Telefon\n" + string.Concat(Enumerable.Range(0, 6).Select(i => $"Kişi {i};055500000{i}0\n")), "b.csv");
        rows.Status.ShouldBe(400);
        rows.Json.GetProperty("errors").GetProperty("file")[0].GetString().ShouldBe("too_large");
        var big = await PreviewAsync("Ad;Telefon\n" + new string('x', 5000), "c.csv");
        big.Json.GetProperty("errors").GetProperty("file")[0].GetString().ShouldBe("too_large");
        var noPhone = await PreviewAsync("Ad;Not\nDerya;x\n", "d.csv");
        noPhone.Json.GetProperty("errors").GetProperty("file")[0].GetString().ShouldBe("invalid_format");
        var binary = await PreviewAsync(Encoding.Latin1.GetString([0x41, 0x64, 0x3B, 0x54, 0x65, 0x6C, 0xFF, 0xFE, 0x0A]), "e.csv", Encoding.Latin1);
        binary.Json.GetProperty("errors").GetProperty("file")[0].GetString().ShouldBe("invalid_format");
        (await CommitAsync("0193a8f0-0000-7000-8000-000000000001", null)).Status.ShouldBe(404);

        var manager = await _api.AddMemberAndSignInAsync(_token, "+905382741906", "BranchManager", _merkez);
        (await PreviewAsync("Ad;Telefon\nDerya;05550002118\n", "f.csv", token: manager)).Status.ShouldBe(403, "customer.import yalnız Owner");
    }

    // ───────── Dışa aktarma ─────────

    [Fact]
    public async Task Export_Csv_BomHeadersDisplayPhone_ExcludesMergedAndAnonymized_Audited()
    {
        var selin = await CreateCustomerAsync("Selin Öztürk", "+905550004418", notes: "=Kısa; saç");
        await _api.ScalarAsync<int>($"UPDATE customers.brand_customers SET visit_count = 12, no_show_count = 0, total_spent_minor = 1234550, first_visit_on = '2025-01-10', last_visit_on = '2026-11-20', birth_year = 1992 WHERE id = '{selin}' RETURNING 1");
        var merged = await CreateCustomerAsync("Selin Ö.", "+905550004419");
        (await MergeAsync(selin, new { sourceBrandCustomerId = merged })).Status.ShouldBe(200);
        var gone = await CreateCustomerAsync("Tuna Oral", "+905550007777");
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/customers/{gone}/deletion", new { confirmFullName = "Tuna Oral" }, _token)).Status.ShouldBe(200);

        var export = await _api.SendRawAsync(HttpMethod.Get, "/api/v1/manage/customers/export", null, _token);

        export.Status.ShouldBe(200);
        export.ContentType.ShouldBe("text/csv; charset=utf-8");
        export.ContentDisposition!.ShouldContain("musteriler-2026-11-25.csv");
        export.Body.Take(3).ShouldBe(new byte[] { 0xEF, 0xBB, 0xBF }, "UTF-8 BOM");
        var lines = export.Text.TrimStart('﻿').Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        lines[0].ShouldBe("Ad;Telefon;Doğum yılı;İlk kanal;İlk ziyaret;Son ziyaret;Ziyaret;Gelmedi;Toplam harcama (TL);Etiketler;Not");
        lines.Length.ShouldBe(2, "birleşmiş ve anonimleştirilmiş kart yazılmaz");
        lines[1].ShouldBe("Selin Öztürk;0555 000 44 18;1992;Panelden;10.01.2025;20.11.2026;12;0;12345,50;Düzenli;\"'=Kısa; saç\"");
        (await _api.ScalarAsync<string>("SELECT action || '/' || (changes->>'filter') || '/' || (changes->>'rowCount') FROM platform.audit_logs WHERE action = 'Export'"))
            .ShouldBe("Export/All/1");

        var lapsed = await _api.SendRawAsync(HttpMethod.Get, "/api/v1/manage/customers/export?filter=NoShowRisk", null, _token);
        lapsed.Text.TrimStart('﻿').Split("\r\n", StringSplitOptions.RemoveEmptyEntries).Length.ShouldBe(1, "yalnız başlık");
        (await _api.SendRawAsync(HttpMethod.Get, "/api/v1/manage/customers/export?filter=Vip", null, _token)).Status.ShouldBe(400);
        var manager = await _api.AddMemberAndSignInAsync(_token, "+905382741906", "BranchManager", _merkez);
        (await _api.SendRawAsync(HttpMethod.Get, "/api/v1/manage/customers/export", null, manager)).Status.ShouldBe(403, "customer.export yalnız Owner");
    }

    // ───────── KVKK silme ─────────

    [Fact]
    public async Task Deletion_PreviewNameCheck_Anonymizes_CancelsUpcoming_KeepsPayment_Audited()
    {
        var (card, appointmentId) = await AyseWithReceivedDepositAsync();
        await _api.ScalarAsync<int>($"UPDATE customers.brand_customers SET notes = 'Boya tonu 7.1', birth_year = 1990 WHERE id = '{card}' RETURNING 1");
        var kaan = await _api.SignupAsync("+905330000001", "Berber Kaan");
        var kaanCard = (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/customers", new { fullName = "Ayşe", phone = AysePhone }, kaan, Key())).Body.GetProperty("id").GetString();
        (await _api.GetAsync($"/api/v1/manage/customers/{card}/deletion", kaan)).Status.ShouldBe(404, "başka markanın kartı");

        var preview = await _api.GetAsync($"/api/v1/manage/customers/{card}/deletion", _token);
        preview.Status.ShouldBe(200, preview.Body.ToString());
        preview.Body.GetProperty("fullName").GetString().ShouldBe("Ayşe Yılmaz");
        preview.Body.GetProperty("appointmentCount").GetInt32().ShouldBe(1);
        preview.Body.GetProperty("paymentCount").GetInt32().ShouldBe(1);
        preview.Body.GetProperty("upcomingAppointmentCount").GetInt32().ShouldBe(1);
        preview.Body.GetProperty("activePackages").GetArrayLength().ShouldBe(0);

        var mismatch = await DeleteAsync(card, "Ayşe Yıldız");
        mismatch.Status.ShouldBe(400);
        mismatch.Body.GetProperty("errors").GetProperty("confirmFullName")[0].GetString().ShouldBe("mismatch");
        var manager = await _api.AddMemberAndSignInAsync(_token, "+905382741906", "BranchManager", _merkez);
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/customers/{card}/deletion", new { confirmFullName = "Ayşe Yılmaz" }, manager)).Status
            .ShouldBe(403, "customer.kvkk yalnız Owner");

        var deleted = await DeleteAsync(card, "  AYŞE YILMAZ ");

        deleted.Status.ShouldBe(200, deleted.Body.ToString());
        deleted.Body.GetProperty("customerDisplayName").GetString().ShouldBe("A•••• Y••••");
        deleted.Body.GetProperty("requestedVia").GetString().ShouldBe("Panel");
        deleted.Body.GetProperty("status").GetString().ShouldBe("Completed");
        deleted.Body.GetProperty("dueOn").GetString().ShouldBe("2026-12-25");
        deleted.Body.GetProperty("completedUtc").GetString().ShouldBe(Iso(_factory.Clock.GetCurrentInstant()));
        (deleted.Body.TryGetProperty("brandCustomerId", out var bc) && bc.ValueKind != JsonValueKind.Null).ShouldBeFalse();
        await AssertAnonymizedAsync(card, kaan, kaanCard!);
        await AssertErasureSideEffectsAsync(card, appointmentId);

        // Talepler listesi; ikinci deneme 409; aynı numara yeniden eklenince yeni kart açılır.
        var list = await _api.GetAsync("/api/v1/manage/customer-deletion-requests?status=Completed", _token);
        list.Status.ShouldBe(200);
        list.Body.GetProperty("items").EnumerateArray().Select(i => i.GetProperty("customerDisplayName").GetString()).ShouldBe(["A•••• Y••••"]);
        (await _api.GetAsync("/api/v1/manage/customer-deletion-requests?status=Pending", _token)).Body.GetProperty("items").GetArrayLength().ShouldBe(0);
        (await _api.GetAsync("/api/v1/manage/customer-deletion-requests", kaan)).Body.GetProperty("items").GetArrayLength().ShouldBe(0, "başka marka görmez");
        (await _api.GetAsync("/api/v1/manage/customer-deletion-requests?status=Done", _token)).Status.ShouldBe(400);
        var twice = await DeleteAsync(card, "Silinmiş müşteri");
        twice.Status.ShouldBe(409);
        twice.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");
        (await _api.GetAsync($"/api/v1/manage/customers/{card}/deletion", _token)).Status.ShouldBe(404);
        var readded = await CreateCustomerAsync("Ayşe Yılmaz", AysePhone);
        string.Equals(readded, card, StringComparison.Ordinal).ShouldBeFalse("anonim kartla bağ koptu; yeni kart açılır");
        AssertNoPersonalDataInLogs("Ayşe", AysePhone, "Boya tonu");
    }

    // ───────── yardımcılar ─────────

    private async Task AssertAnonymizedAsync(string card, string kaan, string kaanCard)
    {
        // Kart: "Silinmiş müşteri", telefon yok, not ve doğum yılı yok; listede ve aramada yok.
        var cardView = await _api.GetAsync($"/api/v1/manage/customers/{card}", _token);
        cardView.Status.ShouldBe(200);
        cardView.Body.GetProperty("fullName").GetString().ShouldBe("Silinmiş müşteri");
        cardView.Body.TryGetProperty("phone", out _).ShouldBeFalse();
        cardView.Body.TryGetProperty("phoneMasked", out _).ShouldBeFalse();
        cardView.Body.TryGetProperty("notes", out _).ShouldBeFalse();
        cardView.Body.TryGetProperty("birthYear", out _).ShouldBeFalse();
        Names((await _api.GetAsync("/api/v1/manage/customers?q=4567", _token)).Body).ShouldBeEmpty();
        Names((await _api.GetAsync("/api/v1/manage/customers?q=ay%C5%9Fe", _token)).Body).ShouldBeEmpty();
        Names((await _api.GetAsync("/api/v1/manage/customers", _token)).Body).ShouldBeEmpty();
        (await _api.ScalarAsync<string>($"""
            SELECT c.full_name || '/' || (c.phone_e164 LIKE '+000%') || '/' || (c.anonymized_utc IS NOT NULL) || '/' || length(c.phone_e164)
            FROM customers.brand_customers b JOIN customers.customers c ON c.id = b.customer_id WHERE b.id = '{card}'
            """)).ShouldBe("Silinmiş müşteri/true/true/20");

        // Gerçek kişi ve başka markadaki kart etkilenmez.
        (await _api.ScalarAsync<string>($"SELECT full_name FROM customers.customers WHERE phone_e164 = '{AysePhone}'")).ShouldBe("Ayşe Yılmaz");
        (await _api.GetAsync($"/api/v1/manage/customers/{kaanCard}", kaan)).Body.GetProperty("fullName").GetString().ShouldBe("Ayşe");
    }

    private async Task AssertErasureSideEffectsAsync(string card, string appointmentId)
    {
        // Yaklaşan randevu iptal (AccountDeleted, sistem), müşteri bağı anonim; hatırlatmalar düşer, iptal SMS'i yok.
        (await _api.ScalarAsync<string>($"""
            SELECT a.status || '/' || a.cancellation_reason || '/' || a.cancelled_by || '/' || (c.phone_e164 LIKE '+000%')
            FROM scheduling.appointments a JOIN customers.customers c ON c.id = a.customer_id WHERE a.id = '{appointmentId}'
            """)).ShouldBe("Cancelled/AccountDeleted/System/true");
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'appointment.reminder' AND status = 'Pending'")).ShouldBe(0);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'appointment.cancelled_by_salon'")).ShouldBe(0);

        // Ödeme kaydı anonim kartta kalır; denetim kaydı Anonymize, kişisel veri yok.
        (await _api.ScalarAsync<string>("SELECT brand_customer_id::text FROM sales.payments")).ShouldBe(card);
        var audit = await _api.ScalarAsync<string>("SELECT action || '/' || entity_type || '/' || entity_id || '/' || changes::text FROM platform.audit_logs WHERE action = 'Anonymize'");
        audit.ShouldStartWith($"Anonymize/BrandCustomer/{card}/");
        audit.ShouldContain("\"cancelledAppointments\": 1");
        audit.ShouldNotContain("Ayşe");
    }

    private async Task<(string Card, string AppointmentId)> AyseWithReceivedDepositAsync()
    {
        var risky = await CreateCustomerAsync("Ayşe Yılmaz", AysePhone);
        await _api.ScalarAsync<int>($"UPDATE customers.brand_customers SET no_show_count = 2 WHERE id = '{risky}' RETURNING 1");
        var created = await BookAsync(AysePhone, "Ayşe Yılmaz", FridayAt1430);
        created.Status.ShouldBe(201, created.Body.ToString());
        var id = created.Body.GetProperty("id").GetString()!;
        var etag = (await _api.GetAsync($"/api/v1/manage/appointments/{id}", _token)).ETag!;
        var marked = await _api.SendAsync(
            HttpMethod.Post, $"/api/v1/manage/appointments/{id}/deposit/mark-received", null, _token,
            new Dictionary<string, string>(Key(), StringComparer.Ordinal) { ["If-Match"] = etag });
        marked.Status.ShouldBe(200, marked.Body.ToString());
        return (risky, id);
    }

    private async Task<string> CreateCustomerAsync(string fullName, string phone, string? notes = null)
    {
        var created = await PostAsync("/api/v1/manage/customers", new { fullName, phone, notes });
        created.Status.ShouldBe(201, created.Body.ToString());
        return created.Body.GetProperty("id").GetString()!;
    }

    private Task<ManageApi.Response> MergeAsync(string target, object body) =>
        _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/customers/{target}/merge", body, _token);

    private Task<ManageApi.Response> DeleteAsync(string card, string confirmFullName) =>
        _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/customers/{card}/deletion", new { confirmFullName }, _token);

    private Task<ManageApi.Response> CommitAsync(string importId, object? body) =>
        _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/customers/imports/{importId}/commit", body, _token);

    private Task<ManageApi.RawResponse> PreviewAsync(string csv, string fileName, Encoding? encoding = null, string? token = null)
    {
        var content = new MultipartFormDataContent();
        var file = new ByteArrayContent((encoding ?? Encoding.UTF8).GetBytes(csv));
        file.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
        content.Add(file, "file", fileName);
        return _api.SendRawAsync(HttpMethod.Post, "/api/v1/manage/customers/imports", content, token ?? _token, Key());
    }

    private async Task<ManageApi.Response> BookAsync(string phone, string fullName, string startUtc) =>
        await _api.SendAsync(HttpMethod.Post, $"/api/v1/public/branches/{_merkez}/appointments", new
        {
            items = new[] { new { serviceId = _balyaj, staffId = _elif } },
            sameStaffForAll = true,
            startUtc,
            fullName,
            privacyNoticeAcknowledged = true,
            privacyNoticeVersion = "2026-11-01",
        }, await VerifyAsync(phone), Key());

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

    private void AssertNoPersonalDataInLogs(params string[] values)
    {
        var text = string.Join('\n', _logs.Messages);
        foreach (var value in values)
        {
            text.ShouldNotContain(value, Case.Sensitive, "günlükte kişisel veri olmamalı");
        }
    }

    private static string Iso(Instant at) => NodaTime.Text.InstantPattern.General.Format(at);

    private static List<string> Names(JsonElement list) =>
        [.. list.GetProperty("items").EnumerateArray().Select(i => i.GetProperty("fullName").GetString()!)];

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _requests):D12}" };

    private sealed class CapturingLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentQueue<string> _messages = new();

        public IReadOnlyCollection<string> Messages => _messages.ToArray();

        public ILogger CreateLogger(string categoryName) => new CapturingLogger(_messages);

        public void Dispose()
        {
        }

        private sealed class CapturingLogger(ConcurrentQueue<string> messages) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state)
                where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
                messages.Enqueue(formatter(state, exception) + (exception is null ? string.Empty : "\n" + exception));
        }
    }
}
