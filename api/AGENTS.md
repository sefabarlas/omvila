# api/: backend ajan kuralları

.NET 10 modüler monolit, PostgreSQL 17, EF Core 10, NodaTime. Önce kök [AGENTS.md](../AGENTS.md)'yi okuyun.

Zorunlu okuma: [docs/mimari/mimari.md](../docs/mimari/mimari.md), [docs/standartlar/kodlama-dotnet.md](../docs/standartlar/kodlama-dotnet.md), [docs/standartlar/test-stratejisi.md](../docs/standartlar/test-stratejisi.md). İşe göre: `docs/alan/*`, ilgili ADR.

## Yapı
```
api/
  Omvila.slnx  global.json  Directory.Build.props  Directory.Packages.props  BannedSymbols.txt  .editorconfig
  src/Omvila.Host/                     # Program.cs; --role=api|worker|all
  src/Omvila.SharedKernel/             # Result, Error, ErrorCodes, Money, PhoneNumber, Ids, IClock, IIdGenerator
  src/Omvila.Infrastructure/           # bağlantı + transaction, RLS kurulumu, outbox, iş zamanlayıcı, idempotency, dış servis adaptörleri
  src/Modules/<Modül>/Omvila.Modules.<Modül>/            # Domain/ Features/ Infrastructure/ Jobs/ IntegrationEventHandlers/
  src/Modules/<Modül>/Omvila.Modules.<Modül>.Contracts/  # public arayüzler + olaylar
  tests/Omvila.Modules.<Modül>.Tests/                    # alan + handler birim
  tests/Omvila.Modules.<Modül>.IntegrationTests/         # Testcontainers
  tests/Omvila.ArchitectureTests/  tests/Omvila.Api.ContractTests/  tests/Omvila.TestKit/
```
Modüller: Identity, Tenancy, Catalog, Customers, Scheduling, Sales, Reviews, Notifications, Integrations, Platform. Hangi varlık hangi modülde: [alan-modeli.md § Modüller](../docs/alan/alan-modeli.md#modüller).

## Yeni bir uç nokta eklerken (sıra)
1. Uç `contracts/openapi.yaml`'da mı? Değilse önce oraya ekleyin ([contracts/AGENTS.md](../contracts/AGENTS.md)).
2. Modülü belirleyin. Klasör: `Features/<FiilIsim>/`.
3. Alan kuralı varsa önce `Domain/` testini yazın, sonra aggregate metodunu.
4. `Request`/`Response` kayıtlarını sözleşmedeki şemayla **aynı adlar ve tiplerle** yazın (camelCase JSON, enum metin).
5. Handler: yükle → alan metodu → kaydet → eşle. Yan etki = entegrasyon olayı veya outbox satırı.
6. Endpoint: `.WithName("<operationId>")`, `.RequirePermission(...)` veya `.AllowAnonymous()` + gerekçe yorumu, gerekiyorsa `.RequireIdempotencyKey()` / `.RequireIfMatch()`.
7. Entegrasyon testi: mutlu yol, 403, 404 (başka marka), iş hatası kodları. Sözleşme testi otomatik koşar.
8. Migration (gerekiyorsa): RLS politikası, indeks (`brand_id` önce), CHECK kısıtları.

## Kesin kurallar
- Katman ve modül sınırları: [mimari.md § Katman kuralları](../docs/mimari/mimari.md#katman-kuralları-mimari-testlerle-zorlanır). İhlal mimari testte kırılır.
- Kiracı bağlamı JWT'den gelir. İstek gövdesinden veya başlıktan `brandId` okunmaz.
- Durum yalnız aggregate metoduyla değişir. Geçiş tablosu: [durum-makinesi.md](../docs/alan/durum-makinesi.md). Her geçiş için test.
- Randevu yazma yolu: kilitler (sıralı) → süresi dolmuş tutmaları iptal → müsaitliği yeniden hesapla → yaz → outbox → commit. `23P01` → `slot_taken`. Ayrıntı: [musaitlik.md](../docs/alan/musaitlik.md).
- Zaman `IClock`; kimlik `IIdGenerator`; para `Money`; telefon `PhoneNumber`.
- Hata kodları yalnız `ErrorCodes` sabitlerinden; yeni kod önce sözleşmeye.
- Ayar ve süre sabitleri `PlatformDefaults` içinde, her biri `sureler-ve-ayarlar.md` satırına atıfla.
- Dış servisler (`ISmsSender`, `IPushSender`, `IEmailSender`, `ICalendarProvider`, `IFileStorage`) arayüz arkasında. Geliştirme ve testte sahte uygulama (`Fake*`). Sahte SMS mesajları `infra.dev_sms_log` tablosuna ve konsola yazar.
- MediatR, AutoMapper, Hangfire, EF InMemory, SQLite kullanılmaz.

## Komutlar
`dotnet test` komutları `api/` içinden çalışır: test platformu (Microsoft.Testing.Platform) ayarı `api/global.json`'dadır; depo kökünden çalıştırılırsa eski VSTest kipine düşer.
```bash
docker compose up -d postgres                                  # kökten
cd api
dotnet build Omvila.slnx -warnaserror
dotnet test --solution Omvila.slnx --filter-not-trait "Category=Integration"   # hızlı
dotnet test --solution Omvila.slnx                                             # tümü (Docker gerekir)
dotnet format Omvila.slnx --verify-no-changes
dotnet run --project src/Omvila.Host -- --role=all             # http://localhost:5080
dotnet ef migrations add <YYYYMMDDHHmm_Ad> -p src/Modules/Scheduling/Omvila.Modules.Scheduling -c SchedulingDbContext -o Infrastructure/Migrations
scripts/export-openapi.sh && oasdiff diff ../contracts/openapi.yaml artifacts/openapi.json --fail-on-diff
```

## Geliştirme verisi
`dotnet run --project api/src/Omvila.Host -- seed` tasarım ekranlarındaki verileri kurar: Kuaför Elif (Kadıköy, Nişantaşı), personel Elif Kaya, Mert Demir, Zeynep Aksoy, Can Yücel; hizmetler Saç/Renk/Bakım/Tırnak; örnek müşteriler. Seed yalnız `Development` ortamında çalışır.

## Analizör istisnaları
`api/.editorconfig` kapatılan kuralları gerekçesiyle listeler (CA1716, CA1000, MA0008, MA0004; testlerde CA1707). Yeni istisna gerekçesiz eklenmez.
