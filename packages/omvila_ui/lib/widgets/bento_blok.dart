import 'package:flutter/material.dart';
import 'package:omvila_ui/format.dart';
import 'package:omvila_ui/theme/omvila_theme.dart';

/// Bento blok tonu (design/tasarim-sistemleri/mobil/README.md § Renk). Bir
/// ekranda her tondan en fazla bir blok olur.
enum BentoTonu {
  /// En önemli tek bilgi (sıradaki randevu).
  marka,

  /// Dikkat isteyen sayı (onay bekleyen).
  vurgu,

  /// Sakin bilgi (sıra, paket).
  kum,
}

/// Ana ekran özet bloğu: üst etiket, büyük değer ([OmvilaYazi.metrik]),
/// isteğe bağlı başlık, açıklama ve alt içerik. [onTap] verilirse ok
/// gösterilir.
class BentoBlok extends StatelessWidget {
  /// Bento blok.
  const BentoBlok({
    required this.ton,
    required this.ustEtiket,
    required this.deger,
    this.baslik,
    this.aciklama,
    this.alt,
    this.onTap,
    super.key,
  });

  /// Renk tonu.
  final BentoTonu ton;

  /// Üst etiket; Türkçe büyük harfle yazılır ("Sıradaki" → "SIRADAKİ").
  final String ustEtiket;

  /// Büyük değer ("13:00", "2").
  final String deger;

  /// Değerin altındaki güçlü satır ("Derya Şahin").
  final String? baslik;

  /// Açıklama ("Saç kesimi · 45 dk").
  final String? aciklama;

  /// Bloğun altındaki ek içerik (ör. "50 dk sonra" hapı).
  final Widget? alt;

  /// Dokununca ilgili ekrana gider.
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final (zemin, metin) = switch (ton) {
      BentoTonu.marka => (r.blokMarka, r.metinTers),
      BentoTonu.vurgu => (r.blokVurgu, r.blokVurguMetin),
      BentoTonu.kum => (r.blokKum, r.blokKumMetin),
    };
    final radius = BorderRadius.circular(OmvilaKose.koseXl);
    final title = baslik;
    final description = aciklama;
    final bottom = alt;
    return Material(
      color: zemin,
      borderRadius: radius,
      child: InkWell(
        onTap: onTap,
        borderRadius: radius,
        child: Padding(
          padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(
                toUpperTr(ustEtiket),
                style: OmvilaYazi.aciklama.copyWith(
                  color: metin,
                  fontWeight: FontWeight.w700,
                  letterSpacing: 0.6,
                ),
              ),
              const SizedBox(height: OmvilaBosluk.bosluk2),
              Row(
                crossAxisAlignment: CrossAxisAlignment.end,
                children: [
                  Expanded(
                    child: Text(
                      deger,
                      style: OmvilaYazi.metrik.copyWith(color: metin),
                    ),
                  ),
                  if (onTap != null)
                    Icon(Icons.chevron_right, size: 20, color: metin),
                ],
              ),
              if (title != null) ...[
                const SizedBox(height: OmvilaBosluk.bosluk1),
                Text(title, style: OmvilaYazi.baslik3.copyWith(color: metin)),
              ],
              if (description != null)
                Text(
                  description,
                  style: OmvilaYazi.ikincil.copyWith(color: metin),
                ),
              if (bottom != null) ...[
                const SizedBox(height: OmvilaBosluk.bosluk4),
                bottom,
              ],
            ],
          ),
        ),
      ),
    );
  }
}
