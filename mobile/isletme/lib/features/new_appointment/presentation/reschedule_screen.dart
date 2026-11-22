import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/features/appointments/data/appointment_repository.dart';
import 'package:omvila_isletme/features/appointments/presentation/appointment_format.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_isletme/features/calendar/presentation/calendar_screen.dart';
import 'package:omvila_isletme/features/calendar/presentation/day_strip.dart';
import 'package:omvila_isletme/features/new_appointment/data/booking_repository.dart';
import 'package:omvila_isletme/features/new_appointment/presentation/pickers.dart';
import 'package:omvila_isletme/shared/ui.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// "Saati veya personeli değiştir" (RandevuIslem → uygun saatler). Tasarımda
/// ayrı ekranı yok; YeniRandevu'nun personel, gün ve saat seçicileriyle
/// yapılır (docs/acik-sorular.md T-109). Personel değişikliği tek personelli
/// randevuda sunulur; tekrarlayan randevuda yalnız bu randevu değişir.
/// Değişirse true ile kapanır.
class RescheduleScreen extends ConsumerStatefulWidget {
  /// Değiştir.
  const RescheduleScreen({required this.appointmentId, super.key});

  /// Randevu.
  final String appointmentId;

  @override
  ConsumerState<RescheduleScreen> createState() => _RescheduleScreenState();
}

class _RescheduleScreenState extends ConsumerState<RescheduleScreen> {
  String? _date;
  String? _staffId;
  bool _staffChosen = false;
  Slot? _slot;
  bool _notify = true;
  bool _busy = false;
  String? _error;

  Future<void> _save(AppointmentDetail a, bool staffChanged) async {
    final slot = _slot!;
    setState(() {
      _busy = true;
      _error = null;
    });
    final navigator = GoRouter.of(context);
    final messenger = ScaffoldMessenger.of(context);
    try {
      await ref
          .read(appointmentActionsProvider)
          .reschedule(
            a,
            ManageRescheduleRequest(
              startUtc: slot.startUtc,
              items: staffChanged
                  ? [
                      for (final item in activeItems(a))
                        ManageRescheduleRequestItemsInner(
                          itemId: item.id,
                          staffId: slot.items
                              .firstWhere(
                                (s) => s.serviceId == item.serviceId,
                                orElse: () => slot.items.first,
                              )
                              .staffId,
                        ),
                    ]
                  : null,
              scope: a.isRecurring
                  ? ManageRescheduleRequestScopeEnum.thisOnly
                  : null,
              notifyCustomer: _notify,
            ),
          );
      messenger.showSnackBar(
        const SnackBar(content: Text('Randevu saati değişti.')),
      );
      navigator.pop(true);
    } on ApiProblem catch (e) {
      setState(() {
        _busy = false;
        _error = errorMessage(e.code);
        if (e.code == 'slot_taken') {
          _slot = null;
        }
      });
      if (e.code == 'slot_taken') {
        ref.invalidate(slotsProvider);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final session = ref.watch(authControllerProvider).value;
    final detail = ref.watch(appointmentProvider(widget.appointmentId));
    final staff = session == null
        ? null
        : ref.watch(staffProvider(session.branches.first.id));
    final a = detail.value;
    final staffList = staff?.value;

    Widget body;
    if (a != null && staffList != null && session != null) {
      final items = activeItems(a);
      final currentStaff = {for (final i in items) i.staffId}
          .whereType<String>();
      final single = currentStaff.length == 1 && !session.isStaff;
      final staffId = _staffChosen ? _staffId : currentStaff.firstOrNull;
      final date = _date ?? toBranchDate(a.startUtc, a.timeZone);
      final stripStart = toBranchDate(a.startUtc, a.timeZone);
      final staffChanged = single && staffId != currentStaff.firstOrNull;
      body = ListView(
        padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
        children: [
          SurfaceCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  a.customer.fullName,
                  style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
                ),
                Text(
                  [
                    formatDate(stripStart, DateStyle.compact),
                    timeRange(a.startUtc, a.endUtc, a.timeZone),
                    servicesOf(a),
                  ].join(' · '),
                  style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
                ),
              ],
            ),
          ),
          if (single) ...[
            const SizedBox(height: OmvilaBosluk.bosluk5),
            const SectionLabel('Personel'),
            const SizedBox(height: OmvilaBosluk.bosluk2),
            StaffPicker(
              staff: staffList,
              selected: staffId,
              allowAny: false,
              onSelect: (id) => setState(() {
                _staffChosen = true;
                _staffId = id;
                _slot = null;
              }),
            ),
          ],
          const SizedBox(height: OmvilaBosluk.bosluk5),
          const SectionLabel('Gün'),
          const SizedBox(height: OmvilaBosluk.bosluk2),
          FullBleed(
            height: dayStripHeight,
            child: DayStrip(
              start: addDays(stripStart, -1),
              count: stripDays,
              selected: date,
              view: null,
              onSelect: (d) => setState(() {
                _date = d;
                _slot = null;
              }),
            ),
          ),
          const SizedBox(height: OmvilaBosluk.bosluk5),
          const SectionLabel('Saat'),
          const SizedBox(height: OmvilaBosluk.bosluk2),
          SlotGrid(
            query: SlotQuery(
              branchId: a.branchId,
              date: date,
              serviceIds: [for (final i in items) i.serviceId],
              staffId: staffId,
            ),
            selected: _slot?.startUtc,
            onSelect: (s) => setState(() => _slot = s),
          ),
          const SizedBox(height: OmvilaBosluk.bosluk3),
          SwitchListTile(
            contentPadding: EdgeInsets.zero,
            value: _notify,
            onChanged: (v) => setState(() => _notify = v),
            title: const Text('Müşteriye bildir'),
          ),
          if (_error != null)
            Text(
              _error!,
              style: OmvilaYazi.ikincil.copyWith(color: r.hata),
            ),
          const SizedBox(height: OmvilaBosluk.bosluk3),
          OmvilaButon(
            etiket: 'Saati değiştir',
            ikon: LucideIcons.calendarClock,
            tamGenislik: true,
            yukleniyor: _busy,
            yukleniyorEtiketi: 'Kaydediliyor…',
            onPressed: _slot == null || _busy
                ? null
                : () => _save(a, staffChanged),
          ),
        ],
      );
    } else if (detail.hasError || (staff?.hasError ?? false)) {
      body = LoadError(
        title: 'Saatler yüklenemedi',
        error: detail.error ?? staff?.error,
        onRetry: () =>
            ref.invalidate(appointmentProvider(widget.appointmentId)),
      );
    } else {
      body = const Loading();
    }

    return Scaffold(
      appBar: AppBar(title: const Text('Saati veya personeli değiştir')),
      body: body,
    );
  }
}
