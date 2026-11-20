# web/platform-panel/: Platform paneli ajan kuralları

OMVILA iç ekibinin paneli: markalar, onay kuyrukları (havale bildirimi eşleştirme, SMS gönderen adı), abonelikler, denetim kaydı, planlar ve fiyat. Önce kök [AGENTS.md](../../AGENTS.md)'yi okuyun.

Ekranlar: `design/ekranlar/platform-paneli/`. Yığın, yapı ve stil kuralları salon paneliyle aynıdır ([web/salon-panel/AGENTS.md](../salon-panel/AGENTS.md)). Aynı `@omvila/ui-web` bileşenleri kullanılır.

## Özel kurallar
- API yüzeyi `/api/v1/admin` (jeton `aud=admin`). Uçlar Dilim 6'da sözleşmeye eklenir ([contracts/yol-haritasi.md](../../contracts/yol-haritasi.md)).
- **Roller:** PlatformAdmin, PlatformSupport, PlatformFinance. Matris: [roller-yetkiler.md § Platform rolleri](../../docs/alan/roller-yetkiler.md#platform-rolleri-iç-ekip). Destek rolünde kontör, plan ve durum butonları pasif ve nedeni yazar ("yönetici yetkisi ister").
- **Gizlilik:** müşteri telefonları her zaman maskeli. Marka detayını açmak denetim kaydına yazılır; ekranda bunu belirten not durur ("Bu görüntüleme denetim kaydına yazıldı").
- **Değiştirme sınırı:** platform paneli marka verisini (randevu, müşteri, hizmet) değiştirmez. Yalnız plan, kontör, marka durumu, eşleştirme ve onay işlemleri yapılır.
- **Havale eşleştirme:** bildirim ↔ banka hareketi seçimi, "Seçili hareketle eşleştir" / "Bulunamadı, markaya yaz". Tutar ve açıklama kodu uyuşmazsa uyarı şeridi gösterilir.
- Yer tutucular (`[AYLIK FİYAT]`, `[FİYAT]`, `[PLAN SINIRI]`) veriden gelir; kodda yazılmaz.

## Komutlar
```bash
pnpm --filter @omvila/platform-panel dev    # http://localhost:5174
pnpm --filter @omvila/platform-panel test
pnpm --filter @omvila/platform-panel lint
```
