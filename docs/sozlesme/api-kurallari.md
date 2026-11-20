# API kuralları

`contracts/openapi.yaml` bu kurallara uyar. Lint kuralları (`contracts/.spectral.yaml`) bu kuralların otomatik kısmını kontrol eder.

## Yollar ve adlandırma
- Önek: `/api/v1/{surface}` → `manage`, `admin`, `me`, `public`.
- Kaynak adları çoğul, kebab-case: `/appointments`, `/service-categories`, `/walk-in-tickets`.
- Kimlik yol parametresi: `/{appointmentId}` (parametre adı tekil + `Id`).
- Durum değiştiren eylemler alt kaynak fiili: `POST /appointments/{appointmentId}/approve`, `/cancel`, `/reschedule`, `/check-in`. Genel `PATCH status` yazılmaz; her geçişin kendi ucu, yetkisi ve gövdesi vardır.
- JSON alan adları camelCase. Enum değerleri PascalCase metin (`PendingApproval`).
- `operationId`: `{surface}{Verb}{Resource}` → `manageCreateAppointment`, `publicGetBranchCatalog`. İstemci fonksiyon adları buradan gelir; değiştirmek kırıcıdır.
- Her işlem tam bir `tags` değeri taşır: `manage-appointments`, `public-booking` gibi.

## Veri tipleri

| Tür | JSON | Şema |
|---|---|---|
| Kimlik | `"0193a1b2-…"` | `string, format: uuid` |
| Anlık zaman | `"2026-11-25T11:30:00Z"` | `string, format: date-time`; her zaman UTC, `Z` ile |
| Yerel tarih | `"2026-11-25"` | `string, format: date` |
| Yerel saat | `"14:30"` | `$ref: LocalTime` (`^([01]\d|2[0-3]):[0-5]\d$`) |
| Para | `{ "amountMinor": 45000, "currency": "TRY" }` | `$ref: Money` |
| Yüzde | `30.0` | `number` |
| Süre | `45` | `integer`, alan adı `...Minutes` |
| Telefon | `"+905321234567"` | `$ref: PhoneE164`. Girdi olarak serbest biçim kabul edilir (`phone` alanı), yanıtta E.164 |
| Maskeli telefon | `"0532 *** ** 21"` | `phoneMasked`; yetki yoksa `phone` alanı **hiç dönmez** |

- Yanıtta yerel saat gösterilecekse şubenin `timeZone` alanı da döner. İstemci kendi cihaz bölgesini kullanmaz.
- `null` ile alanın olmaması farklıdır: isteğe bağlı alanlar `nullable` değil, `required` listesinde değildir. Değeri temizlemek gerekiyorsa `PATCH`'te açıkça `null` gönderilir ve şemada `type: [string, "null"]` yazar.

## Liste ve sayfalama
- İmleç tabanlı: `?limit=50&cursor=…` → `{ "items": [...], "nextCursor": "…" | null }`. `limit` en fazla 100, varsayılan 50.
- Sıralama `?sort=lastVisit` / `-lastVisit` gibi izinli değerlerle; şemada enum.
- Arama `?q=`. Telefonla arama son 4 haneyle de çalışır ("Ad ya da telefonun son 4 hanesiyle arayabilirsiniz").
- Takvim ve rapor gibi aralık sorguları sayfalanmaz; aralık en fazla 31 gündür.

## Eşzamanlı düzenleme
- Düzenlenebilir kaynakların `GET` yanıtı `ETag` başlığı döner (`W/"<xmin>"`). Gövdede ayrıca `version: string`.
- `PUT`/`PATCH` ve durum eylemleri `If-Match` başlığı ister. Uyuşmazsa `412` + `code: concurrency_conflict` döner. Gövdede güncel kaynağın kim tarafından ne zaman değiştirildiği yer alır ("Mert Demir 12:04'te saati değiştirdi").
- `If-Match` gelmezse `428 precondition_required`.

## Idempotency
- Kaynak oluşturan ve para kaydeden `POST` uçları `Idempotency-Key` başlığı ister (UUID). Örnekler: randevu oluşturma, tahsilat, paket satışı, havale bildirimi.
- Aynı anahtar + aynı gövde → ilk yanıt aynen döner. Aynı anahtar + farklı gövde → `422 idempotency_key_reused`. Anahtarlar 24 saat saklanır.

## Hatalar
RFC 9457 `application/problem+json`:

```json
{
  "type": "https://docs.omvila.com/errors/slot_taken",
  "title": "Slot is no longer available",
  "status": 409,
  "code": "slot_taken",
  "detail": "Staff 0193… is busy between 14:30 and 15:15.",
  "traceId": "00-4bf9…",
  "errors": { "items[0].startUtc": ["slot_taken"] },
  "meta": { "alternatives": ["2026-11-25T11:45:00Z", "2026-11-25T12:15:00Z"] }
}
```

- `code` kararlıdır ve istemcinin tek dayanağıdır. `title` ve `detail` İngilizce, geliştirici içindir; kullanıcıya gösterilmez.
- **Kullanıcı metni istemcide** durur: `code` → Türkçe mesaj eşlemesi web'de `packages/ui-web/errors/tr.ts`, Flutter'da `omvila_ui/lib/errors/tr.dart`. Her yeni `code` bu iki dosyaya da eklenir.
- Doğrulama hatası: `400` + `code: validation_failed` + `errors` (alan yolu → kod listesi; kodlar `required`, `too_long`, `invalid_format`, `out_of_range`…).

### Hata kodu kataloğu

| HTTP | code | Ne zaman | Arayüz deseni |
|---|---|---|---|
| 400 | `validation_failed` | Biçim hatası | Alan altında mesaj |
| 401 | `unauthorized` | Jeton yok veya geçersiz | Girişe dön |
| 401 | `token_expired` | Erişim jetonu süresi doldu | Sessizce yenile |
| 403 | `forbidden` | İzin yok | "Bu sayfayı görme yetkiniz yok" |
| 403 | `booking_disabled` | Salon online randevu almıyor (salt okunur abonelik veya kapalı) | "Online randevu kapalı · Salonu arayın" |
| 403 | `subscription_read_only` | Salt okunur abonelikte yazma | Kırmızı şerit |
| 403 | `customer_blocked` | Engellenmiş müşteri online randevu almaya çalıştı | "Randevu için salonu arayın" |
| 404 | `not_found` | Yok veya kapsam dışı | "Bulunamadı" |
| 404 | `access_code_invalid` | SMS bağlantısı geçersiz veya süresi dolmuş | "Bu randevu bulunamadı" |
| 409 | `slot_taken` | Saat doldu (çakışma) | "Saat az önce doldu" + `meta.alternatives` |
| 409 | `invalid_state_transition` | Durum buna izin vermiyor | Yeniden yükle |
| 409 | `cutoff_passed` | İptal veya erteleme süresi geçti | "Salonu arayın" |
| 409 | `hold_expired` | Bekleme teklifi veya tutma süresi doldu | "Bu saatin süresi doldu" |
| 409 | `already_reviewed` | Randevu zaten değerlendirildi | |
| 409 | `phone_already_exists` | Bu numara markada kayıtlı | "Bu numara zaten kayıtlı" + `meta.brandCustomerId` |
| 409 | `deposit_unavailable_no_iban` | IBAN yokken kapora ayarı | "Kapora alınamaz" |
| 409 | `outside_working_hours` | Panelde çalışma saati dışı (onaysız) | Uyarı + "Yine de ekle" |
| 409 | `staff_on_time_off` | Personel izinli (onaysız) | Uyarı |
| 409 | `insufficient_credits` | Kontör yetersiz | "Kontör az" şeridi |
| 412 | `concurrency_conflict` | `If-Match` uyuşmadı | "Kayıt çakışması · Yenile" |
| 422 | `idempotency_key_reused` | Aynı anahtar, farklı gövde | Geliştirici hatası |
| 422 | `otp_invalid` | Kod yanlış | "Kod hatalı" + `meta.attemptsLeft` |
| 422 | `otp_expired` | Kod süresi doldu | "Yeni kod isteyin" |
| 428 | `precondition_required` | `If-Match` yok | Geliştirici hatası |
| 429 | `otp_attempts_exceeded` | Deneme sınırı | "Çok fazla deneme yaptınız" + `meta.retryAfterSeconds` |
| 429 | `otp_resend_too_soon` | Bekleme süresi dolmadı | Geri sayım + `meta.retryAfterSeconds` |
| 429 | `rate_limited` | Genel hız sınırı | `Retry-After` başlığı |
| 503 | `try_again` | Kilit zaman aşımı, geçici | Otomatik bir kez tekrar dene |

## Güvenlik başlıkları ve kimlik
- `Authorization: Bearer <jwt>`. Yüzeyin `aud` değeri tutmazsa `401`.
- Kiracı kimliği istekten alınmaz (ADR-0006). `manage` uçlarında `branchId` sorgu parametresi yalnız **filtre**dir, yetki kapsamı jetondan gelir.
- Public uçlar `X-Client: booking-page|customer-app` başlığı gönderir (analitik, hız sınırı ayrımı).

## Sürümleme
- `/api/v1` içinde yalnız geriye uyumlu değişiklik: yeni isteğe bağlı alan, yeni uç, yeni enum değeri.
- **Enum'a değer eklemek istemciler için kırıcı olabilir.** İstemciler bilinmeyen enum değerini `Unknown` olarak ele almak zorundadır. Üretilen istemcilerde bu ayar açıktır.
- Alan kaldırma, tip değiştirme, zorunlu yapma → kırıcı → `oasdiff` CI'da durdurur.
- `oasdiff` yanıttaki enum'a değer eklemeyi varsayılan olarak kırıcı sayar. Yukarıdaki kurala uymak için bu tek kural (`response-property-enum-value-added`) `contracts/oasdiff-levels.txt` ile bilgi düzeyine indirilmiştir (ürün sahibi onayı 2026-11-20). Diğer bütün kırılmalar hata olarak kalır.

## Örnek ve açıklama zorunluluğu
- Her şemanın ve her alanın `description`'ı vardır (Türkçe veya İngilizce; ekran adı varsa parantez içinde Türkçe: "Deposit transfer code (Açıklama kodu)").
- Her işlemin en az bir başarı yanıt örneği vardır. Prism mock bu örnekleri döndürür. Örnekler tasarım ekranlarındaki gerçekçi verilerle yazılır (Kuaför Elif, Selin Öztürk…).
