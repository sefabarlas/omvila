import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/features/profile/data/profile_repository.dart';

/// Profil (meGetProfile).
final FutureProvider<MeProfile> profileProvider =
    FutureProvider.autoDispose<MeProfile>(
      (ref) => ref.watch(profileRepositoryProvider).profile(),
    );

/// Kampanya izinleri (meListConsents).
final AsyncNotifierProvider<ConsentsController, List<MeConsent>>
consentsProvider =
    AsyncNotifierProvider.autoDispose<ConsentsController, List<MeConsent>>(
      ConsentsController.new,
    );

/// İzinler ve anahtar.
class ConsentsController extends AsyncNotifier<List<MeConsent>> {
  @override
  Future<List<MeConsent>> build() =>
      ref.watch(profileRepositoryProvider).consents();

  /// Anahtar; hata ApiProblem olarak fırlar, liste değişmez.
  Future<void> set(String brandId, {required bool granted}) async {
    final next = await ref
        .read(profileRepositoryProvider)
        .setMarketing(brandId, granted: granted);
    state = AsyncData(next);
  }
}

/// HesapSil önizlemesi.
final FutureProvider<DeletionPreview> deletionPreviewProvider =
    FutureProvider.autoDispose<DeletionPreview>(
      (ref) => ref.watch(profileRepositoryProvider).deletionPreview(),
    );

/// İzinlerim satırının alt metni.
String izinOzeti(List<MeConsent> consents) {
  final on = consents.where((c) => c.marketingGranted).length;
  return on == 0
      ? 'Kampanya mesajları kapalı'
      : '$on salonda kampanya mesajları açık';
}
