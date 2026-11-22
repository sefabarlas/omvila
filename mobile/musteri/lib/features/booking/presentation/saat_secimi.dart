import 'package:flutter/material.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/features/booking/presentation/akis_sayfasi.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Gün şeridi ve saat ızgarası (YRSaat, RandevuErtele ortak gövdesi).
class SaatSecimi extends StatelessWidget {
  /// Gövde. [days] ve [slots] null ise yükleniyor.
  const SaatSecimi({
    required this.days,
    required this.date,
    required this.slots,
    required this.selectedTime,
    required this.onDate,
    required this.onSlot,
    this.slotsError,
    this.onRetry,
    super.key,
  });

  /// Gün şeridi sonucu.
  final DaySearchResult days;

  /// Seçili yerel gün.
  final String? date;

  /// Seçili günün saatleri; yükleniyorsa null.
  final SlotSearchResult? slots;

  /// Saatler okunamadı.
  final Object? slotsError;

  /// Saatleri yeniden oku.
  final VoidCallback? onRetry;

  /// Seçili saat ("14:30").
  final String? selectedTime;

  /// Gün seçildi.
  final ValueChanged<String> onDate;

  /// Saat seçildi.
  final ValueChanged<Slot> onSlot;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final result = slots;
    final error = slotsError;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        GunSeridi(
          gunler: [
            for (final d in days.days)
              GunSecenegi(
                tarih: localDateOf(d.date),
                uygun: d.hasAvailability,
                kapali: d.isClosed ?? false,
              ),
          ],
          secili: date,
          onSecildi: onDate,
        ),
        if (error != null)
          Padding(
            padding: const EdgeInsets.only(top: OmvilaBosluk.bosluk4),
            child: BilgiKutusu(
              ikon: LucideIcons.alertCircle,
              metin: error is ApiProblem ? error.message : unknownErrorMessage,
            ),
          )
        else if (result == null)
          Padding(
            padding: const EdgeInsets.all(OmvilaBosluk.bosluk8),
            child: Center(
              child: CircularProgressIndicator(color: r.marka),
            ),
          )
        else if (result.slots.isEmpty)
          Padding(
            padding: const EdgeInsets.only(top: OmvilaBosluk.bosluk4),
            child: _DoluGun(days: days, date: date, onDate: onDate),
          )
        else
          ..._groups(result),
      ],
    );
  }

  List<Widget> _groups(SlotSearchResult result) {
    final byTime = {for (final s in result.slots) s.localTime: s};
    final all = {
      ...byTime.keys,
      ...result.unavailableTimes,
    }.toList()..sort();
    const groups = [
      ('Sabah', 0, 12 * 60),
      ('Öğleden sonra', 12 * 60, 17 * 60),
      ('Akşam', 17 * 60, 24 * 60),
    ];
    return [
      for (final (label, from, to) in groups)
        if (all.where((t) => parseClock(t) >= from && parseClock(t) < to)
            case final times when times.isNotEmpty) ...[
          BolumBasligi(label),
          SaatIzgarasi(
            saatler: [
              for (final t in times)
                SaatSecenegi(etiket: t, dolu: !byTime.containsKey(t)),
            ],
            secili: selectedTime,
            onSecildi: (t) => onSlot(byTime[t]!),
          ),
        ],
    ];
  }
}

class _DoluGun extends StatelessWidget {
  const _DoluGun({
    required this.days,
    required this.date,
    required this.onDate,
  });

  final DaySearchResult days;
  final String? date;
  final ValueChanged<String> onDate;

  @override
  Widget build(BuildContext context) {
    final next = days.days
        .where(
          (d) =>
              d.hasAvailability &&
              (date == null || localDateOf(d.date).compareTo(date!) > 0),
        )
        .firstOrNull;
    final nextUtc = days.nextAvailableUtc;
    final String text;
    if (next != null) {
      text =
          'Bu gün dolu. En yakın boş gün: '
          '${formatDate(localDateOf(next.date), DateStyle.weekdayDayMonth)}';
    } else if (nextUtc != null) {
      final day = toBranchDate(nextUtc, days.timeZone);
      text =
          'Bu aralıkta boş saat yok. En yakın uygun saat '
          '${formatDate(day, DateStyle.weekdayDayMonth)} '
          '${formatTime(nextUtc, days.timeZone)}';
    } else {
      text = 'Bu gün dolu.';
    }

    final target = next == null ? null : localDateOf(next.date);
    return Material(
      color: Colors.transparent,
      child: InkWell(
        borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
        onTap: target == null ? null : () => onDate(target),
        child: BilgiKutusu(ikon: LucideIcons.calendarX, metin: text),
      ),
    );
  }
}
