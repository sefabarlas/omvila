# packages/api-client-ts/: TypeScript API istemcisi

`contracts/openapi.yaml`'dan üretilen tipler (`src/generated/schema.ts`, `openapi-typescript`) ve ince bir `openapi-fetch` sarmalayıcısı.

## Kurallar
- `src/generated/` **elle düzenlenmez.** Değişiklik için sözleşmeyi değiştirip `pnpm --filter @omvila/contracts generate:ts` çalıştırın. CI yeniden üretip fark arar.
- `src/client.ts` (elle yazılan, küçük): taban URL, `Authorization` başlığı, `X-Client-Type`, otomatik `Idempotency-Key` (UUID v4, istek başına), `If-Match` yardımcısı, `401 token_expired` → tek seferlik yenileme ve tekrar, `503 try_again` → bir kez tekrar.
- Tiplerde `Idempotency-Key`, `X-Client-Type` ve `X-Client` başlıkları isteğe bağlıdır; sarmalayıcı doldurur (`ApiClientOptions.headers`, POST'ta eksik anahtar). `If-Match` zorunlu kalır: `params: { header: ifMatch(data.version) }`.
- `unwrap(client.GET(...))` veriyi döner, hatada `ApiProblem` fırlatır (TanStack Query için).
- Tipler `openapi-typescript --default-non-nullable false` ile üretilir: varsayılanı olan istek alanı isteğe bağlıdır.
- Hata tipi: `ApiProblem` (`code: ErrorCode`, `status`, `meta`). Uygulamalar `title`/`detail` göstermez; mesaj `@omvila/ui-web/errors`.
- Yüzeye göre ayrı istemci fabrikaları: `createManageClient`, `createPublicClient`, `createMeClient`, `createAdminClient`. Her biri yalnız kendi yollarının tiplerini açar.
- Bağımlılık yalnız `openapi-fetch`. Yeni bağımlılık gerekçe ister.

## Komutlar
```bash
pnpm --filter @omvila/contracts generate:ts
pnpm --filter @omvila/api-client-ts build test
```
