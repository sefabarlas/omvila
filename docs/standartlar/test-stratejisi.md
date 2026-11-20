# Test stratejisi

Kodun tamamı ajanlarla yazıldığı için testler, ajanın "bitti" iddiasının tek kanıtıdır. Test yazılmadan veya testi zayıflatarak (atlayarak, `Skip`, beklentiyi gevşeterek) geçirilen iş bitmiş sayılmaz.

## Piramit ve zorunluluklar

| Katman | Araç | Neyi | Zorunlu olduğu yer |
|---|---|---|---|
| Alan birim | xUnit v3 + Shouldly | Aggregate metotları, durum geçişleri, müsaitlik motoru, para ve kapora hesabı | Her alan kuralı; `durum-makinesi.md` tablosundaki **her satır**; `musaitlik.md` M1–M20 |
| Özellik tabanlı | CsCheck | `IntervalSet` cebri, `Money` yuvarlama, telefon normalleştirme | Bu üç yardımcı |
| Uygulama/handler | xUnit + Testcontainers PostgreSQL | Handler'ın uçtan uca davranışı (HTTP'siz), RLS, transaction | Her yazma özelliği |
| API entegrasyon | `WebApplicationFactory` + Testcontainers | HTTP, yetki, hata kodları, sözleşmeye uygun yanıt | Her uç nokta: mutlu yol + yetkisiz (403) + kapsam dışı (404) + ilgili iş hatası |
| Eşzamanlılık | Testcontainers, paralel görevler | Çakışma engeli | `musaitlik.md` C1–C7, CI'da her çalıştırmada |
| Mimari | ArchUnitNET | Katman ve modül sınırları, yasaklı bağımlılıklar | Her derlemede |
| Kiracı izolasyonu | API entegrasyon | A markası B'nin hiçbir kaydına erişemez | Her yeni `manage` uç noktası otomatik kapsanır (uç listesi sözleşmeden okunur) |
| Sözleşme | `Omvila.Api.ContractTests` + oasdiff | Yanıt gövdeleri şemaya uyuyor; uygulamanın OpenAPI'si = `contracts/openapi.yaml` | Her derlemede |
| Web bileşen | Vitest + Testing Library | Bileşen davranışı, biçimlendirme yardımcıları, hata eşleme | `packages/ui-web` her bileşen; uygulamalarda karmaşık form ve takvim mantığı |
| Web uçtan uca | Playwright (Prism mock'a karşı; gece backend'e karşı) | Kritik akışlar | Aşağıdaki liste |
| Flutter widget | `flutter_test` | Ekran durumları (yükleniyor, boş, hata, dolu) | Her ekran |
| Flutter golden | `alchemist` | Açık/koyu tema, 1.3× yazı ölçeği | `omvila_ui` widget'ları ve ana ekranlar |
| Flutter entegrasyon | `integration_test` (Prism mock) | Kritik akışlar | Aşağıdaki liste |
| Görsel karşılaştırma | Playwright ekran görüntüsü | Tasarım ekranlarıyla yan yana kontrol (insan gözü) | Her yeni ekran PR'ında ekran görüntüsü eklenir |

## Kritik akışlar (uçtan uca)
1. Salon kaydı → kurulum adımları → rezervasyon bağlantısını kopyalama
2. Rezervasyon sayfası: hizmet → personel → saat → telefon → kod → onay → başarı (onaysız salon)
3. Aynı akış onaylı salonda → "Talebiniz alındı" → panelde onay → müşteri SMS'i (sahte sağlayıcı günlüğü)
4. Kapora: randevu → havale ekranı → "Havaleyi yaptım" → panelde "Kapora geldi" → onaylandı
5. Kapora süresi dolması → otomatik iptal (saat ileri alınarak)
6. SMS bağlantısından iptal (neden zorunlu) ve saat değiştirme (son süre)
7. Panel takvimi: yeni randevu, sürükle-bırak, çakışma uyarısı, eşzamanlı düzenleme çakışması
8. Bekleme listesi teklifi: boşalma → teklif → kabul / süre dolması
9. Kapıda sıra: ekle → başlat → tamamla ve ödeme al
10. Müşteri uygulaması: giriş → randevularım → iptal → değerlendirme

## Test verisi
- `Omvila.TestKit`: kurucu (builder) sınıfları `ABrand().WithBranch("Kadıköy").WithStaff("Elif Kaya")…`. Testler ham `new Entity()` ile veri kurmaz.
- Örnek adlar tasarımdakilerle aynıdır (Kuaför Elif, Selin Öztürk). Gerçek kişi verisi kullanılmaz.
- Telefonlar test aralığından: `+90555000xxxx`.
- Zaman her testte sabittir: `FakeClock` başlangıcı `2026-11-25T09:00:00Z` (Salı, 12:00 İstanbul). Testler gerçek saate bağlı olamaz 🔒.
- Veritabanı testler arasında Respawn ile sıfırlanır; her test sınıfı kendi markasını kurar ve paralel koşabilir.

## Adlandırma
- .NET: `MetotVeyaDavranis_Kosul_BeklenenSonuc` → `Approve_WhenHoldExpired_ReturnsInvalidStateTransition`.
- Kural bağlantısı: test sınıfının veya metodun üstünde `// docs/alan/durum-makinesi.md#A5`.
- TS/Dart: `describe('formatMoney')` + `it('1.250 ₺ biçiminde yazar')` (Türkçe açıklama serbest).

## Kapsam hedefleri
Satır kapsama hedef değil, **kural kapsaması** hedeftir:
- `Domain` klasörleri: dal kapsaması ≥ %90 (CI'da raporlanır, `Scheduling` ve `Sales` için zorunlu eşik).
- Diğer kodda eşik yok. "Kural tablosundaki her satırın testi var mı?" sorusu PR incelemesinde sorulur. Mümkün olan yerde otomatik eşleştirilir (durum geçiş tablosu ↔ test adları).

## Hızlı geri bildirim
| Komut | Ne çalışır | Hedef süre |
|---|---|---|
| `dotnet test --solution Omvila.slnx --filter-not-trait "Category=Integration"` (api/ içinden) | Alan + mimari | < 30 sn |
| `dotnet test --solution Omvila.slnx` (api/ içinden) | Hepsi (Testcontainers) | < 5 dk |
| `pnpm turbo test --filter=...[origin/main]` | Değişen web paketleri | < 2 dk |
| `melos run test` | Flutter paketleri | < 3 dk |

## Ajan kuralları (özet)
1. Önce testi yazın veya güncelleyin, sonra kodu. Hata düzeltmesi, hatayı yeniden üreten bir testle başlar.
2. Var olan bir testi silmek veya beklentisini değiştirmek, iş kuralının değiştiğini gösteren bir belge değişikliği (veya kullanıcı onayı) gerektirir. PR açıklamasında belirtilir.
3. `Skip`, `[Fact(Skip=…)]`, `test.skip`, `@Skip` ile birleştirilmiş PR kabul edilmez. Kararsız (flaky) test düzeltilir veya ayrı bir sorun kaydıyla karantinaya alınır.
4. Mock yalnız dış sınırda: SMS, push, e-posta, Google, saat. Veritabanı mock'lanmaz; EF InMemory ve SQLite yasaktır.
