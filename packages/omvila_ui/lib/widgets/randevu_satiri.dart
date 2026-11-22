import 'package:flutter/material.dart';
import 'package:omvila_ui/theme/omvila_theme.dart';
import 'package:omvila_ui/widgets/durum_rozeti.dart';

/// Salon uygulamasında günün randevu listesi satırı
/// (design/tasarim-sistemleri/mobil/components/AppointmentListItem): solda
/// başlangıç ve bitiş saati, durum noktası; ortada müşteri, "hizmet ·
/// personel" ve durum rozeti; sağda isteğe bağlı eylem ("Onayla", "Geldi").
/// Kartlar arasında çizgi yok, 8 boşluk bırakılır (listenin işi).
class RandevuSatiri extends StatelessWidget {
  /// Randevu satırı.
  const RandevuSatiri({
    required this.baslangic,
    required this.bitis,
    required this.musteri,
    required this.ayrinti,
    this.durum,
    this.eylem,
    this.onTap,
    super.key,
  });

  /// Başlangıç saati ("12:00"), şubenin saat diliminde biçimlenmiş.
  final String baslangic;

  /// Bitiş saati ("12:45").
  final String bitis;

  /// Müşteri adı.
  final String musteri;

  /// "Saç kesimi · Can".
  final String ayrinti;

  /// Durum; null ise (bilinmeyen durum) nokta ve rozet gösterilmez.
  final RandevuDurumu? durum;

  /// Sağdaki eylem butonu.
  final Widget? eylem;

  /// Satıra dokununca randevu ayrıntısı.
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final status = durum;
    final action = eylem;
    final radius = BorderRadius.circular(OmvilaKose.koseLg);
    return DecoratedBox(
      decoration: BoxDecoration(
        borderRadius: radius,
        boxShadow: context.omvilaGolgeler.golgeSm,
      ),
      child: Material(
        color: r.zeminYukseltilmis,
        borderRadius: radius,
        child: InkWell(
          onTap: onTap,
          borderRadius: radius,
          child: Padding(
            padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
            child: Row(
              children: [
                Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      baslangic,
                      style: OmvilaYazi.rakam.copyWith(
                        color: r.metin,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                    Text(
                      bitis,
                      style: OmvilaYazi.aciklama.copyWith(
                        color: r.metinIkincil,
                      ),
                    ),
                  ],
                ),
                const SizedBox(width: OmvilaBosluk.bosluk3),
                ExcludeSemantics(
                  child: DecoratedBox(
                    decoration: BoxDecoration(
                      color: status == null ? r.cizgi : status.nokta(r),
                      shape: BoxShape.circle,
                      border: Border.all(color: r.zeminYukseltilmis, width: 2),
                    ),
                    child: const SizedBox.square(dimension: 12),
                  ),
                ),
                const SizedBox(width: OmvilaBosluk.bosluk3),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        musteri,
                        style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
                      ),
                      Text(
                        ayrinti,
                        style: OmvilaYazi.ikincil.copyWith(
                          color: r.metinIkincil,
                        ),
                      ),
                      if (status != null) ...[
                        const SizedBox(height: OmvilaBosluk.bosluk1),
                        OmvilaDurumRozeti(durum: status),
                      ],
                    ],
                  ),
                ),
                if (action != null) ...[
                  const SizedBox(width: OmvilaBosluk.bosluk2),
                  action,
                ],
              ],
            ),
          ),
        ),
      ),
    );
  }
}

/// Durum noktasının rengi (rozetteki noktayla aynı token).
extension RandevuDurumuNokta on RandevuDurumu {
  /// `durum-*-nokta` tokenı.
  Color nokta(OmvilaRenkler r) => switch (this) {
    RandevuDurumu.onayBekliyor => r.durumOnayNokta,
    RandevuDurumu.kaporaBekleniyor => r.durumKaporaNokta,
    RandevuDurumu.onaylandi => r.durumOnaylandiNokta,
    RandevuDurumu.geldi => r.durumGeldiNokta,
    RandevuDurumu.tamamlandi => r.durumTamamlandiNokta,
    RandevuDurumu.gelmedi => r.durumGelmediNokta,
    RandevuDurumu.iptal => r.durumIptalNokta,
  };
}
