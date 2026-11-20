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
docker compose up -d postgres                                  # kökten (api/db/init rol betiği ilk açılışta çalışır)
cd api
dotnet tool restore                                            # dotnet-ef (dotnet-tools.json)
dotnet run --project src/Omvila.Host -- migrate                # migration'ları omvila_migrator ile uygular
dotnet build Omvila.slnx -warnaserror
dotnet test --solution Omvila.slnx --filter-not-trait "Category=Integration"   # hızlı
dotnet test --solution Omvila.slnx                                             # tümü (Docker gerekir)
dotnet format Omvila.slnx --verify-no-changes
dotnet run --project src/Omvila.Host -- --role=all             # http://localhost:5080
dotnet ef migrations add <KisaAciklama> -p src/Modules/Scheduling/Omvila.Modules.Scheduling -c SchedulingDbContext -o Infrastructure/Migrations   # EF zaman damgasını ekler
scripts/export-openapi.sh && oasdiff diff ../contracts/openapi.yaml artifacts/openapi.json --fail-on-diff
```

## Veritabanı erişimi
- Uygulama `omvila_app` rolüyle bağlanır (RLS'e tabi, tablo sahibi değil); migration `omvila_migrator` ile çalışır (ADR-0001). Roller `api/db/init/01-roles.sql`'de; docker-compose ve Testcontainers aynı betiği kullanır.
- Her API ucu `MapApiGroup(...)` grubunda tek transaction'da çalışır: başarılı sonuç yanıt yazılmadan commit, 4xx/5xx ve istisna rollback (`TransactionEndpointFilter`). İş (worker) kendi kapsamında `IDbSession.BeginAsync` çağırır.
- Kiracı bağlamı `IDbSession.SetTenantAsync(TenantScope...)` ile kurulur ve `set_config(..., true)` ile yalnız o transaction'a uygulanır.
- Modül DbContext'i `services.AddModuleDbContext<T>("schema")` ile kaydedilir; ilk migration `GrantSchemaUsageToApp`, markaya ait her tablo `EnableTenantIsolation` çağırır. `RlsGuardTests` politikasız `brand_id` tablosunda düşer.
- Testler: `Omvila.TestKit` (`PostgresFixture`, `TestClock`, `RlsGuard`). Veritabanı testleri `[Trait("Category", "Integration")]` ve `[Collection(PostgresTests.Name)]` taşır.
- **HTTP eşlemesi:** uç `return result.ToHttp(r => TypedResults.Ok(r));` ile biter; hata `Error.ToProblem()` ile problem+json olur (code, errors, meta). Veritabanı hataları `DatabaseExceptionHandler`'da eşlenir: eşzamanlılık → 412, `23P01` → `slot_taken`, kayıtlı benzersiz indeks → `services.AddUniqueConstraintError("ix_…", ErrorCodes.…)`. Kayıtsız 23505 → 500.
- **Eşzamanlı düzenleme:** düzenleme ve durum uçları `.RequireIfMatch()`; handler sürümü `httpContext.GetIfMatchVersion()` ile alır (xmin). `GET` yanıtı `ETag: ETagVersion.Format(xmin)` döner.
- **Idempotency:** kaynak oluşturan ve para kaydeden POST'lar `.RequireIdempotencyKey()`. Aynı anahtar + aynı istek ilk başarılı yanıtı döner (`Idempotent-Replayed: true`); farklı istek 422 `idempotency_key_reused`. Kayıt kiracı kapsamlıdır, iş verisiyle aynı transaction'da yazılır; hata sonuçları saklanmaz. Saklama süresi `Idempotency` ayarlarında, `PurgeIdempotencyKeys` siler.
- **Outbox (ADR-0005):** yan etkiler `IOutbox.EnqueueAsync(new XHappened(...))` ile iş transaction'ında yazılır. Olay `IIntegrationEvent` uygulayan bir record'dur (modülün Contracts projesinde) ve markaya aitse `BrandId` taşır. İşleyici `IIntegrationEventHandler<T>`, kayıt `services.AddIntegrationEventHandler<T, THandler>()`. İşleyici kendi transaction'ında çalışır ve kiracı bağlamını olaydan kurar; aynı olay için ikinci kez çağrılmaz. Hata olursa yalnız o işleyici geri çekilmeyle yeniden denenir (`Outbox` ayarları appsettings'te).
- **Periyodik işler:** `IPeriodicJob` (`Key`, `Interval`, `RunAsync`), kayıt `services.AddPeriodicJob<T>()`. Yalnız worker rolünde zamanlanır; advisory lock ile tek örnekte çalışır, sonucu `infra.job_runs`'a yazılır. `JobRunner` işi platform kapsamlı açık bir transaction'da çözer: iş DbContext'i kurucudan alır, oturumu kendisi başlatmaz veya commit etmez; başarıda commit, istisnada rollback. Testte `JobRunner.RunOnceAsync(typeof(T))` ile doğrudan çağrılır, saat `FakeClock` ile ilerletilir.
- Yerelde 5432'de başka bir PostgreSQL çalışıyorsa `localhost` ona gider; `ConnectionStrings__Default/Migrator` ile portu değiştirin.

## Geliştirme verisi
`dotnet run --project api/src/Omvila.Host -- seed` tasarım ekranlarındaki verileri kurar: Kuaför Elif (Kadıköy, Nişantaşı), personel Elif Kaya, Mert Demir, Zeynep Aksoy, Can Yücel; hizmetler Saç/Renk/Bakım/Tırnak; örnek müşteriler. Seed yalnız `Development` ortamında çalışır.

## Analizör istisnaları
`api/.editorconfig` kapatılan kuralları gerekçesiyle listeler (CA1716, CA1000, MA0008, MA0004; testlerde CA1707; EF migration dosyaları `generated_code`). Yeni istisna gerekçesiz eklenmez.
