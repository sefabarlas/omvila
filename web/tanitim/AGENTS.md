# web/tanitim/: Tanıtım sitesi ajan kuralları

Ana sayfa ve Fiyatlar; masaüstü, mobil, açık ve koyu. Önce kök [AGENTS.md](../../AGENTS.md)'yi okuyun. Ekranlar: `design/ekranlar/tanitim-sitesi/`. Marka kuralları: [design/tasarim-sistemleri/marka/README.md](../../design/tasarim-sistemleri/marka/README.md).

## Yığın
Next.js statik üretim (`output: 'export'`), CSS Modules + `@omvila/design-tokens`. Görseller gerçek ürün ekranlarının görüntüleri; koyu temada koyu ekranlar (`design/ekranlar/` kaynaklı, optimize edilmiş kopyalar `public/`).

## Kurallar
- **Uydurma iddia yok:** sahte sayı ("10.000 salon"), müşteri yorumu, marka logosu yok. "En çok seçilen" yerine "Önerilen".
- Fiyatlar ve KDV notu yer tutucudur (`[AYLIK FİYAT]`, `[KDV NOTU]`). Değerler yapılandırma dosyasından (`content/pricing.json`) okunur, metin içine gömülmez.
- Menüde "Özellikler" ve "Nasıl çalışır" aynı sayfada bölüme kaydırır. 900 px altında tek sütun ve menü butonu.
- "Giriş yap" → salon paneli `/giris`, "Ücretsiz dene" → salon paneli `/kayit` (adresler yapılandırmadan).
- Söz verme sınırları: ödeme doğrudan işletmeye gider; WhatsApp yok; salon keşfi "yakında" bile denmez; "güzellik sektörü yazılımı" diye daraltılmaz ("randevuyla çalışan her işletme için").
- Yasal sayfalar (KVKK, kullanım koşulları, iletişim) henüz tasarlanmadı. Bağlantılar yer tutucu sayfaya gider; içerik yazılmaz (hukuk bekleniyor).
- SEO: her sayfada başlık, açıklama, Open Graph; `lang="tr"`.

## Komutlar
```bash
pnpm --filter @omvila/tanitim dev      # http://localhost:3001
pnpm --filter @omvila/tanitim build    # out/ statik çıktı
```
