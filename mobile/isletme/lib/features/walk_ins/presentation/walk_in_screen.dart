import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_isletme/features/shell/presentation/app_shell.dart';
import 'package:omvila_isletme/features/today/data/today_repository.dart';
import 'package:omvila_isletme/features/walk_ins/data/walk_in_repository.dart';
import 'package:omvila_isletme/features/walk_ins/presentation/walk_in_sheets.dart';
import 'package:omvila_isletme/shared/ui.dart';
import 'package:omvila_isletme/shared/widgets.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Kapıda sıra (Sira, SiraBos): bekleyen sayısı, yeni gelen için tahmini
/// bekleme, sıradaki kişi "Başlat", diğerleri "…" → "Sıradan çıkar".
/// İzin walkin.manage (personel dahil). Numara her gün 1'den.
class WalkInScreen extends ConsumerWidget {
  /// Sıra.
  const WalkInScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final session = ref.watch(authControllerProvider).value;
    if (session == null || session.branches.isEmpty) {
      return const SizedBox.shrink();
    }

    final branchId = session.branches.first.id;
    final queue = ref.watch(walkInQueueProvider(branchId));
    final data = queue.value;

    Future<void> add() async {
      final added = await showWalkInAddSheet(
        context,
        branchId: branchId,
        queue: data,
      );
      if (added != null && context.mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(
              '${added.customerName} sıraya eklendi · No ${added.queueNumber}',
            ),
          ),
        );
        ref
          ..invalidate(walkInQueueProvider(branchId))
          ..invalidate(todayProvider);
      }
    }

    final Widget body;
    if (data != null) {
      body = data.items.isEmpty
          ? OmvilaBosDurum(
              ikon: LucideIcons.users,
              baslik: 'Sırada kimse yok',
              aciklama:
                  'Randevusu olmadan gelen müşteriyi sıraya alın. Sırası '
                  'yaklaşınca SMS ile haber verilir.',
              eylemEtiketi: 'Sıraya ekle',
              eylemIkonu: LucideIcons.plus,
              onEylem: add,
              eylemTuru: OmvilaButonTuru.birincil,
            )
          : _QueueContent(queue: data, branchId: branchId);
    } else if (queue.hasError) {
      body = LoadError(
        title: 'Sıra yüklenemedi',
        error: queue.error,
        icon: LucideIcons.users,
        onRetry: () => ref.invalidate(walkInQueueProvider(branchId)),
      );
    } else {
      body = const Loading();
    }

    final hasItems = data?.items.isNotEmpty ?? false;
    return Scaffold(
      backgroundColor: context.omvilaRenkler.zemin,
      body: SafeArea(
        bottom: false,
        child: Column(
          children: [
            Expanded(
              child: RefreshIndicator(
                onRefresh: () =>
                    ref.refresh(walkInQueueProvider(branchId).future),
                child: ListView(
                  padding: const EdgeInsets.fromLTRB(
                    OmvilaBosluk.bosluk4,
                    OmvilaBosluk.bosluk4,
                    OmvilaBosluk.bosluk4,
                    OmvilaBosluk.bosluk6,
                  ),
                  children: [
                    const ScreenTitle(
                      eyebrow: 'Randevusuz gelenler',
                      title: 'Sıra',
                      showBack: true,
                    ),
                    const SizedBox(height: OmvilaBosluk.bosluk4),
                    body,
                  ],
                ),
              ),
            ),
            if (hasItems)
              Padding(
                padding: EdgeInsets.fromLTRB(
                  OmvilaBosluk.bosluk4,
                  0,
                  OmvilaBosluk.bosluk4,
                  session.isStaff ? tabBarClearance : OmvilaBosluk.bosluk6,
                ),
                child: OmvilaButon(
                  etiket: 'Sıraya ekle',
                  ikon: LucideIcons.userPlus,
                  boyut: OmvilaButonBoyutu.lg,
                  tamGenislik: true,
                  onPressed: add,
                ),
              ),
          ],
        ),
      ),
    );
  }
}

/// Bekleme süresi: sunucunun "şimdi"sine göre (cihaz saati yok).
int waitedMinutes(WalkInQueue queue, WalkInTicketView ticket) {
  final minutes = queue.generatedUtc.difference(ticket.joinedUtc).inMinutes;
  return minutes < 0 ? 0 : minutes;
}

/// "Saç kesimi · Mert".
String ticketDetail(WalkInTicketView t) {
  final staff = t.preferredStaffName?.split(' ').first ?? 'Fark etmez';
  final service = t.serviceName;
  return service == null ? staff : '$service · $staff';
}

class _QueueContent extends ConsumerWidget {
  const _QueueContent({required this.queue, required this.branchId});

  final WalkInQueue queue;
  final String branchId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final wait = queue.estimatedWaitMinutesForNew;
    final first = queue.items.first;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        IntrinsicHeight(
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            spacing: OmvilaBosluk.bosluk3,
            children: [
              Expanded(
                child: BentoBlok(
                  ton: BentoTonu.kum,
                  ustEtiket: 'Bekleyen',
                  deger: formatCount(queue.waitingCount),
                  aciklama: 'kişi',
                ),
              ),
              Expanded(
                child: StatTile(
                  label: 'Yeni gelen için',
                  value: wait == null ? '—' : '~$wait dk',
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk5),
        const SectionHeader('Sıradaki'),
        const SizedBox(height: OmvilaBosluk.bosluk2),
        _TicketCard(
          ticket: first,
          detail:
              '${ticketDetail(first)} · '
              "${waitedMinutes(queue, first)} dk'dır bekliyor",
          highlighted: true,
          branchId: branchId,
        ),
        for (final t in queue.items.skip(1)) ...[
          const SizedBox(height: OmvilaBosluk.bosluk2),
          _TicketCard(
            ticket: t,
            detail: '${ticketDetail(t)} · ${waitedMinutes(queue, t)} dk',
            highlighted: false,
            branchId: branchId,
          ),
        ],
        const SizedBox(height: OmvilaBosluk.bosluk3),
        Text(
          'Başlattığınızda kişi takvime randevu olarak işlenir, kanalı '
          '"Kapıdan" görünür.',
          style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
        ),
      ],
    );
  }
}

class _TicketCard extends ConsumerStatefulWidget {
  const _TicketCard({
    required this.ticket,
    required this.detail,
    required this.highlighted,
    required this.branchId,
  });

  final WalkInTicketView ticket;
  final String detail;
  final bool highlighted;
  final String branchId;

  @override
  ConsumerState<_TicketCard> createState() => _TicketCardState();
}

class _TicketCardState extends ConsumerState<_TicketCard> {
  bool _busy = false;

  void _refresh() => ref
    ..invalidate(walkInQueueProvider(widget.branchId))
    ..invalidate(todayProvider);

  Future<void> _start() async {
    final session = ref.read(authControllerProvider).value!;
    final t = widget.ticket;
    var serviceId = t.serviceId;
    // Personel yalnız kendine başlatır.
    var staffId = session.isStaff
        ? session.membership.staffId
        : t.preferredStaffId;
    if (serviceId == null || staffId == null) {
      final choice = await showWalkInStartSheet(
        context,
        branchId: widget.branchId,
        ticket: t,
        askService: serviceId == null,
        askStaff: staffId == null,
      );
      if (choice == null) {
        return;
      }

      serviceId = choice.serviceId;
      staffId = choice.staffId;
    }

    await _run(() async {
      await ref
          .read(walkInActionsProvider)
          .start(
            t,
            serviceId: t.serviceId == null ? serviceId : null,
            staffId: t.preferredStaffId == staffId ? null : staffId,
          );
      return '${t.customerName} randevuya çevrildi.';
    });
  }

  Future<void> _leave() => _run(() async {
    await ref.read(walkInActionsProvider).leave(widget.ticket);
    return '${widget.ticket.customerName} sıradan çıkarıldı.';
  });

  Future<void> _run(Future<String> Function() action) async {
    setState(() => _busy = true);
    final messenger = ScaffoldMessenger.of(context);
    try {
      final message = await action();
      messenger.showSnackBar(SnackBar(content: Text(message)));
    } on ApiProblem catch (e) {
      messenger.showSnackBar(SnackBar(content: Text(errorMessage(e.code))));
    } finally {
      if (mounted) {
        setState(() => _busy = false);
      }
      _refresh();
    }
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final t = widget.ticket;
    final dark = widget.highlighted;
    final foreground = dark ? r.metinTers : r.metin;
    final secondary = dark ? r.metinTers : r.metinIkincil;
    return SurfaceCard(
      color: dark ? r.blokMarka : null,
      padding: const EdgeInsets.symmetric(
        horizontal: OmvilaBosluk.bosluk4,
        vertical: OmvilaBosluk.bosluk3,
      ),
      child: Row(
        children: [
          Container(
            width: 36,
            height: 36,
            alignment: Alignment.center,
            decoration: BoxDecoration(
              // Koyu kartta iki temada da `metin-ters` örtüsü (tasarım);
              // `marka-hover` koyu temada açık nane olduğundan numara
              // okunmuyordu.
              color: dark
                  ? r.metinTers.withValues(alpha: 0.16)
                  : r.zeminIkincil,
              borderRadius: BorderRadius.circular(OmvilaKose.koseMd),
            ),
            child: Text(
              '${t.queueNumber}',
              semanticsLabel: 'Sıra ${t.queueNumber}',
              style: OmvilaYazi.govdeGuclu.copyWith(color: foreground),
            ),
          ),
          const SizedBox(width: OmvilaBosluk.bosluk3),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  t.customerName,
                  style: OmvilaYazi.govdeGuclu.copyWith(color: foreground),
                ),
                Text(
                  widget.detail,
                  style: OmvilaYazi.aciklama.copyWith(color: secondary),
                ),
              ],
            ),
          ),
          if (dark)
            // Koyu kart üstünde açık zeminli buton (tasarım); iki temada
            // da açık kalan `metin-ters` zemin, `blok-marka` yazı.
            FilledButton(
              onPressed: _busy ? null : _start,
              style: FilledButton.styleFrom(
                backgroundColor: r.metinTers,
                foregroundColor: r.blokMarka,
                minimumSize: const Size(0, OmvilaBoyut.kontrolSm),
                textStyle: OmvilaYazi.etiket,
              ),
              child: Text(_busy ? 'Başlatılıyor…' : 'Başlat'),
            ),
          PopupMenuButton<VoidCallback>(
            tooltip: '${t.customerName} için seçenekler',
            enabled: !_busy,
            icon: Icon(LucideIcons.ellipsis, color: foreground),
            onSelected: (action) => action(),
            itemBuilder: (_) => [
              if (!dark)
                PopupMenuItem(value: _start, child: const Text('Başlat')),
              PopupMenuItem(value: _leave, child: const Text('Sıradan çıkar')),
            ],
          ),
        ],
      ),
    );
  }
}
