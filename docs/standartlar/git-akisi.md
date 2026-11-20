# Git akışı, commit ve PR kuralları

## Dallar
- `main` her zaman yayınlanabilir durumdadır. Doğrudan push yoktur; yalnız PR ile ve CI yeşilken birleşir.
- Dal adı: `<tür>/<dilim>-<kısa-açıklama>`, küçük harf, kebab-case, ASCII.
  - `feat/d2-public-booking-flow`
  - `fix/d2-slot-grid-timezone`
  - `contract/d3-approve-decline`
  - `docs/alan-modeli-kapora`
  - `chore/ci-oasdiff`
- Ajan çalışmaları ayrıca `agent/` öneki alabilir: `agent/feat/d2-calendar-day-view`.
- Dal ömrü kısa tutulur (hedef < 3 gün). Büyük özellik, arkasında bayrak olan küçük PR'lara bölünür.
- Birleştirme yöntemi: **squash merge**. PR başlığı tek commit mesajı olur, bu yüzden PR başlığı da aşağıdaki biçime uyar.

## Commit mesajı (Conventional Commits)
```
<tür>(<kapsam>): <özet, Türkçe, küçük harfle, noktasız, ≤ 72 karakter>

<gövde: neden ve ne; isteğe bağlı>

Refs: D2, S-04
```

**Türler**

| Tür | Ne zaman |
|---|---|
| `feat` | Kullanıcıya görünen yeni davranış |
| `fix` | Hata düzeltmesi |
| `contract` | `contracts/openapi.yaml` değişikliği (+ üretilen istemciler) |
| `refactor` | Davranış değiştirmeyen yeniden düzenleme |
| `test` | Yalnız test |
| `docs` | Yalnız belge (`docs/`, `AGENTS.md`) |
| `chore` | Araç, bağımlılık, yapılandırma |
| `ci` | CI iş akışları |
| `perf` | Performans |
| `db` | Yalnız migration (nadiren; genelde `feat` içinde) |

**Kapsamlar:** `api`, `api/scheduling` (modül), `salon-panel`, `platform-panel`, `rezervasyon`, `tanitim`, `isletme`, `musteri`, `ui-web`, `omvila-ui`, `tokens`, `contracts`, `docs`, `repo`.

Örnekler:
```
contract(contracts): randevu onay ve ret uçlarını ekle
feat(api/scheduling): onay süresi dolan talepleri otomatik iptal et
feat(salon-panel): takvim gün görünümünde bekleme dilimini göster
fix(rezervasyon): saat ızgarasında şube saat dilimini kullan
docs(docs): kapora çözüm kuralını netleştir
```

- Kırıcı değişiklik: türden sonra `!` ve gövdede `BREAKING CHANGE:`. API'de kırıcı değişiklik yalnız açık onayla.
- Ajan commit'leri ayrıca şu satırları taşır (otomatik eklenir):
  ```
  Co-Authored-By: <ajan adı> <...>
  ```
- Bir commit tek mantıksal değişiklik içerir. Sözleşme değişikliği backend ve istemci uygulamasından ayrı commit'tir (aynı PR'da olabilir).
- Commitlint CI'da mesajları denetler (`commitlint.config.js`).

## Pull request
**Başlık:** commit biçiminde (squash sonrası mesaj olur).

**Şablon** (`.github/pull_request_template.md`):
```
## Ne
<kısa açıklama>

## Neden / Bağlam
Dilim: D2 · İlgili belgeler: docs/alan/musaitlik.md · Açık soru: S-04

## Nasıl doğrulandı
- [ ] Testler (hangi kurallar kapsandı)
- [ ] Ekran görüntüsü (açık + koyu), tasarım ekranıyla yan yana: design/ekranlar/…
- [ ] Sözleşme lint + breaking + generate

## Tanım-bitti
- [ ] docs/standartlar/tanim-bitti.md maddeleri sağlandı

## Varsayımlar
<ajanın verdiği kararlar; docs/acik-sorular.md'ye işlendiyse belirtin>
```

**Boyut:** ideal < 400 satır üretim kodu (test, üretilen kod ve migration hariç). Daha büyükse bölünür veya gerekçe yazılır.

**İnceleme:**
- Her PR'ı en az bir insan veya bağımsız bir inceleme ajanı onaylar. İnceleme ajanı yazan ajan değildir ve PR'ı ilk kez görür.
- Ürün kuralına (`docs/urun/urun-kurallari.md`) dokunan PR'ı yalnız ürün sahibi onaylar.
- `contracts/`, `docs/alan/`, `docs/mimari/` ve migration içeren PR'lar CODEOWNERS ile ürün sahibine düşer.

## CI kapıları (birleştirme için hepsi zorunlu)
1. Commit/PR başlığı biçimi
2. Değişen projelerde derleme + lint + biçim
3. Testler (değişen projeler; `contracts/` değişirse tümü)
4. Sözleşme: lint, `oasdiff breaking`, üretilen istemcilerde fark yok, backend OpenAPI = sözleşme
5. Mimari testler
6. Gizli değer taraması (gitleaks)
7. Bağımlılık güvenlik taraması (yüksek ve kritik bulgu birleştirmeyi durdurur)

## Etiket ve sürüm
- Proje bazlı etiket: `api-v0.2.0`, `salon-panel-v0.2.0`, `musteri-v0.1.0+12` (mobil build numarası).
- Değişiklik günlüğü commit türlerinden üretilir (release-please, proje bazlı).
