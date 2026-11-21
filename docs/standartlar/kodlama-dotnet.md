# Kodlama standartları: .NET (api/)

Mimari kurallar `docs/mimari/mimari.md` ve ADR'lerdedir. Bu belge kod düzeyindeki kuralları verir. Otomatik kontrol edilenler (analizör, mimari test) "🔒" ile işaretlidir.

## Proje ayarları
- `net10.0`, `<Nullable>enable</Nullable>`, `<ImplicitUsings>enable</ImplicitUsings>`, `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`, `<AnalysisLevel>latest-recommended</AnalysisLevel>` 🔒
- Analizörler: Microsoft.CodeAnalysis.NetAnalyzers, Meziantou.Analyzer, `BannedApiAnalyzers` (`BannedSymbols.txt`) 🔒
- Biçim: `dotnet format` + `.editorconfig`; CI'da `dotnet format --verify-no-changes` 🔒

## Yasaklı API'ler (`BannedSymbols.txt`) 🔒
| Yasak | Yerine |
|---|---|
| `DateTime.Now`, `DateTime.UtcNow`, `DateTimeOffset.Now/UtcNow`, `DateTime.Today` | `IClock.GetCurrentInstant()` |
| `TimeZoneInfo.*` | NodaTime `DateTimeZoneProviders.Tzdb` |
| `Guid.NewGuid()` | `IIdGenerator.New()` (UUID v7) |
| `EntityFrameworkQueryableExtensions.IgnoreQueryFilters` | Yok (ADR-0006) |
| `Console.WriteLine` | `ILogger<T>` |
| `Thread.Sleep`, `Task.Wait()`, `.Result` | `await` |
| `System.Random` (güvenlik amaçlı) | `RandomNumberGenerator` |
| `HttpClient` doğrudan `new` | `IHttpClientFactory` |

## Adlandırma
- Sınıf, metot, özellik: PascalCase. Yerel değişken, parametre: camelCase. Özel alan: `_camelCase`.
- Async metotlar `Async` ekiyle biter; `CancellationToken ct` son parametredir ve her I/O çağrısına geçirilir.
- Özellik klasörü fiil + isim: `CreateAppointment`, `ApproveAppointment`. Dosyalar: `{Ozellik}Endpoint.cs`, `{Ozellik}Request.cs`, `{Ozellik}Response.cs`, `{Ozellik}Handler.cs`, `{Ozellik}Validator.cs`.
- Alan terimleri `docs/alan/sozluk.md`'deki İngilizce karşılıklardır; eş anlamlı kullanılmaz (`Booking` değil `Appointment`, `Employee` değil `Staff`).
- Tablo ve sütun adları EF `snake_case` dönüştürücüsünden gelir; elle ad verilmez.

## Alan katmanı (Domain/)
- Aggregate kökleri `sealed class`, durum alanları `private set`. Dışarıdan durum yazılmaz; her geçiş bir metottur (`Approve(UserId by, Instant now)`) 🔒 (mimari test: `Domain` içindeki `set` erişimcileri `private`/`init`).
- Değer nesneleri `readonly record struct` veya `sealed record`: `Money`, `PhoneNumber`, `TransferCode`, `TimeRange`, `BrandId`, `AppointmentId` (güçlü tipli kimlikler).
- Para işlemleri yalnız `Money` üzerinden yapılır; farklı para birimleri toplanamaz. Yüzde hesabı `Money.Percent(decimal, MidpointRounding)`.
- İş kuralı ihlali: `Result<T>` döner. Programcı hatası (olmaması gereken durum): `DomainException`. Exception akış kontrolü için kullanılmaz.
- Alan olayları aggregate'te toplanır (`DomainEvents`), kayıt sırasında entegrasyon olayına çevrilir.
- Alan katmanı `IClock`'u parametre olarak alır; içinde servis bulucu (service locator) yoktur.

## Features (dikey dilim)
```csharp
internal static class CreateAppointmentEndpoint
{
    public static void Map(RouteGroupBuilder g) =>
        g.MapPost("/appointments", Handle)
         .WithName("manageCreateAppointment")          // = openapi operationId
         .RequirePermission(Permissions.Appointment.Create)
         .RequireIdempotencyKey()
         .Produces<AppointmentCreatedResponse>(StatusCodes.Status201Created)
         .ProducesProblem(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(
        CreateAppointmentRequest request, CreateAppointmentHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(request, ct);
        return result.ToHttp(created: r => $"/api/v1/manage/appointments/{r.Appointment.Id}");
    }
}
```
- `WithName` = sözleşmedeki `operationId` 🔒 (sözleşme testi).
- Uç noktada iş kuralı yok; handler'ı çağır, `Result`'ı HTTP'ye çevir.
- Handler: yükle → alan metodunu çağır → `SaveChangesAsync` → yanıtı elle eşle. Bir handler başka handler'ı çağırmaz.
- Okuma uçları (`Get…`, `List…`) `AsNoTracking()` ve projeksiyon (`Select`) kullanır; aggregate yüklemez.
- Doğrulayıcı yalnız biçim kontrolü yapar (boş, uzunluk, aralık). Veritabanı gerektiren kontrol handler'da veya alanda yapılır.

## EF Core ve PostgreSQL
- Modül başına bir `DbContext`, kendi şemasında (`HasDefaultSchema("scheduling")`).
- Konfigürasyon `IEntityTypeConfiguration<T>` sınıflarında; veri notasyonu (attribute) kullanılmaz.
- Her markaya ait varlık `IBrandOwned` arayüzünü uygular; global filtre ve RLS politikası buna göre kurulur 🔒.
- Migration adı: `KisaAciklama`; EF dosya adına `YYYYMMDDHHmmss_` zaman damgasını kendisi ekler (`dotnet ef migrations add KisaAciklama`). RLS politikaları, exclusion constraint'ler ve özel indeksler migration içinde `migrationBuilder.Sql(...)` ile yazılır; bu SQL dosyaları `Infrastructure/Migrations/Sql/` altında adlandırılmış dosyalardır.
- Döngü içinde sorgu yasak (N+1). Toplu yükleme için `Where(x => ids.Contains(x.Id))`.
- Ham SQL yalnız `FromSql` / `SqlQuery` ile parametreli; string birleştirme yasak 🔒.
- Para `bigint`, zaman `timestamptz`, yerel saat `time`, tarih `date`, aralık `tstzrange`.

## Hata eşleme
| Alan hatası | HTTP | code |
|---|---|---|
| `Error.Validation` | 400 | `validation_failed` |
| `Error.NotFound` | 404 | `not_found` |
| `Error.Conflict(code)` | 409 | verilen kod (`slot_taken`, `cutoff_passed`…) |
| `Error.Forbidden` | 403 | `forbidden` |
| `DbUpdateConcurrencyException` | 412 | `concurrency_conflict` |
| `PostgresException 23P01` | 409 | `slot_taken` |
| `Error.Unavailable()` (randevu kilidi zaman aşımı) | 503 | `try_again` |
| `PostgresException 23505` (bilinen indeks) | 409 | indekse eşlenen kod (`phone_already_exists`) |
Hata kodları `SharedKernel/ErrorCodes.cs` içinde sabittir ve `ErrorCode` enum'uyla birebir aynıdır 🔒 (sözleşme testi).

## Günlük ve gizlilik
- Yapılandırılmış günlük: `logger.LogInformation("Appointment {AppointmentId} created for brand {BrandId}", …)`. Metin birleştirme yok.
- Telefon, ad, e-posta, IBAN, not içeriği **günlüğe yazılmaz**. Gerekirse `PhoneNumber.Masked` kullanılır. `[LogMasked]` öznitelikli alanlar Serilog'da maskelenir.
- OTP kodu hiçbir koşulda günlüğe yazılmaz (geliştirmedeki sahte SMS sağlayıcısı hariç; o da yalnız `Development` ortamında).

## Yorum ve belge
- Kod kendini anlatsın. Yorum "neden"i söyler, "ne"yi değil.
- İş kuralı uygulayan metodun üstünde kuralın belge bağlantısı: `// Kural: docs/alan/durum-makinesi.md#A5`.
- Public `Contracts` tipleri XML belge yorumu taşır.

## Bağımlılıklar
Yeni NuGet paketi eklemek için gerekçe PR açıklamasında yazılır. Onaylı ana paketler: Npgsql.EntityFrameworkCore.PostgreSQL (+ NodaTime eklentisi), EFCore.NamingConventions, NodaTime, FluentValidation, Serilog, OpenTelemetry, Microsoft.AspNetCore.OpenApi, xUnit v3, Shouldly, NSubstitute, Testcontainers.PostgreSql, Respawn, ArchUnitNET, CsCheck, Verify.
