import 'dart:math' as math;

import 'package:flutter/material.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/features/calendar/presentation/day_strip.dart';
import 'package:omvila_isletme/shared/api_date.dart';
import 'package:omvila_isletme/shared/ui.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Gün görünümü: solda saatler, her personel bir kolon; blok = randevu
/// kalemi. `staffBusy = false` dilim taralı "Bekleme · müsait" şerididir.
/// Mola ve izinler taralı gösterilir; bugünse "şimdi" çizgisi.
class CalendarGrid extends StatelessWidget {
  /// Izgara.
  const CalendarGrid({
    required this.view,
    required this.day,
    required this.staff,
    required this.blocks,
    required this.onOpen,
    this.now,
    super.key,
  });

  /// Takvim penceresi.
  final CalendarView view;

  /// Gösterilen gün.
  final CalendarViewDaysInner day;

  /// Kolonlar.
  final List<CalendarViewStaffInner> staff;

  /// Günün blokları (iptaller hariç).
  final List<CalendarAppointment> blocks;

  /// Bloğa dokununca randevu kimliği.
  final ValueChanged<String> onOpen;

  /// Bugünse şimdiki an.
  final DateTime? now;

  static const _hourHeight = 56.0;
  static const _timeColumn = 44.0;
  static const _minColumn = 104.0;

  int _minutes(DateTime t) => localMinutesOfDay(t, view.timeZone);

  /// Izgara aralığı: çalışma saatleri ve bloklar, saat başına yuvarlanır.
  (int, int) _range() {
    final starts = <int>[
      for (final i in day.branchIntervals ?? const <TimeInterval>[])
        parseClock(i.start),
      for (final s in day.staffHours)
        for (final i in s.intervals) parseClock(i.start),
      for (final b in blocks) _minutes(b.startUtc),
    ];
    final ends = <int>[
      for (final i in day.branchIntervals ?? const <TimeInterval>[])
        parseClock(i.end),
      for (final s in day.staffHours)
        for (final i in s.intervals) parseClock(i.end),
      for (final b in blocks) _minutes(b.endUtc),
    ];
    final start = starts.isEmpty ? 9 * 60 : starts.reduce(math.min);
    final end = ends.isEmpty ? 18 * 60 : ends.reduce(math.max);
    return ((start ~/ 60) * 60, math.min(24 * 60, ((end + 59) ~/ 60) * 60));
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final (start, end) = _range();
    final height = (end - start) / 60 * _hourHeight;
    final date = localDateOf(day.date);
    return LayoutBuilder(
      builder: (context, constraints) {
        final available =
            constraints.maxWidth - _timeColumn - OmvilaBosluk.bosluk4 * 2;
        final column = math.max(
          _minColumn,
          available / math.max(1, staff.length),
        );
        final width = column * staff.length;
        double y(DateTime t) => (_minutes(t) - start) / 60 * _hourHeight;

        return SingleChildScrollView(
          scrollDirection: Axis.horizontal,
          padding: const EdgeInsets.symmetric(
            horizontal: OmvilaBosluk.bosluk4,
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Padding(
                padding: const EdgeInsets.only(left: _timeColumn),
                child: Row(
                  children: [
                    for (final s in staff)
                      SizedBox(
                        width: column,
                        child: Padding(
                          padding: const EdgeInsets.symmetric(horizontal: 3),
                          child: _StaffHeader(staff: s),
                        ),
                      ),
                  ],
                ),
              ),
              const SizedBox(height: OmvilaBosluk.bosluk3),
              SizedBox(
                width: _timeColumn + width,
                height: height + 8,
                child: Stack(
                  clipBehavior: Clip.none,
                  children: [
                    for (var m = start; m <= end; m += 60) ...[
                      Positioned(
                        left: 0,
                        top: (m - start) / 60 * _hourHeight - 7,
                        child: Text(
                          formatClock(m),
                          style: OmvilaYazi.aciklama.copyWith(
                            color: r.metinIkincil,
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                      ),
                      Positioned(
                        left: _timeColumn,
                        right: 0,
                        top: (m - start) / 60 * _hourHeight,
                        child: Divider(height: 1, color: r.cizgi),
                      ),
                    ],
                    for (var c = 0; c < staff.length; c++) ...[
                      for (final off in view.timeOffs)
                        if (off.staffId == staff[c].staffId &&
                            !off.isAllDay &&
                            toBranchDate(off.startUtc, view.timeZone) == date)
                          Positioned(
                            left: _timeColumn + c * column + 3,
                            width: column - 6,
                            top: y(off.startUtc),
                            height: math.max(
                              20,
                              y(off.endUtc) - y(off.startUtc),
                            ),
                            child: HatchedBox(
                              radius: OmvilaKose.koseXs,
                              child: Padding(
                                padding: const EdgeInsets.all(4),
                                child: Text(
                                  timeOffLabel(off.type),
                                  style: OmvilaYazi.aciklama.copyWith(
                                    color: r.metinIkincil,
                                    fontWeight: FontWeight.w700,
                                  ),
                                ),
                              ),
                            ),
                          ),
                      for (final b in blocks)
                        if (b.staffId == staff[c].staffId)
                          Positioned(
                            left: _timeColumn + c * column + 3,
                            width: column - 6,
                            top: y(b.startUtc),
                            height: math.max(24, y(b.endUtc) - y(b.startUtc)),
                            child: _Block(
                              block: b,
                              color: staffColor(
                                context,
                                staff[c].calendarColor,
                              ),
                              timeZone: view.timeZone,
                              pxPerMinute: _hourHeight / 60,
                              onTap: () => onOpen(b.appointmentId),
                            ),
                          ),
                    ],
                    if (now != null &&
                        _minutes(now!) >= start &&
                        _minutes(now!) <= end)
                      Positioned(
                        left: _timeColumn - 4,
                        right: 0,
                        top: y(now!) - 4,
                        child: ExcludeSemantics(
                          child: Row(
                            children: [
                              Container(
                                width: 9,
                                height: 9,
                                decoration: BoxDecoration(
                                  color: r.hata,
                                  shape: BoxShape.circle,
                                ),
                              ),
                              Expanded(
                                child: Container(height: 2, color: r.hata),
                              ),
                            ],
                          ),
                        ),
                      ),
                  ],
                ),
              ),
            ],
          ),
        );
      },
    );
  }
}

/// İzin türü adı (sağlık verisi yok: "Rahatsız" yerine "İzin").
String timeOffLabel(TimeOffTypeEnum type) => switch (type) {
  TimeOffTypeEnum.break_ => 'Mola',
  TimeOffTypeEnum.training => 'Eğitim',
  _ => 'İzin',
};

class _StaffHeader extends StatelessWidget {
  const _StaffHeader({required this.staff});

  final CalendarViewStaffInner staff;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return Container(
      height: OmvilaBoyut.kontrolSm,
      padding: const EdgeInsets.symmetric(horizontal: OmvilaBosluk.bosluk2),
      decoration: BoxDecoration(
        color: r.zeminYukseltilmis,
        borderRadius: BorderRadius.circular(OmvilaKose.koseSm),
        boxShadow: context.omvilaGolgeler.golgeSm,
      ),
      child: Row(
        children: [
          Container(
            width: 8,
            height: 8,
            decoration: BoxDecoration(
              color: staffColor(context, staff.calendarColor),
              shape: BoxShape.circle,
            ),
          ),
          const SizedBox(width: OmvilaBosluk.bosluk2),
          Expanded(
            child: Text(
              staff.fullName.split(' ').first,
              overflow: TextOverflow.ellipsis,
              style: OmvilaYazi.etiket.copyWith(color: r.metin),
            ),
          ),
        ],
      ),
    );
  }
}

class _Block extends StatelessWidget {
  const _Block({
    required this.block,
    required this.color,
    required this.timeZone,
    required this.pxPerMinute,
    required this.onTap,
  });

  final CalendarAppointment block;
  final Color color;
  final String timeZone;
  final double pxPerMinute;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final status = RandevuDurumu.fromDisplayStatus(block.displayStatus);
    final (background, accent) = switch (status) {
      RandevuDurumu.onayBekliyor => (r.durumOnayZemin, r.durumOnayNokta),
      RandevuDurumu.kaporaBekleniyor => (
        r.durumKaporaZemin,
        r.durumKaporaNokta,
      ),
      RandevuDurumu.geldi => (r.durumGeldiZemin, color),
      RandevuDurumu.tamamlandi => (r.durumTamamlandiZemin, color),
      RandevuDurumu.gelmedi => (r.durumGelmediZemin, r.durumGelmediNokta),
      _ => (
        Color.alphaBlend(color.withValues(alpha: 0.14), r.zeminYukseltilmis),
        color,
      ),
    };
    final pending =
        status == RandevuDurumu.onayBekliyor ||
        status == RandevuDurumu.kaporaBekleniyor;
    final minutes = block.endUtc.difference(block.startUtc).inMinutes;
    final height = minutes * pxPerMinute;
    final time = formatTime(block.startUtc, timeZone);
    final radius = BorderRadius.circular(OmvilaKose.koseXs + 2);
    final gaps = [
      for (final s in block.segments)
        if (!s.staffBusy) s,
    ];
    return Semantics(
      button: true,
      label:
          '$time ${block.customerName}, ${block.serviceName}'
          '${status == null ? '' : ', ${status.etiket}'}',
      excludeSemantics: true,
      child: Material(
        color: background,
        shape: RoundedRectangleBorder(
          borderRadius: radius,
          side: pending ? BorderSide(color: accent) : BorderSide.none,
        ),
        clipBehavior: Clip.antiAlias,
        child: InkWell(
          onTap: onTap,
          child: Stack(
            children: [
              Positioned(
                left: 0,
                top: 0,
                bottom: 0,
                child: Container(width: 3, color: accent),
              ),
              for (final g in gaps)
                Positioned(
                  left: 8,
                  right: 6,
                  top:
                      g.startUtc.difference(block.startUtc).inMinutes *
                      pxPerMinute,
                  height: math.max(
                    16,
                    g.endUtc.difference(g.startUtc).inMinutes * pxPerMinute,
                  ),
                  child: Container(
                    alignment: Alignment.center,
                    decoration: BoxDecoration(
                      color: r.zeminYukseltilmis,
                      borderRadius: BorderRadius.circular(OmvilaKose.koseXs),
                      border: Border.all(color: r.cizgiGuclu),
                    ),
                    child: Text(
                      'Bekleme · müsait',
                      textAlign: TextAlign.center,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                      style: OmvilaYazi.aciklama.copyWith(
                        color: r.metin,
                        fontSize: 11,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                  ),
                ),
              if (gaps.isEmpty)
                Padding(
                  padding: const EdgeInsets.fromLTRB(8, 3, 4, 2),
                  child: _BlockText(
                    time: time,
                    customer: block.customerName,
                    service: block.serviceName,
                    compact: height < 44,
                    showService: height >= 56,
                  ),
                )
              else
                Padding(
                  padding: const EdgeInsets.fromLTRB(8, 3, 4, 2),
                  child: _BlockText(
                    time: time,
                    customer: block.customerName,
                    service: block.serviceName,
                    compact: true,
                    showService: false,
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }
}

class _BlockText extends StatelessWidget {
  const _BlockText({
    required this.time,
    required this.customer,
    required this.service,
    required this.compact,
    required this.showService,
  });

  final String time;
  final String customer;
  final String service;
  final bool compact;
  final bool showService;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final strong = OmvilaYazi.aciklama.copyWith(
      color: r.metin,
      fontWeight: FontWeight.w800,
    );
    if (compact) {
      return Text(
        '$time ${customer.split(' ').first}',
        maxLines: 1,
        overflow: TextOverflow.ellipsis,
        style: strong,
      );
    }

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          time,
          style: OmvilaYazi.aciklama.copyWith(
            color: r.metin,
            fontWeight: FontWeight.w600,
          ),
        ),
        Text(
          customer,
          maxLines: 1,
          overflow: TextOverflow.ellipsis,
          style: strong,
        ),
        if (showService)
          Text(
            service,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
          ),
      ],
    );
  }
}
