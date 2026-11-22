import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/notifications/presentation/notification_prefs_controller.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_musteri/shared/widgets/ic_sayfa.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Zildeki okunmamış sayısı; okunamazsa 0 (rozet gösterilmez).
final FutureProvider<int> unreadCountProvider = FutureProvider.autoDispose<int>(
  (ref) async {
    try {
      final page = await ref
          .watch(notificationsRepositoryProvider)
          .list(limit: 1);
      return page.unreadCount;
    } on ApiProblem {
      return 0;
    }
  },
);

/// Bildirimler (ilk sayfa).
final FutureProvider<MeNotificationPage> notificationsProvider =
    FutureProvider.autoDispose<MeNotificationPage>(
      (ref) => ref.watch(notificationsRepositoryProvider).list(),
    );

/// Bildirimin açacağı adres; hedef eksikse null.
String? bildirimHedefi(MeNotificationTarget target) {
  final id = target.appointmentId;
  final code = target.accessCode;
  final slug = target.brandSlug;
  return switch (target.type) {
    MeNotificationTargetTypeEnum.appointment when id != null => Routes.randevu(
      id,
    ),
    MeNotificationTargetTypeEnum.review when id != null => Routes.degerlendir(
      id,
    ),
    MeNotificationTargetTypeEnum.waitlistOffer when code != null =>
      Routes.beklemeTeklif(code),
    MeNotificationTargetTypeEnum.booking when slug != null => '/$slug',
    _ => null,
  };
}

// Bildirim satırları şube saat dilimi taşımaz; varsayılan dilim.
const _istanbul = 'Europe/Istanbul';

/// "Şimdi" (bir saatten yeni), "Bugün", "Dün" veya "12 Kas".
String bildirimZamani(DateTime createdUtc, Clock clock) {
  if (clock.now().difference(createdUtc).inMinutes < 60) {
    return 'Şimdi';
  }

  final day = toBranchDate(createdUtc, _istanbul);
  return switch (daysBetween(day, branchToday(clock, _istanbul))) {
    0 => 'Bugün',
    1 => 'Dün',
    _ => formatDate(day, DateStyle.short).split(' ').take(2).join(' '),
  };
}

/// Bildirimler (Bildirimler, BildirimlerBos): açılınca hepsi okundu sayılır
/// (noktalar bu açılışta kalır); satır hedef ekranı açar.
class BildirimlerEkrani extends ConsumerStatefulWidget {
  /// Ekran.
  const BildirimlerEkrani({super.key});

  @override
  ConsumerState<BildirimlerEkrani> createState() => _BildirimlerEkraniState();
}

class _BildirimlerEkraniState extends ConsumerState<BildirimlerEkrani> {
  bool _marked = false;

  void _markRead(MeNotificationPage page) {
    if (_marked || page.unreadCount == 0) {
      return;
    }

    _marked = true;
    unawaited(
      ref
          .read(notificationsRepositoryProvider)
          .markAllRead()
          .then((_) => ref.invalidate(unreadCountProvider), onError: (_) {}),
    );
  }

  @override
  Widget build(BuildContext context) {
    final page = ref.watch(notificationsProvider);
    if (page case AsyncData(:final value)) {
      _markRead(value);
    }

    return IcSayfa(
      baslik: 'Bildirimler',
      onRefresh: () => ref.refresh(notificationsProvider.future),
      children: switch (page) {
        AsyncData(:final value) when value.items.isEmpty => [
          OmvilaBosDurum(
            ikon: LucideIcons.bell,
            baslik: 'Bildiriminiz yok',
            aciklama:
                'Randevu onayları, hatırlatmalar ve bekleme listesinde '
                'boşalan saatler burada görünür.',
            eylemEtiketi: 'Bildirim ayarları',
            eylemIkonu: LucideIcons.settings,
            onEylem: () =>
                ref.read(notificationPrefsProvider.notifier).openSettings(),
          ),
        ],
        AsyncData(:final value) => [
          for (final n in value.items) ...[
            BildirimSatiri(
              bildirim: n,
              onTap: switch (bildirimHedefi(n.target)) {
                final target? => () => context.push(target),
                null => null,
              },
            ),
            const SizedBox(height: OmvilaBosluk.bosluk2),
          ],
        ],
        AsyncError(:final error) => [
          HataDurumu(
            error: error,
            onRetry: () => ref.invalidate(notificationsProvider),
          ),
        ],
        _ => const [
          Padding(
            padding: EdgeInsets.only(top: OmvilaBosluk.bosluk12),
            child: YukleniyorDurumu(),
          ),
        ],
      },
    );
  }
}

/// Bildirim satırı: olay ikonu, başlık, metin, zaman, okunmadıysa nokta.
class BildirimSatiri extends ConsumerWidget {
  /// Satır.
  const BildirimSatiri({required this.bildirim, this.onTap, super.key});

  /// Bildirim.
  final MeNotification bildirim;

  /// Hedefe gider.
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final n = bildirim;
    final (ikon, zemin, renk) = switch (n.event) {
      MeNotificationEvent.appointmentPeriodConfirmed ||
      MeNotificationEvent.appointmentPeriodApproved ||
      MeNotificationEvent.depositPeriodReceived => (
        LucideIcons.calendarCheck,
        r.zeminMarkaYumusak,
        r.metinMarka,
      ),
      MeNotificationEvent.appointmentPeriodRequested => (
        LucideIcons.clock,
        r.bilgiZemin,
        r.bilgi,
      ),
      MeNotificationEvent.reviewPeriodInvite => (
        LucideIcons.star,
        r.avatar3Zemin,
        r.avatar3Metin,
      ),
      MeNotificationEvent.waitlistPeriodOffer => (
        LucideIcons.listChecks,
        r.zeminMarkaYumusak,
        r.metinMarka,
      ),
      MeNotificationEvent.appointmentPeriodCancelledBySalon ||
      MeNotificationEvent.appointmentPeriodDeclined ||
      MeNotificationEvent.appointmentPeriodApprovalExpired => (
        LucideIcons.calendarX,
        r.hataZemin,
        r.hata,
      ),
      MeNotificationEvent.appointmentPeriodRescheduled => (
        LucideIcons.calendarClock,
        r.zeminIkincil,
        r.metin,
      ),
      _ => (LucideIcons.bell, r.zeminIkincil, r.metin),
    };
    final unread = n.readUtc == null;
    final radius = BorderRadius.circular(OmvilaKose.koseLg);
    return Material(
      color: r.zeminYukseltilmis,
      borderRadius: radius,
      child: InkWell(
        borderRadius: radius,
        onTap: onTap,
        child: Padding(
          padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              IkonKutusu(ikon, zemin: zemin, renk: renk),
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
                          bildirimZamani(
                            n.createdUtc,
                            ref.watch(clockProvider),
                          ),
                          style: OmvilaYazi.aciklama.copyWith(
                            color: r.metinIkincil,
                          ),
                        ),
                        if (unread) ...[
                          const SizedBox(width: OmvilaBosluk.bosluk2),
                          Semantics(
                            label: 'Okunmadı',
                            child: Container(
                              margin: const EdgeInsets.only(top: 4),
                              width: 8,
                              height: 8,
                              decoration: BoxDecoration(
                                color: r.vurgu,
                                shape: BoxShape.circle,
                              ),
                            ),
                          ),
                        ],
                      ],
                    ),
                    Text(
                      n.body,
                      style: OmvilaYazi.ikincil.copyWith(
                        color: r.metinIkincil,
                      ),
                    ),
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

/// Ana sayfa zili; okunmamış varsa nokta.
class BildirimZili extends ConsumerWidget {
  /// Zil.
  const BildirimZili({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final unread = ref.watch(unreadCountProvider).value ?? 0;
    return Material(
      color: r.zeminYukseltilmis,
      borderRadius: BorderRadius.circular(OmvilaKose.koseMd),
      child: InkWell(
        borderRadius: BorderRadius.circular(OmvilaKose.koseMd),
        onTap: () async {
          await context.push(Routes.bildirimler);
          ref.invalidate(unreadCountProvider);
        },
        child: Semantics(
          button: true,
          label: unread > 0 ? 'Bildirimler, $unread okunmamış' : 'Bildirimler',
          excludeSemantics: true,
          child: SizedBox(
            width: OmvilaBoyut.dokunmaMin,
            height: OmvilaBoyut.dokunmaMin,
            child: Stack(
              alignment: Alignment.center,
              children: [
                Icon(LucideIcons.bell, size: 20, color: r.metin),
                if (unread > 0)
                  Positioned(
                    top: 10,
                    right: 11,
                    child: Container(
                      width: 8,
                      height: 8,
                      decoration: BoxDecoration(
                        color: r.vurgu,
                        shape: BoxShape.circle,
                        border: Border.all(color: r.zeminYukseltilmis),
                      ),
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
