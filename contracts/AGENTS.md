# contracts/: ajan kuralları

Bu klasör OMVILA HTTP API'sinin **tek doğru kaynağıdır** (ADR-0009). Önce kökteki `AGENTS.md`'yi okuyun.

## Ne zaman buraya dokunulur
- Yeni uç, alan, enum değeri veya hata kodu gerekiyorsa **ilk adım** buradadır. Backend veya istemci kodu sözleşmeden önce değişmez.
- Planlanan uçların adları `yol-haritasi.md`'de sabittir. Oradan alın, yeni ad uydurmayın.

## Kurallar
1. `docs/sozlesme/api-kurallari.md` kurallarına uyun: yol adlandırma, `operationId` biçimi (`{surface}{Verb}{Resource}`), camelCase alanlar, PascalCase enum, `Money`, UTC zaman, `Problem` hataları.
2. Her işlem: tek `tags` değeri, `operationId`, `summary`, gerekiyorsa ekran adını anan `description` (ör. "Yeni randevu" ekranı), en az bir 2xx yanıt, ilgili hata yanıtları (`components/responses`).
3. Kaynak oluşturan ve para kaydeden POST'lar `IdempotencyKey` parametresi alır. Düzenleme ve durum eylemleri `IfMatch` alır.
4. Yeni hata kodu: `components/schemas/ErrorCode` enum'una **ve** `docs/sozlesme/api-kurallari.md` kataloğuna **ve** istemci mesaj dosyalarına (`packages/ui-web/src/errors/tr.ts`, `packages/omvila_ui/lib/errors/tr.dart`) eklenir.
5. Alan adları `docs/alan/sozluk.md` ve `docs/alan/alan-modeli.md` ile uyumlu olmalı. Yeni kavram önce sözlüğe girer.
6. Kırıcı değişiklik (alan silme, tip değiştirme, zorunlu yapma, yol değiştirme) yasaktır. Gerekirse kullanıcıya sorun.
7. Örnekler gerçekçi olsun: tasarım ekranlarındaki adlar ve değerler (Kuaför Elif, Kadıköy, Selin Öztürk, 450 ₺ = `45000`).
8. Telefon ve ad içeren örneklerde gerçek kişi verisi kullanmayın.

## Komutlar
```bash
pnpm --filter @omvila/contracts lint        # redocly lint + spectral
pnpm --filter @omvila/contracts breaking    # oasdiff breaking, origin/main'e karşı
pnpm --filter @omvila/contracts mock        # Prism mock: http://localhost:4010
pnpm --filter @omvila/contracts generate    # TS ve Dart istemcilerini yeniden üretir
pnpm --filter @omvila/contracts bundle      # tek dosya çıktı: dist/openapi.json
```

## Sözleşme değişikliği tamamlandı sayılmaz, eğer
- `lint` veya `breaking` hata veriyorsa,
- `generate` çalıştırılmadıysa ve `packages/api-client-*` güncel değilse,
- `yol-haritasi.md` veya `api-kurallari.md` ile çelişiyorsa.

Commit türü: `contract(<kapsam>): …` (bkz. `docs/standartlar/git-akisi.md`).
