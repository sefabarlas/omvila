import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/appointments/domain/randevu_metinleri.dart';
import 'package:omvila_musteri/features/appointments/presentation/randevu_detay_ekrani.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Geçmiş sayfaları.
@immutable
class PastList {
  /// Liste.
  const PastList(this.items, this.nextCursor, {this.loadingMore = false});

  /// Randevular (azalan).
  final List<MeAppointmentSummary> items;

  /// Sonraki sayfa.
  final String? nextCursor;

  /// Sonraki sayfa okunuyor.
  final bool loadingMore;
}

/// Geçmiş; argüman "Değerlendirilmedi".
final AsyncNotifierProviderFamily<PastController, PastList, bool> pastProvider =
    AsyncNotifierProvider.autoDispose.family<PastController, PastList, bool>(
      PastController.new,
    );

/// Geçmiş listesi ve sonraki sayfalar.
class PastController extends AsyncNotifier<PastList> {
  /// [notReviewed] "Değerlendirilmedi" (family argümanı konumsal gelir).
  // ignore: avoid_positional_boolean_parameters
  PastController(this.notReviewed);

  /// Süzgeç.
  final bool notReviewed;

  @override
  Future<PastList> build() async {
    final page = await ref
        .watch(appointmentsRepositoryProvider)
        .past(notReviewed: notReviewed);
    return PastList(page.items, page.nextCursor);
  }

  /// Sonraki sayfa (liste sonuna gelince).
  Future<void> loadMore() async {
    final current = state.value;
    final cursor = current?.nextCursor;
    if (current == null || cursor == null || current.loadingMore) {
      return;
    }

    state = AsyncData(
      PastList(current.items, cursor, loadingMore: true),
    );
    try {
      final page = await ref
          .read(appointmentsRepositoryProvider)
          .past(notReviewed: notReviewed, cursor: cursor);
      state = AsyncData(
        PastList([...current.items, ...page.items], page.nextCursor),
      );
    } on ApiProblem {
      state = AsyncData(current);
    }
  }
}

/// Geçmiş (Gecmis, GecmisBos): "Tümü" / "Değerlendirilmedi", ay başlıkları
/// (şubenin saat dilimi), "Değerlendir", puan yıldızları, İptal rozeti.
class GecmisEkrani extends ConsumerStatefulWidget {
  /// Ekran.
  const GecmisEkrani({super.key});

  @override
  ConsumerState<GecmisEkrani> createState() => _GecmisEkraniState();
}

class _GecmisEkraniState extends ConsumerState<GecmisEkrani> {
  bool _notReviewed = false;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final list = ref.watch(pastProvider(_notReviewed));
    final bosTumu = !_notReviewed && (list.value?.items.isEmpty ?? false);
    return Scaffold(
      backgroundColor: r.zemin,
      appBar: AppBar(
        backgroundColor: r.zemin,
        surfaceTintColor: r.zemin,
        leading: IconButton(
          tooltip: 'Geri',
          icon: const Icon(LucideIcons.arrowLeft),
          onPressed: () => context.canPop()
              ? context.pop()
              : context.go(Routes.randevularim),
        ),
      ),
      body: RefreshIndicator(
        onRefresh: () => ref.refresh(pastProvider(_notReviewed).future),
        child: ListView(
          physics: const AlwaysScrollableScrollPhysics(),
          padding: const EdgeInsets.fromLTRB(
            OmvilaBosluk.bosluk4,
            0,
            OmvilaBosluk.bosluk4,
            OmvilaBosluk.bosluk8,
          ),
          children: [
            Semantics(
              header: true,
              child: Text(
                'Geçmiş',
                style: OmvilaYazi.buyukBaslik.copyWith(color: r.metin),
              ),
            ),
            const SizedBox(height: OmvilaBosluk.bosluk4),
            // GecmisBos'ta segment yok; "Değerlendirilmedi" boşsa geri
            // dönebilmek için görünür kalır.
            if (!bosTumu)
              OmvilaSegment<bool>(
                ogeler: const [
                  OmvilaSegmentOgesi(false, 'Tümü'),
                  OmvilaSegmentOgesi(true, 'Değerlendirilmedi'),
                ],
                secili: _notReviewed,
                onSecildi: (v) => setState(() => _notReviewed = v),
              ),
            ...switch (list) {
              AsyncData(:final value) when value.items.isEmpty => [
                OmvilaBosDurum(
                  ikon: LucideIcons.history,
                  baslik: _notReviewed
                      ? 'Değerlendirilecek randevunuz yok'
                      : 'Henüz geçmiş randevunuz yok',
                  aciklama: _notReviewed
                      ? 'Tamamlanan randevularınızı burada '
                            'değerlendirebilirsiniz.'
                      : 'Tamamlanan randevularınız burada listelenir. '
                            'Buradan değerlendirme yapabilir ve aynı '
                            'randevuyu tek dokunuşla tekrar alabilirsiniz.',
                ),
              ],
              AsyncData(:final value) => _rows(context, value),
              AsyncError(:final error) => [
                HataDurumu(
                  error: error,
                  onRetry: () => ref.invalidate(pastProvider(_notReviewed)),
                ),
              ],
              _ => [
                const Padding(
                  padding: EdgeInsets.only(top: OmvilaBosluk.bosluk12),
                  child: YukleniyorDurumu(),
                ),
              ],
            },
          ],
        ),
      ),
    );
  }

  List<Widget> _rows(BuildContext context, PastList list) {
    final widgets = <Widget>[];
    String? month;
    for (final a in list.items) {
      final day = toBranchDate(a.startUtc, a.timeZone);
      final m = formatDate(day, DateStyle.monthYear);
      if (m != month) {
        month = m;
        final thisYear = branchToday(
          ref.read(clockProvider),
          a.timeZone,
        ).startsWith(day.substring(0, 4));
        widgets.add(
          Padding(
            padding: const EdgeInsets.only(
              top: OmvilaBosluk.bosluk5,
              bottom: OmvilaBosluk.bosluk2,
            ),
            child: Semantics(
              header: true,
              child: Text(
                thisYear ? m.split(' ').first : m,
                style: OmvilaYazi.baslik3.copyWith(
                  color: context.omvilaRenkler.metin,
                  fontWeight: FontWeight.w800,
                ),
              ),
            ),
          ),
        );
      }

      widgets.add(
        Padding(
          padding: const EdgeInsets.only(bottom: OmvilaBosluk.bosluk2),
          child: GecmisSatiri(randevu: a),
        ),
      );
    }

    if (list.nextCursor != null) {
      widgets.add(
        _SonrakiSayfa(
          onVisible: () =>
              ref.read(pastProvider(_notReviewed).notifier).loadMore(),
        ),
      );
    }

    return widgets;
  }
}

class _SonrakiSayfa extends StatefulWidget {
  const _SonrakiSayfa({required this.onVisible});

  final VoidCallback onVisible;

  @override
  State<_SonrakiSayfa> createState() => _SonrakiSayfaState();
}

class _SonrakiSayfaState extends State<_SonrakiSayfa> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) => widget.onVisible());
  }

  @override
  Widget build(BuildContext context) => const Padding(
    padding: EdgeInsets.all(OmvilaBosluk.bosluk4),
    child: YukleniyorDurumu(),
  );
}

/// Geçmiş satırı.
class GecmisSatiri extends StatelessWidget {
  /// Satır.
  const GecmisSatiri({required this.randevu, super.key});

  /// Randevu.
  final MeAppointmentSummary randevu;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final a = randevu;
    final cancelled = a.displayStatus == DisplayStatus.cancelled;
    final status = RandevuDurumu.fromDisplayStatus(a.displayStatus);
    final rating = a.review?.rating;
    final radius = BorderRadius.circular(OmvilaKose.koseLg);
    final trailing = a.canReview
        ? FilledButton(
            onPressed: () => context.push(Routes.degerlendir(a.id)),
            style: FilledButton.styleFrom(
              minimumSize: const Size(44, OmvilaBoyut.kontrolSm),
              textStyle: OmvilaYazi.etiket,
            ),
            child: const Text('Değerlendir'),
          )
        : rating != null
        ? Yildizlar(puan: rating)
        : (cancelled || a.displayStatus == DisplayStatus.noShow) &&
              status != null
        ? OmvilaDurumRozeti(durum: status)
        : null;
    return Opacity(
      opacity: cancelled ? 0.7 : 1,
      child: Material(
        color: r.zeminYukseltilmis,
        borderRadius: radius,
        child: InkWell(
          borderRadius: radius,
          onTap: () => context.push(Routes.randevu(a.id)),
          child: Padding(
            padding: const EdgeInsets.all(OmvilaBosluk.bosluk3),
            child: Row(
              children: [
                SalonRozeti(ad: a.brand.name),
                const SizedBox(width: OmvilaBosluk.bosluk3),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        a.brand.name,
                        style: OmvilaYazi.govdeGuclu.copyWith(
                          color: r.metin,
                          decoration: cancelled
                              ? TextDecoration.lineThrough
                              : null,
                          decorationColor: r.metin,
                        ),
                      ),
                      Text(
                        ozetSatiri([
                          hizmetlerMetni(a.items),
                          formatDate(
                            toBranchDate(a.startUtc, a.timeZone),
                            DateStyle.short,
                          ).split(' ').take(2).join(' '),
                          if (!cancelled && a.total.amountMinor > 0)
                            formatMoney(a.total.amountMinor),
                        ]),
                        style: OmvilaYazi.ikincil.copyWith(
                          color: r.metinIkincil,
                        ),
                      ),
                    ],
                  ),
                ),
                if (trailing != null) ...[
                  const SizedBox(width: OmvilaBosluk.bosluk2),
                  trailing,
                ],
              ],
            ),
          ),
        ),
      ),
    );
  }
}

/// Beş yıldızlık puan gösterimi.
class Yildizlar extends StatelessWidget {
  /// Gösterim.
  const Yildizlar({required this.puan, this.boyut = 16, super.key});

  /// 1–5.
  final int puan;

  /// İkon boyutu.
  final double boyut;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return Semantics(
      label: '5 üzerinden $puan yıldız',
      excludeSemantics: true,
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          for (var i = 1; i <= 5; i++)
            Icon(
              i <= puan ? Icons.star_rounded : Icons.star_outline_rounded,
              size: boyut,
              color: r.yildiz,
            ),
        ],
      ),
    );
  }
}
