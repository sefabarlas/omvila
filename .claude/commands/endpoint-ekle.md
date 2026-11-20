---
description: Sözleşme önce yaklaşımıyla yeni bir API ucu ekle (sözleşme → test → backend)
argument-hint: <operationId, ör. manageApproveAppointment>
---
`$ARGUMENTS` işlemini ekle. Sırayı bozma:

1. `contracts/yol-haritasi.md` ve `docs/sozlesme/api-kurallari.md`'yi oku. İşlem `contracts/openapi.yaml`'da yoksa ekle (yol, `operationId`, tek etiket, şemalar, hata yanıtları, `IdempotencyKey`/`IfMatch` gerekiyorsa, gerçekçi örnek).
2. `python3 contracts/scripts/check_conventions.py contracts/openapi.yaml` ve `pnpm --filter @omvila/contracts lint` çalıştır, hataları düzelt. Sonra `pnpm --filter @omvila/contracts generate`.
3. İlgili alan kuralını `docs/alan/` içinde bul (durum geçişi, süre, yetki). Önce alan testini yaz, sonra aggregate metodunu.
4. `api/AGENTS.md` § "Yeni bir uç nokta eklerken" adımlarını uygula: Request/Response, Handler, Validator, Endpoint (`.WithName("$ARGUMENTS")`, izin).
5. Entegrasyon testleri: mutlu yol, 403, 404 (başka marka), iş hataları.
6. `dotnet build -warnaserror`, `dotnet test` çalıştır.
7. Commit'leri ayır: `contract(...)` ve `feat(api/<modül>): ...`.
8. Sonunda `docs/standartlar/tanim-bitti.md` § Backend maddelerini işaretleyerek özet ver.
