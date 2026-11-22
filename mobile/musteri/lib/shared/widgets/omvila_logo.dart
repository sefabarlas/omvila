import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// OMVILA amblemi ve kelime markası (components/Logo). [renk] kelime
/// markasının rengi; amblem kendi renkleriyle çizilir.
class OmvilaLogo extends StatelessWidget {
  /// Logo; [dikey] ise amblem üstte (Açılış), değilse yan yana (Giriş).
  const OmvilaLogo({
    required this.renk,
    this.amblem = 32,
    this.dikey = false,
    this.ters = false,
    super.key,
  });

  /// Kelime markası rengi.
  final Color renk;

  /// Amblem boyu.
  final double amblem;

  /// Dikey yerleşim.
  final bool dikey;

  /// Koyu zemin üzerinde (amblem-ters; components/Logo tone="ters").
  final bool ters;

  @override
  Widget build(BuildContext context) {
    final icon = SvgPicture.asset(
      ters ? 'assets/omvila-amblem-ters.svg' : 'assets/omvila-amblem.svg',
      width: amblem,
      height: amblem,
      excludeFromSemantics: true,
    );
    final word = Text(
      'OMVILA',
      style: (dikey ? OmvilaYazi.baslik1 : OmvilaYazi.etiket).copyWith(
        color: renk,
        fontWeight: FontWeight.w800,
        letterSpacing: dikey ? 0.5 : 3,
      ),
    );
    return Semantics(
      label: 'OMVILA',
      excludeSemantics: true,
      child: dikey
          ? Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                icon,
                const SizedBox(height: OmvilaBosluk.bosluk3),
                word,
              ],
            )
          : Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                icon,
                const SizedBox(width: OmvilaBosluk.bosluk3),
                word,
              ],
            ),
    );
  }
}

/// Giriş ve açılıştaki yuvarlak köşeli dekor karesi.
class DekorKare extends StatelessWidget {
  /// Kare; [aci] radyan.
  const DekorKare({
    required this.renk,
    required this.boyut,
    this.aci = 0.2,
    super.key,
  });

  /// Renk (token).
  final Color renk;

  /// Kenar.
  final double boyut;

  /// Dönüş.
  final double aci;

  @override
  Widget build(BuildContext context) => ExcludeSemantics(
    child: Transform.rotate(
      angle: aci,
      child: Container(
        width: boyut,
        height: boyut,
        decoration: BoxDecoration(
          color: renk,
          borderRadius: BorderRadius.circular(boyut * 0.28),
        ),
      ),
    ),
  );
}
