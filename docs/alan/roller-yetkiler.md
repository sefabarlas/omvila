# Roller ve yetkiler

Yetki kontrolü **izin tabanlıdır**: uç nokta bir izin kodu ister (`.RequirePermission(Permissions.Appointment.Approve)`), rol adı istemez. Roller yalnız izin paketleridir. Arayüz de menü ve butonları aynı izinlere göre gizler. Gizlemek yetkilendirme değildir; sunucu her istekte yeniden kontrol eder.

## Salon rolleri

| Rol | Kod | Kapsam | Kimdir |
|---|---|---|---|
| İşletme sahibi | `Owner` | Tüm şubeler | Kaydı yapan kişi; en az bir tane olmak zorunda |
| Şube yöneticisi | `BranchManager` | Atandığı şube(ler) | Müdür, resepsiyon |
| Personel | `Staff` | Atandığı şube(ler), çoğu işlemde **yalnız kendi** kayıtları | Kuaför, berber, uzman |

**Kapsam kuralları**
- `BranchManager` başka şubenin verisini görmez ve değiştiremez. İstenirse **404** döner, 403 değil (varlık sızdırılmaz).
- `Staff` için "kendi" = randevu kaleminin `StaffId`'si veya prim kaydının `StaffId`'si kendi `Staff` kaydıdır.
- Müşteri kartı marka geneldir: aynı markanın şubeleri aynı müşteriyi görür.

## İzin kataloğu ve matris

✔ tam · **Ş** kendi şube(ler)i · **K** yalnız kendi kayıtları · — yok

| İzin kodu | Açıklama | Owner | BranchManager | Staff |
|---|---|---|---|---|
| `calendar.view` | Takvimi görme | ✔ | Ş | K ("Takvimim") |
| `calendar.view.all-staff` | Diğer personelin kolonlarını görme | ✔ | Ş | — |
| `appointment.create` | Randevu ekleme | ✔ | Ş | K (kendine) |
| `appointment.update` | Saat, personel, hizmet değiştirme | ✔ | Ş | K |
| `appointment.cancel` | İptal | ✔ | Ş | K* |
| `appointment.cancel.checkedin` | Gelmiş randevuyu iptal | ✔ | Ş | — |
| `appointment.approve` | Onay bekleyen talebi onaylama veya reddetme | ✔ | Ş | — |
| `appointment.deposit.mark` | "Kapora geldi" işaretleme | ✔ | Ş | K* |
| `appointment.checkin` | "Geldi" ve "Geldiyse geri al" | ✔ | Ş | K |
| `appointment.complete` | "Tamamla ve ödeme al" | ✔ | Ş | — |
| `customer.view` | Müşteri listesi ve kartı | ✔ | ✔ | ✔ |
| `customer.phone.view` | Tam telefon numarası | ✔ | ✔ | —** |
| `customer.edit` | Müşteri ekleme, düzenleme, not | ✔ | ✔ | ✔ (not ekleme) |
| `customer.merge` | Kayıt birleştirme | ✔ | ✔ | — |
| `customer.import` / `customer.export` | CSV içe/dışa aktarma | ✔ | — | — |
| `customer.kvkk` | KVKK silme talebi işleme | ✔ | — | — |
| `walkin.manage` | Kapıda sıra | ✔ | Ş | ✔ |
| `waitlist.manage` | Bekleme listesi | ✔ | Ş | — |
| `payment.collect` | Ödeme alma, kasa | ✔ | Ş | — |
| `cash.close` | Günü kapatma | ✔ | Ş | — |
| `package.sell` | Müşteriye paket satma | ✔ | Ş | — |
| `report.view` | Raporlar | ✔ | Ş | — |
| `commission.view` | Prim | ✔ | Ş | K ("Primim") |
| `review.view` / `review.reply` | Değerlendirmeler, yanıt | ✔ | Ş | — |
| `service.manage` | Hizmet, kategori, fiyat düzenleme | ✔ | ✔*** | — |
| `package.manage` | Paket tanımları | ✔ | — | — |
| `staff.manage` | Personel, çalışma saati, izin | ✔ | Ş | — |
| `timeoff.request` | Kendi iznini girme | ✔ | ✔ | K |
| `settings.manage` | Ayarlar (işletme, şube, kurallar, rezervasyon sayfası, bildirim) | ✔ | — | — |
| `settings.payment` | Kapora ve IBAN | ✔ | — | — |
| `settings.roles` | Kullanıcı ve rol yönetimi | ✔ | — | — |
| `subscription.manage` | Abonelik, kart, havale bildirimi, kontör satın alma | ✔ | — | — |

\* Personelin kendi randevusunu iptal etmesi ve kapora işaretlemesi v1'de **kapalıdır**. Marka ayarıyla açılabilir hâle getirmek v2'ye bırakıldı (`docs/acik-sorular.md` S-07).
\** `MaskCustomerPhoneFromStaff = true` iken personel `0532 *** ** 21` görür, "Ara" ve "SMS" butonları numarayı göstermeden çalışır (mobilde arama, uygulama içinden `tel:` ile açılır; maskesiz numara istemciye yalnız arama anında `manageRevealCustomerPhone` ucuyla verilir ve denetime yazılır). Ayar kapalıysa personel de tam görür.
\*** Tasarımdaki "Hizmet ve fiyat düzenleme" satırında şube yöneticisi işaretli. Yönetici marka geneli hizmeti değil, **kendi şubesinin** fiyat ve uygunluğunu (`BranchServicePrice`) düzenler. Yeni hizmet ve kategori yalnız Owner'dadır.

### Menü görünürlüğü (salon paneli ve işletme uygulaması)
| Menü | İzin |
|---|---|
| Genel bakış | `report.view` yoksa personel özetine ("Merhaba Zeynep") döner |
| Takvim / Takvimim | `calendar.view` |
| Müşteriler | `customer.view` |
| Bekleme listesi | `waitlist.manage` |
| Kapıda sıra | `walkin.manage` |
| Kasa | `payment.collect` |
| Değerlendirmeler | `review.view` |
| Hizmetler | `service.manage` |
| Personel | `staff.manage` |
| Paketler | `package.manage` veya `package.sell` |
| Raporlar ve prim / Primim | `report.view` / `commission.view` |
| Ayarlar | `settings.*` |

## Platform rolleri (iç ekip)

| İzin | PlatformAdmin | PlatformSupport | PlatformFinance |
|---|---|---|---|
| Marka listesi, detay görüntüleme (her açılış denetime yazılır) | ✔ | ✔ | ✔ |
| Havale bildirimi eşleştirme (abonelik, kontör) | ✔ | ✔ | ✔ |
| SMS gönderen adı onay/ret | ✔ | ✔ | — |
| Kontör ekleme (hediye, düzeltme) | ✔ | — | — |
| Plan değiştirme, marka durumu (askıya alma, salt okunur) | ✔ | — | — |
| Plan ve fiyat tanımları | ✔ | — | ✔ |
| Fatura işlemleri, banka hareketi içe aktarma | ✔ | — | ✔ |
| Denetim kaydı görüntüleme | ✔ | ✔ | ✔ |
| Denetim kaydı dışa aktarma | ✔ | — | — |

- Platform kullanıcısı marka verisini **değiştiremez**. Randevu, müşteri ve hizmet gibi kayıtları yalnız görüntüler. Değişiklik yalnız platform kavramlarında yapılır: plan, kontör, durum, eşleştirme.
- Platform ekranlarında müşteri telefonu her zaman maskelidir.

## Müşteri (`me` audience)
- Yalnız kendi `Customer` kaydına bağlı verileri görür: randevular (tüm markalarda), paketler, ödemeler (salt okunur), değerlendirmeler, rızalar.
- Bir markadaki `BrandCustomer.Notes`, etiketler, `StaffNote` ve sayaçlar müşteriye **asla** dönmez.
- Randevu oluşturma `public` akışındadır; uygulama da aynı uçları müşteri jetonuyla çağırır (bkz. `docs/sozlesme/api-kurallari.md`).

## SMS bağlantısıyla gelen kişi (erişim kodu)
`AppointmentAccessCode` ile gelen istek yalnız o randevunun okunmasına, iptaline ve saat değişikliğine izin verir. Bekleme teklifi kodu yalnız o teklifin kabul veya reddine izin verir. Başka hiçbir kaynağa erişim vermez.

## Uygulama kuralları (backend)
1. Her uç nokta ya `.RequirePermission(...)` ya `.AllowAnonymous()` + açık gerekçe taşır. Mimari test, işaretsiz uç noktayı yakalar.
2. "Kendi kaydı" ve "kendi şubesi" kontrolü sorgu düzeyinde yapılır (filtreli sorgu), sonradan bellekte değil.
3. Kiracı izolasyonu yetkiden ayrıdır ve her zaman çalışır (EF global filtre + PostgreSQL RLS, ADR-0006). Yetki kontrolü bunun üstüne eklenir.
4. Yetkisiz istek → `403 forbidden`. Başka markanın veya kapsam dışı şubenin kaydı → `404 not_found`.
5. İzin kodları `Identity.Contracts/Permissions.cs` içinde sabit olarak tanımlanır. Bu belge ile o dosya birebir eşleşir; test eşleşmeyi doğrular.
