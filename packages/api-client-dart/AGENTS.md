# packages/api-client-dart/: Dart API istemcisi (`omvila_api`)

`contracts/openapi.yaml`'dan `openapi-generator` (`dart-dio`, `json_serializable`, `enumUnknownDefaultCase=true`) ile üretilir.

## Kurallar
- Bu paketin içeriği üretilmiştir, **elle düzenlenmez.** Sözleşmeyi değiştirip `pnpm --filter @omvila/contracts generate:dart` çalıştırın; komut `dart pub get` ve `build_runner`'ı da çalıştırır (`.g.dart` dosyaları commit edilir). CI yeniden üretip fark arar.
- Elle tutulan yalnız şu dosyalar (`.openapi-generator-ignore`): `pubspec.yaml` (SDK ^3.8: üretilen JSON kodu null-aware-elements kullanır), `build.yaml` (`disallow_unrecognized_keys: false`: sunucuya eklenen yeni alan eski istemciyi bozmasın), `.gitignore` (`pubspec.lock` commit edilir ki üretim CI'da birebir aynı olsun), `AGENTS.md`.
- Üretici ayarı `contracts/openapitools.json`'da. Üretici, sözleşmenin `contracts/scripts/dart-spec.mjs` ile hazırlanan geçici kopyasını okur: üreticinin iki hatası nedeniyle boolean `const` ve satır içi enum `default` değerleri yalnız bu kopyadan kaldırılır (kuralı sunucu uygular).
- Paket şimdilik Dart workspace üyesi değildir; ilk tüketici (`packages/omvila_ui`) eklenince workspace'e alınır.
- Bilinmeyen enum değerleri `unknownDefaultOpenApi` olarak çözülür; uygulamalar bunu genel durum olarak gösterir.
- Kimlik doğrulama, yenileme, `Idempotency-Key`, `If-Match` ve hata eşleme bu pakette değil, `packages/omvila_ui` içindeki `api/` katmanında (Dio interceptor'ları) yazılır.
