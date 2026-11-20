# ADR-0009: Sözleşme önce (contract-first)

- Durum: Kabul edildi
- Tarih: 2026-11-20

## Bağlam
Altı istemci tek API'yi kullanacak ve kodun tamamı ajanlarla, paralel yazılacak. Backend bitmeden istemcilerin çalışabilmesi ve iki tarafın aynı şemayı kullandığından emin olunması gerekiyor.

## Karar
- Tek doğru kaynak `contracts/openapi.yaml` (OpenAPI 3.1).
- **Akış:** sözleşme değişikliği → lint + kırıcı değişiklik kontrolü → istemci üretimi → backend uygulaması → sözleşme testi. Sözleşme değişikliği ayrı commit'tir (`contract:` türü, bkz. `docs/standartlar/git-akisi.md`).
- **Araçlar** (hepsi `contracts/package.json` betiklerinde):
  - Lint: Redocly CLI (`contracts/redocly.yaml` kuralları) + Spectral kural seti (`contracts/.spectral.yaml`).
  - Kırıcı değişiklik: `oasdiff breaking` (ana dala karşı). Kırıcı değişiklik `/api/v2` veya açık onay ister.
  - Mock: Prism (`pnpm mock` → `http://localhost:4010`). İstemciler backend olmadan bununla geliştirilir.
  - TypeScript istemci: `openapi-typescript` (tipler) + `openapi-fetch` (çağrılar) → `packages/api-client-ts`.
  - Dart istemci: `openapi-generator` (`dart-dio` jeneratörü, `serializationLibrary=json_serializable`) → `packages/api-client-dart`.
- **Backend uyumu:** sunucu DTO'ları elle yazılır, şemayla birebir aynı ad ve tiplerle. CI iki kontrol yapar:
  1. Çalışan uygulamanın ürettiği OpenAPI belgesi (`Microsoft.AspNetCore.OpenApi`) ile `contracts/openapi.yaml` arasında `oasdiff diff` farkı olmamalı (yol, metot, şema).
  2. `Omvila.Api.ContractTests`: entegrasyon testlerinde dönen her yanıt, sözleşmedeki şemaya karşı doğrulanır (JSON Schema).
- Üretilen istemci kodu elle düzenlenmez ve depoya commit edilir. CI yeniden üretip fark olmadığını kontrol eder.

## Sonuçlar
- İstemci ajanları sözleşme ve mock ile backend'i beklemeden çalışır.
- Şema değişikliği tek PR'da sözleşme + istemciler + backend olarak görünür.

## Değerlendirilen alternatifler
- Kod önce (backend'den OpenAPI üretmek): istemciler backend'i bekler; şema ajan kodunun yan ürünü olur.
- Sunucu stub üretimi (NSwag/openapi-generator aspnetcore): Minimal API ve dikey dilim yapısına uymuyor, üretilen kodu düzenleme baskısı yaratıyor.
