import 'package:flutter/material.dart';
import 'package:omvila_ui/theme/omvila_theme.dart';

/// Buton varyantı (design/tasarim-sistemleri/mobil/components/Button).
enum OmvilaButonTuru {
  /// Ekranın asıl işi; bir ekranda tek.
  birincil,

  /// Alternatif eylem: dolu nötr zemin (`zemin-ikincil`), çerçevesiz.
  ikincil,

  /// Koyu hap (`metin` zemin, `zemin` metin; bundle.css `om-btn-koyu`):
  /// başlıktaki "+ Randevu" gibi ekranın ana eylemi olmayan ama öne çıkan
  /// kısa eylem.
  koyu,

  /// Düşük öncelik.
  hayalet,

  /// Geri alınamaz işlem (onay diyaloğunda).
  tehlike,
}

/// Buton yüksekliği.
enum OmvilaButonBoyutu {
  /// 52: alt sabit ana eylem.
  lg(OmvilaBoyut.kontrolLg),

  /// 48: varsayılan.
  md(OmvilaBoyut.kontrolMd),

  /// 36: liste içi (dokunma alanı yine 44).
  sm(OmvilaBoyut.kontrolSm);

  const OmvilaButonBoyutu(this.yukseklik);

  /// Görsel yükseklik.
  final double yukseklik;
}

/// Mobil buton. Metin fiille başlar; uzun işlemde [yukleniyor] ve
/// "Onaylanıyor…" gibi [yukleniyorEtiketi].
class OmvilaButon extends StatelessWidget {
  /// Buton.
  const OmvilaButon({
    required this.etiket,
    required this.onPressed,
    this.tur = OmvilaButonTuru.birincil,
    this.boyut = OmvilaButonBoyutu.md,
    this.ikon,
    this.yukleniyor = false,
    this.yukleniyorEtiketi,
    this.tamGenislik = false,
    super.key,
  });

  /// Buton metni.
  final String etiket;

  /// Null ise devre dışı.
  final VoidCallback? onPressed;

  /// Varyant.
  final OmvilaButonTuru tur;

  /// Yükseklik.
  final OmvilaButonBoyutu boyut;

  /// Metnin önündeki ikon (20).
  final IconData? ikon;

  /// İşlem sürüyor: dokunuş yok sayılır, gösterge ve [yukleniyorEtiketi].
  final bool yukleniyor;

  /// Yüklenirken metin; verilmezse [etiket].
  final String? yukleniyorEtiketi;

  /// Kapsayıcı genişliğini doldurur.
  final bool tamGenislik;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final textStyle = boyut == OmvilaButonBoyutu.sm
        ? OmvilaYazi.etiketKucuk
        : OmvilaYazi.etiket;
    final style = ButtonStyle(
      minimumSize: WidgetStatePropertyAll(
        Size(tamGenislik ? double.infinity : 64, boyut.yukseklik),
      ),
      padding: const WidgetStatePropertyAll(
        EdgeInsets.symmetric(horizontal: OmvilaBosluk.bosluk4),
      ),
      textStyle: WidgetStatePropertyAll(textStyle),
      shape: WidgetStatePropertyAll(
        RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(
            boyut == OmvilaButonBoyutu.sm
                ? OmvilaKose.koseSm
                : OmvilaKose.koseMd,
          ),
        ),
      ),
      tapTargetSize: MaterialTapTargetSize.padded,
    );
    final label = yukleniyor ? (yukleniyorEtiketi ?? etiket) : etiket;
    final foreground = switch (tur) {
      OmvilaButonTuru.birincil || OmvilaButonTuru.tehlike => r.markaUstu,
      OmvilaButonTuru.ikincil => r.metin,
      OmvilaButonTuru.koyu => r.zemin,
      OmvilaButonTuru.hayalet => r.metinMarka,
    };
    final child = Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        if (yukleniyor)
          SizedBox.square(
            dimension: 18,
            child: CircularProgressIndicator(strokeWidth: 2, color: foreground),
          )
        else if (ikon != null)
          Icon(ikon, size: 20),
        if (yukleniyor || ikon != null)
          const SizedBox(width: OmvilaBosluk.bosluk2),
        Flexible(child: Text(label, textAlign: TextAlign.center)),
      ],
    );
    final pressed = onPressed;
    final button = switch (tur) {
      OmvilaButonTuru.birincil => FilledButton(
        onPressed: pressed,
        style: style,
        child: child,
      ),
      OmvilaButonTuru.tehlike => FilledButton(
        onPressed: pressed,
        style: style.copyWith(
          backgroundColor: WidgetStateProperty.resolveWith(
            (s) => s.contains(WidgetState.disabled) ? r.zeminDevreDisi : r.hata,
          ),
        ),
        child: child,
      ),
      OmvilaButonTuru.ikincil => FilledButton(
        onPressed: pressed,
        style: style.copyWith(
          backgroundColor: WidgetStateProperty.resolveWith(
            (s) => s.contains(WidgetState.disabled)
                ? r.zeminDevreDisi
                : r.zeminIkincil,
          ),
          foregroundColor: WidgetStateProperty.resolveWith(
            (s) =>
                s.contains(WidgetState.disabled) ? r.metinDevreDisi : r.metin,
          ),
          overlayColor: WidgetStatePropertyAll(r.zeminHover),
        ),
        child: child,
      ),
      OmvilaButonTuru.koyu => FilledButton(
        onPressed: pressed,
        style: style.copyWith(
          backgroundColor: WidgetStateProperty.resolveWith(
            (s) =>
                s.contains(WidgetState.disabled) ? r.zeminDevreDisi : r.metin,
          ),
          foregroundColor: WidgetStateProperty.resolveWith(
            (s) =>
                s.contains(WidgetState.disabled) ? r.metinDevreDisi : r.zemin,
          ),
          // Tasarımdaki koyu hap (`kose-tam`).
          shape: const WidgetStatePropertyAll(StadiumBorder()),
        ),
        child: child,
      ),
      OmvilaButonTuru.hayalet => TextButton(
        onPressed: pressed,
        style: style,
        child: child,
      ),
    };
    return Semantics(
      liveRegion: yukleniyor,
      child: IgnorePointer(ignoring: yukleniyor, child: button),
    );
  }
}
