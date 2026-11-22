# web/tanitim/: Tanıtım sitesi ajan kuralları

Ana sayfa ve Fiyatlar; masaüstü, mobil, açık ve koyu. Önce kök [AGENTS.md](../../AGENTS.md)'yi okuyun. Ekranlar: `design/ekranlar/tanitim-sitesi/`. Marka kuralları: [design/tasarim-sistemleri/marka/README.md](../../design/tasarim-sistemleri/marka/README.md).

## Yığın
Next.js statik üretim (`output: 'export'`, `trailingSlash`), CSS Modules + `@omvila/design-tokens` (`marka.css`), `@omvila/ui-web/format` (tutar, sayı). Görseller gerçek ürün ekranlarının görüntüleri; koyu temada koyu ekranlar. Kaynak: salon paneli ve rezervasyon sayfasının Playwright `ekranlar` görüntüleri (demo verisi), webp'ye çevrilmiş kopyalar `public/ekranlar/` (T-117). Test: Vitest + Testing Library, Playwright, Lighthouse CI.

## Yapı
```
content/pricing.json   # planlar, fiyatlar (kuruş), plan sınırları, kontör paketleri, deneme süresi, KDV notu: tek kaynak
content/site.json      # sitede anılan varsayılan ayarlar (hatırlatma zamanları, bekleme teklifi süresi), telif yılı
src/app/               # / (ana sayfa), /fiyatlar/, /yasal/[slug]/ (yer tutucu), robots.txt, sitemap.xml
src/components/        # üst menü (900 px altında menü butonu), alt bilgi, logo, ekran görüntüsü
src/lib/               # pricing.ts, site.ts (ortam adresleri), metadata.ts (SEO)
scripts/serve.mjs      # out/'u yerelde sunar (Lighthouse); üretimde Caddy (Dockerfile, Caddyfile)
```

## Kurallar
- **Uydurma iddia yok:** sahte sayı ("10.000 salon"), müşteri yorumu, marka logosu yok. "En çok seçilen" yerine "Önerilen".
- Fiyatlar, plan sınırları ve KDV notu `content/pricing.json`'dan okunur, metin içine gömülmez. Değer yoksa (`null`, S-14) "Fiyat yakında açıklanacak" gibi uydurmasız metin; köşeli parantezli yer tutucu (`[AYLIK FİYAT]`…) ne sitede ne kodda geçer.
- Menüde "Özellikler" ve "Nasıl çalışır" aynı sayfada bölüme kaydırır; "Sık sorulanlar" Fiyatlar'daki bölüme. 900 px altında tek sütun ve menü butonu.
- "Giriş yap" → salon paneli `/giris`, "Ücretsiz dene" → salon paneli `/kayit` (`NEXT_PUBLIC_PANEL_URL`).
- Söz verme sınırları: ödeme doğrudan işletmeye gider; WhatsApp yok; salon keşfi "yakında" bile denmez; "güzellik sektörü yazılımı" diye daraltılmaz ("randevuyla çalışan her işletme için"). Kartla abonelik ödemesi T-06'ya kadar anılmaz (`cardPaymentEnabled`).
- Yasal sayfalar (KVKK, kullanım koşulları, iletişim) henüz tasarlanmadı. Bağlantılar yer tutucu sayfaya gider; içerik yazılmaz (hukuk bekleniyor).
- SEO: her sayfada başlık, açıklama, Open Graph, canonical; `lang="tr"`. `NEXT_PUBLIC_NOINDEX=true` (test ortamı) robots.txt'yi ve meta robots'u kapatır.

## Ortam (derlemede gömülür)
| Değişken | Ne | Yerel varsayılan |
|---|---|---|
| `NEXT_PUBLIC_SITE_URL` | Sitenin kökü (canonical, sitemap) | `http://localhost:3001` |
| `NEXT_PUBLIC_PANEL_URL` | Salon panelinin kökü | `http://localhost:5173` |
| `NEXT_PUBLIC_NOINDEX` | `true`: arama motorlarına kapalı | kapalı değil |

## Komutlar
```bash
pnpm --filter @omvila/tanitim dev          # http://localhost:3001
pnpm --filter @omvila/tanitim build        # out/ statik çıktı
pnpm --filter @omvila/tanitim test
pnpm --filter @omvila/tanitim e2e          # gezinme + ekran görüntüleri (e2e-results/ekranlar)
pnpm --filter @omvila/tanitim lighthouse   # önce build
```
