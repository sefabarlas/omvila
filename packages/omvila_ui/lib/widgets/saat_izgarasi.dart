import 'package:flutter/material.dart';
import 'package:omvila_ui/theme/omvila_theme.dart';

/// Izgaradaki bir saat.
@immutable
class SaatSecenegi {
  /// [etiket] yerel saat ("14:30"); [dolu] ise üstü çizili ve devre dışı.
  const SaatSecenegi({required this.etiket, this.dolu = false});

  /// "14:30".
  final String etiket;

  /// Dolu.
  final bool dolu;
}

/// Saat ızgarası (components/TimeSlotGrid): 4 kolon; dolu saatler üstü çizili
/// ve devre dışı; seçili saat koyu dolgu. Yükseklik `OmvilaBoyut.saatDilimi`
/// (büyük yazıda büyür).
class SaatIzgarasi extends StatelessWidget {
  /// Izgara.
  const SaatIzgarasi({
    required this.saatler,
    required this.secili,
    required this.onSecildi,
    super.key,
  });

  /// Saatler, artan.
  final List<SaatSecenegi> saatler;

  /// Seçili saat etiketi.
  final String? secili;

  /// Saat seçildi.
  final ValueChanged<String> onSecildi;

  static const _kolon = 4;

  @override
  Widget build(BuildContext context) {
    const gap = OmvilaBosluk.bosluk2;
    return Column(
      spacing: gap,
      children: [
        for (var row = 0; row < saatler.length; row += _kolon)
          Row(
            spacing: gap,
            children: [
              for (var i = row; i < row + _kolon; i++)
                Expanded(
                  child: i < saatler.length
                      ? _Saat(
                          saat: saatler[i],
                          secili: saatler[i].etiket == secili,
                          onTap: () => onSecildi(saatler[i].etiket),
                        )
                      : const SizedBox.shrink(),
                ),
            ],
          ),
      ],
    );
  }
}

class _Saat extends StatelessWidget {
  const _Saat({required this.saat, required this.secili, required this.onTap});

  final SaatSecenegi saat;
  final bool secili;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final radius = BorderRadius.circular(OmvilaKose.koseSm);
    final (zemin, kenar, metin) = saat.dolu
        ? (r.zeminDevreDisi, r.zeminDevreDisi, r.metinDevreDisi)
        : secili
        ? (r.zeminMarka, r.zeminMarka, r.metinTers)
        : (r.zeminYukseltilmis, r.cizgi, r.metin);
    return Semantics(
      button: true,
      selected: secili,
      enabled: !saat.dolu,
      label: saat.dolu ? '${saat.etiket}, dolu' : saat.etiket,
      excludeSemantics: true,
      child: Material(
        color: zemin,
        shape: RoundedRectangleBorder(
          borderRadius: radius,
          side: BorderSide(color: kenar),
        ),
        child: InkWell(
          borderRadius: radius,
          onTap: saat.dolu ? null : onTap,
          child: ConstrainedBox(
            constraints: const BoxConstraints(
              minHeight: OmvilaBoyut.saatDilimi,
            ),
            child: Center(
              child: Text(
                saat.etiket,
                style: OmvilaYazi.govdeGuclu.copyWith(
                  color: metin,
                  decoration: saat.dolu ? TextDecoration.lineThrough : null,
                  decorationColor: metin,
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}
