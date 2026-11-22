import 'package:flutter/material.dart';
import 'package:omvila_ui/theme/omvila_theme.dart';

/// Segment kontrolündeki bir seçenek.
@immutable
class OmvilaSegmentOgesi<T> {
  /// Seçenek.
  const OmvilaSegmentOgesi(this.deger, this.etiket);

  /// Değer.
  final T deger;

  /// Etiket (kısa; 1.3× yazıda sığmazsa küçülür, kesilmez).
  final String etiket;
}

/// Segment kontrolü (components/SegmentedControl): `zemin-ikincil` hap
/// içinde eşit genişlikte seçenekler; seçili olan `zemin-yukseltilmis`.
/// Etiketlerin iki yanında boşluk vardır, büyük yazıda birbirine değmez.
class OmvilaSegment<T> extends StatelessWidget {
  /// Segment kontrolü.
  const OmvilaSegment({
    required this.ogeler,
    required this.secili,
    required this.onSecildi,
    super.key,
  });

  /// Seçenekler (2–3).
  final List<OmvilaSegmentOgesi<T>> ogeler;

  /// Seçili değer.
  final T secili;

  /// Seçim değişti.
  final ValueChanged<T> onSecildi;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final radius = BorderRadius.circular(OmvilaKose.koseTam);
    return Container(
      padding: const EdgeInsets.all(OmvilaBosluk.bosluk1),
      decoration: BoxDecoration(color: r.zeminIkincil, borderRadius: radius),
      child: Row(
        children: [
          for (final oge in ogeler)
            Expanded(
              child: Semantics(
                selected: oge.deger == secili,
                button: true,
                child: Material(
                  color: oge.deger == secili
                      ? r.zeminYukseltilmis
                      : Colors.transparent,
                  borderRadius: radius,
                  child: InkWell(
                    borderRadius: radius,
                    onTap: () => onSecildi(oge.deger),
                    child: ConstrainedBox(
                      constraints: const BoxConstraints(
                        minHeight: OmvilaBoyut.kontrolSm,
                      ),
                      child: Padding(
                        padding: const EdgeInsets.symmetric(
                          horizontal: OmvilaBosluk.bosluk2,
                        ),
                        child: Center(
                          child: FittedBox(
                            fit: BoxFit.scaleDown,
                            child: Text(
                              oge.etiket,
                              maxLines: 1,
                              style: OmvilaYazi.etiket.copyWith(
                                color: oge.deger == secili
                                    ? r.metin
                                    : r.metinIkincil,
                              ),
                            ),
                          ),
                        ),
                      ),
                    ),
                  ),
                ),
              ),
            ),
        ],
      ),
    );
  }
}
