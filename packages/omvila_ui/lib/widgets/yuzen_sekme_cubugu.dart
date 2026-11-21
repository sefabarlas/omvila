import 'package:flutter/material.dart';
import 'package:omvila_ui/format.dart';
import 'package:omvila_ui/motion.dart';
import 'package:omvila_ui/theme/omvila_theme.dart';

/// Sekme çubuğundaki bir sekme.
@immutable
class SekmeOgesi {
  /// Sekme.
  const SekmeOgesi({
    required this.ikon,
    required this.etiket,
    this.seciliIkon,
    this.rozet,
  });

  /// İkon (24, `lucide_icons`).
  final IconData ikon;

  /// Seçiliyken ikon (biraz daha kalın); verilmezse [ikon].
  final IconData? seciliIkon;

  /// Sekme adı; seçiliyken yazılır, her zaman ekran okuyucuya okunur.
  final String etiket;

  /// Okunmamış bildirim sayısı; null veya 0 ise rozet yok (sahte rozet yok).
  final int? rozet;
}

/// Yüzen alt sekme çubuğu (design/tasarim-sistemleri/mobil/README.md §
/// Yerleşim): kenarlardan 16, alttan 24 içeride koyu hap; seçili sekme açık
/// hap içinde ikon ve adıyla, diğerleri yalnız ikonla. Sekmeler arası geçişte
/// hareket yoktur; yalnız seçili hapın genişliği değişir.
class YuzenSekmeCubugu extends StatelessWidget {
  /// Sekme çubuğu.
  const YuzenSekmeCubugu({
    required this.ogeler,
    required this.seciliIndex,
    required this.onSecildi,
    super.key,
  });

  /// Sekmeler (işletme 5, müşteri 3).
  final List<SekmeOgesi> ogeler;

  /// Seçili sekme.
  final int seciliIndex;

  /// Sekmeye dokunuldu.
  final ValueChanged<int> onSecildi;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return SafeArea(
      top: false,
      minimum: const EdgeInsets.only(bottom: OmvilaBosluk.bosluk6),
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: OmvilaBosluk.bosluk4),
        child: Container(
          height: OmvilaBoyut.tabbar,
          padding: const EdgeInsets.all(OmvilaBosluk.bosluk2),
          decoration: BoxDecoration(
            color: r.tabbarZemin,
            borderRadius: BorderRadius.circular(OmvilaKose.koseXl),
            boxShadow: context.omvilaGolgeler.golgeMd,
          ),
          child: Row(
            children: [
              for (var i = 0; i < ogeler.length; i++)
                Expanded(
                  flex: i == seciliIndex ? 8 : 3,
                  child: _Sekme(
                    oge: ogeler[i],
                    secili: i == seciliIndex,
                    onTap: () => onSecildi(i),
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }
}

class _Sekme extends StatelessWidget {
  const _Sekme({
    required this.oge,
    required this.secili,
    required this.onTap,
  });

  final SekmeOgesi oge;
  final bool secili;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final color = secili ? r.tabbarAktif : r.tabbarPasif;
    final count = oge.rozet ?? 0;
    final radius = BorderRadius.circular(OmvilaKose.koseLg);
    Widget icon = Icon(
      secili ? (oge.seciliIkon ?? oge.ikon) : oge.ikon,
      size: 24,
      color: color,
    );
    if (count > 0) {
      icon = Badge(
        label: Text(formatCount(count)),
        backgroundColor: r.vurgu,
        textColor: r.vurguUstu,
        textStyle: OmvilaYazi.aciklama,
        child: icon,
      );
    }

    final badgeText = count > 0 ? ', ${formatCount(count)} okunmamış' : '';
    return Semantics(
      button: true,
      selected: secili,
      label: '${oge.etiket}$badgeText',
      excludeSemantics: true,
      child: AnimatedContainer(
        duration: motionDuration(context, OmvilaHareket.sureHizli),
        curve: OmvilaHareket.egriStandart,
        decoration: BoxDecoration(
          color: secili ? r.tabbarAktifZemin : r.tabbarZemin,
          borderRadius: radius,
        ),
        child: Material(
          type: MaterialType.transparency,
          child: InkWell(
            onTap: onTap,
            borderRadius: radius,
            child: Center(
              child: Padding(
                padding: const EdgeInsets.symmetric(
                  horizontal: OmvilaBosluk.bosluk2,
                ),
                child: Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    icon,
                    if (secili) ...[
                      const SizedBox(width: OmvilaBosluk.bosluk2),
                      // 1.3× yazıda metin kesilmez; hapa sığmazsa küçülür.
                      Flexible(
                        child: FittedBox(
                          fit: BoxFit.scaleDown,
                          child: Text(
                            oge.etiket,
                            maxLines: 1,
                            style: OmvilaYazi.etiketKucuk.copyWith(
                              color: color,
                            ),
                          ),
                        ),
                      ),
                    ],
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}
