import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_isletme/features/calendar/presentation/calendar_screen.dart';
import 'package:omvila_isletme/features/notifications/data/notification_repository.dart';
import 'package:omvila_isletme/features/shell/presentation/app_shell.dart';
import 'package:omvila_isletme/features/shell/presentation/tabs.dart';
import 'package:omvila_isletme/features/today/data/today_repository.dart';
import 'package:omvila_isletme/shared/ui.dart';
import 'package:omvila_isletme/shared/widgets.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Bildirimler sekmesi.
enum NotificationTab {
  /// "Tümü".
  all('Tümü'),

  /// "Onay bekleyen" (staff.approval_needed).
  approvals('Onay bekleyen'),

  /// "Okunmamış" (unreadOnly).
  unread('Okunmamış');

  const NotificationTab(this.label);

  /// Sekme metni.
  final String label;
}

/// Bildirimler (Bildirimler, BildirimlerBos): gün gruplu satırlar,
/// "Okundu say", onay bekleyen talepte satır içi "Onayla"/"Reddet"
/// (appointment.approve), satıra dokununca hedef ekran.
class NotificationsScreen extends ConsumerStatefulWidget {
  /// Bildirimler.
  const NotificationsScreen({super.key});

  @override
  ConsumerState<NotificationsScreen> createState() =>
      _NotificationsScreenState();
}

class _NotificationsScreenState extends ConsumerState<NotificationsScreen> {
  NotificationTab _tab = NotificationTab.all;

  bool get _unreadOnly => _tab == NotificationTab.unread;

  void _refresh() => ref
    ..invalidate(notificationsProvider)
    ..invalidate(unreadNotificationCountProvider);

  Future<void> _markAll() async {
    try {
      await ref.read(markNotificationsReadProvider)();
    } on ApiProblem catch (e) {
      if (mounted) {
        showProblem(context, e);
      }
    }
    _refresh();
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final session = ref.watch(authControllerProvider).value;
    final page = ref.watch(notificationsProvider(_unreadOnly));
    final data = page.value;
    final unread = data?.unreadCount ?? 0;
    final items = [
      for (final n in data?.items ?? const <InAppNotification>[])
        if (_tab != NotificationTab.approvals ||
            n.event == StaffNotificationEvent.staffPeriodApprovalNeeded)
          n,
    ];
    final today = branchToday(ref.watch(clockProvider), defaultTimeZone);

    final emptyAll =
        data != null && items.isEmpty && _tab == NotificationTab.all;
    final List<Widget> body;
    if (data != null && items.isEmpty) {
      body = [
        OmvilaBosDurum(
          ikon: LucideIcons.bell,
          baslik: 'Yeni bildirim yok',
          aciklama:
              'Yeni randevu, onay bekleyen talep, iptal ve sıra hareketleri '
              'burada görünür.',
          eylemEtiketi: 'Bildirim ayarları',
          eylemIkonu: LucideIcons.settings,
          onEylem: () => context.push(Routes.notificationSettings),
        ),
      ];
    } else if (data != null) {
      String? lastDay;
      body = [
        for (final n in items) ...[
          if (toBranchDate(n.createdUtc, defaultTimeZone) != lastDay) ...[
            if (lastDay != null) const SizedBox(height: OmvilaBosluk.bosluk3),
            SectionHeader(
              relativeDay(
                lastDay = toBranchDate(n.createdUtc, defaultTimeZone),
                today,
              ),
            ),
            const SizedBox(height: OmvilaBosluk.bosluk2),
          ],
          NotificationRow(
            notification: n,
            canApprove: session?.can('appointment.approve') ?? false,
            onChanged: _refresh,
          ),
          const SizedBox(height: OmvilaBosluk.bosluk2),
        ],
      ];
    } else if (page.hasError) {
      body = [
        LoadError(
          title: 'Bildirimler yüklenemedi',
          error: page.error,
          icon: LucideIcons.bell,
          onRetry: _refresh,
        ),
      ];
    } else {
      body = const [Loading()];
    }

    final branch = session == null || session.branches.isEmpty
        ? null
        : session.branches.first.name;
    return SafeArea(
      bottom: false,
      child: RefreshIndicator(
        onRefresh: () => ref.refresh(notificationsProvider(_unreadOnly).future),
        child: ListView(
          padding: const EdgeInsets.fromLTRB(
            OmvilaBosluk.bosluk4,
            OmvilaBosluk.bosluk6,
            OmvilaBosluk.bosluk4,
            tabBarClearance,
          ),
          children: [
            ScreenTitle(
              eyebrow: unread > 0 ? '$unread okunmamış' : branch,
              title: 'Bildirimler',
              trailing: unread > 0
                  ? TextButton(
                      onPressed: _markAll,
                      child: Text(
                        'Okundu say',
                        style: OmvilaYazi.etiket.copyWith(color: r.metinMarka),
                      ),
                    )
                  : null,
            ),
            const SizedBox(height: OmvilaBosluk.bosluk4),
            // BildirimlerBos'ta segment yok; süzülmüş sekme boşsa geri
            // dönebilmek için görünür kalır.
            if (!emptyAll) ...[
              OmvilaSegment<NotificationTab>(
                ogeler: [
                  for (final tab in NotificationTab.values)
                    OmvilaSegmentOgesi(tab, tab.label),
                ],
                secili: _tab,
                onSecildi: (tab) => setState(() => _tab = tab),
              ),
              const SizedBox(height: OmvilaBosluk.bosluk4),
            ],
            ...body,
          ],
        ),
      ),
    );
  }
}

/// Olay ikonu ve zemini.
(IconData, Color, Color) _eventStyle(
  BuildContext context,
  StaffNotificationEvent event,
) {
  final r = context.omvilaRenkler;
  return switch (event) {
    StaffNotificationEvent.staffPeriodApprovalNeeded ||
    StaffNotificationEvent.staffPeriodAppointmentCreated => (
      LucideIcons.calendarPlus,
      r.bilgiZemin,
      r.bilgi,
    ),
    StaffNotificationEvent.staffPeriodDepositReported => (
      LucideIcons.wallet,
      r.uyariZemin,
      r.uyari,
    ),
    StaffNotificationEvent.staffPeriodAppointmentChanged => (
      LucideIcons.calendarX,
      r.zeminIkincil,
      r.metinIkincil,
    ),
    StaffNotificationEvent.staffPeriodCustomerCheckedIn => (
      LucideIcons.userCheck,
      r.basariZemin,
      r.basari,
    ),
    StaffNotificationEvent.staffPeriodReviewReceived => (
      LucideIcons.star,
      r.uyariZemin,
      r.uyari,
    ),
    StaffNotificationEvent.staffPeriodWalkinQueue => (
      LucideIcons.listOrdered,
      r.zeminMarkaYumusak,
      r.metinMarka,
    ),
    StaffNotificationEvent.ownerPeriodCreditsLow ||
    StaffNotificationEvent.ownerPeriodSubscriptionPaymentFailed ||
    StaffNotificationEvent.ownerPeriodTrialEnding => (
      LucideIcons.triangleAlert,
      r.hataZemin,
      r.hata,
    ),
    _ => (LucideIcons.bell, r.zeminIkincil, r.metinIkincil),
  };
}

/// Bildirim satırı.
class NotificationRow extends ConsumerStatefulWidget {
  /// Satır.
  const NotificationRow({
    required this.notification,
    required this.canApprove,
    required this.onChanged,
    super.key,
  });

  /// Bildirim.
  final InAppNotification notification;

  /// appointment.approve.
  final bool canApprove;

  /// Okundu sayıldı veya eylem yapıldı.
  final VoidCallback onChanged;

  @override
  ConsumerState<NotificationRow> createState() => _NotificationRowState();
}

class _NotificationRowState extends ConsumerState<NotificationRow> {
  bool _busy = false;

  InAppNotification get _n => widget.notification;

  Future<void> _markRead() async {
    if (_n.readUtc != null) {
      return;
    }

    try {
      await ref.read(markNotificationsReadProvider)(ids: {_n.id});
    } on ApiProblem {
      // Okundu sayılamazsa satır okunmamış kalır; gezinme sürer.
    }
    widget.onChanged();
  }

  Future<void> _open() async {
    final router = GoRouter.of(context);
    unawaited(_markRead());
    final id = _n.target.id;
    switch (_n.target.type) {
      case NotificationTargetTypeEnum.appointment:
      case NotificationTargetTypeEnum.deposit:
        if (id != null) {
          unawaited(router.push(Routes.appointment(id)));
        }
      case NotificationTargetTypeEnum.calendar:
        router.go(Routes.calendar);
      case NotificationTargetTypeEnum.today:
        router.go(Routes.today);
      case NotificationTargetTypeEnum.walkInQueue:
        unawaited(router.push(Routes.walkIns));
      // Değerlendirmeler, kontör ve abonelik ekranları salon panelinde.
      case NotificationTargetTypeEnum.reviews:
      case NotificationTargetTypeEnum.smsCredits:
      case NotificationTargetTypeEnum.subscription:
      case NotificationTargetTypeEnum.unknownDefaultOpenApi:
        break;
    }
  }

  Future<void> _decide({required bool approve}) async {
    final id = _n.target.id!;
    setState(() => _busy = true);
    final messenger = ScaffoldMessenger.of(context);
    final router = GoRouter.of(context);
    try {
      if (approve) {
        await ref.read(approveAppointmentProvider)(id);
      } else {
        await ref.read(declineAppointmentProvider)(id);
      }
      messenger.showSnackBar(
        SnackBar(
          content: Text(approve ? 'Talep onaylandı.' : 'Talep iptal edildi.'),
        ),
      );
      ref.invalidate(todayProvider);
      await _markRead();
    } on ApiProblem catch (e) {
      messenger.showSnackBar(SnackBar(content: Text(errorMessage(e.code))));
      if (e.code == 'deposit_required') {
        // "Kapora geldi" / "Kaporasız onayla" seçenekleri ayrıntıda.
        unawaited(router.push(Routes.appointment(id)));
      } else if (e.code == 'invalid_state_transition' ||
          e.code == 'hold_expired') {
        await _markRead();
      }
    } finally {
      if (mounted) {
        setState(() => _busy = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final n = _n;
    final now = ref.watch(clockProvider).now();
    final hold = n.target.holdExpiresUtc;
    final unread = n.readUtc == null;
    final pending =
        n.event == StaffNotificationEvent.staffPeriodApprovalNeeded &&
        n.target.type == NotificationTargetTypeEnum.appointment &&
        n.target.id != null &&
        unread &&
        (hold == null || hold.isAfter(now));
    final remaining = pending && hold != null
        ? ' Yanıt için ${formatDuration(hold.difference(now).inMinutes)} '
              'kaldı.'
        : '';
    final (icon, background, foreground) = _eventStyle(context, n.event);
    return SurfaceCard(
      padding: EdgeInsets.zero,
      child: InkWell(
        borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
        onTap: _open,
        child: Padding(
          padding: const EdgeInsets.all(OmvilaBosluk.bosluk3 + 2),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Container(
                width: 40,
                height: 40,
                decoration: BoxDecoration(
                  color: background,
                  borderRadius: BorderRadius.circular(OmvilaKose.koseMd),
                ),
                child: Icon(icon, size: 20, color: foreground),
              ),
              const SizedBox(width: OmvilaBosluk.bosluk3),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Expanded(
                          child: Text(
                            n.title,
                            style: OmvilaYazi.govdeGuclu.copyWith(
                              color: r.metin,
                            ),
                          ),
                        ),
                        Text(
                          formatTime(n.createdUtc, defaultTimeZone),
                          style: OmvilaYazi.aciklama.copyWith(
                            color: r.metinIkincil,
                          ),
                        ),
                        if (unread) ...[
                          const SizedBox(width: OmvilaBosluk.bosluk2),
                          Padding(
                            padding: const EdgeInsets.only(top: 6),
                            child: Semantics(
                              label: 'Okunmadı',
                              child: Container(
                                width: 8,
                                height: 8,
                                decoration: BoxDecoration(
                                  color: r.vurgu,
                                  shape: BoxShape.circle,
                                ),
                              ),
                            ),
                          ),
                        ],
                      ],
                    ),
                    const SizedBox(height: OmvilaBosluk.bosluk1),
                    Text(
                      '${n.body}$remaining',
                      style: OmvilaYazi.ikincil.copyWith(
                        color: r.metinIkincil,
                      ),
                    ),
                    if (pending && widget.canApprove) ...[
                      const SizedBox(height: OmvilaBosluk.bosluk3),
                      Row(
                        spacing: OmvilaBosluk.bosluk2,
                        children: [
                          OmvilaButon(
                            etiket: 'Onayla',
                            boyut: OmvilaButonBoyutu.sm,
                            yukleniyor: _busy,
                            onPressed: _busy
                                ? null
                                : () => _decide(approve: true),
                          ),
                          OmvilaButon(
                            etiket: 'Reddet',
                            tur: OmvilaButonTuru.ikincil,
                            boyut: OmvilaButonBoyutu.sm,
                            onPressed: _busy
                                ? null
                                : () => _decide(approve: false),
                          ),
                        ],
                      ),
                    ],
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
