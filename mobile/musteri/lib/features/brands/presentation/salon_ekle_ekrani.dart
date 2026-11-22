import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/appointments/presentation/home_provider.dart';
import 'package:omvila_musteri/features/brands/data/brands_repository.dart';
import 'package:omvila_musteri/shared/widgets/baglanti_gecersiz_ekrani.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Salon bağlantısıyla eklenen salon (derin bağlantı `/{brandSlug}`).
// Aile türü riverpod'un dışa açık API'sinde yok.
// ignore: specify_nonobvious_property_types
final _saveBrandProvider = FutureProvider.autoDispose.family<MeBrand, String>(
  (ref, slug) =>
      ref.read(brandsRepositoryProvider).save(slug, BrandSource.link),
);

/// Derin bağlantı `randevu.omvila.com.tr/{brandSlug}`: salonu ekler ve
/// Randevularım'a döner. Bilinmeyen bağlantıda "Bu bağlantı geçersiz".
/// Salonun randevu akışına geçiş randevu alma PR'ında (T-106).
class SalonEkleEkrani extends ConsumerWidget {
  /// Ekran.
  const SalonEkleEkrani({required this.brandSlug, super.key});

  /// Bağlantı adı.
  final String brandSlug;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    ref.listen(_saveBrandProvider(brandSlug), (_, next) {
      if (next case AsyncData(:final value)) {
        ref.invalidate(homeProvider);
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('${value.name} salonlarınıza eklendi.')),
        );
        context.go(Routes.randevularim);
      }
    });
    final state = ref.watch(_saveBrandProvider(brandSlug));
    return switch (state) {
      AsyncError(:final error)
          when error is ApiProblem && error.code == ErrorCode.notFound.value =>
        const BaglantiGecersizEkrani(),
      AsyncError(:final error) => Scaffold(
        backgroundColor: context.omvilaRenkler.zemin,
        body: HataDurumu(
          error: error,
          onRetry: () => ref.invalidate(_saveBrandProvider(brandSlug)),
        ),
      ),
      _ => Scaffold(
        backgroundColor: context.omvilaRenkler.zemin,
        body: const YukleniyorDurumu(),
      ),
    };
  }
}
