import 'package:flutter/material.dart';
import 'package:omvila_ui/format.dart';
import 'package:omvila_ui/theme/omvila_theme.dart';

/// Gün şeridindeki bir gün.
@immutable
class GunSecenegi {
  /// [tarih] şubenin yerel günü ("2026-11-26").
  const GunSecenegi({
    required this.tarih,
    this.uygun = false,
    this.kapali = false,
  });

  /// Yerel gün.
  final String tarih;

  /// Boş saati var (nokta).
  final bool uygun;

  /// Şube kapalı (devre dışı, "Kapalı").
  final bool kapali;
}

/// Yatay gün şeridi (components/DayStrip): kısa gün adı, gün numarası; boş
/// saati olan günde nokta; kapalı gün devre dışı. Seçili gün marka dolgu.
class GunSeridi extends StatelessWidget {
  /// Şerit.
  const GunSeridi({
    required this.gunler,
    required this.secili,
    required this.onSecildi,
    super.key,
  });

  /// Günler (bugünden ileri).
  final List<GunSecenegi> gunler;

  /// Seçili yerel gün.
  final String? secili;

  /// Gün seçildi.
  final ValueChanged<String> onSecildi;

  @override
  Widget build(BuildContext context) {
    final scale = MediaQuery.textScalerOf(context).scale(1);
    return SizedBox(
      height: 72 * scale.clamp(1, 1.6),
      child: ListView.separated(
        scrollDirection: Axis.horizontal,
        itemCount: gunler.length,
        separatorBuilder: (_, _) => const SizedBox(width: OmvilaBosluk.bosluk2),
        itemBuilder: (context, i) => _Gun(
          gun: gunler[i],
          secili: gunler[i].tarih == secili,
          onTap: () => onSecildi(gunler[i].tarih),
        ),
      ),
    );
  }
}

class _Gun extends StatelessWidget {
  const _Gun({required this.gun, required this.secili, required this.onTap});

  final GunSecenegi gun;
  final bool secili;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final kapali = gun.kapali;
    final metin = secili
        ? r.markaUstu
        : kapali
        ? r.metinDevreDisi
        : r.metin;
    final radius = BorderRadius.circular(OmvilaKose.koseMd);
    final label = formatDate(gun.tarih, DateStyle.weekdayDayMonth);
    return Semantics(
      button: true,
      selected: secili,
      enabled: !kapali,
      label: kapali
          ? '$label, kapalı'
          : gun.uygun
          ? '$label, boş saat var'
          : label,
      excludeSemantics: true,
      child: Material(
        color: secili ? r.marka : r.zeminYukseltilmis,
        shape: RoundedRectangleBorder(
          borderRadius: radius,
          side: BorderSide(color: secili ? r.marka : r.cizgi),
        ),
        child: InkWell(
          borderRadius: radius,
          onTap: kapali ? null : onTap,
          child: SizedBox(
            width: 56,
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Text(
                  formatDate(gun.tarih, DateStyle.weekdayShort),
                  style: OmvilaYazi.aciklama.copyWith(color: metin),
                ),
                Text(
                  formatDate(gun.tarih, DateStyle.day),
                  style: OmvilaYazi.baslik3.copyWith(
                    color: metin,
                    fontWeight: FontWeight.w800,
                  ),
                ),
                if (kapali)
                  Text(
                    'Kapalı',
                    maxLines: 1,
                    style: OmvilaYazi.aciklama.copyWith(
                      color: metin,
                      fontSize: 10,
                    ),
                  )
                else
                  Container(
                    width: 5,
                    height: 5,
                    margin: const EdgeInsets.only(top: OmvilaBosluk.bosluk1),
                    decoration: BoxDecoration(
                      shape: BoxShape.circle,
                      color: gun.uygun
                          ? (secili ? r.markaUstu : r.marka)
                          : Colors.transparent,
                    ),
                  ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
