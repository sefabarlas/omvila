import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_isletme/features/appointments/data/appointment_repository.dart';
import 'package:omvila_isletme/features/appointments/presentation/actions_sheet.dart';
import 'package:omvila_isletme/features/appointments/presentation/appointment_format.dart';
import 'package:omvila_isletme/features/appointments/presentation/checkout_sheet.dart';
import 'package:omvila_isletme/features/appointments/presentation/deposit_sheet.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_isletme/features/calendar/data/calendar_repository.dart';
import 'package:omvila_isletme/features/shell/presentation/tabs.dart';
import 'package:omvila_isletme/features/today/data/today_repository.dart';
import 'package:omvila_isletme/shared/ui.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Randevu ayrıntısı (RandevuDetay). Alttaki birincil eylem duruma ve izne
/// göre değişir: kapora bekleniyorsa "Kaporayı kontrol et" (KaporaOnay),
/// onay bekliyorsa "Onayla", onaylıysa "Geldi", gelmişse "Tamamla ve ödeme
/// al" (OdemeAl). "…" RandevuIslem'i açar. Personel rolünde telefon maskeli
/// kalır; "Ara" ve "SMS" numarayı arama anında açar (KK10).
class AppointmentDetailScreen extends ConsumerWidget {
  /// Ayrıntı.
  const AppointmentDetailScreen({required this.appointmentId, super.key});

  /// Randevu kimliği.
  final String appointmentId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final detail = ref.watch(appointmentProvider(appointmentId));
    final a = detail.value;
    final Widget body;
    if (a != null) {
      body = RefreshIndicator(
        onRefresh: () => ref.refresh(appointmentProvider(appointmentId).future),
        child: _DetailBody(appointment: a),
      );
    } else if (detail.hasError) {
      body = Center(
        child: LoadError(
          title: 'Randevu yüklenemedi',
          error: detail.error,
          onRetry: () => ref.invalidate(appointmentProvider(appointmentId)),
        ),
      );
    } else {
      body = const Loading();
    }

    return Scaffold(
      appBar: AppBar(
        title: const Text('Randevu'),
        centerTitle: true,
        leading: IconButton(
          tooltip: 'Geri',
          icon: const Icon(LucideIcons.arrowLeft),
          onPressed: () =>
              context.canPop() ? context.pop() : context.go(Routes.today),
        ),
      ),
      body: body,
      bottomNavigationBar: a == null ? null : _BottomBar(appointment: a),
    );
  }
}

/// Eylemden sonra ayrıntı, Bugün ve takvim yenilenir.
void refreshAfterChange(WidgetRef ref, String appointmentId) {
  ref
    ..invalidate(appointmentProvider(appointmentId))
    ..invalidate(todayProvider)
    ..invalidate(calendarProvider);
}

class _DetailBody extends StatelessWidget {
  const _DetailBody({required this.appointment});

  final AppointmentDetail appointment;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final a = appointment;
    final tz = a.timeZone;
    final deposit = depositValue(a);
    final note = [
      a.customerNote,
      a.staffNote,
    ].whereType<String>().where((n) => n.trim().isNotEmpty).join('\n');
    return ListView(
      padding: const EdgeInsets.fromLTRB(
        OmvilaBosluk.bosluk4,
        OmvilaBosluk.bosluk2,
        OmvilaBosluk.bosluk4,
        OmvilaBosluk.bosluk6,
      ),
      children: [
        _HeaderCard(appointment: a),
        const SizedBox(height: OmvilaBosluk.bosluk3),
        _CustomerCard(appointment: a),
        const SizedBox(height: OmvilaBosluk.bosluk3),
        SurfaceCard(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              const SectionLabel('Hizmetler'),
              for (final item in activeItems(a))
                Padding(
                  padding: const EdgeInsets.only(top: OmvilaBosluk.bosluk3),
                  child: IntrinsicHeight(
                    child: Row(
                      crossAxisAlignment: CrossAxisAlignment.stretch,
                      children: [
                        Container(
                          width: 4,
                          decoration: BoxDecoration(
                            color: r.marka,
                            borderRadius: BorderRadius.circular(2),
                          ),
                        ),
                        const SizedBox(width: OmvilaBosluk.bosluk3),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                item.serviceName,
                                style: OmvilaYazi.govdeGuclu.copyWith(
                                  color: r.metin,
                                ),
                              ),
                              Text(
                                [
                                  timeRange(item.startUtc, item.endUtc, tz),
                                  if (item.staffName != null)
                                    firstNameOf(item.staffName),
                                ].join(' · '),
                                style: OmvilaYazi.aciklama.copyWith(
                                  color: r.metinIkincil,
                                ),
                              ),
                            ],
                          ),
                        ),
                        Center(
                          child: Text(
                            (item.paidFromPackage ?? false)
                                ? 'Paketten'
                                : formatMoney(itemAmount(item)),
                            style: OmvilaYazi.govdeGuclu.copyWith(
                              color: r.metin,
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
            ],
          ),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk3),
        SurfaceCard(
          padding: const EdgeInsets.symmetric(
            horizontal: OmvilaBosluk.bosluk4,
            vertical: OmvilaBosluk.bosluk1,
          ),
          child: LabelValueRows(
            rows: [
              LabelValue('Toplam', money(a.total)),
              if (deposit != null) LabelValue('Kapora', deposit),
              LabelValue('Kalan', money(a.amountDue)),
            ],
          ),
        ),
        if (note.isNotEmpty) ...[
          const SizedBox(height: OmvilaBosluk.bosluk3),
          SurfaceCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const SectionLabel('Not'),
                const SizedBox(height: OmvilaBosluk.bosluk2),
                Text(note, style: OmvilaYazi.govde.copyWith(color: r.metin)),
              ],
            ),
          ),
        ],
      ],
    );
  }
}

class _HeaderCard extends StatelessWidget {
  const _HeaderCard({required this.appointment});

  final AppointmentDetail appointment;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final a = appointment;
    final status = RandevuDurumu.fromDisplayStatus(a.displayStatus);
    final arrived = checkedInAt(a);
    return Container(
      padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
      decoration: BoxDecoration(
        color: r.zeminMarka,
        borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Expanded(
                child: Text(
                  headerLine(a),
                  style: OmvilaYazi.aciklama.copyWith(
                    color: r.metinTers,
                    fontWeight: FontWeight.w700,
                    letterSpacing: 0.6,
                  ),
                ),
              ),
              if (status == RandevuDurumu.geldi && arrived != null)
                _ArrivedPill(time: formatTime(arrived, a.timeZone))
              else if (status != null)
                OmvilaDurumRozeti(durum: status),
            ],
          ),
          const SizedBox(height: OmvilaBosluk.bosluk2),
          Text(
            timeRange(a.startUtc, a.endUtc, a.timeZone),
            style: OmvilaYazi.buyukBaslik.copyWith(color: r.metinTers),
          ),
          Text(
            summaryLine(a),
            style: OmvilaYazi.ikincil.copyWith(color: r.metinTers),
          ),
        ],
      ),
    );
  }
}

/// "Geldi · 10:34".
class _ArrivedPill extends StatelessWidget {
  const _ArrivedPill({required this.time});

  final String time;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return Container(
      padding: const EdgeInsets.symmetric(
        horizontal: OmvilaBosluk.bosluk2 + 2,
        vertical: OmvilaBosluk.bosluk1,
      ),
      decoration: BoxDecoration(
        color: r.durumGeldiZemin,
        borderRadius: BorderRadius.circular(OmvilaKose.koseTam),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          ExcludeSemantics(
            child: DecoratedBox(
              decoration: BoxDecoration(
                color: r.durumGeldiNokta,
                shape: BoxShape.circle,
              ),
              child: const SizedBox.square(dimension: 6),
            ),
          ),
          const SizedBox(width: OmvilaBosluk.bosluk1 + 2),
          Text(
            'Geldi · $time',
            style: OmvilaYazi.aciklama.copyWith(
              color: r.durumGeldiMetin,
              fontWeight: FontWeight.w700,
            ),
          ),
        ],
      ),
    );
  }
}

class _CustomerCard extends ConsumerStatefulWidget {
  const _CustomerCard({required this.appointment});

  final AppointmentDetail appointment;

  @override
  ConsumerState<_CustomerCard> createState() => _CustomerCardState();
}

class _CustomerCardState extends ConsumerState<_CustomerCard> {
  bool _busy = false;

  /// Numara yanıtta varsa (customer.phone.view) doğrudan; yoksa
  /// manageRevealCustomerPhone ile arama anında açılır. Numara gösterilmez.
  Future<void> _contact(PhoneRevealRequestPurposeEnum purpose) async {
    setState(() => _busy = true);
    final messenger = ScaffoldMessenger.of(context);
    final customer = widget.appointment.customer;
    try {
      final phone =
          customer.phone ??
          await ref
              .read(appointmentActionsProvider)
              .revealPhone(customer.brandCustomerId, purpose);
      final scheme = purpose == PhoneRevealRequestPurposeEnum.sms
          ? 'sms'
          : 'tel';
      final opened = await ref
          .read(externalLauncherProvider)
          .open(Uri(scheme: scheme, path: phone));
      if (!opened) {
        messenger.showSnackBar(
          const SnackBar(content: Text('Bu cihazda arama açılamadı.')),
        );
      }
    } on ApiProblem catch (e) {
      messenger.showSnackBar(SnackBar(content: Text(errorMessage(e.code))));
    } finally {
      if (mounted) {
        setState(() => _busy = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final c = widget.appointment.customer;
    return SurfaceCard(
      padding: const EdgeInsets.all(OmvilaBosluk.bosluk3),
      child: Row(
        children: [
          InitialsAvatar(name: c.fullName),
          const SizedBox(width: OmvilaBosluk.bosluk3),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  c.fullName,
                  style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
                ),
                Text(
                  visitLine(c),
                  style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
                ),
              ],
            ),
          ),
          _RoundIconButton(
            icon: LucideIcons.phone,
            tooltip: 'Ara',
            onPressed: _busy
                ? null
                : () => _contact(PhoneRevealRequestPurposeEnum.call),
          ),
          const SizedBox(width: OmvilaBosluk.bosluk2),
          _RoundIconButton(
            icon: LucideIcons.messageSquare,
            tooltip: 'SMS',
            onPressed: _busy
                ? null
                : () => _contact(PhoneRevealRequestPurposeEnum.sms),
          ),
        ],
      ),
    );
  }
}

class _RoundIconButton extends StatelessWidget {
  const _RoundIconButton({
    required this.icon,
    required this.tooltip,
    required this.onPressed,
  });

  final IconData icon;
  final String tooltip;
  final VoidCallback? onPressed;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return IconButton(
      onPressed: onPressed,
      tooltip: tooltip,
      style: IconButton.styleFrom(
        backgroundColor: r.zeminIkincil,
        fixedSize: const Size.square(OmvilaBoyut.dokunmaMin),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(OmvilaKose.koseSm),
        ),
      ),
      icon: Icon(icon, size: 20, color: r.metin),
    );
  }
}

enum _Primary { deposit, approve, checkIn, complete }

_Primary? _primaryFor(ManageSession session, AppointmentDetail a) {
  if (depositAwaited(a)) {
    return session.can('appointment.deposit.mark') ? _Primary.deposit : null;
  }

  return switch (a.status) {
    AppointmentStatus.pendingApproval when session.can('appointment.approve') =>
      _Primary.approve,
    AppointmentStatus.confirmed when session.can('appointment.checkin') =>
      _Primary.checkIn,
    AppointmentStatus.checkedIn when session.can('appointment.complete') =>
      _Primary.complete,
    _ => null,
  };
}

class _BottomBar extends ConsumerStatefulWidget {
  const _BottomBar({required this.appointment});

  final AppointmentDetail appointment;

  @override
  ConsumerState<_BottomBar> createState() => _BottomBarState();
}

class _BottomBarState extends ConsumerState<_BottomBar> {
  bool _busy = false;

  AppointmentDetail get _a => widget.appointment;

  Future<void> _run(
    Future<void> Function(AppointmentActions actions) action,
    String done,
  ) async {
    setState(() => _busy = true);
    final messenger = ScaffoldMessenger.of(context);
    try {
      await action(ref.read(appointmentActionsProvider));
      messenger.showSnackBar(SnackBar(content: Text(done)));
    } on ApiProblem catch (e) {
      messenger.showSnackBar(SnackBar(content: Text(errorMessage(e.code))));
    } finally {
      refreshAfterChange(ref, _a.id);
      if (mounted) {
        setState(() => _busy = false);
      }
    }
  }

  Future<void> _sheet(Widget child) async {
    final changed = await showOmvilaSheet<bool>(
      context,
      builder: (_) => child,
    );
    if (changed ?? false) {
      refreshAfterChange(ref, _a.id);
    }
  }

  Future<void> _more(List<AppointmentAction> actions) async {
    final router = GoRouter.of(context);
    final chosen = await showOmvilaSheet<AppointmentAction>(
      context,
      builder: (_) => ActionsSheet(appointment: _a, actions: actions),
    );
    if (!mounted || chosen == null) {
      return;
    }

    switch (chosen) {
      case AppointmentAction.reschedule:
        final changed = await router.push<bool>(Routes.reschedule(_a.id));
        if (changed ?? false) {
          refreshAfterChange(ref, _a.id);
        }
      case AppointmentAction.undoNoShow:
        await _run(
          (x) => x.undoNoShow(_a),
          'Randevu Geldi olarak işaretlendi.',
        );
      case AppointmentAction.cancel:
        await _sheet(CancelSheet(appointment: _a));
    }
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final session = ref.watch(authControllerProvider).value;
    if (session == null) {
      return const SizedBox.shrink();
    }

    final actions = availableActions(session, _a);
    final primary = _primaryFor(session, _a);
    if (actions.isEmpty && primary == null) {
      return const SizedBox.shrink();
    }

    final Widget? button = switch (primary) {
      _Primary.deposit => OmvilaButon(
        etiket: 'Kaporayı kontrol et',
        ikon: LucideIcons.banknote,
        boyut: OmvilaButonBoyutu.lg,
        tamGenislik: true,
        onPressed: _busy ? null : () => _sheet(DepositSheet(appointment: _a)),
      ),
      _Primary.approve => OmvilaButon(
        etiket: 'Onayla',
        ikon: LucideIcons.check,
        boyut: OmvilaButonBoyutu.lg,
        tamGenislik: true,
        yukleniyor: _busy,
        yukleniyorEtiketi: 'Onaylanıyor…',
        onPressed: _busy
            ? null
            : () => _run(
                (_) => ref.read(approveAppointmentProvider)(_a.id),
                'Talep onaylandı.',
              ),
      ),
      _Primary.checkIn => OmvilaButon(
        etiket: 'Geldi',
        ikon: LucideIcons.userCheck,
        boyut: OmvilaButonBoyutu.lg,
        tamGenislik: true,
        yukleniyor: _busy,
        yukleniyorEtiketi: 'İşaretleniyor…',
        onPressed: _busy
            ? null
            : () => _run(
                (x) => x.checkIn(_a),
                'Müşteri geldi olarak işaretlendi.',
              ),
      ),
      _Primary.complete => OmvilaButon(
        etiket: 'Tamamla ve ödeme al',
        ikon: LucideIcons.check,
        boyut: OmvilaButonBoyutu.lg,
        tamGenislik: true,
        onPressed: _busy ? null : () => _sheet(CheckoutSheet(appointment: _a)),
      ),
      null => null,
    };

    return Material(
      color: r.zeminYukseltilmis,
      child: SafeArea(
        top: false,
        child: Padding(
          padding: const EdgeInsets.fromLTRB(
            OmvilaBosluk.bosluk4,
            OmvilaBosluk.bosluk3,
            OmvilaBosluk.bosluk4,
            OmvilaBosluk.bosluk3,
          ),
          child: Row(
            children: [
              if (actions.isNotEmpty)
                IconButton(
                  onPressed: _busy ? null : () => unawaited(_more(actions)),
                  tooltip: 'Diğer işlemler',
                  style: IconButton.styleFrom(
                    backgroundColor: r.zeminIkincil,
                    fixedSize: const Size.square(OmvilaBoyut.kontrolLg - 8),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(OmvilaKose.koseSm),
                    ),
                  ),
                  icon: Icon(LucideIcons.ellipsis, color: r.metin),
                ),
              if (actions.isNotEmpty && button != null)
                const SizedBox(width: OmvilaBosluk.bosluk3),
              if (button != null) Expanded(child: button),
            ],
          ),
        ),
      ),
    );
  }
}
