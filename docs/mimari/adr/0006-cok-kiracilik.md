# ADR-0006: Çok kiracılık

- Durum: Kabul edildi
- Tarih: 2026-11-20 (K4 istisnası: 2026-11-22)

## Bağlam
"Müşteri verisi işletmeye aittir" ürünün temel sözüdür. Bir salonun başka salonun verisini görmesi kabul edilemez. Ajanlar tarafından yazılan kodda bir sorguya filtre eklemeyi unutmak olasıdır; tek savunma hattı yetmez.

## Karar
**İki katman birlikte:**

1. **EF Core global query filter:** Markaya ait her varlıkta `BrandId == tenant.BrandId`. `IgnoreQueryFilters()` yasaktır (analizör + mimari test). Yalnız `Platform` modülündeki açıkça işaretli okuma servisleri platform bağlamında çalışır.
2. **PostgreSQL Row-Level Security:** Markaya ait her tabloda `ENABLE ROW LEVEL SECURITY` + `FORCE ROW LEVEL SECURITY`. Politika:
   ```sql
   CREATE POLICY tenant_isolation ON scheduling.appointments
     USING (
       brand_id = nullif(current_setting('app.brand_id', true), '')::uuid
       OR current_setting('app.is_platform', true) = 'true'
       OR (customer_id = nullif(current_setting('app.customer_id', true), '')::uuid)  -- müşteri erişimi olan tablolarda
     );
   ```
   Uygulama her transaction başında `SELECT set_config('app.brand_id', @id, true)` çalıştırır (`true` = transaction'a yerel). Bağlantı havuzunda değer sızmaz.

**Kiracı bağlamı nereden gelir**

| Yüzey | Bağlam |
|---|---|
| `manage` | JWT'deki üyelik (`brand_id`, `branch_ids`, izinler). İstek başlığından veya gövdesinden marka kimliği **alınmaz**. |
| `admin` | `app.is_platform = true`; yazma platform tablolarına; başka modülün verisine yalnız aşağıdaki K4 istisnasıyla |
| `me` | `app.customer_id`; müşterinin tüm markalardaki kendi satırları |
| `public` | URL'deki marka slug'ı veya şube kimliğinden çözülen marka; erişim kodu uçlarında koddan çözülen randevunun markası |
| worker | İş her markanın satırını işlerken o markanın bağlamını kurar; marka ötesi tarama yapan işler (`ExpireAppointmentHolds`) `app.is_platform` ile çalışır ve yalnız kendi modülünün tablosuna dokunur |

- Global `customers.customers` tablosunda `brand_id` yoktur. Marka bağlamı bu tabloyu yalnız `brand_customers` üzerinden bağlı satırlar için okur (politika `EXISTS`). Bir marka, telefonla arama yaparken başka markanın müşterisinin varlığını öğrenemez: arama `brand_customers` üzerinden yapılır; yeni müşteri eklerken global kayıt sunucuda bulunur veya oluşturulur ve yanıtta yalnız markanın kendi görünümü döner.
- Kapsam dışı kayıt → `404`.

**İstisna K4: platformun başka modüllere yazması (2026-11-22, ürün sahibi kararı)**

Platform paneli ve abonelik işleri bazı durumlarda başka modülün verisini değiştirmek zorundadır: havale eşleşince kontör eklemek (Notifications), SMS gönderen adını onaylamak (Notifications), markayı askıya almak veya iptalden sonra arşive almak (Tenancy). Bu yazımlar:

1. Yalnız **o modülün Contracts arayüzü** üzerinden yapılır (ör. `ICreditLedger.PurchaseAsync`, Tenancy `IBrandBilling.ArchiveAsync`). Platform başka modülün tablosuna veya iç sınıfına dokunmaz; kural, doğrulama ve kayıt şekli sahibi modülde kalır.
2. Çağrı, işin kendi transaction'ında yapılır; mümkünse oturum o markanın bağlamına geçirilir (`TenantScope.ForBrand`), böylece RLS yine çalışır. Marka ötesi tarama yapan iş (`AdvanceSubscriptionStatuses`) platform bağlamında kalır ve yalnız Contracts arayüzünü çağırır.
3. Platform kullanıcısının yaptığı her yazım denetim kaydına (`IAuditLog`, `ActorType = PlatformUser`) yazılır.
4. Platform kullanıcısı salonun iş verisini (randevu, müşteri, tahsilat) değiştiremez; istisna yalnız abonelik, kontör, SMS gönderen adı ve marka durumu içindir (guvenlik-kvkk.md § Platform erişimi).

## Sonuçlar
- Her yeni tablo için RLS politikası migration'da zorunludur. `Omvila.ArchitectureTests` içindeki veritabanı testi, `brand_id` sütunu olan ve politikası olmayan tablo bulursa başarısız olur.
- Kiracı izolasyon testi: iki marka oluştur, A bağlamında B'nin her uç noktasını çağır → hepsi 404 (`Omvila.Api.TenantIsolationTests`).
- RLS performansı: `brand_id` her tabloda indeksin ilk sütunudur.

## Değerlendirilen alternatifler
- Marka başına şema veya veritabanı: binlerce küçük salon için operasyon yükü, göç zorluğu.
- Yalnız EF filtresi: tek hat; ham SQL ve hatalı sorguda koruma yok.
