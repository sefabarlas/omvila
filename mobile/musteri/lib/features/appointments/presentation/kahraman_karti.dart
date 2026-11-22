import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/features/appointments/domain/randevu_metinleri.dart';
import 'package:omvila_musteri/shared/external_actions.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Sıradaki randevunun koyu kartı (Main.png): gün ve durum, kalan süre,
/// saat, salon, hizmet/personel/şube; "Yol tarifi" ve "Takvime ekle".
class KahramanKarti extends ConsumerWidget {
  /// Kart.
  const KahramanKarti({
    required this.randevu,
    required this.clock,
    this.onTap,
    super.key,
  });

  /// Randevu detayına gider.
  final VoidCallback? onTap;

  /// Sıradaki randevu.
  final MeAppointmentSummary randevu;

  /// Enjekte saat.
  final Clock clock;

  Future<void> _directions(BuildContext context, WidgetRef ref) async {
    final messenger = ScaffoldMessenger.of(context);
    final platform = Theme.of(context).platform;
    try {
      final detail = await ref
          .read(appointmentsRepositoryProvider)
          .appointment(randevu.id);
      final branch = detail.branch;
      final opened = await ref
          .read(externalActionsProvider)
          .openUrl(
            directionsUrl(
              address: ozetSatiri([
                branch.addressLine,
                branch.district,
              ]).replaceAll(' · ', ', '),
              latitude: branch.latitude,
              longitude: branch.longitude,
              platform: platform,
            ),
          );
      if (!opened) {
        messenger.showSnackBar(
          const SnackBar(content: Text('Harita uygulaması açılamadı.')),
        );
      }
    } on ApiProblem catch (e) {
      messenger.showSnackBar(SnackBar(content: Text(e.message)));
    }
  }

  Future<void> _calendar(WidgetRef ref) => ref
      .read(externalActionsProvider)
      .addToCalendar(
        CalendarEntry(
          title: '${randevu.brand.name} randevusu',
          startUtc: randevu.startUtc,
          endUtc: randevu.endUtc,
          timeZone: randevu.timeZone,
          description: ozetSatiri([
            hizmetlerMetni(randevu.items),
            personelMetni(randevu.items),
          ]),
          location: ozetSatiri([randevu.brand.name, randevu.branch.name]),
        ),
      );

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final a = randevu;
    final status = RandevuDurumu.fromDisplayStatus(a.displayStatus);
    final top = ozetSatiri([
      gunEtiketi(a.startUtc, a.timeZone, clock),
      status?.etiket ?? '',
    ]);
    final remaining = kalanSureEtiketi(a.startUtc, clock);
    final onCard = r.metinTers;
    final radius = BorderRadius.circular(OmvilaKose.koseXl);
    return Material(
      color: r.blokMarka,
      borderRadius: radius,
      child: InkWell(
        borderRadius: radius,
        onTap: onTap,
        child: Padding(
          padding: const EdgeInsets.all(OmvilaBosluk.bosluk5),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Expanded(
                    child: Text(
                      toUpperTr(top),
                      style: OmvilaYazi.aciklama.copyWith(
                        color: onCard,
                        fontWeight: FontWeight.w700,
                        letterSpacing: 0.6,
                      ),
                    ),
                  ),
                  if (remaining != null)
                    Text(
                      remaining,
                      style: OmvilaYazi.aciklama.copyWith(
                        color: onCard.withValues(alpha: 0.85),
                      ),
                    ),
                ],
              ),
              const SizedBox(height: OmvilaBosluk.bosluk2),
              Text(
                formatTime(a.startUtc, a.timeZone),
                style: OmvilaYazi.metrik.copyWith(color: onCard, fontSize: 48),
              ),
              Text(
                a.brand.name,
                style: OmvilaYazi.baslik3.copyWith(color: onCard),
              ),
              Text(
                ozetSatiri([
                  hizmetlerMetni(a.items),
                  personelMetni(a.items),
                  a.branch.district,
                ]),
                style: OmvilaYazi.ikincil.copyWith(
                  color: onCard.withValues(alpha: 0.85),
                ),
              ),
              const SizedBox(height: OmvilaBosluk.bosluk4),
              Row(
                children: [
                  Expanded(
                    child: _KartButonu(
                      etiket: 'Yol tarifi',
                      ikon: LucideIcons.mapPin,
                      zemin: onCard,
                      metin: r.blokMarka,
                      onPressed: () => _directions(context, ref),
                    ),
                  ),
                  const SizedBox(width: OmvilaBosluk.bosluk2),
                  Expanded(
                    child: _KartButonu(
                      etiket: 'Takvime ekle',
                      ikon: LucideIcons.calendarPlus,
                      zemin: onCard.withValues(alpha: 0.14),
                      metin: onCard,
                      onPressed: () => _calendar(ref),
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _KartButonu extends StatelessWidget {
  const _KartButonu({
    required this.etiket,
    required this.ikon,
    required this.zemin,
    required this.metin,
    required this.onPressed,
  });

  final String etiket;
  final IconData ikon;
  final Color zemin;
  final Color metin;
  final AsyncCallback onPressed;

  @override
  Widget build(BuildContext context) => FilledButton.icon(
    onPressed: onPressed,
    icon: Icon(ikon, size: 20),
    label: Text(etiket, textAlign: TextAlign.center),
    style: FilledButton.styleFrom(
      backgroundColor: zemin,
      foregroundColor: metin,
      minimumSize: const Size.fromHeight(OmvilaBoyut.kontrolMd),
      padding: const EdgeInsets.symmetric(horizontal: OmvilaBosluk.bosluk3),
      textStyle: OmvilaYazi.etiket,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(OmvilaKose.koseMd),
      ),
    ),
  );
}
