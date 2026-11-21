import 'package:flutter/material.dart';
import 'package:omvila_ui/format.dart';
import 'package:omvila_ui/theme/omvila_theme.dart';
import 'package:omvila_ui/widgets/durum_rozeti.dart';

/// Müşteri uygulamasında randevu kartı (components/AppointmentTicket): salon,
/// hizmet ve personel, tarih/saat/süre, tutar, eylemler. Metinler biçimlenmiş
/// gelir (`format.dart`, `time.dart`); kart hesap yapmaz.
class RandevuKarti extends StatelessWidget {
  /// Randevu kartı.
  const RandevuKarti({
    required this.salonAdi,
    required this.hizmetVePersonel,
    required this.tarih,
    required this.saat,
    required this.sure,
    this.durum,
    this.tutar,
    this.not,
    this.eylemler = const [],
    this.onTap,
    super.key,
  });

  /// Salon adı; avatar baş harfleri buradan.
  final String salonAdi;

  /// "Manikür · Gizem".
  final String hizmetVePersonel;

  /// "3 Ara Çar".
  final String tarih;

  /// "11:00".
  final String saat;

  /// "40 dk".
  final String sure;

  /// Durum rozeti; bilinmeyen durumda null.
  final RandevuDurumu? durum;

  /// "350 ₺ salonda".
  final String? tutar;

  /// Eylem yerine bilgi ("Salon onaylayınca SMS ile bildireceğiz").
  final String? not;

  /// Alt eylemler ("İptal et", "Takvime ekle").
  final List<Widget> eylemler;

  /// Detaya gider.
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final radius = BorderRadius.circular(OmvilaKose.koseLg);
    final status = durum;
    final amount = tutar;
    final note = not;
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
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    ExcludeSemantics(
                      child: Container(
                        width: OmvilaBoyut.dokunmaMin,
                        height: OmvilaBoyut.dokunmaMin,
                        alignment: Alignment.center,
                        decoration: BoxDecoration(
                          color: r.blokMarka,
                          borderRadius: BorderRadius.circular(
                            OmvilaKose.koseSm,
                          ),
                        ),
                        child: Text(
                          initials(salonAdi),
                          style: OmvilaYazi.etiketKucuk.copyWith(
                            color: r.metinTers,
                          ),
                        ),
                      ),
                    ),
                    const SizedBox(width: OmvilaBosluk.bosluk3),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            salonAdi,
                            style: OmvilaYazi.baslik3.copyWith(color: r.metin),
                          ),
                          Text(
                            hizmetVePersonel,
                            style: OmvilaYazi.ikincil.copyWith(
                              color: r.metinIkincil,
                            ),
                          ),
                        ],
                      ),
                    ),
                    if (status != null) ...[
                      const SizedBox(width: OmvilaBosluk.bosluk2),
                      OmvilaDurumRozeti(durum: status),
                    ],
                  ],
                ),
                const SizedBox(height: OmvilaBosluk.bosluk3),
                DecoratedBox(
                  decoration: BoxDecoration(
                    color: r.zeminIkincil,
                    borderRadius: BorderRadius.circular(OmvilaKose.koseSm),
                  ),
                  child: Padding(
                    padding: const EdgeInsets.all(OmvilaBosluk.bosluk3),
                    child: Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Expanded(flex: 4, child: _Bilgi('Tarih', tarih)),
                        Expanded(flex: 3, child: _Bilgi('Saat', saat)),
                        Expanded(flex: 3, child: _Bilgi('Süre', sure)),
                      ],
                    ),
                  ),
                ),
                if (amount != null || note != null) ...[
                  const SizedBox(height: OmvilaBosluk.bosluk3),
                  Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      if (amount != null)
                        Text(
                          amount,
                          style: OmvilaYazi.govdeGuclu.copyWith(
                            color: r.metin,
                          ),
                        ),
                      if (amount != null && note != null)
                        const SizedBox(width: OmvilaBosluk.bosluk3),
                      if (note != null)
                        Expanded(
                          child: Text(
                            note,
                            textAlign: amount == null
                                ? TextAlign.start
                                : TextAlign.end,
                            style: OmvilaYazi.aciklama.copyWith(
                              color: r.metinIkincil,
                            ),
                          ),
                        ),
                    ],
                  ),
                ],
                if (eylemler.isNotEmpty) ...[
                  const SizedBox(height: OmvilaBosluk.bosluk3),
                  Wrap(
                    spacing: OmvilaBosluk.bosluk2,
                    runSpacing: OmvilaBosluk.bosluk2,
                    children: eylemler,
                  ),
                ],
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class _Bilgi extends StatelessWidget {
  const _Bilgi(this.etiket, this.deger);

  final String etiket;
  final String deger;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return MergeSemantics(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            etiket,
            style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
          ),
          Text(deger, style: OmvilaYazi.rakam.copyWith(color: r.metin)),
        ],
      ),
    );
  }
}
