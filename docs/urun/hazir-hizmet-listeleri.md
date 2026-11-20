# Sektör hazır hizmet listeleri (taslak)

Kayıt sonrası "Hazır listeden başla" (HizmetlerBos) ekranının içeriği. Liste platform verisidir (`catalog.service_templates`); salon seçtiği şablonları fiyat girerek hizmete çevirir (S-24), kopyadan sonra şablonla bağı yoktur (alan-modeli.md § Industry, ServiceTemplate).

**Durum: taslak.** Ad, kategori ve süreler ürün onayı bekler (acik-sorular.md T-31). Değişiklik bu belgede ve aynı içerikli yeni bir migration ile yapılır; var olan salonların hizmetleri etkilenmez.

Süre: varsayılan dakika; salon seçerken değiştirebilir. Fiyat şablonda yoktur.

## Kuaför (`hair_salon`) · 18 hizmet

| Kategori | Hizmet | Süre (dk) |
|---|---|---|
| Kesim | Kadın saç kesimi | 45 |
| Kesim | Çocuk saç kesimi | 30 |
| Kesim | Kahkül kesimi | 15 |
| Fön ve şekillendirme | Fön | 30 |
| Fön ve şekillendirme | Maşa | 45 |
| Fön ve şekillendirme | Topuz | 60 |
| Fön ve şekillendirme | Gelin saçı | 120 |
| Renk | Dip boya | 90 |
| Renk | Tüm boya | 120 |
| Renk | Röfle | 150 |
| Renk | Balyaj | 180 |
| Renk | Ombre | 180 |
| Renk | Renk açma | 150 |
| Bakım | Keratin bakımı | 120 |
| Bakım | Saç bakım maskesi | 30 |
| Bakım | Brezilya fönü | 150 |
| Bakım | Perma | 120 |
| Bakım | Saç yıkama | 15 |

## Berber (`barber`) · 12 hizmet

| Kategori | Hizmet | Süre (dk) |
|---|---|---|
| Kesim | Saç kesimi | 30 |
| Kesim | Çocuk saç kesimi | 20 |
| Kesim | Saç ve sakal | 45 |
| Sakal | Sakal tıraşı | 20 |
| Sakal | Sakal şekillendirme | 20 |
| Sakal | Ustura tıraşı | 30 |
| Bakım | Yüz maskesi | 20 |
| Bakım | Cilt temizliği | 30 |
| Bakım | Kulak ve burun ağdası | 10 |
| Renk | Saç boyama | 45 |
| Renk | Sakal boyama | 20 |
| Kesim | Damat tıraşı | 90 |

## Güzellik salonu (`beauty`) · 14 hizmet

| Kategori | Hizmet | Süre (dk) |
|---|---|---|
| Cilt bakımı | Klasik cilt bakımı | 60 |
| Cilt bakımı | Derin cilt temizliği | 75 |
| Cilt bakımı | Hydrafacial | 60 |
| Ağda | Tüm bacak ağda | 45 |
| Ağda | Yarım bacak ağda | 30 |
| Ağda | Koltuk altı ağda | 15 |
| Ağda | Yüz ağda | 15 |
| Kaş ve kirpik | Kaş tasarımı | 30 |
| Kaş ve kirpik | Kaş laminasyonu | 45 |
| Kaş ve kirpik | Kirpik lifting | 60 |
| Kaş ve kirpik | İpek kirpik | 120 |
| Makyaj | Günlük makyaj | 45 |
| Makyaj | Gece makyajı | 60 |
| Makyaj | Gelin makyajı | 120 |

## Spa ve masaj (`spa`) · 10 hizmet

| Kategori | Hizmet | Süre (dk) |
|---|---|---|
| Masaj | Klasik masaj | 60 |
| Masaj | Aromaterapi masajı | 60 |
| Masaj | Derin doku masajı | 60 |
| Masaj | Sıcak taş masajı | 75 |
| Masaj | Refleksoloji | 45 |
| Masaj | Sırt ve boyun masajı | 30 |
| Hamam | Kese köpük | 45 |
| Hamam | Hamam ve masaj paketi | 105 |
| Bakım | Vücut peelingi | 45 |
| Bakım | Vücut maskesi | 60 |

## Tırnak (`nail`) · 10 hizmet

| Kategori | Hizmet | Süre (dk) |
|---|---|---|
| Manikür | Manikür | 45 |
| Manikür | Kalıcı oje | 60 |
| Manikür | Kalıcı oje çıkarma | 20 |
| Pedikür | Pedikür | 60 |
| Pedikür | Kalıcı oje pedikür | 75 |
| Protez tırnak | Jel tırnak | 90 |
| Protez tırnak | Akrilik tırnak | 120 |
| Protez tırnak | Protez tırnak dolgu | 75 |
| Tasarım | Nail art | 30 |
| Tasarım | Fransız manikür | 60 |

## Diyetisyen (`dietitian`) · 6 hizmet

| Kategori | Hizmet | Süre (dk) |
|---|---|---|
| Görüşme | İlk görüşme | 60 |
| Görüşme | Kontrol görüşmesi | 30 |
| Görüşme | Online görüşme | 30 |
| Ölçüm | Vücut analizi | 15 |
| Program | Sporcu beslenmesi görüşmesi | 45 |
| Program | Çocuk beslenmesi görüşmesi | 45 |

## Diğer (`other`) · 4 hizmet

| Kategori | Hizmet | Süre (dk) |
|---|---|---|
| Hizmetler | Danışmanlık | 30 |
| Hizmetler | Standart seans | 60 |
| Hizmetler | Uzun seans | 90 |
| Hizmetler | Kısa seans | 15 |
