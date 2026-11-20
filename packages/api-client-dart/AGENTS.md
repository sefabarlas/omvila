# packages/api-client-dart/: Dart API istemcisi (`omvila_api`)

`contracts/openapi.yaml`'dan `openapi-generator` (`dart-dio`, `json_serializable`, `enumUnknownDefaultCase=true`) ile üretilir.

## Kurallar
- Bu paketin içeriği **tamamen üretilmiştir, elle düzenlenmez.** Sözleşmeyi değiştirip `pnpm --filter @omvila/contracts generate:dart` çalıştırın, ardından `dart run build_runner build -d`.
- Bilinmeyen enum değerleri `unknownDefaultOpenApi` olarak çözülür; uygulamalar bunu genel durum olarak gösterir.
- Kimlik doğrulama, yenileme, `Idempotency-Key`, `If-Match` ve hata eşleme bu pakette değil, `packages/omvila_ui` içindeki `api/` katmanında (Dio interceptor'ları) yazılır.
