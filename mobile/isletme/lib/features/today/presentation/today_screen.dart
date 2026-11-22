import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_isletme/features/push/data/push_prompt_controller.dart';
import 'package:omvila_isletme/features/shell/presentation/app_shell.dart';
import 'package:omvila_isletme/features/shell/presentation/tabs.dart';
import 'package:omvila_isletme/features/today/data/today_repository.dart';
import 'package:omvila_isletme/features/today/presentation/today_empty.dart';
import 'package:omvila_isletme/features/today/presentation/today_format.dart';
import 'package:omvila_isletme/features/today/presentation/today_widgets.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Bugün (Main, MainBos, MainBildirimKapali). Kartlar yetkiye bağlıdır:
/// sunucu izni olmayan kartı null döner, uygulama göstermez.
class TodayScreen extends ConsumerStatefulWidget {
  /// Bugün.
  const TodayScreen({super.key});

  @override
  ConsumerState<TodayScreen> createState() => _TodayScreenState();
}

class _TodayScreenState extends ConsumerState<TodayScreen> {
  late final AppLifecycleListener _lifecycle;

  @override
  void initState() {
    super.initState();
    // Arka plandan dönünce kritik veri yenilenir (kodlama-flutter.md).
    _lifecycle = AppLifecycleListener(
      onResume: () => ref.invalidate(todayProvider),
    );
  }

  @override
  void dispose() {
    _lifecycle.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final session = ref.watch(authControllerProvider).value;
    final today = ref.watch(todayProvider);
    final showBanner = ref.watch(pushPromptProvider).value?.showBanner ?? false;
    final canCreate = session?.can('appointment.create') ?? false;
    final snapshot = today.value;

    final Widget body;
    if (snapshot != null) {
      body = snapshot.today.items.isEmpty
          ? TodayEmpty(
              canQueueWalkIn: session?.can('walkin.manage') ?? false,
            )
          : _TodayContent(
              snapshot: snapshot,
              canApprove: session?.can('appointment.approve') ?? false,
            );
    } else if (today.hasError) {
      final error = today.error;
      body = OmvilaBosDurum(
        ikon: LucideIcons.calendarDays,
        baslik: 'Bugün yüklenemedi',
        aciklama: errorMessage(error is ApiProblem ? error.code : null),
        eylemEtiketi: 'Tekrar dene',
        onEylem: () => ref.invalidate(todayProvider),
      );
    } else {
      body = const Padding(
        padding: EdgeInsets.all(OmvilaBosluk.bosluk12),
        child: Center(
          child: CircularProgressIndicator(semanticsLabel: 'Yükleniyor'),
        ),
      );
    }

    return SafeArea(
      bottom: false,
      child: RefreshIndicator(
        onRefresh: () => ref.refresh(todayProvider.future),
        child: ListView(
          padding: const EdgeInsets.fromLTRB(
            OmvilaBosluk.bosluk4,
            OmvilaBosluk.bosluk6,
            OmvilaBosluk.bosluk4,
            tabBarClearance,
          ),
          children: [
            TodayHeader(
              subtitle: snapshot == null ? null : todayHeader(snapshot.today),
              onNewAppointment: canCreate
                  ? () => context.push(Routes.newAppointment)
                  : null,
            ),
            const SizedBox(height: OmvilaBosluk.bosluk4),
            if (showBanner) ...[
              NotificationsOffBanner(
                canApprove: session?.can('appointment.approve') ?? false,
                onEnable: () => ref.read(pushPromptProvider.notifier).request(),
                onDismiss: () =>
                    ref.read(pushPromptProvider.notifier).dismissBanner(),
              ),
              const SizedBox(height: OmvilaBosluk.bosluk3),
            ],
            body,
          ],
        ),
      ),
    );
  }
}

class _TodayContent extends ConsumerWidget {
  const _TodayContent({required this.snapshot, required this.canApprove});

  final TodaySnapshot snapshot;
  final bool canApprove;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final today = snapshot.today;
    final now = snapshot.serverNow(ref.watch(clockProvider));
    final commission = today.commission;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        TodayBento(today: today, now: now),
        if (commission != null) ...[
          const SizedBox(height: OmvilaBosluk.bosluk3),
          CommissionCard(
            commission: commission,
            onTap: () => context.go(Routes.profile),
          ),
        ],
        const SizedBox(height: OmvilaBosluk.bosluk6),
        Row(
          children: [
            Expanded(
              child: Semantics(
                header: true,
                child: Text(
                  'Gün akışı',
                  style: OmvilaYazi.baslik3.copyWith(
                    color: r.metin,
                    fontWeight: FontWeight.w800,
                  ),
                ),
              ),
            ),
            Text(
              '${today.completedCount}/${today.totalCount} bitti',
              style: OmvilaYazi.ikincil.copyWith(
                color: r.metinIkincil,
                fontWeight: FontWeight.w600,
              ),
            ),
          ],
        ),
        const SizedBox(height: OmvilaBosluk.bosluk3),
        for (final item in today.items) ...[
          RandevuSatiri(
            baslangic: formatTime(item.startUtc, today.timeZone),
            bitis: formatTime(item.endUtc, today.timeZone),
            musteri: item.customerName,
            ayrinti: serviceAndStaff(item),
            durum: RandevuDurumu.fromDisplayStatus(item.displayStatus),
            eylem:
                canApprove && item.status == AppointmentStatus.pendingApproval
                ? _ApproveButton(appointmentId: item.appointmentId)
                : null,
            onTap: () => context.push(Routes.appointment(item.appointmentId)),
          ),
          const SizedBox(height: OmvilaBosluk.bosluk2),
        ],
      ],
    );
  }
}

class _ApproveButton extends ConsumerStatefulWidget {
  const _ApproveButton({required this.appointmentId});

  final String appointmentId;

  @override
  ConsumerState<_ApproveButton> createState() => _ApproveButtonState();
}

class _ApproveButtonState extends ConsumerState<_ApproveButton> {
  bool _busy = false;

  Future<void> _approve() async {
    setState(() => _busy = true);
    final messenger = ScaffoldMessenger.of(context);
    final router = GoRouter.of(context);
    try {
      await ref.read(approveAppointmentProvider)(widget.appointmentId);
      messenger.showSnackBar(const SnackBar(content: Text('Talep onaylandı.')));
      ref.invalidate(todayProvider);
    } on ApiProblem catch (e) {
      messenger.showSnackBar(SnackBar(content: Text(errorMessage(e.code))));
      if (e.code == 'deposit_required') {
        // "Kapora geldi" / "Kaporasız onayla" seçenekleri ayrıntıda.
        unawaited(router.push(Routes.appointment(widget.appointmentId)));
      } else {
        ref.invalidate(todayProvider);
      }
    } finally {
      if (mounted) {
        setState(() => _busy = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return OmvilaButon(
      etiket: 'Onayla',
      ikon: LucideIcons.check,
      boyut: OmvilaButonBoyutu.sm,
      yukleniyor: _busy,
      yukleniyorEtiketi: 'Onaylanıyor…',
      onPressed: _busy ? null : _approve,
    );
  }
}
