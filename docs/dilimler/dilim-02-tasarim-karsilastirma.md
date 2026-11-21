# Dilim 2: tasarım karşılaştırması ve Lighthouse

Kabul kriteri 20'nin kaydı. Ekran görüntüleri Playwright ile alınır ve `design/ekranlar/` altındaki görüntüyle yan yana gözle karşılaştırılır; otomatik piksel karşılaştırması yok (test verisi tasarımdaki veriye yakın ama aynı değil).

- Salon paneli: `pnpm --filter @omvila/salon-panel e2e` → `web/salon-panel/e2e-results/ekranlar/` (`e2e/ekranlar.e2e.ts`, `e2e/takvim.e2e.ts`). Masaüstü 1440×900; tablet tasarım ölçüsünde (1024×768, 768×1024) ve kırılım sınırlarında (1199×900, 899×1100).
- Rezervasyon sayfası: `pnpm --filter @omvila/rezervasyon e2e` → `web/rezervasyon/e2e-results/ekranlar/` (`e2e/ekranlar.e2e.ts`, `e2e/randevum.e2e.ts`). Telefon 390×844 (Pixel 7), masaüstü 1280. Salon kartları `e2e/api-stub.mjs`'ten (tek şube, iki şube, online randevu kapalı).
- CI: `e2e` işi iki paketi de çalıştırır, görüntüler yapıt olarak saklanır (`salon-panel-e2e`, `rezervasyon-e2e`).

Durum: ✓ tasarımla uyumlu, ≈ bilinçli/kapsam dışı farkla uyumlu, ✗ düzeltilmesi gereken fark. Bu PR'da yeni eklenen görüntüler (tablet, Sube, Durumlar, TakvimSurukle, MasaustuOnay) ayrıntılı incelendi; diğerleri ekranın kendi PR'ında (#53, #56–#61) karşılaştırılmıştı ve bu PR'da yeniden alınıp göz atıldı.

## Salon paneli

| Ekran | Açık | Koyu | Kırılım | Durum | Not |
|---|---|---|---|---|---|
| Main (takvim gün) | Main | MainKoyu | 1440 | ≈ | Üst bardaki genel arama ve bildirim zili yok (ayrı iş, T-57 (2)); "Kapıda sırada" bloğu D3'te, bento üç blok. Gezinme okları çerçevesizdi, düzeltildi. |
| HaftaTakvim | HaftaTakvim | HaftaTakvimKoyu | 1440 | ✓ | |
| TakvimKapali | TakvimKapali | TakvimKapaliKoyu | 1440 | ✓ | |
| RandevuOnayli (yan panel) | RandevuOnayli | RandevuOnayliKoyu | 1440 | ≈ | Onay butonları D3. |
| TakvimSurukle | TakvimSurukle | — | 1440×1200 | ≈ | Sürükleme sırasında alınır (yalnız açık tema). Hedef kolon gri zeminle vurgulanır; tasarımda vurgu yok. |
| YeniRandevu | YeniRandevu | YeniRandevuKoyu | 1440 | ✓ | |
| Durumlar: kayıt çakışması | DurumKayitCakismasi | — | 1440×1200 | ✓ | Yalnız açık tema (akış testinde alınır). |
| Durumlar: saat doldu | DurumSaatDolu | — | 1440×1200 | ✓ | Yalnız açık tema. |
| Musteriler | Musteriler | MusterilerKoyu | 1440 | ≈ | Özet metrikleri, içe/dışa aktar ve sıralama seçimi yok (T-57). |
| MusterilerBos | MusterilerBos | MusterilerBosKoyu | 1440 | ✓ | |
| MusterilerAramaYok | MusterilerAramaYok | MusterilerAramaYokKoyu | 1440 | ✓ | |
| MusteriEkle | MusteriEkle | MusteriEkleKoyu | 1440 | ✓ | |
| MusteriKarti | MusteriKarti | MusteriKartiKoyu | 1440 | ✓ | |
| TabletTakvim | TabletTakvim | TabletTakvimKoyu | 1024×768 | ≈ | 72 px ikon şeridi tasarımdaki gibi. Şerit üstündeki şube seçici ve D3+ menü öğeleri yok; sayfa kenar boşluğu 24 px (tasarım 28 px). |
| TabletTakvim (sınır) | TabletTakvim1199 | TabletTakvim1199Koyu | 1199×900 | ✓ | İkon şeridine geçiş doğru. |
| TabletDikeyTakvim | TabletDikeyTakvim | TabletDikeyTakvimKoyu | 768×1024 | ≈ | Kolon başlığındaki "4 randevu · bekleyen var" kesiliyordu, tasarımdaki gibi alt satıra kayacak şekilde düzeltildi. Personel adı dar kolonda hâlâ kesilir ("Zeynep Aks…"; tasarımda tam). Bento ikinci satırda tek blok (Kapıda sırada D3). |
| TabletDikeyTakvim (sınır) | TabletDikeyTakvim899 | TabletDikeyTakvim899Koyu | 899×1100 | ✓ | |
| TabletDikeyMusteriler | TabletDikeyMusteriler | TabletDikeyMusterilerKoyu | 768×1024 | ≈ | Tablo yatay kayar (Etiket kolonu kısmen görünür, tasarımda da öyle). Özet metrikleri ve aktarım butonları yok (T-57). |
| TabletDikeyMusteriler (sınır) | TabletDikeyMusteriler899 | TabletDikeyMusteriler899Koyu | 899×1100 | ✓ | |
| TakvimBos, TekrarIptal | — | — | — | — | Bu PR'da görüntü yok; birim testleriyle doğrulanıyor (calendar.test.tsx). |

Tam sayfa görüntülerde kenar menü pencere yüksekliğinde (900 px) biter; bu görüntü alma yönteminden kaynaklanır (menü `100vh` yapışkan), tarayıcıda fark yok.

## Rezervasyon sayfası

| Ekran | Açık | Koyu | Kırılım | Durum | Not |
|---|---|---|---|---|---|
| Sube | Sube | SubeKoyu | 390 | ≈ | Adres satırı ad ile hizasızdı (4 px içerde), düzeltildi. Tasarımda başlık bölümü kısa koyu kart (logo, ad, açıklama), uygulamada Main'deki kapaklı salon kartı. Şube görsel kutuları tasarımda koyu temada da açık renk; uygulamada koyu token. Kapalı şube etiketi testte "Şu an kapalı" (aşağıdaki not). |
| Main (hizmet) | Main | MainKoyu | 390 | ✓ | |
| Personel | Personel | PersonelKoyu | 390 | ✓ | Tasarım görüntüsünde personel listesi boş çıkmış (dışa aktarım hatası); Main ve Masaustu'daki kart deseni esas alındı. |
| Saat | Saat | SaatKoyu | 390 | ✓ | |
| DoluGun | DoluGun | DoluGunKoyu | 390 | ≈ | Bekleme listesi formu D5. Tasarımda ilerleme çubuğu ve "Kasım 2026" satırı yok, metin personel adıyla ("Elif Kaya'nın 27 Kasım'da boş saati yok"); uygulamada akışın diğer adımlarıyla aynı üst bölüm ve "26 Kasım için boş saat yok". |
| Telefon | Telefon | TelefonKoyu | 390 | ✓ | |
| Kod | Kod | KodKoyu | 390 | ✓ | |
| Onay | Onay | OnayKoyu | 390 | ✓ | "Fark etmez"de atanan ad kritik akış 2 testinde doğrulanır. |
| Basari | Basari | BasariKoyu | 390 | ✓ | |
| Masaustu | Masaustu, MasaustuSaat, MasaustuOnay | …Koyu | 1280 | ≈ | İki kolon düzeni tasarımdaki gibi. Üst şeritte salon logosu ve OMVILA işareti yok; hizmetler kategori başlıklarıyla gruplu (tasarımda başlıksız ızgara); alt çubukta toplam da yazar (tasarımda yalnız "Devam et"). Puan satırı test verisinde yok (rating null). |
| Randevum | Randevum | RandevumKoyu | 390 | ✓ | Süresi geçmiş hâl: RandevumSureGecti ("Salonu ara"). |
| Ertele | Ertele | ErteleKoyu | 390 | ✓ | |
| Iptal | Iptal | IptalKoyu | 390 | ✓ | |
| Durumlar: saat az önce doldu | DurumSaatDoldu | DurumSaatDolduKoyu | 390 | ✓ | |
| Durumlar: SMS kodu deneme sınırı | DurumKodSiniri | DurumKodSiniriKoyu | 390 | ✓ | |
| Durumlar: online randevu kapalı | DurumOnlineKapali | DurumOnlineKapaliKoyu | 390 | ✓ | |
| Durumlar: bağlantı geçersiz | DurumBulunamadi | — | 390 | ✓ | |
| Durumlar: kapora gelmedi; Talep, Havale | — | — | — | — | D3/D4. |

Şube adımı sunucuda çizilir ve `branchOpenStatus` "şimdi"ye bakar ("Yarın 10:00'da açılır"). Testte tarayıcı saati sabit, sunucu saati gerçek olduğundan bu etiket hidrasyon uyuşmazlığı verir; test verisinde açılış saati boş bırakıldı. Üretimde sunucu ile tarayıcı gün dönümünde ayrışabilir (acik-sorular.md T-60 (3)).

## Lighthouse (rezervasyon sayfası)

`pnpm --filter @omvila/rezervasyon lighthouse` (lhci, mobil + yavaş 4G benzetimi, 3 koşu, Prism mock, `next start`). 21 Kasım 2026, yerel makine. `lighthouserc.cjs`'e `/r/{kod}` eklendi.

| Adres | Performans | Erişilebilirlik | En iyi uygulamalar | SEO | FCP | LCP | TBT | CLS | İlk yükleme JS (gzip) |
|---|---|---|---|---|---|---|---|---|---|
| `/kuafor-elif` | 100 | 100 | 96 | 100 | 0,6 sn | 0,6 sn | 2–4 ms | 0 | 158 KB |
| `/r/7Kx2Pq9a` | 100 | 100 | 96 | 50 | 0,9 sn | 0,9 sn | 2–11 ms | 0 | 149 KB |

Bütçe: performans ≥ 0,9, erişilebilirlik ≥ 0,95, LCP ≤ 2,5 sn, CLS ≤ 0,1 geçer. JS ölçütü (< 120 KB gzip) yalnız uyarı ve aşılıyor: iskelette ~133 KB olan aktarım akış ve SMS sayfasıyla 158 KB'a çıktı (T-55 (1), karar bekliyor). `/r` SEO puanı sayfa arama motoruna bilerek kapalı olduğu için düşük (`noindex`).
