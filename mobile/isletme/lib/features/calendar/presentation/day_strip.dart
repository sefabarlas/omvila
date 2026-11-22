import 'package:flutter/material.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Gün şeridinin yüksekliği.
const double dayStripHeight = 76;

/// Gün şeridi: "Sal 25" çipleri; randevulu günde nokta, kapalı günde
/// "Kapalı". Seçili gün görünür alana kaydırılır.
class DayStrip extends StatefulWidget {
  /// Şerit.
  const DayStrip({
    required this.start,
    required this.count,
    required this.selected,
    required this.view,
    required this.onSelect,
    super.key,
  });

  /// İlk gün.
  final String start;

  /// Gün sayısı.
  final int count;

  /// Seçili gün.
  final String selected;

  /// Pencerenin takvimi (yüklenirken null).
  final CalendarView? view;

  /// Gün seçildi.
  final ValueChanged<String> onSelect;

  @override
  State<DayStrip> createState() => _DayStripState();
}

class _DayStripState extends State<DayStrip> {
  static const _width = 56.0;
  static const double _gap = OmvilaBosluk.bosluk2;
  final _scroll = ScrollController();

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) => _reveal());
  }

  @override
  void didUpdateWidget(DayStrip old) {
    super.didUpdateWidget(old);
    if (old.selected != widget.selected || old.start != widget.start) {
      WidgetsBinding.instance.addPostFrameCallback((_) => _reveal());
    }
  }

  void _reveal() {
    if (!_scroll.hasClients) {
      return;
    }

    final index = daysBetween(widget.start, widget.selected);
    final target = ((index - 2) * (_width + _gap)).clamp(
      0.0,
      _scroll.position.maxScrollExtent,
    );
    _scroll.jumpTo(target);
  }

  @override
  void dispose() {
    _scroll.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final view = widget.view;
    final busy = <String>{
      if (view != null)
        for (final a in view.appointments)
          if (a.status != AppointmentStatus.cancelled)
            toBranchDate(a.startUtc, view.timeZone),
    };
    final closed = <String>{
      if (view != null)
        for (final d in view.days)
          if (d.isClosed) localDateOf(d.date),
    };
    return SizedBox(
      height: dayStripHeight,
      child: ListView.separated(
        controller: _scroll,
        scrollDirection: Axis.horizontal,
        padding: const EdgeInsets.symmetric(horizontal: OmvilaBosluk.bosluk4),
        itemCount: widget.count,
        separatorBuilder: (_, _) => const SizedBox(width: _gap),
        itemBuilder: (context, i) {
          final date = addDays(widget.start, i);
          return _DayChip(
            date: date,
            selected: date == widget.selected,
            closed: closed.contains(date),
            busy: busy.contains(date),
            onTap: () => widget.onSelect(date),
          );
        },
      ),
    );
  }
}

class _DayChip extends StatelessWidget {
  const _DayChip({
    required this.date,
    required this.selected,
    required this.closed,
    required this.busy,
    required this.onTap,
  });

  final String date;
  final bool selected;
  final bool closed;
  final bool busy;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final foreground = selected
        ? r.metinTers
        : (closed ? r.metinDevreDisi : r.metin);
    final background = selected
        ? r.zeminMarka
        : (closed ? r.zeminIkincil : r.zeminYukseltilmis);
    final radius = BorderRadius.circular(OmvilaKose.koseMd);
    return Semantics(
      button: true,
      selected: selected,
      label:
          '${formatDate(date, DateStyle.weekdayDayMonth)}'
          '${closed ? ', kapalı' : ''}',
      excludeSemantics: true,
      child: Material(
        color: background,
        shape: RoundedRectangleBorder(
          borderRadius: radius,
          side: BorderSide(
            color: selected || closed ? background : r.cizgi,
          ),
        ),
        child: InkWell(
          onTap: onTap,
          borderRadius: radius,
          child: SizedBox(
            width: 56,
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Text(
                  formatDate(date, DateStyle.weekdayShort),
                  style: OmvilaYazi.aciklama.copyWith(color: foreground),
                ),
                Text(
                  formatDate(date, DateStyle.day),
                  style: OmvilaYazi.baslik3.copyWith(
                    color: foreground,
                    fontWeight: FontWeight.w800,
                  ),
                ),
                if (closed)
                  Text(
                    'Kapalı',
                    style: OmvilaYazi.aciklama.copyWith(
                      color: foreground,
                      fontSize: 10,
                    ),
                  )
                else
                  Container(
                    width: 5,
                    height: 5,
                    margin: const EdgeInsets.only(top: 3),
                    decoration: BoxDecoration(
                      color: busy
                          ? (selected ? r.metinTers : r.marka)
                          : Colors.transparent,
                      shape: BoxShape.circle,
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

/// Taralı zemin (kapalı gün, mola, bekleme dilimi).
class HatchedBox extends StatelessWidget {
  /// Taralı kutu.
  const HatchedBox({required this.child, this.radius, super.key});

  /// İçerik.
  final Widget child;

  /// Köşe.
  final double? radius;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final corner = BorderRadius.circular(radius ?? OmvilaKose.koseXl);
    return ClipRRect(
      borderRadius: corner,
      child: CustomPaint(
        painter: _HatchPainter(background: r.zeminIkincil, line: r.cizgi),
        child: child,
      ),
    );
  }
}

class _HatchPainter extends CustomPainter {
  const _HatchPainter({required this.background, required this.line});

  final Color background;
  final Color line;

  @override
  void paint(Canvas canvas, Size size) {
    canvas.drawRect(Offset.zero & size, Paint()..color = background);
    final paint = Paint()
      ..color = line
      ..strokeWidth = 4;
    for (var x = -size.height; x < size.width; x += 14) {
      canvas.drawLine(
        Offset(x, size.height),
        Offset(x + size.height, 0),
        paint,
      );
    }
  }

  @override
  bool shouldRepaint(_HatchPainter old) =>
      old.background != background || old.line != line;
}
