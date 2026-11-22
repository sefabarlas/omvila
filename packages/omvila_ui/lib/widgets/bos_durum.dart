import 'package:flutter/material.dart';
import 'package:omvila_ui/theme/omvila_theme.dart';
import 'package:omvila_ui/widgets/buton.dart';

/// Boş listenin davet ekranı (design/tasarim-sistemleri/mobil/README.md § Boş
/// durumlar): 80'lik ikon kutusu, tek satır başlık ("Henüz … yok"), ne zaman
/// dolacağını anlatan cümle, varsa tek eylem. Sahte sayı veya rozet yok.
class OmvilaBosDurum extends StatelessWidget {
  /// Boş durum.
  const OmvilaBosDurum({
    required this.ikon,
    required this.baslik,
    required this.aciklama,
    this.eylemEtiketi,
    this.eylemIkonu,
    this.onEylem,
    this.eylemTuru = OmvilaButonTuru.ikincil,
    super.key,
  });

  /// Kutudaki ikon (`lucide_icons`).
  final IconData ikon;

  /// Başlık.
  final String baslik;

  /// Açıklama cümlesi.
  final String aciklama;

  /// Eylem metni (fiille başlar); null ise buton yok.
  final String? eylemEtiketi;

  /// Eylem ikonu.
  final IconData? eylemIkonu;

  /// Eylem.
  final VoidCallback? onEylem;

  /// Eylemin varyantı: ekranın tek işi boş listeyi doldurmaksa
  /// (TakvimBos, SiraBos, MusterilerBos) birincil; "Tekrar dene" gibi
  /// yardımcı eylemde ikincil.
  final OmvilaButonTuru eylemTuru;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final label = eylemEtiketi;
    return Padding(
      padding: const EdgeInsets.symmetric(
        horizontal: OmvilaBosluk.bosluk6,
        vertical: OmvilaBosluk.bosluk12,
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          ExcludeSemantics(
            child: Container(
              width: 80,
              height: 80,
              decoration: BoxDecoration(
                color: r.zeminSecili,
                borderRadius: BorderRadius.circular(OmvilaKose.koseXl),
              ),
              child: Icon(ikon, size: 32, color: r.metinMarka),
            ),
          ),
          const SizedBox(height: OmvilaBosluk.bosluk6),
          Semantics(
            header: true,
            child: Text(
              baslik,
              textAlign: TextAlign.center,
              style: OmvilaYazi.baslik2.copyWith(color: r.metin),
            ),
          ),
          const SizedBox(height: OmvilaBosluk.bosluk2),
          Text(
            aciklama,
            textAlign: TextAlign.center,
            style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
          ),
          if (label != null) ...[
            const SizedBox(height: OmvilaBosluk.bosluk5),
            OmvilaButon(
              etiket: label,
              ikon: eylemIkonu,
              tur: eylemTuru,
              onPressed: onEylem,
            ),
          ],
        ],
      ),
    );
  }
}
