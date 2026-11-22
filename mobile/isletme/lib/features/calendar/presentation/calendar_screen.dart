import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_isletme/features/calendar/data/calendar_repository.dart';
import 'package:omvila_isletme/features/calendar/presentation/calendar_grid.dart';
import 'package:omvila_isletme/features/calendar/presentation/day_strip.dart';
import 'package:omvila_isletme/features/shell/presentation/app_shell.dart';
import 'package:omvila_isletme/features/shell/presentation/tabs.dart';
import 'package:omvila_isletme/shared/local_date.dart';
import 'package:omvila_isletme/shared/ui.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Şubenin saat dilimi takvim okunana kadar bilinmez; ilk gün varsayılan
/// dilimle seçilir, yanıt farklı dilim getirirse düzeltilir (AGENTS.md § 5).
const defaultTimeZone = 'Europe/Istanbul';

/// Gün şeridindeki gün sayısı (seçili günden 2 gün önce başlar).
const stripDays = 14;

/// Takvim / Takvimim (Takvim, TakvimBos, TakvimKapali): gün şeridi, personel
/// kolonları, kaydırarak gün geçişi, personel filtresi. Personel rolünde
/// sunucu yalnız kendi kolonunu döner; filtre gösterilmez.
class CalendarScreen extends ConsumerStatefulWidget {
  /// Takvim.
  const CalendarScreen({super.key});

  @override
  ConsumerState<CalendarScreen> createState() => _CalendarScreenState();
}

class _CalendarScreenState extends ConsumerState<CalendarScreen> {
  String _timeZone = defaultTimeZone;
  late String _selected = branchToday(ref.read(clockProvider), _timeZone);
  late String _windowStart = addDays(_selected, -2);
  Set<String>? _staffFilter;

  /// Filtre sayfasındaki personel: son filtresiz yanıttan (filtreli yanıt
  /// yalnız seçilen kolonları döner).
  List<CalendarViewStaffInner> _allStaff = const [];

  void _select(String date) {
    setState(() {
      _selected = date;
      final offset = daysBetween(_windowStart, date);
      if (offset < 0 || offset >= stripDays) {
        _windowStart = addDays(date, -2);
      }
    });
  }

  void _syncTimeZone(String timeZone) {
    if (timeZone == _timeZone) {
      return;
    }

    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!mounted) {
        return;
      }

      setState(() => _timeZone = timeZone);
      _select(branchToday(ref.read(clockProvider), timeZone));
    });
  }

  Future<void> _openFilter(List<CalendarViewStaffInner> staff) async {
    final result = await showOmvilaSheet<Set<String>?>(
      context,
      builder: (_) => _StaffFilterSheet(
        staff: staff,
        selected: _staffFilter ?? {for (final s in staff) s.staffId},
      ),
    );
    if (result != null) {
      setState(
        () => _staffFilter = result.length == staff.length ? null : result,
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final session = ref.watch(authControllerProvider).value;
    if (session == null) {
      return const SizedBox.shrink();
    }

    final branch = session.branches.first;
    final range = (
      branchId: branch.id,
      from: _windowStart,
      to: addDays(_windowStart, stripDays - 1),
      staffIds: _staffFilter == null
          ? null
          : (_staffFilter!.toList()..sort()).join(','),
    );
    final calendar = ref.watch(calendarProvider(range));
    final view = calendar.value;
    if (view != null) {
      _syncTimeZone(view.timeZone);
      if (range.staffIds == null) {
        _allStaff = view.staff;
      }
    }

    final day = view?.days
        .where((d) => localDateOf(d.date) == _selected)
        .firstOrNull;
    final closed = day?.isClosed ?? false;
    final readOnly =
        session.subscription.status == SubscriptionBannerStatusEnum.readOnly;
    final canCreate = session.can('appointment.create');
    final canFilter =
        session.can('calendar.view.all-staff') && _allStaff.length > 1;

    final Widget body;
    if (view != null && day != null) {
      body = _DayBody(
        view: view,
        day: day,
        canCreate: canCreate && !readOnly,
        onNewAppointment: () => _newAppointment(context),
        now: ref.watch(clockProvider).now(),
      );
    } else if (calendar.hasError) {
      body = LoadError(
        title: 'Takvim yüklenemedi',
        error: calendar.error,
        onRetry: () => ref.invalidate(calendarProvider(range)),
      );
    } else {
      body = const Loading();
    }

    return SafeArea(
      bottom: false,
      child: RefreshIndicator(
        onRefresh: () => ref.refresh(calendarProvider(range).future),
        child: GestureDetector(
          // Sola/sağa kaydırınca sonraki/önceki gün.
          onHorizontalDragEnd: (details) {
            final v = details.primaryVelocity ?? 0;
            if (v.abs() > 300) {
              _select(addDays(_selected, v < 0 ? 1 : -1));
            }
          },
          child: ListView(
            padding: const EdgeInsets.fromLTRB(
              0,
              OmvilaBosluk.bosluk6,
              0,
              tabBarClearance,
            ),
            children: [
              Padding(
                padding: const EdgeInsets.symmetric(
                  horizontal: OmvilaBosluk.bosluk4,
                ),
                child: Row(
                  crossAxisAlignment: CrossAxisAlignment.end,
                  children: [
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            '${formatDate(_selected, DateStyle.monthYear)} · '
                            '${branch.name}',
                            style: OmvilaYazi.ikincil.copyWith(
                              color: r.metinIkincil,
                              fontWeight: FontWeight.w600,
                            ),
                          ),
                          Semantics(
                            header: true,
                            child: Text(
                              session.isStaff ? 'Takvimim' : 'Takvim',
                              style: OmvilaYazi.buyukBaslik.copyWith(
                                color: r.metin,
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),
                    if (canFilter)
                      _HeaderButton(
                        icon: LucideIcons.slidersHorizontal,
                        tooltip: 'Personeli filtrele',
                        onPressed: () => _openFilter(_allStaff),
                      ),
                    if (canCreate) ...[
                      const SizedBox(width: OmvilaBosluk.bosluk2),
                      _HeaderButton(
                        icon: LucideIcons.plus,
                        tooltip: 'Yeni randevu',
                        primary: true,
                        onPressed: readOnly || closed
                            ? null
                            : () => _newAppointment(context),
                      ),
                    ],
                  ],
                ),
              ),
              const SizedBox(height: OmvilaBosluk.bosluk4),
              DayStrip(
                start: _windowStart,
                count: stripDays,
                selected: _selected,
                view: view,
                onSelect: _select,
              ),
              const SizedBox(height: OmvilaBosluk.bosluk4),
              body,
            ],
          ),
        ),
      ),
    );
  }

  void _newAppointment(BuildContext context) {
    context.push(Routes.newAppointmentOn(_selected));
  }
}

class _HeaderButton extends StatelessWidget {
  const _HeaderButton({
    required this.icon,
    required this.tooltip,
    required this.onPressed,
    this.primary = false,
  });

  final IconData icon;
  final String tooltip;
  final VoidCallback? onPressed;
  final bool primary;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final enabled = onPressed != null;
    return IconButton(
      onPressed: onPressed,
      tooltip: tooltip,
      style: IconButton.styleFrom(
        fixedSize: const Size.square(OmvilaBoyut.dokunmaMin),
        backgroundColor: primary
            ? (enabled ? r.metin : r.zeminDevreDisi)
            : r.zeminIkincil,
        disabledBackgroundColor: r.zeminDevreDisi,
        shape: const CircleBorder(),
      ),
      icon: Icon(
        icon,
        size: 22,
        color: primary ? (enabled ? r.zemin : r.metinDevreDisi) : r.metin,
      ),
    );
  }
}

class _DayBody extends StatelessWidget {
  const _DayBody({
    required this.view,
    required this.day,
    required this.canCreate,
    required this.onNewAppointment,
    required this.now,
  });

  final CalendarView view;
  final CalendarViewDaysInner day;
  final bool canCreate;
  final VoidCallback onNewAppointment;
  final DateTime now;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final date = localDateOf(day.date);
    if (day.isClosed) {
      return Padding(
        padding: const EdgeInsets.symmetric(horizontal: OmvilaBosluk.bosluk4),
        child: Column(
          children: [
            HatchedBox(
              child: OmvilaBosDurum(
                ikon: LucideIcons.calendarOff,
                baslik: '${formatDate(date, DateStyle.weekday)} · salon kapalı',
                aciklama:
                    '${day.closureNote ?? 'Kapalı gün.'} Bu güne randevu '
                    'alınmaz.',
              ),
            ),
            const SizedBox(height: OmvilaBosluk.bosluk3),
            SurfaceCard(
              color: r.zeminIkincil,
              child: Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Icon(LucideIcons.info, size: 18, color: r.metinIkincil),
                  const SizedBox(width: OmvilaBosluk.bosluk3),
                  Expanded(
                    child: Text(
                      'Günü yalnızca bu tarih için açmak ya da çalışma '
                      'saatlerini değiştirmek salon panelinden yapılır.',
                      style: OmvilaYazi.ikincil.copyWith(color: r.metin),
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      );
    }

    final staff = view.staff;
    final blocks = [
      for (final a in view.appointments)
        if (a.status != AppointmentStatus.cancelled &&
            toBranchDate(a.startUtc, view.timeZone) == date)
          a,
    ];
    if (blocks.isEmpty) {
      final hours = day.branchIntervals ?? const <TimeInterval>[];
      final hoursText = hours.isEmpty
          ? ''
          : ' Çalışma saatleri '
                '${hours.map((h) => '${h.start}–${h.end}').join(', ')}.';
      return OmvilaBosDurum(
        ikon: LucideIcons.calendarPlus,
        baslik: '${formatDate(date, DateStyle.weekdayDayMonth)} boş',
        aciklama: 'Bu güne henüz randevu yok.$hoursText',
        eylemEtiketi: canCreate ? 'Randevu ekle' : null,
        eylemIkonu: LucideIcons.plus,
        onEylem: canCreate ? onNewAppointment : null,
      );
    }

    return CalendarGrid(
      view: view,
      day: day,
      staff: staff,
      blocks: blocks,
      now: toBranchDate(now, view.timeZone) == date ? now : null,
      onOpen: (id) => context.push(Routes.appointment(id)),
    );
  }
}

class _StaffFilterSheet extends StatefulWidget {
  const _StaffFilterSheet({required this.staff, required this.selected});

  final List<CalendarViewStaffInner> staff;
  final Set<String> selected;

  @override
  State<_StaffFilterSheet> createState() => _StaffFilterSheetState();
}

class _StaffFilterSheetState extends State<_StaffFilterSheet> {
  late final Set<String> _selected = {...widget.selected};

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text(
          'Personel',
          style: OmvilaYazi.baslik2.copyWith(color: r.metin),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk3),
        Wrap(
          spacing: OmvilaBosluk.bosluk2,
          runSpacing: OmvilaBosluk.bosluk2,
          children: [
            for (final s in widget.staff)
              SelectChip(
                label: s.fullName,
                selected: _selected.contains(s.staffId),
                onTap: () => setState(() {
                  if (!_selected.remove(s.staffId)) {
                    _selected.add(s.staffId);
                  }
                }),
              ),
          ],
        ),
        const SizedBox(height: OmvilaBosluk.bosluk4),
        OmvilaButon(
          etiket: 'Uygula',
          tamGenislik: true,
          onPressed: _selected.isEmpty
              ? null
              : () => Navigator.of(context).pop(_selected),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk2),
        OmvilaButon(
          etiket: 'Tümünü göster',
          tur: OmvilaButonTuru.hayalet,
          tamGenislik: true,
          onPressed: () => Navigator.of(
            context,
          ).pop({for (final s in widget.staff) s.staffId}),
        ),
      ],
    );
  }
}
