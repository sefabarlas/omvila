import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_isletme/features/profile/data/profile_repository.dart';
import 'package:omvila_isletme/features/shell/presentation/app_shell.dart';
import 'package:omvila_isletme/features/shell/presentation/tabs.dart';
import 'package:omvila_isletme/features/today/presentation/today_format.dart';
import 'package:omvila_isletme/shared/api_date.dart';
import 'package:omvila_isletme/shared/ui.dart';
import 'package:omvila_isletme/shared/widgets.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// "Son primler"de "Tümü"nden önce gösterilen kayıt sayısı (yerleşim).
const _visibleEntries = 2;

/// Rol adı (salon paneliyle aynı).
String roleLabel(ManageSessionMembershipRoleEnum role) => switch (role) {
  ManageSessionMembershipRoleEnum.owner => 'İşletme sahibi',
  ManageSessionMembershipRoleEnum.branchManager => 'Şube yöneticisi',
  _ => 'Personel',
};

/// Profil: kişisel prim özeti (commission.view ve personel kaydı varsa;
/// yoksa bölüm gizli), bildirim ayarları, "Çıkış yap". Yalnız kendi primi
/// (KK10, K2).
class ProfileScreen extends ConsumerWidget {
  /// Profil.
  const ProfileScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final session = ref.watch(authControllerProvider).value;
    if (session == null) {
      return const SizedBox.shrink();
    }

    final canSeeCommission = session.can('commission.view');
    final commission = canSeeCommission
        ? ref.watch(myCommissionProvider)
        : const AsyncData<MyCommissionSummary?>(null);
    final summary = commission.value;
    final branch = session.branches.isEmpty
        ? null
        : session.branches.first.name;

    final List<Widget> body;
    if (summary != null) {
      body = [_Commission(summary: summary)];
    } else if (commission.hasError) {
      body = [
        LoadError(
          title: 'Primim yüklenemedi',
          error: commission.error,
          icon: LucideIcons.wallet,
          onRetry: () => ref.invalidate(myCommissionProvider),
        ),
      ];
    } else if (commission.isLoading) {
      body = const [Loading()];
    } else {
      body = const [];
    }

    return SafeArea(
      bottom: false,
      child: RefreshIndicator(
        onRefresh: () async {
          if (canSeeCommission) {
            ref.invalidate(myCommissionProvider);
            await ref.read(myCommissionProvider.future);
          }
        },
        child: ListView(
          padding: const EdgeInsets.fromLTRB(
            OmvilaBosluk.bosluk4,
            OmvilaBosluk.bosluk6,
            OmvilaBosluk.bosluk4,
            tabBarClearance,
          ),
          children: [
            Row(
              children: [
                InitialsAvatar(name: session.user.fullName, size: 56),
                const SizedBox(width: OmvilaBosluk.bosluk3),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Semantics(
                        header: true,
                        child: Text(
                          session.user.fullName,
                          style: OmvilaYazi.baslik1.copyWith(color: r.metin),
                        ),
                      ),
                      Text(
                        [
                          roleLabel(session.membership.role),
                          ?branch,
                        ].join(' · '),
                        style: OmvilaYazi.ikincil.copyWith(
                          color: r.metinIkincil,
                        ),
                      ),
                    ],
                  ),
                ),
                IconButton(
                  tooltip: 'Bildirim ayarları',
                  onPressed: () => context.push(Routes.notificationSettings),
                  style: IconButton.styleFrom(
                    backgroundColor: r.zeminIkincil,
                    foregroundColor: r.metin,
                    minimumSize: const Size.square(44),
                  ),
                  icon: const Icon(LucideIcons.settings, size: 20),
                ),
              ],
            ),
            const SizedBox(height: OmvilaBosluk.bosluk4),
            ...body,
            const SizedBox(height: OmvilaBosluk.bosluk6),
            OmvilaButon(
              etiket: 'Çıkış yap',
              ikon: LucideIcons.logOut,
              tur: OmvilaButonTuru.ikincil,
              tamGenislik: true,
              onPressed: () =>
                  ref.read(authControllerProvider.notifier).logout(),
            ),
          ],
        ),
      ),
    );
  }
}

/// "1–25 Kasım" (aynı ay) veya "28 Ekim–3 Kasım".
String periodLabel(String from, String to) {
  final end = formatDate(to, DateStyle.dayMonth);
  return from.substring(0, 7) == to.substring(0, 7)
      ? '${formatDate(from, DateStyle.day)}–$end'
      : '${formatDate(from, DateStyle.dayMonth)}–$end';
}

class _Commission extends ConsumerStatefulWidget {
  const _Commission({required this.summary});

  final MyCommissionSummary summary;

  @override
  ConsumerState<_Commission> createState() => _CommissionState();
}

class _CommissionState extends ConsumerState<_Commission> {
  bool _all = false;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final s = widget.summary;
    final currency = s.currency.value;
    final rating = s.averageRating;
    final noShow = s.noShowPercent;
    final next = s.nextAccrual;
    final today = branchToday(ref.watch(clockProvider), s.timeZone);
    final entries = _all ? s.entries : s.entries.take(_visibleEntries);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        SurfaceCard(
          color: r.blokMarka,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Expanded(
                    child: Text(
                      toUpperTr('${commissionLabel(s.month)}m'),
                      style: OmvilaYazi.aciklama.copyWith(
                        color: r.metinTers,
                        fontWeight: FontWeight.w700,
                        letterSpacing: 0.6,
                      ),
                    ),
                  ),
                  Text(
                    periodLabel(localDateOf(s.from), localDateOf(s.to)),
                    style: OmvilaYazi.aciklama.copyWith(color: r.metinTers),
                  ),
                ],
              ),
              const SizedBox(height: OmvilaBosluk.bosluk2),
              Text(
                formatMoney(s.totalMinor, currency: currency),
                style: OmvilaYazi.metrik.copyWith(color: r.metinTers),
              ),
              Text(
                'Hizmet geliri '
                '${formatMoney(s.serviceRevenueMinor, currency: currency)}',
                style: OmvilaYazi.aciklama.copyWith(color: r.metinTers),
              ),
            ],
          ),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk3),
        IntrinsicHeight(
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            spacing: OmvilaBosluk.bosluk2,
            children: [
              Expanded(
                child: StatTile(
                  label: 'Randevu',
                  value: formatCount(s.appointmentCount),
                  tone: BentoTonu.kum,
                ),
              ),
              Expanded(
                child: StatTile(
                  label: 'Puan',
                  value: rating == null
                      ? '—'
                      : rating.toStringAsFixed(1).replaceAll('.', ','),
                ),
              ),
              Expanded(
                child: StatTile(
                  label: 'Gelmedi',
                  value: noShow == null ? '—' : '%${noShow.round()}',
                ),
              ),
            ],
          ),
        ),
        if (s.thisWeek.isNotEmpty) ...[
          const SizedBox(height: OmvilaBosluk.bosluk3),
          _WeekChart(days: s.thisWeek, currency: currency),
        ],
        const SizedBox(height: OmvilaBosluk.bosluk5),
        SectionHeader(
          'Son primler',
          action: s.entries.length > _visibleEntries && !_all ? 'Tümü' : null,
          onAction: () => setState(() => _all = true),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk2),
        if (s.entries.isEmpty)
          Text(
            'Bu ay henüz prim yok.',
            style: OmvilaYazi.govde.copyWith(color: r.metinIkincil),
          ),
        for (final e in entries) ...[
          _EntryRow(entry: e, today: today, timeZone: s.timeZone),
          const SizedBox(height: OmvilaBosluk.bosluk2),
        ],
        Text(
          next == null
              ? 'Prim randevu tamamlandığında hak edilir.'
              : 'Prim randevu tamamlandığında hak edilir. '
                    '${next.customerName} randevusu bitince eklenecek.',
          style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
        ),
      ],
    );
  }
}

/// "Bu hafta": gün başına prim çubukları; en çok randevulu gün vurgulu.
class _WeekChart extends StatelessWidget {
  const _WeekChart({required this.days, required this.currency});

  final List<CommissionWeekDay> days;
  final String currency;

  static const _barHeight = 56.0;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final maxAmount = days.fold<int>(
      0,
      (m, d) => d.amountMinor > m ? d.amountMinor : m,
    );
    final busiest = days.reduce(
      (a, b) => b.appointmentCount > a.appointmentCount ? b : a,
    );
    final busiestLabel = busiest.appointmentCount == 0
        ? null
        : '${formatDate(localDateOf(busiest.date), DateStyle.weekdayShort)} '
              'en yoğun';
    return SurfaceCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Row(
            children: [
              Expanded(
                child: Text(
                  'Bu hafta',
                  style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
                ),
              ),
              if (busiestLabel != null)
                Text(
                  busiestLabel,
                  style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
                ),
            ],
          ),
          const SizedBox(height: OmvilaBosluk.bosluk3),
          Row(
            crossAxisAlignment: CrossAxisAlignment.end,
            spacing: OmvilaBosluk.bosluk2,
            children: [
              for (final d in days)
                Expanded(
                  child: Semantics(
                    label:
                        '${formatDate(localDateOf(d.date), DateStyle.weekday)} '
                        '${formatMoney(d.amountMinor, currency: currency)}',
                    excludeSemantics: true,
                    child: Column(
                      children: [
                        Container(
                          height: maxAmount == 0
                              ? 4
                              : 4 +
                                    (_barHeight - 4) *
                                        d.amountMinor /
                                        maxAmount,
                          decoration: BoxDecoration(
                            color:
                                identical(d, busiest) &&
                                    busiest.appointmentCount > 0
                                ? r.marka
                                : r.zeminMarkaYumusak,
                            borderRadius: BorderRadius.circular(
                              OmvilaKose.koseSm,
                            ),
                          ),
                        ),
                        const SizedBox(height: OmvilaBosluk.bosluk1),
                        Text(
                          formatDate(
                            localDateOf(d.date),
                            DateStyle.weekdayShort,
                          ),
                          style: OmvilaYazi.aciklama.copyWith(
                            color: r.metinIkincil,
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
            ],
          ),
        ],
      ),
    );
  }
}

class _EntryRow extends StatelessWidget {
  const _EntryRow({
    required this.entry,
    required this.today,
    required this.timeZone,
  });

  final MyCommissionEntry entry;
  final String today;
  final String timeZone;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final e = entry;
    final reversed = e.status == CommissionEntryStatus.reversed;
    final amount = formatMoney(e.amountMinor.abs());
    final day = relativeDay(toBranchDate(e.occurredUtc, timeZone), today);
    return SurfaceCard(
      child: Row(
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  e.customerName,
                  style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
                ),
                Text(
                  '$day ${formatTime(e.occurredUtc, timeZone)} · '
                  '${e.description}',
                  style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
                ),
              ],
            ),
          ),
          Text(
            reversed || e.amountMinor < 0 ? '−$amount' : '+$amount',
            style: OmvilaYazi.govdeGuclu.copyWith(
              color: reversed || e.amountMinor < 0 ? r.hata : r.basari,
            ),
          ),
        ],
      ),
    );
  }
}
