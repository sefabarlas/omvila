// Üretildi: pnpm --filter @omvila/design-tokens build. Elle düzenlemeyin; kaynak
// packages/design-tokens/src/tokens.mobil.json. Renkler yalnız semantik adlarla açılır.
// ignore_for_file: lines_longer_than_80_chars, sort_constructors_first
// dart format off
import 'package:flutter/material.dart';

/// Semantik renkler. Kullanım: `context.omvilaRenkler.marka`.
@immutable
class OmvilaRenkler extends ThemeExtension<OmvilaRenkler> {
  /// Temalar için `acik` ve `koyu` sabitlerini kullanın.
  const OmvilaRenkler({
    required this.zemin,
    required this.zeminYukseltilmis,
    required this.zeminIkincil,
    required this.zeminMarka,
    required this.zeminMarkaYumusak,
    required this.zeminVurguYumusak,
    required this.metinVurgu,
    required this.metin,
    required this.metinIkincil,
    required this.metinMarka,
    required this.metinTers,
    required this.marka,
    required this.markaHover,
    required this.markaUstu,
    required this.vurgu,
    required this.vurguUstu,
    required this.cizgi,
    required this.cizgiGuclu,
    required this.odak,
    required this.basari,
    required this.uyari,
    required this.hata,
    required this.bilgi,
    required this.zeminHover,
    required this.zeminSecili,
    required this.zeminDevreDisi,
    required this.metinDevreDisi,
    required this.basariZemin,
    required this.uyariZemin,
    required this.hataZemin,
    required this.bilgiZemin,
    required this.ustKatman,
    required this.durumOnayZemin,
    required this.durumOnayMetin,
    required this.durumOnayNokta,
    required this.durumKaporaZemin,
    required this.durumKaporaMetin,
    required this.durumKaporaNokta,
    required this.durumOnaylandiZemin,
    required this.durumOnaylandiMetin,
    required this.durumOnaylandiNokta,
    required this.durumGeldiZemin,
    required this.durumGeldiMetin,
    required this.durumGeldiNokta,
    required this.durumTamamlandiZemin,
    required this.durumTamamlandiMetin,
    required this.durumTamamlandiNokta,
    required this.durumGelmediZemin,
    required this.durumGelmediMetin,
    required this.durumGelmediNokta,
    required this.durumIptalZemin,
    required this.durumIptalMetin,
    required this.durumIptalNokta,
    required this.tabbarZemin,
    required this.tabbarPasif,
    required this.tabbarAktif,
    required this.tabbarAktifZemin,
    required this.blokMarka,
    required this.blokVurgu,
    required this.blokKum,
    required this.blokKumMetin,
    required this.blokMarkaHap,
    required this.blokMarkaHapMetin,
    required this.iskelet,
    required this.tutamak,
    required this.yildiz,
    required this.avatar1Zemin,
    required this.avatar1Metin,
    required this.avatar2Zemin,
    required this.avatar2Metin,
    required this.avatar3Zemin,
    required this.avatar3Metin,
    required this.sidebarIkonAktif,
    required this.blokVurguMetin,
    required this.kapakPetrol,
    required this.kapakMercan,
    required this.kapakKum,
  });

  /// Açık tema değerleri.
  static const acik = OmvilaRenkler(
    zemin: Color(0xFFFAFAF8),
    zeminYukseltilmis: Color(0xFFFFFFFF),
    zeminIkincil: Color(0xFFF1F1EE),
    zeminMarka: Color(0xFF0B3B37),
    zeminMarkaYumusak: Color(0xFFEFF8F6),
    zeminVurguYumusak: Color(0xFFFDF1EC),
    metinVurgu: Color(0xFF66291B),
    metin: Color(0xFF151816),
    metinIkincil: Color(0xFF52554F),
    metinMarka: Color(0xFF0F6D64),
    metinTers: Color(0xFFFBF8F3),
    marka: Color(0xFF0F6D64),
    markaHover: Color(0xFF0D564F),
    markaUstu: Color(0xFFFFFFFF),
    vurgu: Color(0xFFE9785B),
    vurguUstu: Color(0xFF401A11),
    cizgi: Color(0xFFE9E9E4),
    cizgiGuclu: Color(0xFF6E706A),
    odak: Color(0xFF14857A),
    basari: Color(0xFF2E7D32),
    uyari: Color(0xFF8A5A00),
    hata: Color(0xFFC0362C),
    bilgi: Color(0xFF2F6BB8),
    zeminHover: Color(0xFFF2F2EF),
    zeminSecili: Color(0xFFEFF8F6),
    zeminDevreDisi: Color(0xFFEDEDEA),
    metinDevreDisi: Color(0xFF94958E),
    basariZemin: Color(0xFFEEF7ED),
    uyariZemin: Color(0xFFFCF0D6),
    hataZemin: Color(0xFFFBE4E1),
    bilgiZemin: Color(0xFFEDF3FB),
    ustKatman: Color.fromRGBO(11, 35, 32, 0.48),
    durumOnayZemin: Color(0xFFE2ECF8),
    durumOnayMetin: Color(0xFF1C4275),
    durumOnayNokta: Color(0xFF2F6BB8),
    durumKaporaZemin: Color(0xFFFCF0D6),
    durumKaporaMetin: Color(0xFF8A5A00),
    durumKaporaNokta: Color(0xFFB07A12),
    durumOnaylandiZemin: Color(0xFFEFF8F6),
    durumOnaylandiMetin: Color(0xFF0D564F),
    durumOnaylandiNokta: Color(0xFF14857A),
    durumGeldiZemin: Color(0xFFE3F2E1),
    durumGeldiMetin: Color(0xFF1B4D1F),
    durumGeldiNokta: Color(0xFF2E7D32),
    durumTamamlandiZemin: Color(0xFFF4EFE6),
    durumTamamlandiMetin: Color(0xFF3D3328),
    durumTamamlandiNokta: Color(0xFF7A654B),
    durumGelmediZemin: Color(0xFFFBE4E1),
    durumGelmediMetin: Color(0xFF7A1F18),
    durumGelmediNokta: Color(0xFFC0362C),
    durumIptalZemin: Color(0xFFEDEDEA),
    durumIptalMetin: Color(0xFF52554F),
    durumIptalNokta: Color(0xFF6E706A),
    tabbarZemin: Color(0xFF062320),
    tabbarPasif: Color(0xFF9DB3AE),
    tabbarAktif: Color(0xFF0B3B37),
    tabbarAktifZemin: Color(0xFFFFFFFF),
    blokMarka: Color(0xFF0B3B37),
    blokVurgu: Color(0xFFB54A30),
    blokKum: Color(0xFFE9E0D0),
    blokKumMetin: Color(0xFF241E18),
    blokMarkaHap: Color(0xFF74C6B8),
    blokMarkaHapMetin: Color(0xFF062320),
    iskelet: Color(0xFFF4EFE6),
    tutamak: Color(0xFFBDBDB6),
    yildiz: Color(0xFFB57A0A),
    avatar1Zemin: Color(0xFFD3EEE8),
    avatar1Metin: Color(0xFF0B4540),
    avatar2Zemin: Color(0xFFE9E0D0),
    avatar2Metin: Color(0xFF3D3328),
    avatar3Zemin: Color(0xFFFADCD1),
    avatar3Metin: Color(0xFF66291B),
    sidebarIkonAktif: Color(0xFFEF9A7E),
    blokVurguMetin: Color(0xFFFFFFFF),
    kapakPetrol: Color(0xFF0B3B37),
    kapakMercan: Color(0xFFB54A30),
    kapakKum: Color(0xFF7A654B),
  );

  /// Koyu tema değerleri.
  static const koyu = OmvilaRenkler(
    zemin: Color(0xFF0E1513),
    zeminYukseltilmis: Color(0xFF16201E),
    zeminIkincil: Color(0xFF121B19),
    zeminMarka: Color(0xFF11403B),
    zeminMarkaYumusak: Color(0xFF122E2A),
    zeminVurguYumusak: Color(0xFF33201A),
    metinVurgu: Color(0xFFEF9A7E),
    metin: Color(0xFFEEF2F0),
    metinIkincil: Color(0xFFA7B0AC),
    metinMarka: Color(0xFF74C6B8),
    metinTers: Color(0xFFEEF2F0),
    marka: Color(0xFF74C6B8),
    markaHover: Color(0xFFA8DDD3),
    markaUstu: Color(0xFF062320),
    vurgu: Color(0xFFEF9A7E),
    vurguUstu: Color(0xFF401A11),
    cizgi: Color(0xFF26322F),
    cizgiGuclu: Color(0xFF7C8884),
    odak: Color(0xFF74C6B8),
    basari: Color(0xFF7BC47F),
    uyari: Color(0xFFF0C35A),
    hata: Color(0xFFF28B82),
    bilgi: Color(0xFF8AB4EA),
    zeminHover: Color(0xFF1B2624),
    zeminSecili: Color(0xFF13302C),
    zeminDevreDisi: Color(0xFF1E2826),
    metinDevreDisi: Color(0xFF5E6966),
    basariZemin: Color(0xFF15301A),
    uyariZemin: Color(0xFF33280F),
    hataZemin: Color(0xFF3A1A17),
    bilgiZemin: Color(0xFF14243A),
    ustKatman: Color.fromRGBO(0, 0, 0, 0.6),
    durumOnayZemin: Color(0xFF14243A),
    durumOnayMetin: Color(0xFF9DC0EE),
    durumOnayNokta: Color(0xFF5E93D6),
    durumKaporaZemin: Color(0xFF33280F),
    durumKaporaMetin: Color(0xFFF0C35A),
    durumKaporaNokta: Color(0xFFE5A823),
    durumOnaylandiZemin: Color(0xFF12302C),
    durumOnaylandiMetin: Color(0xFF8FD6C8),
    durumOnaylandiNokta: Color(0xFF3FA899),
    durumGeldiZemin: Color(0xFF15301A),
    durumGeldiMetin: Color(0xFF9AD39D),
    durumGeldiNokta: Color(0xFF5BB060),
    durumTamamlandiZemin: Color(0xFF2A241D),
    durumTamamlandiMetin: Color(0xFFD9CBB3),
    durumTamamlandiNokta: Color(0xFFBFAA8A),
    durumGelmediZemin: Color(0xFF3A1A17),
    durumGelmediMetin: Color(0xFFF4A39B),
    durumGelmediNokta: Color(0xFFE0665C),
    durumIptalZemin: Color(0xFF1E2826),
    durumIptalMetin: Color(0xFFA7B0AC),
    durumIptalNokta: Color(0xFF6E7A76),
    tabbarZemin: Color(0xFF1E2B28),
    tabbarPasif: Color(0xFF8A9692),
    tabbarAktif: Color(0xFF062320),
    tabbarAktifZemin: Color(0xFFA8DDD3),
    blokMarka: Color(0xFF11403B),
    blokVurgu: Color(0xFFB54A30),
    blokKum: Color(0xFF2A2520),
    blokKumMetin: Color(0xFFF4EFE6),
    blokMarkaHap: Color(0xFF74C6B8),
    blokMarkaHapMetin: Color(0xFF062320),
    iskelet: Color(0xFF1C2624),
    tutamak: Color(0xFF3A4643),
    yildiz: Color(0xFFF0C35A),
    avatar1Zemin: Color(0xFF0B4540),
    avatar1Metin: Color(0xFFD3EEE8),
    avatar2Zemin: Color(0xFF3D3328),
    avatar2Metin: Color(0xFFE9E0D0),
    avatar3Zemin: Color(0xFF66291B),
    avatar3Metin: Color(0xFFFADCD1),
    sidebarIkonAktif: Color(0xFFEF9A7E),
    blokVurguMetin: Color(0xFFFFFFFF),
    kapakPetrol: Color(0xFF0B3B37),
    kapakMercan: Color(0xFFB54A30),
    kapakKum: Color(0xFF7A654B),
  );

  /// Sayfa zemini. Marka materyallerinin ve rezervasyon sayfasının ana zemini.
  final Color zemin;
  /// Kart, panel ve modal yüzeyi; `zemin` üzerinde bir kat yukarıda.
  final Color zeminYukseltilmis;
  /// Bölüm ayırıcı bantlar, gruplanmış alanlar, giriş alanı zemini.
  final Color zeminIkincil;
  /// Koyu marka bloğu: kahraman alanları, işletme uygulaması ikonu, vurgu bantları.
  final Color zeminMarka;
  /// Seçili satır, bilgi kutusu, marka tonunda sakin vurgu.
  final Color zeminMarkaYumusak;
  /// Mercan rozet ve 'yeni' etiketi zemini.
  final Color zeminVurguYumusak;
  /// `zemin-vurgu-yumusak` üzerindeki metin ve ikon (koyu temada da 4.5:1 üstü).
  final Color metinVurgu;
  /// Gövde ve başlık metni; `zemin`, `zemin-yukseltilmis`, `zemin-ikincil` ve yumuşak zeminler üzerinde.
  final Color metin;
  /// Açıklama, yardımcı metin, meta bilgi; `zemin`, `zemin-yukseltilmis` ve `zemin-ikincil` üzerinde.
  final Color metinIkincil;
  /// Bağlantılar ve marka renginde metin; `zemin`, `zemin-yukseltilmis` ve `zemin-marka-yumusak` üzerinde.
  final Color metinMarka;
  /// `zemin-marka` üzerindeki metin.
  final Color metinTers;
  /// Ana eylem dolgusu (Randevu al), seçili saat, etkin sekme. Üzerindeki metin `marka-ustu`.
  final Color marka;
  /// `marka` dolgusunun üzerine gelme ve basılı hali.
  final Color markaHover;
  /// `marka` dolgusu üzerindeki metin ve ikon.
  final Color markaUstu;
  /// Amblemdeki takvim başlığı, bildirim noktası, 'yeni' rozeti. Metin taşıyan küçük dolgularda üstündeki metin `vurgu-ustu`.
  final Color vurgu;
  /// `vurgu` dolgusu üzerindeki metin. Beyaz kullanılmaz (kontrast yetmez).
  final Color vurguUstu;
  /// Kart kenarı, ayraç, tablo çizgisi. Dekoratiftir; anlam taşıyan sınır için `cizgi-guclu`.
  final Color cizgi;
  /// Giriş alanı kenarı ve anlam taşıyan sınırlar; zeminlerle en az 3:1.
  final Color cizgiGuclu;
  /// Klavye odak halkası: 2px boşluk + 2px düz halka. Tüm zeminlerde en az 3:1.
  final Color odak;
  /// Onaylandı, geldi, ödendi. Her zaman ikon veya kelimeyle birlikte.
  final Color basari;
  /// Kapora bekleniyor, yaklaşan randevu. Metin olarak; dolgu için `uyari-400`.
  final Color uyari;
  /// İptal, gelmedi, hata mesajı, silme. Mercan ile karıştırılmaz: daha koyu ve kırmızıdır.
  final Color hata;
  /// Bilgilendirme mesajı ve nötr durum etiketi.
  final Color bilgi;
  /// Satır, menü öğesi ve hayalet butonun üzerine gelme zemini.
  final Color zeminHover;
  /// Seçili satır, seçili menü öğesi, seçili takvim günü. Üzerindeki metin `metin` veya `metin-marka`.
  final Color zeminSecili;
  /// Devre dışı kontrol zemini.
  final Color zeminDevreDisi;
  /// Devre dışı etiket ve değer. Kontrast şartından muaftır; yanında her zaman açıklama bulunur.
  final Color metinDevreDisi;
  /// Başarı bildirimi ve bant zemini; üstündeki metin `basari`.
  final Color basariZemin;
  /// Uyarı bildirimi ve bant zemini; üstündeki metin `uyari`.
  final Color uyariZemin;
  /// Hata bildirimi ve bant zemini; üstündeki metin `hata`.
  final Color hataZemin;
  /// Bilgi bildirimi ve bant zemini; üstündeki metin `bilgi`.
  final Color bilgiZemin;
  /// Modal ve yan panel arkasındaki karartma.
  final Color ustKatman;
  /// Randevu durumu: Onay bekliyor. Takvim bloğu ve durum rozeti zemini.
  final Color durumOnayZemin;
  /// Randevu durumu: Onay bekliyor. `durum-onay-zemin` üzerindeki metin.
  final Color durumOnayMetin;
  /// Randevu durumu: Onay bekliyor. Durum noktası ve takvim bloğunun üst şeridi; her zaman durum adıyla birlikte.
  final Color durumOnayNokta;
  /// Randevu durumu: Kapora bekleniyor. Takvim bloğu ve durum rozeti zemini.
  final Color durumKaporaZemin;
  /// Randevu durumu: Kapora bekleniyor. `durum-kapora-zemin` üzerindeki metin.
  final Color durumKaporaMetin;
  /// Randevu durumu: Kapora bekleniyor. Durum noktası ve takvim bloğunun üst şeridi; her zaman durum adıyla birlikte.
  final Color durumKaporaNokta;
  /// Randevu durumu: Onaylandı. Takvim bloğu ve durum rozeti zemini.
  final Color durumOnaylandiZemin;
  /// Randevu durumu: Onaylandı. `durum-onaylandi-zemin` üzerindeki metin.
  final Color durumOnaylandiMetin;
  /// Randevu durumu: Onaylandı. Durum noktası ve takvim bloğunun üst şeridi; her zaman durum adıyla birlikte.
  final Color durumOnaylandiNokta;
  /// Randevu durumu: Geldi (check-in). Takvim bloğu ve durum rozeti zemini.
  final Color durumGeldiZemin;
  /// Randevu durumu: Geldi (check-in). `durum-geldi-zemin` üzerindeki metin.
  final Color durumGeldiMetin;
  /// Randevu durumu: Geldi (check-in). Durum noktası ve takvim bloğunun üst şeridi; her zaman durum adıyla birlikte.
  final Color durumGeldiNokta;
  /// Randevu durumu: Tamamlandı. Takvim bloğu ve durum rozeti zemini.
  final Color durumTamamlandiZemin;
  /// Randevu durumu: Tamamlandı. `durum-tamamlandi-zemin` üzerindeki metin.
  final Color durumTamamlandiMetin;
  /// Randevu durumu: Tamamlandı. Durum noktası ve takvim bloğunun üst şeridi; her zaman durum adıyla birlikte.
  final Color durumTamamlandiNokta;
  /// Randevu durumu: Gelmedi. Takvim bloğu ve durum rozeti zemini.
  final Color durumGelmediZemin;
  /// Randevu durumu: Gelmedi. `durum-gelmedi-zemin` üzerindeki metin.
  final Color durumGelmediMetin;
  /// Randevu durumu: Gelmedi. Durum noktası ve takvim bloğunun üst şeridi; her zaman durum adıyla birlikte.
  final Color durumGelmediNokta;
  /// Randevu durumu: İptal. Takvim bloğu ve durum rozeti zemini.
  final Color durumIptalZemin;
  /// Randevu durumu: İptal. `durum-iptal-zemin` üzerindeki metin.
  final Color durumIptalMetin;
  /// Randevu durumu: İptal. Durum noktası ve takvim bloğunun üst şeridi; her zaman durum adıyla birlikte.
  final Color durumIptalNokta;
  /// Yüzen alt sekme çubuğunun zemini (koyu hap).
  final Color tabbarZemin;
  /// Seçili olmayan sekme ikonu; `tabbar-zemin` üzerinde.
  final Color tabbarPasif;
  /// Seçili sekmenin ikon ve etiketi; `tabbar-aktif-zemin` hapının üzerinde.
  final Color tabbarAktif;
  /// Seçili sekmenin beyaz hapı.
  final Color tabbarAktifZemin;
  /// Bento blok: en önemli bilgi (sıradaki randevu). Üstünde `metin-ters`.
  final Color blokMarka;
  /// Bento blok: dikkat isteyen sayı (onay bekleyen). Üstünde `gri-0`.
  final Color blokVurgu;
  /// Bento blok: sakin bilgi (sıra, paket). Üstünde `blok-kum-metin`.
  final Color blokKum;
  /// `blok-kum` üzerindeki metin.
  final Color blokKumMetin;
  /// `blok-marka` içindeki hap ("50 dk sonra"); iki temada aynı açık petrol. Üstünde `blok-marka-hap-metin`.
  final Color blokMarkaHap;
  /// `blok-marka-hap` üzerindeki metin.
  final Color blokMarkaHapMetin;
  /// Yüklenirken gösterilen iskelet blokları.
  final Color iskelet;
  /// Alt sayfa (bottom sheet) tutamağı.
  final Color tutamak;
  /// Değerlendirme yıldızları (dolu). Yanında her zaman sayısal puan.
  final Color yildiz;
  /// Avatar baş harf zemini, ton 1 (anlam taşımaz).
  final Color avatar1Zemin;
  /// Avatar baş harfi, ton 1; `avatar-1-zemin` üzerinde.
  final Color avatar1Metin;
  /// Avatar baş harf zemini, ton 2.
  final Color avatar2Zemin;
  /// Avatar baş harfi, ton 2; `avatar-2-zemin` üzerinde.
  final Color avatar2Metin;
  /// Avatar baş harf zemini, ton 3.
  final Color avatar3Zemin;
  /// Avatar baş harfi, ton 3; `avatar-3-zemin` üzerinde.
  final Color avatar3Metin;
  /// Kenar menüsünde etkin öğenin ikonu; `sidebar-secili` üzerinde.
  final Color sidebarIkonAktif;
  /// `blok-vurgu` özet bloğu üzerindeki metin.
  final Color blokVurguMetin;
  /// Rezervasyon sayfası kapak rengi `petrol` (BookingPageSettings.coverColor); SalonHeader kapağı. Panel ölçeğindeki aynı adlı tokenla aynı değer.
  final Color kapakPetrol;
  /// Rezervasyon sayfası kapak rengi `mercan` (BookingPageSettings.coverColor); SalonHeader kapağı. Panel ölçeğindeki aynı adlı tokenla aynı değer.
  final Color kapakMercan;
  /// Rezervasyon sayfası kapak rengi `kum` (BookingPageSettings.coverColor); SalonHeader kapağı. Panel ölçeğindeki aynı adlı tokenla aynı değer.
  final Color kapakKum;

  @override
  OmvilaRenkler copyWith({
    Color? zemin,
    Color? zeminYukseltilmis,
    Color? zeminIkincil,
    Color? zeminMarka,
    Color? zeminMarkaYumusak,
    Color? zeminVurguYumusak,
    Color? metinVurgu,
    Color? metin,
    Color? metinIkincil,
    Color? metinMarka,
    Color? metinTers,
    Color? marka,
    Color? markaHover,
    Color? markaUstu,
    Color? vurgu,
    Color? vurguUstu,
    Color? cizgi,
    Color? cizgiGuclu,
    Color? odak,
    Color? basari,
    Color? uyari,
    Color? hata,
    Color? bilgi,
    Color? zeminHover,
    Color? zeminSecili,
    Color? zeminDevreDisi,
    Color? metinDevreDisi,
    Color? basariZemin,
    Color? uyariZemin,
    Color? hataZemin,
    Color? bilgiZemin,
    Color? ustKatman,
    Color? durumOnayZemin,
    Color? durumOnayMetin,
    Color? durumOnayNokta,
    Color? durumKaporaZemin,
    Color? durumKaporaMetin,
    Color? durumKaporaNokta,
    Color? durumOnaylandiZemin,
    Color? durumOnaylandiMetin,
    Color? durumOnaylandiNokta,
    Color? durumGeldiZemin,
    Color? durumGeldiMetin,
    Color? durumGeldiNokta,
    Color? durumTamamlandiZemin,
    Color? durumTamamlandiMetin,
    Color? durumTamamlandiNokta,
    Color? durumGelmediZemin,
    Color? durumGelmediMetin,
    Color? durumGelmediNokta,
    Color? durumIptalZemin,
    Color? durumIptalMetin,
    Color? durumIptalNokta,
    Color? tabbarZemin,
    Color? tabbarPasif,
    Color? tabbarAktif,
    Color? tabbarAktifZemin,
    Color? blokMarka,
    Color? blokVurgu,
    Color? blokKum,
    Color? blokKumMetin,
    Color? blokMarkaHap,
    Color? blokMarkaHapMetin,
    Color? iskelet,
    Color? tutamak,
    Color? yildiz,
    Color? avatar1Zemin,
    Color? avatar1Metin,
    Color? avatar2Zemin,
    Color? avatar2Metin,
    Color? avatar3Zemin,
    Color? avatar3Metin,
    Color? sidebarIkonAktif,
    Color? blokVurguMetin,
    Color? kapakPetrol,
    Color? kapakMercan,
    Color? kapakKum,
  }) {
    return OmvilaRenkler(
      zemin: zemin ?? this.zemin,
      zeminYukseltilmis: zeminYukseltilmis ?? this.zeminYukseltilmis,
      zeminIkincil: zeminIkincil ?? this.zeminIkincil,
      zeminMarka: zeminMarka ?? this.zeminMarka,
      zeminMarkaYumusak: zeminMarkaYumusak ?? this.zeminMarkaYumusak,
      zeminVurguYumusak: zeminVurguYumusak ?? this.zeminVurguYumusak,
      metinVurgu: metinVurgu ?? this.metinVurgu,
      metin: metin ?? this.metin,
      metinIkincil: metinIkincil ?? this.metinIkincil,
      metinMarka: metinMarka ?? this.metinMarka,
      metinTers: metinTers ?? this.metinTers,
      marka: marka ?? this.marka,
      markaHover: markaHover ?? this.markaHover,
      markaUstu: markaUstu ?? this.markaUstu,
      vurgu: vurgu ?? this.vurgu,
      vurguUstu: vurguUstu ?? this.vurguUstu,
      cizgi: cizgi ?? this.cizgi,
      cizgiGuclu: cizgiGuclu ?? this.cizgiGuclu,
      odak: odak ?? this.odak,
      basari: basari ?? this.basari,
      uyari: uyari ?? this.uyari,
      hata: hata ?? this.hata,
      bilgi: bilgi ?? this.bilgi,
      zeminHover: zeminHover ?? this.zeminHover,
      zeminSecili: zeminSecili ?? this.zeminSecili,
      zeminDevreDisi: zeminDevreDisi ?? this.zeminDevreDisi,
      metinDevreDisi: metinDevreDisi ?? this.metinDevreDisi,
      basariZemin: basariZemin ?? this.basariZemin,
      uyariZemin: uyariZemin ?? this.uyariZemin,
      hataZemin: hataZemin ?? this.hataZemin,
      bilgiZemin: bilgiZemin ?? this.bilgiZemin,
      ustKatman: ustKatman ?? this.ustKatman,
      durumOnayZemin: durumOnayZemin ?? this.durumOnayZemin,
      durumOnayMetin: durumOnayMetin ?? this.durumOnayMetin,
      durumOnayNokta: durumOnayNokta ?? this.durumOnayNokta,
      durumKaporaZemin: durumKaporaZemin ?? this.durumKaporaZemin,
      durumKaporaMetin: durumKaporaMetin ?? this.durumKaporaMetin,
      durumKaporaNokta: durumKaporaNokta ?? this.durumKaporaNokta,
      durumOnaylandiZemin: durumOnaylandiZemin ?? this.durumOnaylandiZemin,
      durumOnaylandiMetin: durumOnaylandiMetin ?? this.durumOnaylandiMetin,
      durumOnaylandiNokta: durumOnaylandiNokta ?? this.durumOnaylandiNokta,
      durumGeldiZemin: durumGeldiZemin ?? this.durumGeldiZemin,
      durumGeldiMetin: durumGeldiMetin ?? this.durumGeldiMetin,
      durumGeldiNokta: durumGeldiNokta ?? this.durumGeldiNokta,
      durumTamamlandiZemin: durumTamamlandiZemin ?? this.durumTamamlandiZemin,
      durumTamamlandiMetin: durumTamamlandiMetin ?? this.durumTamamlandiMetin,
      durumTamamlandiNokta: durumTamamlandiNokta ?? this.durumTamamlandiNokta,
      durumGelmediZemin: durumGelmediZemin ?? this.durumGelmediZemin,
      durumGelmediMetin: durumGelmediMetin ?? this.durumGelmediMetin,
      durumGelmediNokta: durumGelmediNokta ?? this.durumGelmediNokta,
      durumIptalZemin: durumIptalZemin ?? this.durumIptalZemin,
      durumIptalMetin: durumIptalMetin ?? this.durumIptalMetin,
      durumIptalNokta: durumIptalNokta ?? this.durumIptalNokta,
      tabbarZemin: tabbarZemin ?? this.tabbarZemin,
      tabbarPasif: tabbarPasif ?? this.tabbarPasif,
      tabbarAktif: tabbarAktif ?? this.tabbarAktif,
      tabbarAktifZemin: tabbarAktifZemin ?? this.tabbarAktifZemin,
      blokMarka: blokMarka ?? this.blokMarka,
      blokVurgu: blokVurgu ?? this.blokVurgu,
      blokKum: blokKum ?? this.blokKum,
      blokKumMetin: blokKumMetin ?? this.blokKumMetin,
      blokMarkaHap: blokMarkaHap ?? this.blokMarkaHap,
      blokMarkaHapMetin: blokMarkaHapMetin ?? this.blokMarkaHapMetin,
      iskelet: iskelet ?? this.iskelet,
      tutamak: tutamak ?? this.tutamak,
      yildiz: yildiz ?? this.yildiz,
      avatar1Zemin: avatar1Zemin ?? this.avatar1Zemin,
      avatar1Metin: avatar1Metin ?? this.avatar1Metin,
      avatar2Zemin: avatar2Zemin ?? this.avatar2Zemin,
      avatar2Metin: avatar2Metin ?? this.avatar2Metin,
      avatar3Zemin: avatar3Zemin ?? this.avatar3Zemin,
      avatar3Metin: avatar3Metin ?? this.avatar3Metin,
      sidebarIkonAktif: sidebarIkonAktif ?? this.sidebarIkonAktif,
      blokVurguMetin: blokVurguMetin ?? this.blokVurguMetin,
      kapakPetrol: kapakPetrol ?? this.kapakPetrol,
      kapakMercan: kapakMercan ?? this.kapakMercan,
      kapakKum: kapakKum ?? this.kapakKum,
    );
  }

  @override
  OmvilaRenkler lerp(ThemeExtension<OmvilaRenkler>? other, double t) {
    if (other is! OmvilaRenkler) {
      return this;
    }
    return OmvilaRenkler(
      zemin: Color.lerp(zemin, other.zemin, t)!,
      zeminYukseltilmis: Color.lerp(zeminYukseltilmis, other.zeminYukseltilmis, t)!,
      zeminIkincil: Color.lerp(zeminIkincil, other.zeminIkincil, t)!,
      zeminMarka: Color.lerp(zeminMarka, other.zeminMarka, t)!,
      zeminMarkaYumusak: Color.lerp(zeminMarkaYumusak, other.zeminMarkaYumusak, t)!,
      zeminVurguYumusak: Color.lerp(zeminVurguYumusak, other.zeminVurguYumusak, t)!,
      metinVurgu: Color.lerp(metinVurgu, other.metinVurgu, t)!,
      metin: Color.lerp(metin, other.metin, t)!,
      metinIkincil: Color.lerp(metinIkincil, other.metinIkincil, t)!,
      metinMarka: Color.lerp(metinMarka, other.metinMarka, t)!,
      metinTers: Color.lerp(metinTers, other.metinTers, t)!,
      marka: Color.lerp(marka, other.marka, t)!,
      markaHover: Color.lerp(markaHover, other.markaHover, t)!,
      markaUstu: Color.lerp(markaUstu, other.markaUstu, t)!,
      vurgu: Color.lerp(vurgu, other.vurgu, t)!,
      vurguUstu: Color.lerp(vurguUstu, other.vurguUstu, t)!,
      cizgi: Color.lerp(cizgi, other.cizgi, t)!,
      cizgiGuclu: Color.lerp(cizgiGuclu, other.cizgiGuclu, t)!,
      odak: Color.lerp(odak, other.odak, t)!,
      basari: Color.lerp(basari, other.basari, t)!,
      uyari: Color.lerp(uyari, other.uyari, t)!,
      hata: Color.lerp(hata, other.hata, t)!,
      bilgi: Color.lerp(bilgi, other.bilgi, t)!,
      zeminHover: Color.lerp(zeminHover, other.zeminHover, t)!,
      zeminSecili: Color.lerp(zeminSecili, other.zeminSecili, t)!,
      zeminDevreDisi: Color.lerp(zeminDevreDisi, other.zeminDevreDisi, t)!,
      metinDevreDisi: Color.lerp(metinDevreDisi, other.metinDevreDisi, t)!,
      basariZemin: Color.lerp(basariZemin, other.basariZemin, t)!,
      uyariZemin: Color.lerp(uyariZemin, other.uyariZemin, t)!,
      hataZemin: Color.lerp(hataZemin, other.hataZemin, t)!,
      bilgiZemin: Color.lerp(bilgiZemin, other.bilgiZemin, t)!,
      ustKatman: Color.lerp(ustKatman, other.ustKatman, t)!,
      durumOnayZemin: Color.lerp(durumOnayZemin, other.durumOnayZemin, t)!,
      durumOnayMetin: Color.lerp(durumOnayMetin, other.durumOnayMetin, t)!,
      durumOnayNokta: Color.lerp(durumOnayNokta, other.durumOnayNokta, t)!,
      durumKaporaZemin: Color.lerp(durumKaporaZemin, other.durumKaporaZemin, t)!,
      durumKaporaMetin: Color.lerp(durumKaporaMetin, other.durumKaporaMetin, t)!,
      durumKaporaNokta: Color.lerp(durumKaporaNokta, other.durumKaporaNokta, t)!,
      durumOnaylandiZemin: Color.lerp(durumOnaylandiZemin, other.durumOnaylandiZemin, t)!,
      durumOnaylandiMetin: Color.lerp(durumOnaylandiMetin, other.durumOnaylandiMetin, t)!,
      durumOnaylandiNokta: Color.lerp(durumOnaylandiNokta, other.durumOnaylandiNokta, t)!,
      durumGeldiZemin: Color.lerp(durumGeldiZemin, other.durumGeldiZemin, t)!,
      durumGeldiMetin: Color.lerp(durumGeldiMetin, other.durumGeldiMetin, t)!,
      durumGeldiNokta: Color.lerp(durumGeldiNokta, other.durumGeldiNokta, t)!,
      durumTamamlandiZemin: Color.lerp(durumTamamlandiZemin, other.durumTamamlandiZemin, t)!,
      durumTamamlandiMetin: Color.lerp(durumTamamlandiMetin, other.durumTamamlandiMetin, t)!,
      durumTamamlandiNokta: Color.lerp(durumTamamlandiNokta, other.durumTamamlandiNokta, t)!,
      durumGelmediZemin: Color.lerp(durumGelmediZemin, other.durumGelmediZemin, t)!,
      durumGelmediMetin: Color.lerp(durumGelmediMetin, other.durumGelmediMetin, t)!,
      durumGelmediNokta: Color.lerp(durumGelmediNokta, other.durumGelmediNokta, t)!,
      durumIptalZemin: Color.lerp(durumIptalZemin, other.durumIptalZemin, t)!,
      durumIptalMetin: Color.lerp(durumIptalMetin, other.durumIptalMetin, t)!,
      durumIptalNokta: Color.lerp(durumIptalNokta, other.durumIptalNokta, t)!,
      tabbarZemin: Color.lerp(tabbarZemin, other.tabbarZemin, t)!,
      tabbarPasif: Color.lerp(tabbarPasif, other.tabbarPasif, t)!,
      tabbarAktif: Color.lerp(tabbarAktif, other.tabbarAktif, t)!,
      tabbarAktifZemin: Color.lerp(tabbarAktifZemin, other.tabbarAktifZemin, t)!,
      blokMarka: Color.lerp(blokMarka, other.blokMarka, t)!,
      blokVurgu: Color.lerp(blokVurgu, other.blokVurgu, t)!,
      blokKum: Color.lerp(blokKum, other.blokKum, t)!,
      blokKumMetin: Color.lerp(blokKumMetin, other.blokKumMetin, t)!,
      blokMarkaHap: Color.lerp(blokMarkaHap, other.blokMarkaHap, t)!,
      blokMarkaHapMetin: Color.lerp(blokMarkaHapMetin, other.blokMarkaHapMetin, t)!,
      iskelet: Color.lerp(iskelet, other.iskelet, t)!,
      tutamak: Color.lerp(tutamak, other.tutamak, t)!,
      yildiz: Color.lerp(yildiz, other.yildiz, t)!,
      avatar1Zemin: Color.lerp(avatar1Zemin, other.avatar1Zemin, t)!,
      avatar1Metin: Color.lerp(avatar1Metin, other.avatar1Metin, t)!,
      avatar2Zemin: Color.lerp(avatar2Zemin, other.avatar2Zemin, t)!,
      avatar2Metin: Color.lerp(avatar2Metin, other.avatar2Metin, t)!,
      avatar3Zemin: Color.lerp(avatar3Zemin, other.avatar3Zemin, t)!,
      avatar3Metin: Color.lerp(avatar3Metin, other.avatar3Metin, t)!,
      sidebarIkonAktif: Color.lerp(sidebarIkonAktif, other.sidebarIkonAktif, t)!,
      blokVurguMetin: Color.lerp(blokVurguMetin, other.blokVurguMetin, t)!,
      kapakPetrol: Color.lerp(kapakPetrol, other.kapakPetrol, t)!,
      kapakMercan: Color.lerp(kapakMercan, other.kapakMercan, t)!,
      kapakKum: Color.lerp(kapakKum, other.kapakKum, t)!,
    );
  }
}

/// Gölgeler (açık ve koyu temada farklı). Kullanım: `context.omvilaGolgeler.golgeSm`.
@immutable
class OmvilaGolgeler extends ThemeExtension<OmvilaGolgeler> {
  /// Temalar için `acik` ve `koyu` sabitlerini kullanın.
  const OmvilaGolgeler({
    required this.golgeSm,
    required this.golgeMd,
    required this.golgeLg,
    required this.odakHalkasi,
  });

  /// Açık tema değerleri.
  static const acik = OmvilaGolgeler(
    golgeSm: [BoxShadow(color: Color.fromRGBO(21, 24, 22, 0.04), offset: Offset(0, 1), blurRadius: 2), BoxShadow(color: Color.fromRGBO(21, 24, 22, 0.05), offset: Offset(0, 6), blurRadius: 20)],
    golgeMd: [BoxShadow(color: Color.fromRGBO(11, 59, 55, 0.12), offset: Offset(0, 8), blurRadius: 24)],
    golgeLg: [BoxShadow(color: Color.fromRGBO(11, 59, 55, 0.16), offset: Offset(0, -8), blurRadius: 32)],
    odakHalkasi: [BoxShadow(color: Color(0xFFFFFFFF), spreadRadius: 2), BoxShadow(color: Color(0xFF14857A), spreadRadius: 4)],
  );

  /// Koyu tema değerleri.
  static const koyu = OmvilaGolgeler(
    golgeSm: [BoxShadow(color: Color.fromRGBO(0, 0, 0, 0.4), offset: Offset(0, 1), blurRadius: 3)],
    golgeMd: [BoxShadow(color: Color.fromRGBO(0, 0, 0, 0.5), offset: Offset(0, 8), blurRadius: 24)],
    golgeLg: [BoxShadow(color: Color.fromRGBO(0, 0, 0, 0.6), offset: Offset(0, -8), blurRadius: 32)],
    odakHalkasi: [BoxShadow(color: Color(0xFF16201E), spreadRadius: 2), BoxShadow(color: Color(0xFF74C6B8), spreadRadius: 4)],
  );

  /// Kart (çizgisiz kullanıldığında).
  final List<BoxShadow> golgeSm;
  /// Yüzen buton, snackbar.
  final List<BoxShadow> golgeMd;
  /// Alt sayfa (yukarı doğru gölge).
  final List<BoxShadow> golgeLg;
  /// Klavye ve erişilebilirlik odağı (web rezervasyon sayfası, harici klavye).
  final List<BoxShadow> odakHalkasi;

  @override
  OmvilaGolgeler copyWith({
    List<BoxShadow>? golgeSm,
    List<BoxShadow>? golgeMd,
    List<BoxShadow>? golgeLg,
    List<BoxShadow>? odakHalkasi,
  }) {
    return OmvilaGolgeler(
      golgeSm: golgeSm ?? this.golgeSm,
      golgeMd: golgeMd ?? this.golgeMd,
      golgeLg: golgeLg ?? this.golgeLg,
      odakHalkasi: odakHalkasi ?? this.odakHalkasi,
    );
  }

  @override
  OmvilaGolgeler lerp(ThemeExtension<OmvilaGolgeler>? other, double t) {
    if (other is! OmvilaGolgeler) {
      return this;
    }
    return OmvilaGolgeler(
      golgeSm: BoxShadow.lerpList(golgeSm, other.golgeSm, t)!,
      golgeMd: BoxShadow.lerpList(golgeMd, other.golgeMd, t)!,
      golgeLg: BoxShadow.lerpList(golgeLg, other.golgeLg, t)!,
      odakHalkasi: BoxShadow.lerpList(odakHalkasi, other.odakHalkasi, t)!,
    );
  }
}

/// Boşluk ölçeği (mantıksal piksel).
abstract final class OmvilaBosluk {
  /// İkon ile metin arası (sıkı).
  static const double bosluk1 = 4;

  /// Çipler ve saat dilimleri arası.
  static const double bosluk2 = 8;

  /// Liste öğesi içi, kart içi öğeler arası.
  static const double bosluk3 = 12;

  /// Ekran kenar boşluğu, kart iç boşluğu.
  static const double bosluk4 = 16;

  /// Form alanları arası.
  static const double bosluk5 = 20;

  /// Bölümler arası.
  static const double bosluk6 = 24;

  /// Büyük bölüm ayrımı, alt sayfa iç boşluğu (alt).
  static const double bosluk8 = 32;

  /// Boş durum iç boşluğu.
  static const double bosluk12 = 48;
}

/// Köşe yarıçapları.
abstract final class OmvilaKose {
  /// Rozet.
  static const double koseXs = 6;

  /// Saat dilimi, çip, küçük buton.
  static const double koseSm = 12;

  /// Buton, giriş alanı.
  static const double koseMd = 14;

  /// Liste kartı, hizmet kartı, liste grubu.
  static const double koseLg = 20;

  /// Bento blok, büyük kart, yüzen sekme çubuğu, diyalog.
  static const double koseXl = 24;

  /// Avatar, hap çip, switch.
  static const double koseTam = 999;
}

/// Boyutlar: dokunma alanı, kontrol yükseklikleri, çubuklar.
abstract final class OmvilaBoyut {
  /// En küçük dokunma alanı; görsel öğe küçük olsa da dokunma alanı bu kadardır.
  static const double dokunmaMin = 44;

  /// Küçük buton (liste içi eylem), çip, segment.
  static const double kontrolSm = 36;

  /// Giriş alanı, varsayılan buton.
  static const double kontrolMd = 48;

  /// Ekranın ana eylemi (alt sabit buton).
  static const double kontrolLg = 52;

  /// Üst bar yüksekliği (durum çubuğu hariç).
  static const double ustBar = 56;

  /// Alt sekme çubuğu yüksekliği (güvenli alan hariç).
  static const double tabbar = 64;

  /// Saat seçicide bir saat düğmesinin yüksekliği.
  static const double saatDilimi = 44;

  /// Tasarım referans genişliği; rezervasyon sayfası masaüstünde en fazla 480px ortalanır.
  static const double ekranGenislik = 390;
}

/// Süreler ve eğriler. Azaltılmış hareket açıkken kayma ve büyüme yerine yalnız `sureAzaltilmis` opaklık geçişi kalır.
abstract final class OmvilaHareket {
  /// Basma geri bildirimi.
  static const Duration sureAnlik = Duration(milliseconds: 80);

  /// Hover, çip, anahtar, sekme göstergesi.
  static const Duration sureHizli = Duration(milliseconds: 150);

  /// Ekrandan çıkan öğe (alt sayfa kapanışı); girişten kısadır.
  static const Duration sureCikis = Duration(milliseconds: 180);

  /// Alt sayfa, çekmece, sayfa geçişi, menü daralması.
  static const Duration sureOrta = Duration(milliseconds: 240);

  /// Tam ekran modal.
  static const Duration sureYavas = Duration(milliseconds: 360);

  /// Azaltılmış hareket açıkken kayma ve büyüme yerine kalan tek opaklık geçişi.
  static const Duration sureAzaltilmis = Duration(milliseconds: 120);

  /// Yerinde değişim.
  static const Cubic egriStandart = Cubic(0.2, 0, 0, 1);

  /// Ekrana giren öğe.
  static const Cubic egriGiris = Cubic(0, 0, 0, 1);

  /// Ekrandan çıkan öğe.
  static const Cubic egriCikis = Cubic(0.3, 0, 1, 1);
}

/// Yazı stilleri. Renk vermez; rengi bağlamdan alır. Yazı tipi `omvila_ui` paketinin varlığıdır.
abstract final class OmvilaYazi {
  /// Yazı ailesi (Latin alt kümesi).
  static const String aile = 'PlusJakartaSans';

  /// Latin Extended alt kümesi (ğ, ş, İ, ı…); aile ile birlikte yüklenir.
  static const List<String> yedekAileler = ['PlusJakartaSansTR'];

  /// Varlıkların bulunduğu paket.
  static const String paket = 'omvila_ui';

  /// Sekme kök ekranlarının büyük başlığı; kaydırınca `ust-bar-basligi`na küçülür.
  static const TextStyle buyukBaslik = TextStyle(
    fontFamily: aile,
    fontFamilyFallback: yedekAileler,
    package: paket,
    fontSize: 34,
    height: 1.118,
    fontWeight: FontWeight.w800,
    letterSpacing: -1.19,
  );

  /// Detay ekranı başlığı, rezervasyon sayfasında salon adı.
  static const TextStyle baslik1 = TextStyle(
    fontFamily: aile,
    fontFamilyFallback: yedekAileler,
    package: paket,
    fontSize: 24,
    height: 1.25,
    fontWeight: FontWeight.w700,
    letterSpacing: -0.24,
  );

  /// Bölüm başlığı, alt sayfa başlığı, adım başlığı.
  static const TextStyle baslik2 = TextStyle(
    fontFamily: aile,
    fontFamilyFallback: yedekAileler,
    package: paket,
    fontSize: 20,
    height: 1.3,
    fontWeight: FontWeight.w600,
  );

  /// Kart ve liste öğesi başlığı.
  static const TextStyle baslik3 = TextStyle(
    fontFamily: aile,
    fontFamilyFallback: yedekAileler,
    package: paket,
    fontSize: 17,
    height: 1.412,
    fontWeight: FontWeight.w600,
  );

  /// Üst bar ortasındaki başlık.
  static const TextStyle ustBarBasligi = TextStyle(
    fontFamily: aile,
    fontFamilyFallback: yedekAileler,
    package: paket,
    fontSize: 17,
    height: 1.294,
    fontWeight: FontWeight.w600,
  );

  /// Paket seansı, günlük ciro gibi tek büyük sayı; `tabular-nums`.
  static const TextStyle metrik = TextStyle(
    fontFamily: aile,
    fontFamilyFallback: yedekAileler,
    package: paket,
    fontSize: 36,
    height: 1.111,
    fontWeight: FontWeight.w800,
    letterSpacing: -1.44,
    fontFeatures: [FontFeature.tabularFigures()],
  );

  /// Varsayılan metin; mobilde okunacak hiçbir metin 16px'in altına inmez (açıklama hariç).
  static const TextStyle govde = TextStyle(
    fontFamily: aile,
    fontFamilyFallback: yedekAileler,
    package: paket,
    fontSize: 16,
    height: 1.5,
    fontWeight: FontWeight.w400,
  );

  /// Liste öğesinin birincil değeri.
  static const TextStyle govdeGuclu = TextStyle(
    fontFamily: aile,
    fontFamilyFallback: yedekAileler,
    package: paket,
    fontSize: 16,
    height: 1.5,
    fontWeight: FontWeight.w600,
  );

  /// Liste öğesinin ikinci satırı, yardımcı metin.
  static const TextStyle ikincil = TextStyle(
    fontFamily: aile,
    fontFamilyFallback: yedekAileler,
    package: paket,
    fontSize: 14,
    height: 1.429,
    fontWeight: FontWeight.w400,
  );

  /// Buton metni.
  static const TextStyle etiket = TextStyle(
    fontFamily: aile,
    fontFamilyFallback: yedekAileler,
    package: paket,
    fontSize: 16,
    height: 1.25,
    fontWeight: FontWeight.w600,
  );

  /// Küçük buton, çip, segment.
  static const TextStyle etiketKucuk = TextStyle(
    fontFamily: aile,
    fontFamilyFallback: yedekAileler,
    package: paket,
    fontSize: 14,
    height: 1.286,
    fontWeight: FontWeight.w600,
  );

  /// Rozet, zaman damgası, form ipucu.
  static const TextStyle aciklama = TextStyle(
    fontFamily: aile,
    fontFamilyFallback: yedekAileler,
    package: paket,
    fontSize: 12,
    height: 1.333,
    fontWeight: FontWeight.w500,
  );

  /// Alt sekme çubuğu etiketi.
  static const TextStyle sekme = TextStyle(
    fontFamily: aile,
    fontFamilyFallback: yedekAileler,
    package: paket,
    fontSize: 11,
    height: 1.273,
    fontWeight: FontWeight.w600,
  );

  /// Saat ve tutar; `tabular-nums`.
  static const TextStyle rakam = TextStyle(
    fontFamily: aile,
    fontFamilyFallback: yedekAileler,
    package: paket,
    fontSize: 16,
    height: 1.5,
    fontWeight: FontWeight.w600,
    fontFeatures: [FontFeature.tabularFigures()],
  );
}

/// Uygulamanın ThemeData'sı. `MaterialApp(theme: OmvilaTema.acik(), darkTheme: OmvilaTema.koyu())`
abstract final class OmvilaTema {
  /// Açık tema.
  static ThemeData acik() =>
      _tema(OmvilaRenkler.acik, OmvilaGolgeler.acik, Brightness.light);

  /// Koyu tema.
  static ThemeData koyu() =>
      _tema(OmvilaRenkler.koyu, OmvilaGolgeler.koyu, Brightness.dark);

  static RoundedRectangleBorder _kose(double k) =>
      RoundedRectangleBorder(borderRadius: BorderRadius.circular(k));

  static OutlineInputBorder _girisKenari(Color renk, [double kalinlik = 1]) =>
      OutlineInputBorder(
        borderRadius: BorderRadius.circular(OmvilaKose.koseMd),
        borderSide: BorderSide(color: renk, width: kalinlik),
      );

  static ThemeData _tema(OmvilaRenkler r, OmvilaGolgeler g, Brightness b) {
    final scheme = ColorScheme(
      brightness: b,
      primary: r.marka,
      onPrimary: r.markaUstu,
      primaryContainer: r.zeminSecili,
      onPrimaryContainer: r.metinMarka,
      secondary: r.vurgu,
      onSecondary: r.vurguUstu,
      error: r.hata,
      onError: r.markaUstu,
      surface: r.zeminYukseltilmis,
      onSurface: r.metin,
      onSurfaceVariant: r.metinIkincil,
      surfaceContainerHighest: r.zeminIkincil,
      outline: r.cizgiGuclu,
      outlineVariant: r.cizgi,
      scrim: r.ustKatman,
    );
    return ThemeData(
      brightness: b,
      colorScheme: scheme,
      fontFamily: OmvilaYazi.aile,
      fontFamilyFallback: OmvilaYazi.yedekAileler,
      package: OmvilaYazi.paket,
      scaffoldBackgroundColor: r.zemin,
      materialTapTargetSize: MaterialTapTargetSize.padded,
      extensions: [r, g],
      textTheme: const TextTheme(
        displaySmall: OmvilaYazi.buyukBaslik,
        headlineSmall: OmvilaYazi.baslik1,
        titleLarge: OmvilaYazi.baslik2,
        titleMedium: OmvilaYazi.baslik3,
        bodyLarge: OmvilaYazi.govde,
        bodyMedium: OmvilaYazi.ikincil,
        labelLarge: OmvilaYazi.etiket,
        labelMedium: OmvilaYazi.etiketKucuk,
        labelSmall: OmvilaYazi.aciklama,
      ).apply(bodyColor: r.metin, displayColor: r.metin),
      appBarTheme: AppBarTheme(
        backgroundColor: r.zemin,
        foregroundColor: r.metin,
        elevation: 0,
        scrolledUnderElevation: 0,
        centerTitle: true,
        toolbarHeight: OmvilaBoyut.ustBar,
        titleTextStyle: OmvilaYazi.ustBarBasligi.copyWith(color: r.metin),
      ),
      filledButtonTheme: FilledButtonThemeData(
        style: FilledButton.styleFrom(
          backgroundColor: r.marka,
          foregroundColor: r.markaUstu,
          disabledBackgroundColor: r.zeminDevreDisi,
          disabledForegroundColor: r.metinDevreDisi,
          minimumSize: const Size(64, OmvilaBoyut.kontrolMd),
          textStyle: OmvilaYazi.etiket,
          shape: _kose(OmvilaKose.koseMd),
        ),
      ),
      outlinedButtonTheme: OutlinedButtonThemeData(
        style: OutlinedButton.styleFrom(
          foregroundColor: r.metin,
          disabledForegroundColor: r.metinDevreDisi,
          side: BorderSide(color: r.cizgiGuclu),
          minimumSize: const Size(64, OmvilaBoyut.kontrolMd),
          textStyle: OmvilaYazi.etiket,
          shape: _kose(OmvilaKose.koseMd),
        ),
      ),
      textButtonTheme: TextButtonThemeData(
        style: TextButton.styleFrom(
          foregroundColor: r.metinMarka,
          minimumSize: const Size(OmvilaBoyut.dokunmaMin, OmvilaBoyut.dokunmaMin),
          textStyle: OmvilaYazi.etiket,
        ),
      ),
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: r.zeminYukseltilmis,
        contentPadding: const EdgeInsets.symmetric(
          horizontal: OmvilaBosluk.bosluk4,
          vertical: 14,
        ),
        border: _girisKenari(r.cizgiGuclu),
        enabledBorder: _girisKenari(r.cizgiGuclu),
        focusedBorder: _girisKenari(r.odak, 2),
        errorBorder: _girisKenari(r.hata),
        focusedErrorBorder: _girisKenari(r.hata, 2),
        labelStyle: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
        hintStyle: OmvilaYazi.govde.copyWith(color: r.metinIkincil),
        errorStyle: OmvilaYazi.aciklama.copyWith(color: r.hata),
      ),
      cardTheme: CardThemeData(
        color: r.zeminYukseltilmis,
        elevation: 0,
        margin: EdgeInsets.zero,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
          side: BorderSide(color: r.cizgi),
        ),
      ),
      navigationBarTheme: NavigationBarThemeData(
        backgroundColor: r.tabbarZemin,
        height: OmvilaBoyut.tabbar,
        indicatorColor: r.tabbarAktifZemin,
        elevation: 0,
        iconTheme: WidgetStateProperty.resolveWith(
          (s) => IconThemeData(
            color: s.contains(WidgetState.selected) ? r.tabbarAktif : r.tabbarPasif,
            size: 24,
          ),
        ),
        labelTextStyle: WidgetStateProperty.resolveWith(
          (s) => OmvilaYazi.sekme.copyWith(
            color: s.contains(WidgetState.selected) ? r.tabbarAktif : r.tabbarPasif,
          ),
        ),
      ),
      bottomSheetTheme: BottomSheetThemeData(
        backgroundColor: r.zeminYukseltilmis,
        showDragHandle: true,
        dragHandleColor: r.tutamak,
        modalBarrierColor: r.ustKatman,
        shape: const RoundedRectangleBorder(
          borderRadius: BorderRadius.vertical(
            top: Radius.circular(OmvilaKose.koseXl),
          ),
        ),
      ),
      dialogTheme: DialogThemeData(
        backgroundColor: r.zeminYukseltilmis,
        barrierColor: r.ustKatman,
        shape: _kose(OmvilaKose.koseXl),
      ),
      chipTheme: ChipThemeData(
        shape: _kose(OmvilaKose.koseTam),
        side: BorderSide(color: r.cizgiGuclu),
        selectedColor: r.zeminSecili,
        labelStyle: OmvilaYazi.etiketKucuk.copyWith(color: r.metin),
      ),
      snackBarTheme: SnackBarThemeData(
        behavior: SnackBarBehavior.floating,
        backgroundColor: r.zeminMarka,
        contentTextStyle: OmvilaYazi.ikincil.copyWith(color: r.metinTers),
        actionTextColor: r.metinTers,
        shape: _kose(OmvilaKose.koseMd),
      ),
      dividerTheme: DividerThemeData(color: r.cizgi, thickness: 1, space: 1),
      progressIndicatorTheme: ProgressIndicatorThemeData(color: r.marka),
      switchTheme: SwitchThemeData(
        trackColor: WidgetStateProperty.resolveWith(
          (s) => s.contains(WidgetState.selected) ? r.marka : r.cizgiGuclu,
        ),
        thumbColor: WidgetStatePropertyAll(r.zeminYukseltilmis),
        trackOutlineColor: const WidgetStatePropertyAll(Colors.transparent),
      ),
    );
  }
}

/// Bağlamdan tema uzantıları: `context.omvilaRenkler.marka`.
extension OmvilaTemaBaglami on BuildContext {
  /// Semantik renkler.
  OmvilaRenkler get omvilaRenkler => Theme.of(this).extension<OmvilaRenkler>()!;

  /// Gölgeler.
  OmvilaGolgeler get omvilaGolgeler =>
      Theme.of(this).extension<OmvilaGolgeler>()!;
}
