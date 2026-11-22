import 'package:flutter/material.dart';
import 'package:omvila_ui/format.dart';
import 'package:omvila_ui/theme/omvila_theme.dart';

/// Avatar tonu (`avatar-1…3`); anlam taşımaz, addan türetilir.
enum OmvilaAvatarTonu {
  /// `avatar-1` (nane).
  bir,

  /// `avatar-2` (kum).
  iki,

  /// `avatar-3` (mercan).
  uc;

  /// Addan ton: aynı kişi her ekranda ve web'de aynı tonda görünür
  /// (packages/ui-web/src/panel/Avatar.tsx `tone` ile aynı karma).
  static OmvilaAvatarTonu adiIcin(String ad) {
    var hash = 0;
    for (final rune in ad.runes) {
      hash = (hash * 31 + rune) % 997;
    }
    return values[hash % 3];
  }

  /// Zemin ve baş harf rengi.
  (Color zemin, Color metin) renkler(OmvilaRenkler r) => switch (this) {
    bir => (r.avatar1Zemin, r.avatar1Metin),
    iki => (r.avatar2Zemin, r.avatar2Metin),
    uc => (r.avatar3Zemin, r.avatar3Metin),
  };
}

/// Kişinin baş harfleri (components/Avatar): Türkçe büyük harf, en çok iki;
/// ton addan türetilir veya [ton] ile verilir. Ad yanında yazıldığı için
/// ekran okuyucudan gizlenir.
class OmvilaAvatar extends StatelessWidget {
  /// Avatar.
  const OmvilaAvatar({
    required this.ad,
    this.boyut = 44,
    this.ton,
    this.ikon,
    this.yazi,
    super.key,
  });

  /// Kişinin veya salonun adı.
  final String ad;

  /// Çap (32, 44, 56).
  final double boyut;

  /// Verilmezse [OmvilaAvatarTonu.adiIcin].
  final OmvilaAvatarTonu? ton;

  /// Baş harf yerine ikon (ör. "Fark etmez").
  final IconData? ikon;

  /// Baş harf yazı stili; verilmezse `etiket`.
  final TextStyle? yazi;

  @override
  Widget build(BuildContext context) {
    final (zemin, metin) = (ton ?? OmvilaAvatarTonu.adiIcin(ad)).renkler(
      context.omvilaRenkler,
    );
    final icon = ikon;
    return ExcludeSemantics(
      child: Container(
        width: boyut,
        height: boyut,
        alignment: Alignment.center,
        decoration: BoxDecoration(color: zemin, shape: BoxShape.circle),
        child: icon != null
            ? Icon(icon, size: boyut / 2, color: metin)
            : Text(
                initials(ad),
                maxLines: 1,
                style: (yazi ?? OmvilaYazi.etiket).copyWith(color: metin),
              ),
      ),
    );
  }
}
