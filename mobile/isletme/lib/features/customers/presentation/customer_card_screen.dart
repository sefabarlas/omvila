import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_isletme/features/appointments/data/appointment_repository.dart';
import 'package:omvila_isletme/features/appointments/presentation/appointment_format.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_isletme/features/calendar/presentation/calendar_screen.dart';
import 'package:omvila_isletme/features/customers/data/customer_repository.dart';
import 'package:omvila_isletme/features/customers/presentation/customers_screen.dart';
import 'package:omvila_isletme/features/shell/presentation/tabs.dart';
import 'package:omvila_isletme/shared/api_date.dart';
import 'package:omvila_isletme/shared/ui.dart';
import 'package:omvila_isletme/shared/widgets.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Geçmişte "Tümü"nden önce gösterilen satır sayısı (yerleşim, iş kuralı
/// değil).
const _visibleHistory = 2;

/// Müşteri kartı (MusteriKarti, MusteriKartiYeni): sayaçlar, "Randevu ver",
/// Ara/SMS (maskeli numara arama anında açılır), etkin paket, not, geçmiş.
/// Not düzenleme ve KVKK izinleri için manage uç yok (T-111): not salt
/// okunur, izinler gösterilmez.
class CustomerCardScreen extends ConsumerWidget {
  /// Kart.
  const CustomerCardScreen({required this.customerId, super.key});

  /// BrandCustomer kimliği.
  final String customerId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final customer = ref.watch(customerProvider(customerId));
    final data = customer.value;
    final Widget body;
    if (data != null) {
      body = _CardContent(customer: data);
    } else if (customer.hasError) {
      body = LoadError(
        title: 'Müşteri kartı yüklenemedi',
        error: customer.error,
        icon: LucideIcons.user,
        onRetry: () => ref.invalidate(customerProvider(customerId)),
      );
    } else {
      body = const Loading();
    }

    return Scaffold(
      backgroundColor: context.omvilaRenkler.zemin,
      body: SafeArea(
        child: ListView(
          padding: const EdgeInsets.fromLTRB(
            OmvilaBosluk.bosluk4,
            OmvilaBosluk.bosluk2,
            OmvilaBosluk.bosluk4,
            OmvilaBosluk.bosluk8,
          ),
          children: [
            Align(
              alignment: Alignment.centerLeft,
              child: IconButton(
                tooltip: 'Geri',
                onPressed: () => context.canPop()
                    ? context.pop()
                    : context.go(Routes.customers),
                icon: Icon(
                  LucideIcons.arrowLeft,
                  color: context.omvilaRenkler.metin,
                ),
              ),
            ),
            body,
          ],
        ),
      ),
    );
  }
}

class _CardContent extends ConsumerStatefulWidget {
  const _CardContent({required this.customer});

  final CustomerSummary customer;

  @override
  ConsumerState<_CardContent> createState() => _CardContentState();
}

class _CardContentState extends ConsumerState<_CardContent> {
  bool _busy = false;
  bool _allHistory = false;

  /// Numara yanıtta varsa (customer.phone.view) doğrudan; yoksa
  /// manageRevealCustomerPhone ile açılır. Numara ekrana yazılmaz.
  Future<void> _contact(PhoneRevealRequestPurposeEnum purpose) async {
    setState(() => _busy = true);
    final messenger = ScaffoldMessenger.of(context);
    final c = widget.customer;
    try {
      final phone =
          c.phone ??
          await ref.read(appointmentActionsProvider).revealPhone(c.id, purpose);
      final opened = await ref
          .read(externalLauncherProvider)
          .open(
            Uri(
              scheme: purpose == PhoneRevealRequestPurposeEnum.sms
                  ? 'sms'
                  : 'tel',
              path: phone,
            ),
          );
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
    final c = widget.customer;
    final session = ref.watch(authControllerProvider).value;
    final today = branchToday(ref.watch(clockProvider), defaultTimeZone);
    final history = ref.watch(customerAppointmentsProvider(c.id));
    final packages = ref.watch(customerPackagesProvider(c.id)).value;
    final active = packages
        ?.where((p) => p.status == CustomerPackageStatus.active)
        .firstOrNull;
    final firstVisit = c.visitCount == 0;
    final phone = customerPhone(c);
    final notes = c.notes;
    final total = c.totalSpent;
    final next = c.nextAppointmentUtc;
    final allRows = history.value ?? const <CustomerAppointmentRow>[];
    // İlk ziyaretin randevusu üstteki kartta; geçmişte tekrar edilmez.
    final rows = [
      for (final a in allRows)
        if (!firstVisit || a.startUtc != next) a,
    ];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Row(
          children: [
            InitialsAvatar(name: c.fullName, size: 56),
            const SizedBox(width: OmvilaBosluk.bosluk4),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Semantics(
                    header: true,
                    child: Text(
                      c.fullName,
                      style: OmvilaYazi.baslik1.copyWith(color: r.metin),
                    ),
                  ),
                  const SizedBox(height: OmvilaBosluk.bosluk1),
                  Wrap(
                    spacing: OmvilaBosluk.bosluk2,
                    runSpacing: OmvilaBosluk.bosluk1,
                    crossAxisAlignment: WrapCrossAlignment.center,
                    children: [
                      for (final tag in c.tags) TagPill(tag),
                      if (phone != null)
                        Text(
                          phone,
                          style: OmvilaYazi.aciklama.copyWith(
                            color: r.metinIkincil,
                          ),
                        ),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
        const SizedBox(height: OmvilaBosluk.bosluk4),
        if (firstVisit && next != null)
          _FirstVisit(
            when:
                '${relativeDay(toBranchDate(next, defaultTimeZone), today)} '
                '${formatTime(next, defaultTimeZone)}',
            detail: allRows
                .where((a) => a.startUtc == next)
                .map(
                  (a) =>
                      '${a.serviceNames.join(', ')} · '
                      '${a.staffNames.map(firstNameOf).join(', ')}',
                )
                .firstOrNull,
          )
        else if (!firstVisit)
          IntrinsicHeight(
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              spacing: OmvilaBosluk.bosluk2,
              children: [
                Expanded(
                  child: StatTile(
                    label: 'Ziyaret',
                    value: formatCount(c.visitCount),
                    tone: BentoTonu.marka,
                  ),
                ),
                Expanded(
                  child: StatTile(
                    label: 'Toplam',
                    value: total == null ? '—' : money(total),
                  ),
                ),
                Expanded(
                  child: StatTile(
                    label: 'Gelmedi',
                    value: formatCount(c.noShowCount),
                  ),
                ),
              ],
            ),
          ),
        const SizedBox(height: OmvilaBosluk.bosluk3),
        Row(
          spacing: OmvilaBosluk.bosluk2,
          children: [
            if (session?.can('appointment.create') ?? false)
              Expanded(
                child: OmvilaButon(
                  etiket: 'Randevu ver',
                  ikon: LucideIcons.calendarPlus,
                  tamGenislik: true,
                  onPressed: () =>
                      context.push(Routes.newAppointment, extra: c),
                ),
              )
            else
              const Spacer(),
            if (phone != null) ...[
              _SquareButton(
                icon: LucideIcons.phone,
                tooltip: 'Ara',
                onPressed: _busy
                    ? null
                    : () => _contact(PhoneRevealRequestPurposeEnum.call),
              ),
              _SquareButton(
                icon: LucideIcons.messageSquare,
                tooltip: 'SMS',
                onPressed: _busy
                    ? null
                    : () => _contact(PhoneRevealRequestPurposeEnum.sms),
              ),
            ],
          ],
        ),
        if (active != null) ...[
          const SizedBox(height: OmvilaBosluk.bosluk5),
          const SectionHeader('Paket'),
          const SizedBox(height: OmvilaBosluk.bosluk2),
          _PackageCard(package: active),
        ],
        const SizedBox(height: OmvilaBosluk.bosluk5),
        const SectionHeader('Not'),
        const SizedBox(height: OmvilaBosluk.bosluk2),
        if (notes != null && notes.trim().isNotEmpty)
          SurfaceCard(
            child: Text(
              notes,
              style: OmvilaYazi.govde.copyWith(color: r.metin),
            ),
          ),
        const SizedBox(height: OmvilaBosluk.bosluk2),
        Text(
          'Notlar yalnızca salon ekibine görünür. Sağlık bilgisi yazmayın.',
          style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk5),
        SectionHeader(
          'Geçmiş',
          action: rows.length > _visibleHistory && !_allHistory ? 'Tümü' : null,
          onAction: () => setState(() => _allHistory = true),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk2),
        if (history.hasError && rows.isEmpty)
          LoadError(
            title: 'Geçmiş yüklenemedi',
            error: history.error,
            onRetry: () => ref.invalidate(customerAppointmentsProvider(c.id)),
          )
        else if (history.isLoading && rows.isEmpty)
          const Loading()
        else if (rows.isEmpty)
          Text(
            'Henüz tamamlanan randevusu yok. İlk randevudan sonra hizmetler '
            've ödemeler burada listelenir.',
            style: OmvilaYazi.govde.copyWith(color: r.metinIkincil),
          )
        else
          for (final a in _allHistory ? rows : rows.take(_visibleHistory)) ...[
            _HistoryRow(row: a, today: today),
            const SizedBox(height: OmvilaBosluk.bosluk2),
          ],
      ],
    );
  }
}

class _FirstVisit extends StatelessWidget {
  const _FirstVisit({required this.when, required this.detail});

  final String when;
  final String? detail;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final line = detail;
    return SurfaceCard(
      color: r.blokKum,
      child: Row(
        children: [
          Icon(LucideIcons.sparkles, color: r.blokKumMetin),
          const SizedBox(width: OmvilaBosluk.bosluk3),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'İlk ziyareti $when',
                  style: OmvilaYazi.govdeGuclu.copyWith(
                    color: r.blokKumMetin,
                  ),
                ),
                if (line != null)
                  Text(
                    line,
                    style: OmvilaYazi.aciklama.copyWith(
                      color: r.blokKumMetin,
                    ),
                  ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _SquareButton extends StatelessWidget {
  const _SquareButton({
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
      tooltip: tooltip,
      onPressed: onPressed,
      style: IconButton.styleFrom(
        backgroundColor: r.zeminIkincil,
        foregroundColor: r.metin,
        minimumSize: const Size.square(OmvilaBoyut.kontrolMd),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(OmvilaKose.koseMd),
        ),
      ),
      icon: Icon(icon, size: 20),
    );
  }
}

class _PackageCard extends ConsumerWidget {
  const _PackageCard({required this.package});

  final CustomerPackage package;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final p = package;
    final branches = ref.watch(authControllerProvider).value?.branches ?? [];
    final branch = branches.where((b) => b.id == p.branchId).firstOrNull;
    final used = p.totalSessions - p.remainingSessions;
    return SurfaceCard(
      color: r.blokKum,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            p.packageName,
            style: OmvilaYazi.baslik3.copyWith(color: r.blokKumMetin),
          ),
          if (branch != null)
            Text(
              branch.name,
              style: OmvilaYazi.aciklama.copyWith(color: r.blokKumMetin),
            ),
          const SizedBox(height: OmvilaBosluk.bosluk2),
          Text.rich(
            TextSpan(
              text: '${p.remainingSessions}',
              style: OmvilaYazi.metrik.copyWith(color: r.blokKumMetin),
              children: [
                TextSpan(
                  text: ' / ${p.totalSessions} seans kaldı',
                  style: OmvilaYazi.govde.copyWith(
                    color: r.blokKumMetin,
                    letterSpacing: 0,
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(height: OmvilaBosluk.bosluk2),
          ExcludeSemantics(
            child: Row(
              spacing: OmvilaBosluk.bosluk1,
              children: [
                for (var i = 0; i < p.totalSessions; i++)
                  Expanded(
                    child: Container(
                      height: 6,
                      decoration: BoxDecoration(
                        color: i < used ? r.cizgiGuclu : r.blokMarka,
                        borderRadius: BorderRadius.circular(
                          OmvilaKose.koseTam,
                        ),
                      ),
                    ),
                  ),
              ],
            ),
          ),
          const SizedBox(height: OmvilaBosluk.bosluk3),
          Row(
            children: [
              Icon(LucideIcons.clock, size: 14, color: r.blokKumMetin),
              const SizedBox(width: OmvilaBosluk.bosluk1),
              Text(
                'Son kullanım '
                '${formatDate(localDateOf(p.expiresOn), DateStyle.dayMonth)}',
                style: OmvilaYazi.aciklama.copyWith(color: r.blokKumMetin),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

class _HistoryRow extends StatelessWidget {
  const _HistoryRow({required this.row, required this.today});

  final CustomerAppointmentRow row;
  final String today;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final day = relativeDay(toBranchDate(row.startUtc, defaultTimeZone), today);
    final staff = row.staffNames.map(firstNameOf).join(', ');
    final durum = RandevuDurumu.fromDisplayStatus(row.displayStatus);
    final status = durum == null ? null : OmvilaDurumRozeti(durum: durum);
    return SurfaceCard(
      padding: EdgeInsets.zero,
      child: InkWell(
        borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
        onTap: () => context.push(Routes.appointment(row.appointmentId)),
        child: Padding(
          padding: const EdgeInsets.all(OmvilaBosluk.bosluk3 + 2),
          child: Row(
            children: [
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      row.serviceNames.join(', '),
                      style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
                    ),
                    Text(
                      [
                        day,
                        if (staff.isNotEmpty) staff,
                        money(row.total),
                      ].join(' · '),
                      style: OmvilaYazi.aciklama.copyWith(
                        color: r.metinIkincil,
                      ),
                    ),
                  ],
                ),
              ),
              ?status,
            ],
          ),
        ),
      ),
    );
  }
}
