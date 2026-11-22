import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/appointments/domain/randevu_metinleri.dart';
import 'package:omvila_musteri/features/appointments/presentation/randevularim_ekrani.dart';
import 'package:omvila_musteri/features/booking/domain/booking_draft.dart';
import 'package:omvila_musteri/features/booking/presentation/booking_controller.dart';
import 'package:omvila_musteri/features/brands/presentation/salon_ekleme.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Salonlarım (meListBrands).
final FutureProvider<List<MeBrand>> brandsProvider =
    FutureProvider.autoDispose<List<MeBrand>>(
      (ref) => ref.watch(brandsRepositoryProvider).list(),
    );

/// Randevu alma akışının başlangıcı "Hangi salon?" (YRSalon, YRSalonBos).
/// Salon seçilince (birden çok şubede önce şube) hizmet adımı açılır.
class SalonSecimiEkrani extends ConsumerWidget {
  /// Ekran.
  const SalonSecimiEkrani({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final brands = ref.watch(brandsProvider);
    return Scaffold(
      backgroundColor: r.zemin,
      appBar: AppBar(
        backgroundColor: r.zemin,
        centerTitle: true,
        title: const Text('Randevu al'),
        leading: context.canPop()
            ? IconButton(
                tooltip: 'Geri',
                icon: const Icon(LucideIcons.arrowLeft),
                onPressed: () => context.pop(),
              )
            : null,
        actions: [
          IconButton(
            tooltip: 'Kapat',
            icon: const Icon(LucideIcons.x),
            onPressed: () => context.go(Routes.randevularim),
          ),
        ],
      ),
      body: switch (brands) {
        AsyncData(:final value) => ListView(
          padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
          children: [
            Semantics(
              header: true,
              child: Text(
                'Hangi salon?',
                style: OmvilaYazi.buyukBaslik.copyWith(color: r.metin),
              ),
            ),
            if (value.isEmpty) ...[
              const SizedBox(height: OmvilaBosluk.bosluk4),
              IlkSalonKarti(
                baslik: 'Henüz bir salonunuz yok',
                aciklama:
                    'Randevu almak için salonunuzun bağlantısını açın ya da '
                    'salondaki QR kodu okutun. Randevu aldığınız salonlar '
                    'burada listelenir.',
                onQr: () => SalonEkleme.qrIle(context, ref),
                onPaste: () => SalonEkleme.panodan(context, ref),
              ),
              const SizedBox(height: OmvilaBosluk.bosluk4),
              Text(
                "OMVILA'da salon araması yoktur. Salonunuz OMVILA "
                'kullanıyorsa bağlantısını ondan isteyebilirsiniz.',
                textAlign: TextAlign.center,
                style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
              ),
            ] else ...[
              Text(
                'Daha önce randevu aldığınız salonlar.',
                style: OmvilaYazi.govde.copyWith(color: r.metinIkincil),
              ),
              const SizedBox(height: OmvilaBosluk.bosluk4),
              for (final brand in value) ...[
                _SalonSatiri(brand: brand),
                const SizedBox(height: OmvilaBosluk.bosluk2),
              ],
              const SizedBox(height: OmvilaBosluk.bosluk2),
              DecoratedBox(
                decoration: BoxDecoration(
                  color: r.zeminIkincil,
                  borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
                ),
                child: Padding(
                  padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
                  child: Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Icon(LucideIcons.info, size: 20, color: r.metinIkincil),
                      const SizedBox(width: OmvilaBosluk.bosluk3),
                      Expanded(
                        child: Text(
                          'Yeni bir salonda randevu için salonun paylaştığı '
                          'bağlantıyı açın. Randevunuz buraya kendiliğinden '
                          'eklenir.',
                          style: OmvilaYazi.ikincil.copyWith(
                            color: r.metinIkincil,
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ],
          ],
        ),
        AsyncError(:final error) => HataDurumu(
          error: error,
          onRetry: () => ref.invalidate(brandsProvider),
        ),
        _ => const YukleniyorDurumu(),
      },
    );
  }
}

/// Salon seçildi: tek şubede doğrudan, birden çok şubede şube sorulur.
Future<void> salonSecildi(
  BuildContext context,
  WidgetRef ref,
  MeBrand brand,
) async {
  final branches = brand.branches;
  final branch = branches.length == 1
      ? branches.single
      : await showModalBottomSheet<MeBranchRef>(
          context: context,
          showDragHandle: true,
          builder: (context) => SafeArea(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Padding(
                  padding: const EdgeInsets.symmetric(
                    horizontal: OmvilaBosluk.bosluk5,
                  ),
                  child: Text(
                    'Hangi şube?',
                    style: OmvilaYazi.baslik2.copyWith(
                      color: context.omvilaRenkler.metin,
                    ),
                  ),
                ),
                const SizedBox(height: OmvilaBosluk.bosluk2),
                for (final b in branches)
                  ListTile(
                    title: Text(b.name),
                    subtitle: b.district == b.name ? null : Text(b.district),
                    trailing: const Icon(LucideIcons.chevronRight),
                    onTap: () => Navigator.of(context).pop(b),
                  ),
                const SizedBox(height: OmvilaBosluk.bosluk4),
              ],
            ),
          ),
        );
  if (branch == null || !context.mounted) {
    return;
  }

  ref
      .read(bookingProvider.notifier)
      .start(
        BookingDraft(
          brandSlug: brand.slug,
          brandName: brand.name,
          branchId: branch.id,
          branchName: branch.name,
        ),
      );
  await context.push(Routes.yrHizmet);
}

class _SalonSatiri extends ConsumerWidget {
  const _SalonSatiri({required this.brand});

  final MeBrand brand;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final (zemin, metin) = switch (brand.coverColor) {
      // Kapak rengi avatar tonuna: kum → kum, mercan → mercan, petrol ve
      // diğerleri → nane (tasarım Profil/YRSalon).
      MeBrandCoverColorEnum.kum => (r.avatar2Zemin, r.avatar2Metin),
      MeBrandCoverColorEnum.mercan => (r.avatar3Zemin, r.avatar3Metin),
      _ => (r.avatar1Zemin, r.avatar1Metin),
    };
    final lastVisit = brand.lastVisitOn;
    final visit = lastVisit == null
        ? ''
        : 'son ziyaret ${formatDate(_date(lastVisit), DateStyle.dayMonth)}';
    final package = brand.activePackages.firstOrNull;
    final enabled = brand.bookingEnabled && brand.branches.isNotEmpty;
    final radius = BorderRadius.circular(OmvilaKose.koseLg);
    return Material(
      color: r.zeminYukseltilmis,
      borderRadius: radius,
      child: InkWell(
        borderRadius: radius,
        onTap: enabled ? () => salonSecildi(context, ref, brand) : null,
        child: Padding(
          padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Container(
                width: 52,
                height: 52,
                alignment: Alignment.center,
                decoration: BoxDecoration(
                  color: zemin,
                  borderRadius: BorderRadius.circular(OmvilaKose.koseMd),
                ),
                child: Text(
                  initials(brand.name),
                  style: OmvilaYazi.govdeGuclu.copyWith(color: metin),
                ),
              ),
              const SizedBox(width: OmvilaBosluk.bosluk3),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      brand.name,
                      style: OmvilaYazi.baslik3.copyWith(color: r.metin),
                    ),
                    Text(
                      ozetSatiri([
                        brand.branches
                            .map((b) => b.district)
                            .toSet()
                            .join(' ve '),
                        visit,
                      ]),
                      style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
                    ),
                    if (!brand.bookingEnabled)
                      Text(
                        'Online randevu kapalı · Salonu arayın',
                        style: OmvilaYazi.aciklama.copyWith(color: r.uyari),
                      ),
                    if (package != null) ...[
                      const SizedBox(height: OmvilaBosluk.bosluk2),
                      DecoratedBox(
                        decoration: BoxDecoration(
                          color: r.zeminMarkaYumusak,
                          borderRadius: BorderRadius.circular(
                            OmvilaKose.koseTam,
                          ),
                        ),
                        child: Padding(
                          padding: const EdgeInsets.symmetric(
                            horizontal: OmvilaBosluk.bosluk3,
                            vertical: OmvilaBosluk.bosluk1,
                          ),
                          child: Text(
                            '${package.packageName} var',
                            style: OmvilaYazi.aciklama.copyWith(
                              color: r.metinMarka,
                              fontWeight: FontWeight.w700,
                            ),
                          ),
                        ),
                      ),
                    ],
                  ],
                ),
              ),
              if (enabled)
                Padding(
                  padding: const EdgeInsets.only(top: OmvilaBosluk.bosluk4),
                  child: Icon(
                    LucideIcons.chevronRight,
                    size: 20,
                    color: r.metin,
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }

  static String _date(DateTime day) =>
      '${day.year}-${day.month.toString().padLeft(2, '0')}-'
      '${day.day.toString().padLeft(2, '0')}';
}
