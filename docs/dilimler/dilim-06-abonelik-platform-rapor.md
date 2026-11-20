# Dilim 6: Abonelik + platform paneli + raporlar

## Amaç
Salonun OMVILA'ya ödemesi (kart veya havale/EFT), abonelik durumlarının otomatik ilerlemesi (deneme → aktif → gecikti → salt okunur), iç ekibin platform paneli ve salonun raporları. Tanıtım sitesi yayına hazır hâle gelir.

## Ön koşullar (karar)
- T-06 kart çekim sağlayıcısı, T-07 barındırma, S-14 fiyat ve plan değerleri. Karar gelmeden kart akışı yazılmaz; havale akışı ve durum makinesi yazılabilir.

## Kapsam
- `Subscription` durum makinesi ve `AdvanceSubscriptionStatuses` işi; şeritler (deneme, gecikti, salt okunur); salt okunurda online rezervasyon, yeni randevu ve SMS kapalı.
- Ayarlar → Abonelik ve fatura: plan, şube/personel sayısı, faturalar (PDF), kartla öde (sağlayıcı), havale/EFT bilgisi (`[OMVILA ŞİRKET UNVANI]`, `[OMVILA IBAN]`, `ABN-…` kodu, tutar) ve "Havaleyi bildir" (dekont isteğe bağlı); kontör satın alma (kart veya havale `KNT-…`).
- Platform paneli: genel bakış (aktivasyon hunisi, dikkat isteyen markalar), markalar ve marka detayı (görüntüleme denetime yazılır), onay kuyrukları (havale bildirimi ↔ banka hareketi eşleştirme, SMS gönderen adı), abonelikler, denetim kaydı, planlar ve fiyat, banka hareketi CSV içe aktarma.
- Faturalama: boşluksuz fatura numarası, ek şube ücreti.
- Raporlar ve prim: hak edilen gelir / tahsilat ayrımı, günlük tahsilat grafiği, gelmedi oranı, online payı, personel bazında prim; Genel bakış bento ve haftalık doluluk.
- Ayarlar → Roller ve yetkiler (kullanıcı listesi, rol matrisi görüntüleme), Ayarlar → KVKK ve veri, Google Takvim tek yönlü aktarım.
- Tanıtım sitesi: Ana sayfa ve Fiyatlar (yer tutucular yapılandırmadan).

## Ekranlar
`platform-paneli/*`, `salon-paneli/` (AyarlarAbonelik, GenelBakisGecikti, TakvimSaltOkunur, Raporlar(+Bos), AyarlarRoller, AyarlarKvkk, Durumlar: abonelik), `isletme-uygulamasi/MainSaltOkunur`, `bildirim-dagitim/` (EpostaDeneme, EpostaKontor, EpostaOdemeHata), `tanitim-sitesi/*`.

## Kabul kriterleri (seçme)
1. Deneme bitti, ödeme yok → `PastDue`, turuncu şerit; 7 gün sonra `ReadOnly`: rezervasyon sayfası "Online randevu kapalı", panelde yeni randevu pasif, var olan randevular ve hatırlatmalar sürer.
2. Salon "Havaleyi bildir" → platform onay kuyruğunda; Destek rolü "Kod tutuyor" hareketini seçip eşleştirir → fatura `Paid`, abonelik `Active`, markaya bildirim, denetim kaydı.
3. Tutar veya kod uyuşmayan bildirimde "Kontrol edin" uyarısı; "Bulunamadı, markaya yaz" → `NotFound`.
4. Destek rolü kontör ekleyemez ve plan değiştiremez (butonlar pasif, API 403); Yönetici ekler, denetime yazılır.
5. Marka detayını açmak `AuditLog(View)` üretir; telefonlar maskeli.
6. Raporlarda peşin satılan paket tahsilatta görünür, hak edilen gelirde seans kullanıldıkça görünür.
7. Tanıtım sitesinde uydurma sayı, yorum veya logo yok; fiyatlar yapılandırmadan; "Ücretsiz dene" panel kaydına gider.
