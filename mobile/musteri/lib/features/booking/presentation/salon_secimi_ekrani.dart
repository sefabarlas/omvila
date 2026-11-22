import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/features/appointments/domain/randevu_metinleri.dart';
import 'package:omvila_musteri/features/brands/presentation/salon_ekleme.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Salonlarım (meListBrands).
final FutureProvider<List<MeBrand>> brandsProvider =
    FutureProvider.autoDispose<List<MeBrand>>(
      (ref) => ref.watch(brandsRepositoryProvider).list(),
    );

/// Randevu alma akışının başlangıcı "Hangi salon?" (YRSalon, YRSalonBos).
/// Hizmet, personel, saat ve onay adımları sonraki PR'da; bu PR'da salonlar
/// listelenir, seçim henüz ileri gitmez (T-106).
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
        leading: IconButton(
          tooltip: 'Geri',
          icon: const Icon(LucideIcons.arrowLeft),
          onPressed: () => context.pop(),
        ),
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
            if (value.isEmpty)
              OmvilaBosDurum(
                ikon: LucideIcons.store,
                baslik: 'Henüz salonunuz yok',
                aciklama:
                    'Salonun paylaştığı bağlantıyı açın ya da salondaki QR '
                    'kodu okutun.',
                eylemEtiketi: 'QR kodu okut',
                eylemIkonu: LucideIcons.scanLine,
                onEylem: () => SalonEkleme.qrIle(context, ref),
              )
            else ...[
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

class _SalonSatiri extends StatelessWidget {
  const _SalonSatiri({required this.brand});

  final MeBrand brand;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final (zemin, metin) = switch (brand.coverColor) {
      MeBrandCoverColorEnum.mercan => (r.avatar3Zemin, r.avatar3Metin),
      MeBrandCoverColorEnum.petrol => (r.avatar2Zemin, r.avatar2Metin),
      _ => (r.avatar1Zemin, r.avatar1Metin),
    };
    final lastVisit = brand.lastVisitOn;
    final visit = lastVisit == null
        ? ''
        : 'son ziyaret ${formatDate(_date(lastVisit), DateStyle.dayMonth)}';
    final package = brand.activePackages.firstOrNull;
    return DecoratedBox(
      decoration: BoxDecoration(
        color: r.zeminYukseltilmis,
        borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
        boxShadow: context.omvilaGolgeler.golgeSm,
      ),
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
                  if (package != null) ...[
                    const SizedBox(height: OmvilaBosluk.bosluk2),
                    DecoratedBox(
                      decoration: BoxDecoration(
                        color: r.zeminMarkaYumusak,
                        borderRadius: BorderRadius.circular(OmvilaKose.koseTam),
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
          ],
        ),
      ),
    );
  }

  static String _date(DateTime day) =>
      '${day.year}-${day.month.toString().padLeft(2, '0')}-'
      '${day.day.toString().padLeft(2, '0')}';
}
