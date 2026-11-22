import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_isletme/features/calendar/presentation/calendar_screen.dart';
import 'package:omvila_isletme/features/customers/data/customer_repository.dart';
import 'package:omvila_isletme/features/customers/presentation/new_customer_sheet.dart';
import 'package:omvila_isletme/features/shell/presentation/app_shell.dart';
import 'package:omvila_isletme/features/shell/presentation/tabs.dart';
import 'package:omvila_isletme/shared/api_date.dart';
import 'package:omvila_isletme/shared/ui.dart';
import 'package:omvila_isletme/shared/widgets.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Müşteriler (Musteriler, MusterilerAramaYok, MusterilerBos). Toplam
/// müşteri sayısı sözleşmede olmadığı için üst satırda yalnız şube adı.
class CustomersScreen extends ConsumerStatefulWidget {
  /// Müşteriler.
  const CustomersScreen({
    this.initialQuery = '',
    this.initialFilter = CustomerFilter.all,
    super.key,
  });

  /// Başlangıç araması (golden: MusterilerAramaYok).
  final String initialQuery;

  /// Başlangıç süzgeci.
  final CustomerFilter initialFilter;

  @override
  ConsumerState<CustomersScreen> createState() => _CustomersScreenState();
}

class _CustomersScreenState extends ConsumerState<CustomersScreen> {
  late final _search = TextEditingController(text: widget.initialQuery);
  Timer? _debounce;
  late String _q = widget.initialQuery;
  late CustomerFilter _filter = widget.initialFilter;

  CustomerQuery get _query => (q: _q, filter: _filter);

  @override
  void dispose() {
    _debounce?.cancel();
    _search.dispose();
    super.dispose();
  }

  void _onSearch(String text) {
    _debounce?.cancel();
    _debounce = Timer(const Duration(milliseconds: 300), () {
      if (mounted) {
        setState(() => _q = text.trim());
      }
    });
  }

  void _clear() {
    _debounce?.cancel();
    _search.clear();
    setState(() {
      _q = '';
      _filter = CustomerFilter.all;
    });
  }

  Future<void> _add() async {
    final created = await showNewCustomerSheet(context, initial: _q);
    if (created != null && mounted) {
      ref.invalidate(customerListProvider);
      unawaited(context.push(Routes.customer(created)));
    }
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final session = ref.watch(authControllerProvider).value;
    final list = ref.watch(customerListProvider(_query));
    final page = list.value;
    final searching = _q.isNotEmpty || _filter != CustomerFilter.all;
    final branch = session == null || session.branches.isEmpty
        ? null
        : session.branches.first.name;

    final List<Widget> body;
    if (page != null && page.items.isNotEmpty) {
      body = [
        if (_filter != CustomerFilter.all) ...[
          SectionHeader(_filter.label),
          const SizedBox(height: OmvilaBosluk.bosluk2),
        ],
        for (final c in page.items) ...[
          _CustomerRow(customer: c),
          const SizedBox(height: OmvilaBosluk.bosluk2),
        ],
        if (page.nextCursor != null)
          OmvilaButon(
            etiket: 'Daha fazla göster',
            tur: OmvilaButonTuru.hayalet,
            yukleniyor: page.loadingMore,
            yukleniyorEtiketi: 'Yükleniyor…',
            onPressed: page.loadingMore
                ? null
                : () => ref
                      .read(customerListProvider(_query).notifier)
                      .loadMore()
                      .catchError((Object e) {
                        if (context.mounted) {
                          showProblem(context, e);
                        }
                      }),
          ),
      ];
    } else if (page != null && searching) {
      body = [
        OmvilaBosDurum(
          ikon: LucideIcons.search,
          baslik: _q.isEmpty ? 'Sonuç yok' : '“$_q” için sonuç yok',
          aciklama: _filter == CustomerFilter.all
              ? 'Ad ya da telefonun son 4 hanesiyle arayabilirsiniz.'
              : '“${_filter.label}” filtresi açık. Ad ya da telefonun son 4 '
                    'hanesiyle arayabilirsiniz.',
        ),
        OmvilaButon(
          etiket: 'Filtreyi temizle',
          ikon: LucideIcons.x,
          tur: OmvilaButonTuru.ikincil,
          tamGenislik: true,
          onPressed: _clear,
        ),
        const SizedBox(height: OmvilaBosluk.bosluk2),
        OmvilaButon(
          etiket: 'Yeni müşteri olarak ekle',
          ikon: LucideIcons.userPlus,
          tamGenislik: true,
          onPressed: _add,
        ),
      ];
    } else if (page != null) {
      body = [
        OmvilaBosDurum(
          ikon: LucideIcons.users,
          baslik: 'Henüz müşteriniz yok',
          aciklama:
              'Randevu alan herkes buraya kendiliğinden eklenir. Eski '
              'müşteri listenizi salon panelinden içe aktarabilirsiniz.',
          eylemEtiketi: 'Müşteri ekle',
          eylemIkonu: LucideIcons.userPlus,
          onEylem: _add,
        ),
      ];
    } else if (list.hasError) {
      body = [
        LoadError(
          title: 'Müşteriler yüklenemedi',
          error: list.error,
          icon: LucideIcons.users,
          onRetry: () => ref.invalidate(customerListProvider(_query)),
        ),
      ];
    } else {
      body = const [Loading()];
    }

    final empty = page != null && page.items.isEmpty && !searching;
    return SafeArea(
      bottom: false,
      child: RefreshIndicator(
        onRefresh: () => ref.refresh(customerListProvider(_query).future),
        child: ListView(
          padding: const EdgeInsets.fromLTRB(
            OmvilaBosluk.bosluk4,
            OmvilaBosluk.bosluk6,
            OmvilaBosluk.bosluk4,
            tabBarClearance,
          ),
          children: [
            ScreenTitle(
              eyebrow: branch,
              title: 'Müşteriler',
              trailing: IconButton.filled(
                tooltip: 'Müşteri ekle',
                onPressed: _add,
                style: IconButton.styleFrom(
                  backgroundColor: r.metin,
                  foregroundColor: r.zemin,
                  minimumSize: const Size.square(44),
                ),
                icon: const Icon(LucideIcons.userPlus, size: 20),
              ),
            ),
            if (!empty) ...[
              const SizedBox(height: OmvilaBosluk.bosluk4),
              TextField(
                controller: _search,
                onChanged: _onSearch,
                textInputAction: TextInputAction.search,
                decoration: const InputDecoration(
                  hintText: 'Ad veya telefon ara',
                  prefixIcon: Icon(LucideIcons.search, size: 20),
                ),
              ),
              const SizedBox(height: OmvilaBosluk.bosluk3),
              SingleChildScrollView(
                scrollDirection: Axis.horizontal,
                child: Row(
                  spacing: OmvilaBosluk.bosluk2,
                  children: [
                    for (final f in CustomerFilter.values.skip(1))
                      SelectChip(
                        label: f.label,
                        selected: _filter == f,
                        onTap: () => setState(
                          () => _filter = _filter == f ? CustomerFilter.all : f,
                        ),
                      ),
                  ],
                ),
              ),
            ],
            const SizedBox(height: OmvilaBosluk.bosluk4),
            ...body,
          ],
        ),
      ),
    );
  }
}

/// Satırın alt metni: yaklaşan randevu ("Bugün 10:30") yoksa son ziyaret
/// ("Son ziyaret 12 Kas"), ardından ziyaret sayısı.
String customerSubtitle(CustomerSummary c, String today) {
  final next = c.nextAppointmentUtc;
  final last = c.lastVisitOn;
  final when = next != null
      ? '${relativeDay(toBranchDate(next, defaultTimeZone), today)} '
            '${formatTime(next, defaultTimeZone)}'
      : last != null
      ? 'Son ziyaret ${relativeDay(localDateOf(last), today)}'
      : null;
  final visits = '${c.visitCount} ziyaret';
  return when == null ? visits : '$when · $visits';
}

/// Telefon: tam numara yalnız sunucu verirse; personel rolünde maskeli.
String? customerPhone(CustomerSummary c) {
  final phone = c.phone;
  return phone != null ? formatPhone(phone) : c.phoneMasked;
}

class _CustomerRow extends ConsumerWidget {
  const _CustomerRow({required this.customer});

  final CustomerSummary customer;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final today = branchToday(ref.watch(clockProvider), defaultTimeZone);
    final tag = customer.tags.isEmpty ? null : customer.tags.first;
    return SurfaceCard(
      padding: EdgeInsets.zero,
      child: InkWell(
        borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
        onTap: () => context.push(Routes.customer(customer.id)),
        child: Padding(
          padding: const EdgeInsets.symmetric(
            horizontal: OmvilaBosluk.bosluk3 + 2,
            vertical: OmvilaBosluk.bosluk3,
          ),
          child: Row(
            children: [
              InitialsAvatar(name: customer.fullName, size: 36),
              const SizedBox(width: OmvilaBosluk.bosluk3),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      customer.fullName,
                      style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
                    ),
                    Text(
                      customerSubtitle(customer, today),
                      style: OmvilaYazi.aciklama.copyWith(
                        color: r.metinIkincil,
                      ),
                    ),
                  ],
                ),
              ),
              if (tag != null) ...[
                const SizedBox(width: OmvilaBosluk.bosluk2),
                TagPill(tag),
              ],
              const SizedBox(width: OmvilaBosluk.bosluk2),
              Icon(LucideIcons.chevronRight, size: 18, color: r.metin),
            ],
          ),
        ),
      ),
    );
  }
}
